using System;

namespace Common.Server.Enums
{
    [Flags]
    public enum EnumServiceAccess : uint
    {
        None = 0b00000000_00000000_00000000_00000000,
        /// <summary>
        /// 中继
        /// </summary>
        Relay = 0b00000000_00000000_00000000_00000001,
        /// <summary>
        /// 配置
        /// </summary>
        Setting = 0b00000000_00000000_00000000_00000010,
        /// <summary>
        /// 全部权限
        /// </summary>
        All = uint.MaxValue
    }
}