using System;

namespace Common.Server.Attributes
{
    /// <summary>
    /// 消息
    /// </summary>

    [AttributeUsage(AttributeTargets.Enum)]
    public sealed class MessengerIdEnumAttribute : Attribute
    {
    }
}