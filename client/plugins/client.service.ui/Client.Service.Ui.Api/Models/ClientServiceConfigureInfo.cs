namespace client.service.ui.api.Models
{
    /// <summary>
    /// 前端配置服务信息
    /// </summary>
    public sealed class ClientServiceConfigureInfo
    {
        /// <summary>
        /// 名字
        /// </summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// 类名
        /// </summary>
        public string ClassName { get; set; } = string.Empty;
        /// <summary>
        /// 作者
        /// </summary>
        public string Author { get; set; } = string.Empty;
        /// <summary>
        /// 描述
        /// </summary>
        public string Desc { get; set; } = string.Empty;
        /// <summary>
        /// 启用
        /// </summary>
        public bool Enable { get; set; } = false;
    }
}