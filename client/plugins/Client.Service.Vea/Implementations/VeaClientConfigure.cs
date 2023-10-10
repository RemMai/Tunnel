using System.Threading.Tasks;
using client.service.ui.api.Interfaces;
using Common.Libs.AutoInject.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace Client.Service.Vea.Implementations
{
    /// <summary>
    /// 组网配置文件
    /// </summary>
    [AutoInject(ServiceLifetime.Singleton)]
    public sealed class VeaClientConfigure : IClientConfigure
    {
        private readonly Models.Config config;
        private readonly VeaTransfer veaTransfer;

        public VeaClientConfigure(Models.Config config, VeaTransfer VeaTransfer)
        {
            this.config = config;
            this.veaTransfer = VeaTransfer;
        }

        /// <summary>
        /// 名字
        /// </summary>
        public string Name => "虚拟网卡组网";

        /// <summary>
        /// 作者
        /// </summary>
        public string Author => "snltty";

        /// <summary>
        /// 描述
        /// </summary>
        public string Desc => "虚拟网卡";

        /// <summary>
        /// 启用
        /// </summary>
        public bool Enable => config.ConnectEnable;

        /// <summary>
        /// 加载
        /// </summary>
        /// <returns></returns>
        public async Task<string> Load()
        {
            return await config.ReadString();
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="jsonStr"></param>
        /// <returns></returns>
        public async Task<bool> Save(string jsonStr)
        {
            await config.SaveConfig(jsonStr).ConfigureAwait(false);
            veaTransfer.UpdateIp();
            return true;
        }
    }
}