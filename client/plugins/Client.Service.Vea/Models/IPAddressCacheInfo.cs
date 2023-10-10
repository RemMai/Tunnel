using Client.Messengers.Clients;

namespace Client.Service.Vea.Models
{
    /// <summary>
    /// ip缓存
    /// </summary>
    public sealed class IPAddressCacheInfo
    {
        /// <summary>
        /// ip 小端
        /// </summary>
        public uint IP { get; set; }
        /// <summary>
        /// 局域网网段
        /// </summary>
        public VeaLanIPAddress[] LanIPs { get; set; }
        /// <summary>
        /// 客户端
        /// </summary>

        [System.Text.Json.Serialization.JsonIgnore]
        public ClientInfo Client { get; set; }

        /// <summary>
        /// 网络号，小端
        /// </summary>
        public uint NetWork { get; set; }
        /// <summary>
        /// 掩码长度 24 16 8什么的
        /// </summary>
        public byte MaskLength { get; set; }
        /// <summary>
        /// 掩码具体值 24就是 0xffffff00
        /// </summary>
        public uint MaskValue { get; set; }
    }
}