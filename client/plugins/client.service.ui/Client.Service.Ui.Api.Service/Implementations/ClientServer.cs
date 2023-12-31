﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Client.Service.Ui.Api;
using Client.Service.Ui.api.Enums;
using Client.Service.Ui.api.Interfaces;
using Client.Service.Ui.api.Models;
using Client.Service.Ui.api.service.Models;
using Common.Extensions.AutoInject.Attributes;
using Common.Libs.Extends;
using Common.Server.Servers.pipeLine;
using Common.Server.Servers.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Client.Service.Ui.Api.Service.Implementations
{
    /// <summary>
    /// 前端接口服务
    /// </summary>
    [AutoInject(ServiceLifetime.Singleton, typeof(IClientServer))]
    public sealed class ClientServer : IClientServer
    {
        private readonly Dictionary<string, PluginPathCacheInfo> plugins = new();
        private readonly Dictionary<string, IClientConfigure> settingPlugins = new();

        private readonly Config config;
        private readonly IServiceProvider _serviceProvider;
        private WebSocketServer server;

        public ClientServer(Config config, IServiceScopeFactory serviceScopeFactory)
        {
            this.config = config;
            _serviceProvider = serviceScopeFactory.CreateScope().ServiceProvider;
        }

        /// <summary>
        /// 加载插件
        /// </summary>
        public void LoadPlugins()
        {
            Type voidType = typeof(void);
            foreach (var service in _serviceProvider.GetServices<IClientService>())
            {
                var item = service.GetType();
                string path = item.Name.Replace("ClientService", "");
                object obj = _serviceProvider.GetService(item);
                foreach (MethodInfo method in item.GetMethods(BindingFlags.Instance | BindingFlags.Public |
                                                              BindingFlags.DeclaredOnly))
                {
                    string key = $"{path}/{method.Name}".ToLower();
                    if (!plugins.ContainsKey(key))
                    {
                        plugins.TryAdd(key, new PluginPathCacheInfo
                        {
                            IsVoid = method.ReturnType == voidType,
                            Method = method,
                            Target = obj,
                            IsTask = method.ReturnType.GetProperty("IsCompleted") != null &&
                                     method.ReturnType.GetMethod("GetAwaiter") != null,
                            IsTaskResult = method.ReturnType.GetProperty("Result") != null
                        });
                    }
                }
            }

            foreach (var item in _serviceProvider.GetServices<IClientConfigure>())
            {
                settingPlugins.TryAdd(item.GetType().Name, item);
            }
        }

        /// <summary>
        /// 开启WebSocket
        /// </summary>
        public void WebSocket()
        {
            server = new WebSocketServer();
            try
            {
                server.Start(config.Websocket.BindIp, config.Websocket.Port);
            }
            catch (Exception ex)
            {
                Log.Error($"{ex.Message}\r\n{ex.StackTrace}");
            }

            server.OnMessage = (connection, frame, message) =>
            {
                var req = message.DeJson<ClientServiceRequestInfo>();
                OnMessage(req).ContinueWith(result =>
                {
                    var resp = result.Result.ToJson().ToBytes();
                    connection.SendFrameText(resp);
                });
            };
        }

        /// <summary>
        /// 获取插件配置列表
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        public IClientConfigure GetConfigure(string className)
        {
            settingPlugins.TryGetValue(className, out IClientConfigure plugin);
            return plugin;
        }

        /// <summary>
        /// 获取某个插件配置
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ClientServiceConfigureInfo> GetConfigures()
        {
            return settingPlugins.Select(c => new ClientServiceConfigureInfo
            {
                Name = c.Value.Name,
                Author = c.Value.Author,
                Desc = c.Value.Desc,
                ClassName = c.Value.GetType().Name,
                Enable = c.Value.Enable
            });
        }

        /// <summary>
        /// 获取服务列表
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetServices()
        {
            return plugins.Select(c => c.Value.Target.GetType().Name).Distinct();
        }

        /// <summary>
        /// 发送通知
        /// </summary>
        /// <param name="resp"></param>
        public void Notify(ClientServiceResponseInfo resp)
        {
            byte[] msg = resp.ToJson().ToBytes();
            foreach (var item in server.Connections)
            {
                item.SendFrameText(msg);
            }
        }

        /// <summary>
        /// 收到消息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ClientServiceResponseInfo> OnMessage(ClientServiceRequestInfo model)
        {
            model.Path = model.Path.ToLower();
            if (plugins.ContainsKey(model.Path) == false)
            {
                return new ClientServiceResponseInfo
                {
                    Content = "not exists this path",
                    RequestId = model.RequestId,
                    Path = model.Path,
                    Code = ClientServiceResponseCodes.NotFound
                };
            }

            PluginPathCacheInfo plugin = plugins[model.Path];
            try
            {
                ClientServiceParamsInfo param = new()
                {
                    RequestId = model.RequestId,
                    Content = model.Content
                };
                dynamic resultAsync = plugin.Method.Invoke(plugin.Target, new object[] { param });
                object resultObject = null;
                if (plugin.IsVoid == false)
                {
                    if (plugin.IsTask)
                    {
                        await resultAsync.ConfigureAwait(false);
                        if (plugin.IsTaskResult)
                        {
                            resultObject = resultAsync.Result;
                        }
                    }
                    else
                    {
                        resultObject = resultAsync;
                    }
                }

                return new ClientServiceResponseInfo
                {
                    Code = param.Code,
                    Content = param.Code != ClientServiceResponseCodes.Error ? resultObject : param.ErrorMessage,
                    RequestId = model.RequestId,
                    Path = model.Path,
                };
            }
            catch (Exception ex)
            {
                Log.Error($"{ex.Message}\r\n{ex.StackTrace}");
                return new ClientServiceResponseInfo
                {
                    Content = ex.Message,
                    RequestId = model.RequestId,
                    Path = model.Path,
                    Code = ClientServiceResponseCodes.Error
                };
            }
        }


        private const string PipeName = "Client.cmd";

        /// <summary>
        /// 开启具名管道
        /// </summary>
        public void NamedPipe()
        {
            PipelineServer pipelineServer = new(PipeName,
                message => OnMessage(message.DeJson<ClientServiceRequestInfo>()).Result.ToJson());
            pipelineServer.BeginAccept();
        }
    }
}