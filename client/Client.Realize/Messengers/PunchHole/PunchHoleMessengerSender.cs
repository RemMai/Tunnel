using Common.Libs;
using Microsoft.Extensions.DependencyInjection;
using Common.Server;
using Common.Server.Model;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Client.Messengers.Clients;
using Client.Messengers.PunchHole;
using Client.Messengers.PunchHole.udp;
using Client.Messengers.Signin;
using Common.Libs.AutoInject.Attributes;

namespace Client.Realize.Messengers.PunchHole
{
    /// <summary>
    /// 打洞消息
    /// </summary>
    [AutoInject(ServiceLifetime.Singleton)]
    public sealed class PunchHoleMessengerSender
    {
        private Dictionary<PunchHoleTypes, IPunchHole> plugins = new Dictionary<PunchHoleTypes, IPunchHole>();

        private readonly MessengerSender messengerSender;
        private readonly SignInStateInfo signInState;
        private readonly IServiceProvider serviceProvider;

        private readonly NumberSpace numberSpace = new NumberSpace();
        private readonly WheelTimer<TimeoutState> wheelTimer = new WheelTimer<TimeoutState>();

        private ConcurrentDictionary<ulong, WheelTimerTimeout<TimeoutState>> sends =
            new ConcurrentDictionary<ulong, WheelTimerTimeout<TimeoutState>>();

        public PunchHoleMessengerSender(MessengerSender messengerSender, SignInStateInfo signInState,
            IServiceScopeFactory serviceScopeFactory)
        {
            this.messengerSender = messengerSender;
            this.signInState = signInState;
            this.serviceProvider = serviceScopeFactory.CreateScope().ServiceProvider;
        }

        /// <summary>
        /// 加载插件
        /// </summary>
        /// <param name="assemblies"></param>
        public void LoadPlugins(Assembly[] assemblies)
        {
            foreach (Type item in ReflectionHelper.GetInterfaceSchieves(assemblies, typeof(IPunchHole)))
            {
                IPunchHole obj = (IPunchHole)serviceProvider.GetService(item);
                if (!plugins.ContainsKey(obj.Type))
                {
                    plugins.Add(obj.Type, obj);
                }
            }
        }

        /// <summary>
        /// 收到打洞消息
        /// </summary>
        /// <param name="arg"></param>
        public async Task OnPunchHole(IConnection connection, PunchHoleRequestInfo info)
        {
            PunchHoleTypes type = (PunchHoleTypes)info.PunchType;
            if (plugins.TryGetValue(type, out IPunchHole value))
            {
                await value?.Execute(connection, info);
            }
        }

        /// <summary>
        /// 收到回执
        /// </summary>
        /// <param name="response"></param>
        public void OnResponse(PunchHoleResponseInfo response)
        {
            if (sends.TryRemove(response.RequestId, out WheelTimerTimeout<TimeoutState> timeout))
            {
                timeout.Task.State.Tcs.SetResult(true);
            }
        }

        /// <summary>
        /// 发送请求
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arg"></param>
        /// <param name="requestid"></param>
        /// <returns></returns>
        public async Task<bool> Request<T>(SendPunchHoleArg<T> arg, ulong requestid = 0) where T : IPunchHoleStepInfo
        {
            IPunchHoleStepInfo msg = arg.Data;
            return await messengerSender.SendOnly(new MessageRequestWrap
            {
                Connection = arg.Connection,
                MessengerId = (ushort)PunchHoleMessengerIds.Request,
                Payload = new PunchHoleRequestInfo
                {
                    RequestId = requestid,
                    Data = msg.ToBytes(),
                    PunchForwardType = msg.ForwardType,
                    FromId = signInState.ConnectId,
                    PunchStep = msg.Step,
                    PunchType = (byte)msg.PunchType,
                    ToId = arg.ToId,
                    NewTunnel = arg.NewTunnel
                }.ToBytes()
            }).ConfigureAwait(false);
        }

        /// <summary>
        /// 发送等待回执的请求
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arg"></param>
        /// <returns></returns>
        public async Task<bool> RequestReply<T>(SendPunchHoleArg<T> arg) where T : IPunchHoleStepInfo
        {
            ulong requestid = numberSpace.Increment();
            TimeoutState timeoutState = new TimeoutState
            {
                RequestId = requestid,
                Tcs = new TaskCompletionSource<bool>()
            };
            var timeout =
                wheelTimer.NewTimeout(
                    new WheelTimerTimeoutTask<TimeoutState> { Callback = Callback, State = timeoutState }, 2000);
            sends.TryAdd(requestid, timeout);

            bool res = await Request(arg, requestid);
            if (res == false)
            {
                timeoutState.Tcs.SetResult(false);
            }

            return await timeoutState.Tcs.Task.ConfigureAwait(false);
        }

        private void Callback(WheelTimerTimeout<TimeoutState> timeout)
        {
            if (sends.TryRemove(timeout.Task.State.RequestId, out _))
            {
                timeout.Task.State.Tcs.SetResult(false);
            }
        }

        /// <summary>
        /// 收到反向连接
        /// </summary>
        public Action<PunchHoleRequestInfo> OnReverse { get; set; } = (param) => { };

        /// <summary>
        /// 通知其反向连接
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public async Task SendReverse(ClientInfo info)
        {
            byte times = info.TryReverseValue;
            await Request(new SendPunchHoleArg<PunchHoleReverseInfo>
            {
                Connection = signInState.Connection,
                ToId = info.ConnectionId,
                Data = new PunchHoleReverseInfo { Value = times }
            }).ConfigureAwait(false);
        }

        /// <summary>
        /// 通知其重置注册
        /// </summary>
        /// <param name="toid"></param>
        /// <returns></returns>
        public async Task SendReset(ulong toid)
        {
            await Request(new SendPunchHoleArg<PunchHoleResetInfo>
            {
                Connection = signInState.Connection,
                ToId = toid,
                Data = new PunchHoleResetInfo { }
            }).ConfigureAwait(false);
        }

        /// <summary>
        /// 通知其掉线
        /// </summary>
        /// <param name="toid"></param>
        /// <returns></returns>
        public async Task SendOffline(ulong toid)
        {
            await Request(new SendPunchHoleArg<PunchHoleOfflineInfo>
            {
                Connection = signInState.Connection,
                ToId = toid,
                Data = new PunchHoleOfflineInfo { }
            }).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// 发送打洞消息的参数
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class SendPunchHoleArg<T>
    {
        /// <summary>
        /// 连接
        /// </summary>
        public IConnection Connection { get; set; }

        /// <summary>
        /// 给谁
        /// </summary>
        public ulong ToId { get; set; }

        /// <summary>
        /// 给谁
        /// </summary>
        public byte NewTunnel { get; set; }

        /// <summary>
        /// 数据
        /// </summary>
        public T Data { get; set; }
    }

    /// <summary>
    /// 请求消息超时缓存
    /// </summary>
    public sealed class TimeoutState
    {
        /// <summary>
        /// 请求id
        /// </summary>
        public ulong RequestId { get; set; }

        /// <summary>
        /// task
        /// </summary>
        public TaskCompletionSource<bool> Tcs { get; set; }
    }
}