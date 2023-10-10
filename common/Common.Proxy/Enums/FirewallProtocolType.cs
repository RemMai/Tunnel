namespace Common.Proxy.Enums
{
    public enum FirewallProtocolType : byte
    {
        TCP = 1,
        UDP = 2,
        TCP_UDP = TCP | UDP,
    }
}