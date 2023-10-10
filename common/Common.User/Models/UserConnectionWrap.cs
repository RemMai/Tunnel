using Common.Server.Interfaces;

namespace Common.User.Models
{
    public sealed class UserConnectionWrap
    {
        public ulong LastSentBytes { get; set; }
        public IConnection Connection { get; set; }
    }
}