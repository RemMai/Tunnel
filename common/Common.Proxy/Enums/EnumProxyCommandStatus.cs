namespace Common.Proxy.Enums
{
    /// <summary>
    /// 请求的回复数据的指令
    /// </summary>
    public enum EnumProxyCommandStatus : byte
    {
        /// <summary>
        /// 代理服务器连接目标服务器成功
        /// </summary>
        ConnecSuccess = 0,
        /// <summary>
        /// 代理服务器故障
        /// </summary>
        ServerError = 1,
        /// <summary>
        /// 代理服务器规则集不允许连接
        /// </summary>
        ConnectNotAllow = 2,
        /// <summary>
        /// 网络无法访问
        /// </summary>
        NetworkError = 3,
        /// <summary>
        /// 目标服务器无法访问（主机名无效）
        /// </summary>
        ConnectFail = 4,
        /// <summary>
        /// 连接目标服务器被拒绝
        /// </summary>
        DistReject = 5,
        /// <summary>
        /// TTL已过期
        /// </summary>
        TTLTimeout = 6,
        /// <summary>
        /// 不支持的命令
        /// </summary>
        CommandNotAllow = 7,
        /// <summary>
        /// 不支持的目标服务器地址类型
        /// </summary>
        AddressNotAllow = 8,
        /// <summary>
        /// 未分配
        /// </summary>
        Unknow = 8,
    }
}