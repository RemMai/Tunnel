using System;
using System.Collections.Generic;
using Common.Libs;

namespace Client.Service.logger.Models
{
    /// <summary>
    /// 日志分页返回
    /// </summary>
    public sealed class PageInfo
    {
        /// <summary>
        /// 当前页
        /// </summary>
        public int Page { get; set; } = 1;
        /// <summary>
        /// 页大小
        /// </summary>
        public int PageSize { get; set; } = 10;
        /// <summary>
        /// 总数
        /// </summary>
        public int Count { get; set; } = 10;
        /// <summary>
        /// 数据
        /// </summary>
        public IEnumerable<LoggerModel> Data { get; set; } = Array.Empty<LoggerModel>();
    }
}