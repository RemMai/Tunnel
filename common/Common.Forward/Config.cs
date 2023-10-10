using Common.Libs.Extends;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Common.ForWard.Models;
using Common.Libs.AutoInject.Attributes;
using Common.Libs.DataBase;
using Common.Server.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Common.ForWard
{
    /// <summary>
    /// tcp转发配置文件
    /// </summary>
    [Table("forward-appsettings")]
    [AutoInject(ServiceLifetime.Singleton)]
    public sealed class Config
    {
        public Config()
        {
        }

        private readonly IConfigDataProvider<Config> configDataProvider;

        public Config(IConfigDataProvider<Config> configDataProvider)
        {
            this.configDataProvider = configDataProvider;

            Config config = ReadConfig().Result;
            ConnectEnable = config.ConnectEnable;
            BufferSize = config.BufferSize;
            WebListens = config.WebListens;
            TunnelListenRange = config.TunnelListenRange;
            SaveConfig().Wait();
        }

        [JsonIgnore] public byte Plugin => 1;

        /// <summary>
        /// 允许连接
        /// </summary>
        public bool ConnectEnable { get; set; } = true;

        public EnumBufferSize BufferSize { get; set; } = EnumBufferSize.KB_8;

        /// <summary>
        /// 短连接端口
        /// </summary>
        public ushort[] WebListens { get; set; } = Array.Empty<ushort>();

        /// <summary>
        /// 域名列表
        /// </summary>
        public string[] Domains { get; set; } = Array.Empty<string>();

        /// <summary>
        /// 长链接端口范围
        /// </summary>
        public TunnelListenRangeInfo TunnelListenRange { get; set; } = new TunnelListenRangeInfo();

        /// <summary>
        /// 读取
        /// </summary>
        /// <returns></returns>
        public async Task<Config> ReadConfig()
        {
            return await configDataProvider.Load() ?? new Config();
        }

        /// <summary>
        /// 读取
        /// </summary>
        /// <returns></returns>
        public async Task<string> ReadString()
        {
            return await configDataProvider.LoadString();
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="jsonStr"></param>
        /// <returns></returns>
        public async Task SaveConfig(string jsonStr)
        {
            var _config = jsonStr.DeJson<Config>();

            ConnectEnable = _config.ConnectEnable;
            BufferSize = _config.BufferSize;
            WebListens = _config.WebListens;
            TunnelListenRange = _config.TunnelListenRange;

            await configDataProvider.Save(jsonStr).ConfigureAwait(false);
        }

        public async Task SaveConfig()
        {
            await configDataProvider.Save(this).ConfigureAwait(false);
        }
    }
}