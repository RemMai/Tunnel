using System;

namespace Common.Socks5.Enums
{
    [Flags]
    public enum Socks5MessengerIds : ushort
    {
        Min = 800,
        GetSetting = 804,
        Setting = 805,
        Max = 899,
    }
}