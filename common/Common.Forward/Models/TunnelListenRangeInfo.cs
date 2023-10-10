namespace Common.ForWard.Models
{
    /// <summary>
    /// 长链接端口范围
    /// </summary>
    public sealed class TunnelListenRangeInfo
    {
        /// <summary>
        /// 最小
        /// </summary>
        public ushort Min { get; set; } = 10000;

        /// <summary>
        /// 最大
        /// </summary>
        public ushort Max { get; set; } = 60000;
    }

}