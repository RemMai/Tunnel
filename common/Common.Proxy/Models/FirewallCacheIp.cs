using System.Runtime.InteropServices;

namespace Common.Proxy
{

    [StructLayout(LayoutKind.Explicit)]
    public struct FirewallCacheIp
    {
        [FieldOffset(0)]
        public ulong Value;
        [FieldOffset(0)]
        public uint MaskValue;
        [FieldOffset(4)]
        public uint NetWork;
        public FirewallCacheIp(ulong value)
        {
            Value = value;
        }
        public FirewallCacheIp(uint maskValue, uint netWork)
        {
            MaskValue = maskValue;
            NetWork = netWork;
        }
    }
}