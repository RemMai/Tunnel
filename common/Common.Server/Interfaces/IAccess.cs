namespace Common.Server.Interfaces
{
    public interface IAccess
    {
        /// <summary>
        /// 权限
        /// </summary>
        public uint Access { get; }

        /// <summary>
        /// 名字
        /// </summary>
        public string Name { get; }
    }
}