namespace client.service.ui.api.Models
{
    /// <summary>
    /// 前端接口request
    /// </summary>
    public sealed class ClientServiceRequestInfo
    {
        /// <summary>
        /// 路径
        /// </summary>
        public string Path { get; set; } = string.Empty;
        /// <summary>
        /// 请求id
        /// </summary>
        public uint RequestId { get; set; } = 0;
        /// <summary>
        /// 数据
        /// </summary>
        public string Content { get; set; } = string.Empty;
    }
}