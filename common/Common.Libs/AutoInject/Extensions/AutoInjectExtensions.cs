using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Common.Libs.AutoInject.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Common.Libs.AutoInject.Extensions;

public static class AutoInjectExtensions
{
    /// <summary>
    /// 自动注册标识的服务
    /// </summary>
    /// <param name="services"></param>
    /// <param name="assemblies"></param>
    /// <returns></returns>
    public static IServiceCollection AddAutoInject(this IServiceCollection services, IEnumerable<Assembly> assemblies)
    {
        var allTypes = assemblies
            .SelectMany(assembly => assembly.GetTypes().Where(type =>
                type is { IsClass: true, IsAbstract: false }
                && type.IsDefined(typeof(AutoInjectAttribute), false)
            ))
            .ToList();

        Log.Information("服务自动注入中...");
        foreach (var type in allTypes)
        {
            var attribute = type.GetCustomAttribute<AutoInjectAttribute>()!;
            var interfaceTypes = attribute.ImplementationInterfaces;
            var hasInterface = interfaceTypes.Count > 0;

            switch (attribute.Life)
            {
                case ServiceLifetime.Scoped:
                {
                    if (hasInterface)
                    {
                        foreach (Type interfaceType in interfaceTypes)
                        {
                            services.AddScoped(interfaceType, type);
                        }
                    }
                    else
                    {
                        services.AddScoped(type);
                    }
                }
                    break;
                case ServiceLifetime.Singleton:
                {
                    if (hasInterface)
                    {
                        foreach (Type interfaceType in interfaceTypes)
                        {
                            services.AddSingleton(interfaceType, type);
                        }
                    }
                    else
                    {
                        services.AddSingleton(type);
                    }
                }
                    break;
                case ServiceLifetime.Transient:
                {
                    if (hasInterface)
                    {
                        foreach (Type interfaceType in interfaceTypes)
                        {
                            services.AddTransient(interfaceType, type);
                        }
                    }
                    else
                    {
                        services.AddTransient(type);
                    }
                }
                    break;
                default:
                    throw new Exception($"{type.Name} 自动注入失败，请检查代码");
            }
        }

        return services;
    }
}