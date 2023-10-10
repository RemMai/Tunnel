using client.service.ui.api.Enums;

namespace client.service.ui.api.Models
{
    /// <summary>
    /// 前端接口执行参数
    /// </summary>
    public sealed class ClientServiceParamsInfo
    {
        /// <summary>
        /// 请求id
        /// </summary>
        public uint RequestId { get; set; } = 0;
        /// <summary>
        /// 数据
        /// </summary>
        public string Content { get; set; } = string.Empty;
        /// <summary>
        /// 状态码
        /// </summary>
        public ClientServiceResponseCodes Code { get; private set; } = ClientServiceResponseCodes.Success;
        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorMessage { get; private set; } = string.Empty;

        /// <summary>
        /// 设置状态码
        /// </summary>
        /// <param name="code"></param>
        /// <param name="errormsg"></param>
        public void SetCode(ClientServiceResponseCodes code, string errormsg = "")
        {
            Code = code;
            ErrorMessage = errormsg;
        }
        /// <summary>
        /// 设置错误信息
        /// </summary>
        /// <param name="msg"></param>
        public void SetErrorMessage(string msg)
        {
            Code = ClientServiceResponseCodes.Error;
            ErrorMessage = msg;
        }
    }
}