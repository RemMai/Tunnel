﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Common.Extensions.AutoInject.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Client.Service.Ui.Api.Service.WebServer
{
    /// <summary>
    /// 本地web管理端服务器
    /// </summary>
    [AutoInject(ServiceLifetime.Singleton, typeof(IWebServer))]
    public sealed class WebServer : IWebServer
    {
        private readonly Config config;
        private readonly IWebServerFileReader webServerFileReader;

        public WebServer(Config config, IWebServerFileReader webServerFileReader)
        {
            this.config = config;
            this.webServerFileReader = webServerFileReader;
        }

        /// <summary>
        /// 开启web
        /// </summary>
        public void Start()
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    HttpListener http = new();
                    http.IgnoreWriteExceptions = true;
                    http.Prefixes.Add($"http://{config.Web.BindIp}:{config.Web.Port}/");
                    http.Start();
                    http.BeginGetContext(Callback, http);
                }
                catch (Exception ex)
                {
                    Log.Error($"{ex.Message}\r\n{ex.StackTrace}");
                }
            }, TaskCreationOptions.LongRunning);
        }

        private void Callback(IAsyncResult result)
        {
            HttpListener http = result.AsyncState as HttpListener;
            if (http != null)
            {
                HttpListenerContext context = http.EndGetContext(result);
                HttpListenerRequest request = context.Request;
                HttpListenerResponse response = context.Response;

                try
                {
                    response.Headers.Set("Server", "RemMai Server");
                    string path = request.Url!.AbsolutePath;
                    //默认页面
                    if (path == "/") path = "index.html";

                    byte[] bytes = webServerFileReader.Read(path, out DateTime last);
                    if (bytes.Length > 0)
                    {
                        response.ContentLength64 = bytes.Length;
                        response.ContentType = GetContentType(path);
                        response.Headers.Set("Last-Modified", last.ToString(CultureInfo.InvariantCulture));
                        response.OutputStream.Write(bytes, 0, bytes.Length);
                        response.OutputStream.Flush();
                        response.OutputStream.Close();
                    }
                    else
                    {
                        response.StatusCode = (int)HttpStatusCode.NotFound;
                    }
                }
                catch (Exception)
                {
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                }

                response.Close();
            }

            http?.BeginGetContext(Callback, http);
        }


        private readonly Dictionary<string, string> types = new()
        {
            { ".webp", "image/webp" },
            { ".png", "image/png" },
            { ".jpg", "image/jpg" },
            { ".jpeg", "image/jpeg" },
            { ".gif", "image/gif" },
            { ".svg", "image/svg+xml" },
            { ".ico", "image/x-icon" },
            { ".js", "text/javascript; charset=utf-8" },
            { ".html", "text/html; charset=utf-8" },
            { ".css", "text/css; charset=utf-8" },
            { ".pac", "application/x-ns-proxy-autoconfig; charset=utf-8" },
        };

        private string GetContentType(string path)
        {
            string ext = Path.GetExtension(path);
            return types.TryGetValue(ext, out string type) ? type : "application/octet-stream";
        }
    }
}