using System;
using Common.Server.Attributes;

namespace Common.Server.Models
{
    /// <summary>
    /// 心跳相关消息id
    /// </summary>
    [Flags, MessengerIdEnum]
    public enum HeartMessengerIds : ushort
    {
        /// <summary>
        /// 
        /// </summary>
        Min = 300,
        /// <summary>
        /// 活着
        /// </summary>
        Alive = 301,
        Test = 302,
        /// <summary>
        /// 
        /// </summary>
        Max = 399,
    }
}
