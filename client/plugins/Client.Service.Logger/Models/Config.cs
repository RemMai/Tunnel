﻿using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using Common.Extensions.AutoInject.Attributes;
using Common.Libs;
using Common.Libs.DataBase;
using Common.Libs.Extends;
using Microsoft.Extensions.DependencyInjection;

namespace Client.Service.logger.Models
{
    /// <summary>
    /// 日志配置文件
    /// </summary>
    [Table("logger-appsettings")]
    [AutoInject(ServiceLifetime.Singleton)]
    public sealed class Config
    {
        public Config() { }
        private readonly IConfigDataProvider<Config> configDataProvider;
        public Config(IConfigDataProvider<Config> configDataProvider)
        {
            this.configDataProvider = configDataProvider;

            Config config = ReadConfig().Result;
            Enable = config.Enable;
            MaxLength = config.MaxLength;
            SaveConfig().Wait();
        }


        /// <summary>
        /// 开启
        /// </summary>
        public bool Enable { get; set; } = true;
        /// <summary>
        /// 行数
        /// </summary>
        public int MaxLength { get; set; } = 100;
#if DEBUG
        public LoggerTypes LoggerLevel { get; set; } = LoggerTypes.DEBUG;
#else
        public LoggerTypes LoggerLevel { get; set; } = LoggerTypes.WARNING;
#endif

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
            Config config = jsonStr.DeJson<Config>();

            Enable = config.Enable;
            MaxLength = config.MaxLength;
            LoggerLevel = config.LoggerLevel;
            Common.Libs.Logger.Instance.LoggerLevel = LoggerLevel;
            await configDataProvider.Save(jsonStr).ConfigureAwait(false);
        }
        public async Task SaveConfig()
        {
            Common.Libs.Logger.Instance.LoggerLevel = LoggerLevel;
            await configDataProvider.Save(this).ConfigureAwait(false);
        }
    }
}
