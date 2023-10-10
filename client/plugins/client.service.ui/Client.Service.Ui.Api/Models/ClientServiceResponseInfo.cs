using client.service.ui.api.Enums;

namespace client.service.ui.api.Models
{
    /// <summary>
    /// 前段接口response
    /// </summary>
    public sealed class ClientServiceResponseInfo
    {
        /// <summary>
        /// 路径
        /// </summary>
        public string Path { get; set; } = string.Empty;
        /// <summary>
        /// 请求id
        /// </summary>
        public long RequestId { get; set; } = 0;
        /// <summary>
        /// 状态码
        /// </summary>
        public ClientServiceResponseCodes Code { get; set; } = ClientServiceResponseCodes.Success;
        /// <summary>
        /// 数据
        /// </summary>
        public object Content { get; set; } = string.Empty;
    }
}