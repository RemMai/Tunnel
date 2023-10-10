using System.Text.Json.Serialization;

namespace Client.Service.Users
{
    public sealed class UserMapInfo
    {
        public ulong ID { get; set; }
        public uint Access { get; set; }
        [JsonIgnore]
        public ulong  ConnectionId { get; set; }
    }
}