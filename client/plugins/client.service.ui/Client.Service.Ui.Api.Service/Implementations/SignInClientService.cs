using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Client.Messengers.Signin;
using client.service.ui.api.Enums;
using client.service.ui.api.Interfaces;
using client.service.ui.api.Models;
using client.service.ui.api.service.Models;
using Common.Extensions.AutoInject.Attributes;
using Common.Libs.Extends;
using Microsoft.Extensions.DependencyInjection;

namespace client.service.ui.api.service.Implementations
{
    /// <summary>
    /// 注册
    /// </summary>
    [AutoInject(ServiceLifetime.Singleton)]
    public sealed class SignInClientService : IClientService
    {

        private readonly ISignInTransfer signinTransfer;
        private readonly SignInStateInfo signInStateInfo;
        private readonly Client.Config config;
        public SignInClientService(ISignInTransfer signinTransfer, SignInStateInfo signInStateInfo, Client.Config config)
        {
            this.signinTransfer = signinTransfer;
            this.signInStateInfo = signInStateInfo;
            this.config = config;
        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public async Task<bool> Start(ClientServiceParamsInfo arg)
        {
            var result = await signinTransfer.SignIn().ConfigureAwait(false);
            if (!result.Data)
            {
                arg.SetCode(ClientServiceResponseCodes.Error, result.ErrorMsg);
            }
            return result.Data;
        }
        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="arg"></param>
        public bool Exit(ClientServiceParamsInfo arg)
        {
            signinTransfer.Exit();
            return true;
        }
        /// <summary>
        /// 获取配置文件和信息
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public SignInInfo Info(ClientServiceParamsInfo arg)
        {
            return new SignInInfo
            {
                ClientConfig = config.Client,
                ServerConfig = config.Server,
                LocalInfo = signInStateInfo.LocalInfo,
                RemoteInfo = signInStateInfo.RemoteInfo,
            };
        }
        /// <summary>
        /// 更新配置文件
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public async Task<bool> Config(ClientServiceParamsInfo arg)
        {
            ConfigureParamsInfo model = arg.Content.DeJson<ConfigureParamsInfo>();

            config.Client = model.ClientConfig;
            config.Server = model.ServerConfig;

            await config.SaveConfig(config).ConfigureAwait(false);
            return true;
        }

        /// <summary>
        /// ping ip地址
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public async Task<long[]> Ping(ClientServiceParamsInfo arg)
        {
            string[] ips = arg.Content.DeJson<string[]>();

            PingModel[] result = ips.Select(c =>
            {
                Ping ping = new Ping();
                return new PingModel
                {
                    Ping = ping,
                    Task = ping.SendPingAsync(c, 1000)
                };
            }).ToArray();
            await Task.WhenAll(result.Select(c => c.Task));
            foreach (var item in result)
            {
                item.Ping.Dispose();
            }

            return result.Select(c =>
            {
                return c.Task.Result.Status == IPStatus.Success ? c.Task.Result.RoundtripTime : -1;
            }).ToArray();
        }

    }
}
