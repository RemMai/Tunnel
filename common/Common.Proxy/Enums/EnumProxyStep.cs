namespace Common.Proxy.Enums
{
    /// <summary>
    /// 当前处于协议的哪一步， 2位
    /// </summary>
    public enum EnumProxyStep : byte
    {
        Command = 1,
        /// <summary>
        /// TCP转发
        /// </summary>
        ForwardTcp = 2,
        /// <summary>
        /// UDP转发
        /// </summary>
        ForwardUdp = 3
    }
}