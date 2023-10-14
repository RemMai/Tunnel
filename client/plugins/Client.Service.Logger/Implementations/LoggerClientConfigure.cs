using System.Threading.Tasks;
using Client.Service.Logger;
using Client.Service.logger.Models;
using Client.Service.Ui.api.Interfaces;
using Common.Extensions.AutoInject.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace Client.Service.Logger.Implementations
{
    /// <summary>
    /// 日志配置文件
    /// </summary>
    [AutoInject(ServiceLifetime.Singleton)]
    public sealed class LoggerClientConfigure : IClientConfigure
    {
        private readonly Client.Service.logger.Models.Config config;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        public LoggerClientConfigure(Client.Service.logger.Models.Config config)
        {
            this.config = config;
        }

        /// <summary>
        /// 名字
        /// </summary>
        public string Name => "日志信息";
        /// <summary>
        /// 作者
        /// </summary>
        public string Author => "snltty";
        /// <summary>
        /// 描述
        /// </summary>
        public string Desc => "收集日志输出到前端";
        /// <summary>
        /// 启用
        /// </summary>
        public bool Enable => config.Enable;
        /// <summary>
        /// 读取
        /// </summary>
        /// <returns></returns>
        public async Task<string> Load()
        {
            return await config.ReadString();
        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="jsonStr"></param>
        /// <returns></returns>
        public async Task<bool> Save(string jsonStr)
        {
            await config.SaveConfig(jsonStr).ConfigureAwait(false);
            return true;
        }
    }

}
