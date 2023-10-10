namespace Common.Server.Interfaces
{
    /// <summary>
    /// 中继权限验证
    /// </summary>
    public interface IRelayValidator
    {
        public bool Validate(IConnection connection);
    }
}
