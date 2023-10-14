namespace Client.Service.Ui.api.service.Models
{
    /// <summary>
    /// 保存
    /// </summary>
    public sealed class SaveParamsInfo
    {
        /// <summary>
        /// 类名
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// 配置内容
        /// </summary>
        public string Content { get; set; }
    }
}