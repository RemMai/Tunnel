using Common.Server.Interfaces;

namespace Common.Vea.Implementations
{
    public sealed class VeaAccessValidateResult
    {
        public string Key;
        public string Name;
        public IConnection Connection;
    }
}