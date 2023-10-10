using System;
using System.ComponentModel;

namespace Common.Proxy.Enums
{
    [Flags]
    public enum EnumProxyCommandStatusMsg : byte
    {
        [Description("成功")]
        Success = 0,
        [Description("未监听")]
        Listen = 1,
        [Description("服务类型未允许")]
        Address = 2,
        [Description("与目标节点未连接")]
        Connection = 3,
        [Description("目标节点未允许通信")]
        Denied = 4,
        [Description("目标节点相应插件未找到")]
        Plugin = 5,
        [Description("目标节点相应插件未允许连接，且未拥有该权限")]
        EnableOrAccess = 6,
        [Description("目标节点防火墙阻止")]
        Firewail = 7,
        [Description("目标服务连接失败")]
        Connect = 8
    }
}