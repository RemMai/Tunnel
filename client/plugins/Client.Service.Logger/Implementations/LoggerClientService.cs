using System.Collections.Generic;
using System.Linq;
using Client.Service.Logger;
using Client.Service.logger.Models;
using Client.Service.Ui.api.Interfaces;
using Client.Service.Ui.api.Models;
using Common.Extensions.AutoInject.Attributes;
using Common.Libs;
using Common.Libs.Extends;
using Microsoft.Extensions.DependencyInjection;

namespace Client.Service.Logger.Implementations
{
    /// <summary>
    /// 日志
    /// </summary>
    [AutoInject(ServiceLifetime.Singleton)]
    public sealed class LoggerClientService : IClientService
    {
        /// <summary>
        /// 日志
        /// </summary>
        public List<LoggerModel> Data { get; } = new();

        /// <summary>
        /// 获取日志列表
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public PageInfo List(ClientServiceParamsInfo arg)
        {
            PageParamsInfo model = arg.Content.DeJson<PageParamsInfo>();

            IEnumerable<LoggerModel> res = Data.OrderByDescending(c => c.Time);

            return new PageInfo
            {
                Page = model.Page,
                PageSize = model.PageSize,
                Count = Data.Count(),
                Data = res.Skip((model.Page - 1) * model.PageSize).Take(model.PageSize)
            };
        }

        /// <summary>
        /// 清除日志
        /// </summary>
        /// <param name="arg"></param>
        public void Clear(ClientServiceParamsInfo arg)
        {
            Data.Clear();
        }
    }
}
