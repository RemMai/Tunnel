using Common.Libs.Extends;

namespace Client.Service.Vea.Server.Models
{
    public sealed class ModifyIPModel
    {
        public ulong ConnectionId { get; set; }
        public byte IP { get; set; }

        public byte[] ToBytes()
        {
            byte[] bytes = new byte[8 + 1];
            ConnectionId.ToBytes(bytes);
            bytes[8] = IP;
            return bytes;
        }
    }
}