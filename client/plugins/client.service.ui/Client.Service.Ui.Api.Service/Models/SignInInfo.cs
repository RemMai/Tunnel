using Client;
using Client.Messengers.Signin;

namespace client.service.ui.api.service.Models
{
    /// <summary>
    /// 注册信息
    /// </summary>
    public sealed class SignInInfo
    {
        /// <summary>
        /// 客户端配置
        /// </summary>
        public ClientConfig ClientConfig { get; set; } = new ClientConfig();
        /// <summary>
        /// 服务端配置
        /// </summary>
        public ServerConfig ServerConfig { get; set; } = new ServerConfig();
        /// <summary>
        /// 本地数据
        /// </summary>
        public LocalInfo LocalInfo { get; set; } = new LocalInfo();
        /// <summary>
        /// 远程数据
        /// </summary>
        public RemoteInfo RemoteInfo { get; set; } = new RemoteInfo();
    }
}