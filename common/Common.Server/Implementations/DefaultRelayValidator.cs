using Common.Server;
using Common.Server.Interfaces;

namespace Common.Server.Implementations
{
    /// <summary>
    /// 默认的验证
    /// </summary>
    public sealed class DefaultRelayValidator : IRelayValidator
    {
        public DefaultRelayValidator()
        {
        }
        public bool Validate(IConnection connection)
        {
            return true;
        }
    }
}