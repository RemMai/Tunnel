using System;
using System.Net;
using Common.proxy.Enums;
using Common.Server;
using Common.Server.Interfaces;
using Common.Server.Models;

namespace Common.Proxy
{
    public interface IProxyPlugin : IAccess
    {
        public byte Id { get; }
        public bool ConnectEnable { get; }
        public EnumBufferSize BufferSize { get; }
        public IPAddress BroadcastBind { get; }
        public HttpHeaderCacheInfo Headers { get; set; }
        public Memory<byte> HeadersBytes { get; set; }

        /// <summary>
        /// 验证数据完整性
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public EnumProxyValidateDataResult ValidateData(ProxyInfo info);

        /// <summary>
        /// 请求数据预处理
        /// </summary>
        /// <param name="info"></param>
        /// <returns>是否发送给目标端</returns>
        public bool HandleRequestData(ProxyInfo info);

        /// <summary>
        /// 回复数据预处理
        /// </summary>
        /// <param name="info"></param>
        /// <returns>是否发送给来源端</returns>
        public bool HandleAnswerData(ProxyInfo info);

        public void Started(ushort port)
        {
        }

        public void Stoped(ushort port)
        {
        }
    }

}