using System;

namespace Common.Server.Models
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class MessageResponeInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public MessageResponeCodes Code { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ReadOnlyMemory<byte> Data { get; set; }
    }
}