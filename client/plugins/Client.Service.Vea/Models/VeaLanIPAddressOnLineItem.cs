namespace Client.Service.Vea.Models
{
    /// <summary>
    /// 局域网在线设备
    /// </summary>
    public sealed class VeaLanIPAddressOnLineItem
    {
        public bool Online { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}