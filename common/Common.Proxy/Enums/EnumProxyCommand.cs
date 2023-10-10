namespace Common.Proxy.Enums
{
    /// <summary>
    /// 请求指令 2位
    /// </summary>
    public enum EnumProxyCommand : byte
    {
        /// <summary>
        /// 连接上游服务器
        /// </summary>
        Connect = 1,
        /// <summary>
        /// 绑定，客户端会接收来自代理服务器的链接，著名的FTP被动模式
        /// </summary>
        Bind = 2,
        /// <summary>
        /// UDP中继
        /// </summary>
        UdpAssociate = 3
    }
}