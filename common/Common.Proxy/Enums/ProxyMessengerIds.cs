using System;

namespace Common.Proxy.Enums
{
    /// <summary>
    /// 代理消息编号
    /// </summary>
    [Flags]
    public enum ProxyMessengerIds : ushort
    {
        Min = 900,
        Request = 901,
        Response = 902,
        GetFirewall = 903,
        AddFirewall = 904,
        RemoveFirewall = 905,
        Test = 906,
        Max = 999,
    }
}