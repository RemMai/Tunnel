using System;

namespace client.service.ui.api.Attributes;

/// <summary>
/// 前端接口标识特性
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class ClientServiceAttribute : Attribute
{
    /// <summary>
    /// 参数类型
    /// </summary>
    public Type Param { get; set; }
    public ClientServiceAttribute(Type param)
    {
        Param = param;
    }
}