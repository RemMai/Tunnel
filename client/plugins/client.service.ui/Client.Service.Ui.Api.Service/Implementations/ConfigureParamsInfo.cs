using Client;

namespace Client.Service.Ui.Api.Service.Implementations
{
    /// <summary>
    /// 配置信息
    /// </summary>
    public sealed class ConfigureParamsInfo
    {
        /// <summary>
        /// 客户端配置信息
        /// </summary>
        public ClientConfig ClientConfig { get; set; } = new ClientConfig();
        /// <summary>
        /// 服务端配置信息
        /// </summary>
        public ServerConfig ServerConfig { get; set; } = new ServerConfig();
    }
}