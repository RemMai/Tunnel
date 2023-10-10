namespace Common.Socks5.Enums
{
    /// <summary>
    /// socks5的认证状态0成功 其它失败
    /// </summary>
    public enum Socks5EnumAuthState : byte
    {
        Success = 0x00,
        UnKnow = 0xff,
    }
}