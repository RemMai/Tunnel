using Common.Server;

namespace Common.Vea
{
    public interface IVeaAccessValidator
    {
        public bool Validate(ulong connectionId,out VeaAccessValidateResult result);  
    }

    public sealed class VeaAccessValidateResult
    {
        public string Key;
        public string Name;
        public IConnection Connection;
    }
}
