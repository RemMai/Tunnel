using System;

namespace Common.Server.Attributes
{
    /// <summary>
    /// 消息id范围
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class MessengerIdRangeAttribute : Attribute
    {
        /// <summary>
        /// 最小
        /// </summary>
        public ushort Min { get; set; }
        /// <summary>
        /// 最大
        /// </summary>
        public ushort Max { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public MessengerIdRangeAttribute(ushort min, ushort max)
        {
            Min = min;
            Max = max;
        }
    }
}