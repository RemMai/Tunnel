using System.Threading.Tasks;

namespace Common.Server.Models
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class TimeoutState
    {
        /// <summary>
        /// 
        /// </summary>
        public uint RequestId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public TaskCompletionSource<MessageResponeInfo> Tcs { get; set; }
    }
}