using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Libs.AutoInject.Attributes;

/// <summary>
/// life: <br /> <para>1: Scoped (default) </para> <para>2: Singleton</para><para>3: Transient</para>
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class AutoInjectAttribute : Attribute
{
    /// <summary>
    /// 生命周期
    /// </summary>
    public ServiceLifetime Life { get; }

    /// <summary>
    /// 实现接口
    /// </summary>
    public List<Type> ImplementationInterfaces { get; } = new();

    public bool InjectSelf { get; set; } = false;


    /// <summary>
    /// AutoInject , Custom definition 
    /// </summary>
    /// <param name="life"> <para>1: Scoped </para> <para>2: Singleton</para><para>3: Transient</para> </param>
    /// <param name="injectSelf"></param>
    /// <param name="implementationInterface"></param>
    public AutoInjectAttribute(ServiceLifetime life, bool injectSelf, params Type[] implementationInterface)
    {
        Life = life;
        InjectSelf = injectSelf;
        ImplementationInterfaces.AddRange(implementationInterface);
    }

    public AutoInjectAttribute(ServiceLifetime life, params Type[] implementationInterface)
    {
        Life = life;
        ImplementationInterfaces.AddRange(implementationInterface);
    }


    /// <summary>
    ///  AutoInject, default Scoped
    /// </summary>
    /// <param name="implementationInterface"></param>
    public AutoInjectAttribute(bool injectSelf, Type implementationInterface)
    {
        Life = ServiceLifetime.Scoped;
        InjectSelf = injectSelf;
        ImplementationInterfaces.Add(implementationInterface);
    }

    /// <summary>
    ///  AutoInject, default Scoped
    /// </summary>
    /// <param name="implementationInterfaces"></param>
    public AutoInjectAttribute(bool injectSelf, params Type[] implementationInterfaces)
    {
        Life = ServiceLifetime.Scoped;
        InjectSelf = injectSelf;
        ImplementationInterfaces.AddRange(implementationInterfaces);
    }

    public AutoInjectAttribute(ServiceLifetime life)
    {
        Life = life;
    }

    public AutoInjectAttribute()
    {
        Life = ServiceLifetime.Scoped;
    }
}