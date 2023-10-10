namespace Common.Server.Interfaces
{

    public interface IServiceAccessValidator
    {
        /// <summary>
        /// 验证权限
        /// </summary>
        /// <param name="connectionid">连接id</param>
        /// <param name="service">验证哪个权限</param>
        /// <returns></returns>
        public bool Validate(ulong connectionid, uint service);
        /// <summary>
        /// 验证权限
        /// </summary>
        /// <param name="access">你有的权限</param>
        /// <param name="service">验证哪个权限</param>
        /// <returns></returns>
        public bool Validate(uint access, uint service);

    }
}
