namespace client.service.ui.api.Enums
{
    /// <summary>
    /// 前端接口状态码
    /// </summary>
    public enum ClientServiceResponseCodes : byte
    {
        /// <summary>
        /// 成功
        /// </summary>
        Success = 0,
        /// <summary>
        /// 没找到
        /// </summary>
        NotFound = 1,
        /// <summary>
        /// 失败
        /// </summary>
        Error = 0xff,

    }
}