using System;
using System.ComponentModel;

namespace Common.ForWard.Enums
{
    [Flags]
    public enum ForwardAliveTypes : byte
    {
        [Description("长连接")] Tunnel = 0,
        [Description("短连接")] Web = 1
    }
}