namespace client.service.logger.Models
{
    /// <summary>
    /// 日志分页参数
    /// </summary>
    public sealed class PageParamsInfo
    {
        /// <summary>
        /// 当前页
        /// </summary>
        public int Page { get; set; } = 1;
        /// <summary>
        /// 页大小
        /// </summary>
        public int PageSize { get; set; } = 10;
    }
}