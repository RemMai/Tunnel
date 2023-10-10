using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Common.Libs;
using Common.Libs.AutoInject.Attributes;
using Common.Libs.Extends;
using Common.Server;
using Common.Server.Attributes;
using Common.Server.Interfaces;
using Common.Server.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Server.Implementations
{
    /// <summary>
    /// 消息处理总线
    /// </summary>
    [AutoInject(ServiceLifetime.Singleton)]
    public sealed class MessengerResolver
    {
        delegate void VoidDelegate(IConnection connection);

        delegate Task TaskDelegate(IConnection connection);

        private readonly Dictionary<ushort, MessengerCacheInfo> messengers = new();

        private readonly ITcpServer _tcpServer;
        private readonly IUdpServer _udpServer;
        private readonly MessengerSender messengerSender;
        private readonly IRelaySourceConnectionSelector sourceConnectionSelector;
        private readonly IRelayValidator _relayValidator;
        private readonly IServiceProvider _serviceProvider;


        public MessengerResolver(IUdpServer udpServer, ITcpServer tcpServer, MessengerSender messengerSender,
            IRelaySourceConnectionSelector sourceConnectionSelector, IRelayValidator relayValidator,
            IServiceScopeFactory serviceScopeFactory)
        {
            this._tcpServer = tcpServer;
            this._udpServer = udpServer;
            this.messengerSender = messengerSender;
            this._tcpServer.OnPacket = InputData;
            this._udpServer.OnPacket = InputData;
            this.sourceConnectionSelector = sourceConnectionSelector;
            this._relayValidator = relayValidator;
            this._serviceProvider = serviceScopeFactory.CreateScope().ServiceProvider;
        }

        public void LoadMessenger(Assembly[] assemblies)
        {
            Type voidType = typeof(void);
            Type midType = typeof(MessengerIdAttribute);

            var messages = _serviceProvider.GetServices<IMessenger>();
            foreach (IMessenger messenger in messages)
            {
                Console.WriteLine(messenger.GetType().FullName);
            }


            foreach (IMessenger messenger in _serviceProvider.GetServices<IMessenger>())
            {
                foreach (var method in messenger.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public |
                                                                      BindingFlags.DeclaredOnly))
                {
                    if (method.GetCustomAttribute(midType) is MessengerIdAttribute mid)
                    {
                        if (messengers.ContainsKey(mid.Id) == false)
                        {
                            MessengerCacheInfo cache = new()
                            {
                                Target = messenger
                            };
                            if (method.ReturnType == voidType)
                            {
                                cache.VoidMethod =
                                    (VoidDelegate)Delegate.CreateDelegate(typeof(VoidDelegate), messenger, method);
                            }
                            else if (method.ReturnType.GetProperty("IsCompleted") != null &&
                                     method.ReturnType.GetMethod("GetAwaiter") != null)
                            {
                                cache.TaskMethod =
                                    (TaskDelegate)Delegate.CreateDelegate(typeof(TaskDelegate), messenger, method);
                            }

                            messengers.TryAdd(mid.Id, cache);
                            Logger.Instance.Info($"{messenger.GetType().Name}->{method.Name}->{mid.Id} 消息id已存在");
                        }
                        else
                        {
                            Logger.Instance.Error($"{messenger.GetType().Name}->{method.Name}->{mid.Id} 消息id已存在");
                        }
                    }
                }
            }


            List<Tuple<string, ushort, ushort>> uShorts = ReflectionHelper.GetEnums(assemblies)
                .Where(c => c.Name.EndsWith("MessengerIds")).Distinct().Select(item =>
                {
                    var fields = item
                        .GetFields(BindingFlags.Static | BindingFlags.Public)
                        .Select(c => (ushort)c.GetValue(null)!).ToArray();

                    return new Tuple<string, ushort, ushort>(item.Name, fields.Min(), fields.Max());
                }).OrderBy(c => c.Item2).ToList();

            Logger.Instance.Warning(string.Empty.PadRight(Logger.Instance.PaddingWidth, '='));
            Logger.Instance.Debug($"枚举类型ushort，已存在消息列表如下:");
            foreach (var item in uShorts)
            {
                Logger.Instance.Info($"{item.Item1.PadLeft(32, '-')}  {item.Item2}-{item.Item3}");
            }

            Logger.Instance.Warning(string.Empty.PadRight(Logger.Instance.PaddingWidth, '='));
        }

        public bool GetMessenger(ushort id, out object obj)
        {
            obj = null;
            if (messengers.TryGetValue(id, out MessengerCacheInfo cache))
            {
                obj = cache.Target;
                return true;
            }

            return false;
        }

        /// <summary>
        /// 收到消息
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public async Task InputData(IConnection connection)
        {
            Memory<byte> receive = connection.ReceiveData;
            //去掉表示数据长度的4字节
            Memory<byte> readReceive = receive.Slice(4);
            MessageResponseWrap responseWrap = connection.ReceiveResponseWrap;
            MessageRequestWrap requestWrap = connection.ReceiveRequestWrap;

            /*
             * 中继
             * request   A<-->B<-->C  计入A流量
             * response  C<-->B<-->A  计入A流量
             */

            connection.FromConnection = connection;
            try
            {
                //回复的消息
                if ((MessageTypes)(readReceive.Span[0] & MessageRequestWrap.TypeBits) == MessageTypes.RESPONSE)
                {
                    responseWrap.FromArray(readReceive);
                    if (responseWrap.Relay && responseWrap.RelayIdLength - 1 - responseWrap.RelayIdIndex >= 0)
                    {
                        ulong nextId = responseWrap.RelayIds.Span
                            .Slice(responseWrap.RelayIdIndex * MessageRequestWrap.RelayIdSize).ToUInt64();
                        //目的地连接对象
                        IConnection _connection = sourceConnectionSelector.Select(connection, nextId);
                        if (_connection == null || ReferenceEquals(connection, _connection)) return;
                        //RelayIdIndex 后移一位
                        receive.Span[MessageRequestWrap.RelayIdIndexPos]++;
                        if (_connection.SendDenied == 0)
                        {
                            await _connection.WaitOne();
                            await _connection.Send(receive).ConfigureAwait(false);
                            _connection.SentBytes += (ulong)receive.Length;
                            _connection.Release();
                        }
                    }
                    else
                    {
                        if (connection.EncodeEnabled && responseWrap.Encode)
                        {
                            if (responseWrap.Relay)
                            {
                                connection.FromConnection =
                                    sourceConnectionSelector.Select(connection, responseWrap.RelayIds.Span.ToUInt64());
                            }

                            responseWrap.Payload = connection.FromConnection.Crypto.Decode(responseWrap.Payload);
                        }

                        messengerSender.Response(responseWrap);
                    }

                    return;
                }

                //新的请求
                requestWrap.FromArray(readReceive);
                //是中继数据
                if (requestWrap.Relay)
                {
                    //还在路上
                    if (requestWrap.RelayIdLength - 1 - requestWrap.RelayIdIndex >= 0)
                    {
                        if (_relayValidator.Validate(connection))
                        {
                            ulong nextId = requestWrap.RelayIds.Span
                                .Slice(requestWrap.RelayIdIndex * MessageRequestWrap.RelayIdSize).ToUInt64();
                            //目的地连接对象
                            IConnection _connection = sourceConnectionSelector.Select(connection, nextId);
                            if (_connection == null || ReferenceEquals(connection, _connection)) return;
                            //RelayIdIndex 后移一位
                            receive.Span[MessageRequestWrap.RelayIdIndexPos]++;
                            if (_connection.SendDenied == 0)
                            {
                                await _connection.WaitOne();
                                //中继数据不再次序列化，直接在原数据上更新数据然后发送
                                await _connection.Send(receive).ConfigureAwait(false);
                                connection.SentBytes += (ulong)receive.Length;
                                _connection.Release();
                            }
                        }

                        return;
                    }
                }

                if (requestWrap.Relay)
                {
                    connection.FromConnection =
                        sourceConnectionSelector.Select(connection, requestWrap.RelayIds.Span.ToUInt64());
                }

                IConnection responseConnection = connection;
                if (connection.EncodeEnabled && requestWrap.Encode)
                {
                    responseConnection = connection.FromConnection;
                    requestWrap.Payload = connection.FromConnection.Crypto.Decode(requestWrap.Payload);
                }

                //404,没这个插件
                if (messengers.TryGetValue(requestWrap.MessengerId, out MessengerCacheInfo plugin) == false)
                {
                    if (Logger.Instance.LoggerLevel <= LoggerTypes.DEBUG)
                        Logger.Instance.Error($"{requestWrap.MessengerId},{connection.ServerType}, not found");
                    if (requestWrap.Reply == true)
                    {
                        bool res = await messengerSender.ReplyOnly(new MessageResponseWrap
                        {
                            Connection = responseConnection,
                            Encode = requestWrap.Encode,
                            Code = MessageResponeCodes.NOT_FOUND,
                            RelayIds = requestWrap.RelayIds,
                            RequestId = requestWrap.RequestId
                        }).ConfigureAwait(false);
                    }

                    return;
                }

                if (plugin.VoidMethod != null)
                {
                    plugin.VoidMethod(connection);
                }
                else if (plugin.TaskMethod != null)
                {
                    await plugin.TaskMethod(connection);
                }

                if (requestWrap.Reply == true)
                {
                    bool res = await messengerSender.ReplyOnly(new MessageResponseWrap
                    {
                        Connection = responseConnection,
                        Encode = requestWrap.Encode,
                        Payload = connection.ResponseData,
                        RelayIds = requestWrap.RelayIds,
                        RequestId = requestWrap.RequestId
                    }).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                if (Logger.Instance.LoggerLevel <= LoggerTypes.DEBUG)
                {
                    Logger.Instance.Error(ex);
                    if (receive.Length > 1024)
                    {
                        Logger.Instance.Error(
                            $"{connection.Address}:{string.Join(",", receive.Slice(0, 1024).ToArray())}");
                    }
                    else
                    {
                        Logger.Instance.Error($"{connection.Address}:{string.Join(",", receive.ToArray())}");
                    }
                }
                //connection.Disponse();
            }
            finally
            {
                connection.Return();
            }
        }


        /// <summary>
        /// 消息插件缓存
        /// </summary>
        private struct MessengerCacheInfo
        {
            /// <summary>
            /// 对象
            /// </summary>
            public IMessenger Target { get; set; }

            /// <summary>
            /// 空返回方法
            /// </summary>
            public VoidDelegate VoidMethod { get; set; }

            /// <summary>
            /// Task返回方法
            /// </summary>
            public TaskDelegate TaskMethod { get; set; }
        }
    }
}