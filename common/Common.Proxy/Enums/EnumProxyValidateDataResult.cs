using System;

namespace Common.Proxy.Enums
{
    /// <summary>
    /// 数据验证结果
    /// </summary>
    [Flags]
    public enum EnumProxyValidateDataResult : byte
    {
        Equal = 1,
        TooShort = 2,
        TooLong = 4,
        Bad = 8,
    }
}