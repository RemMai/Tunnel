using Common.Vea.Implementations;

namespace Common.Vea.Interfaces
{
    public interface IVeaAccessValidator
    {
        public bool Validate(ulong connectionId,out VeaAccessValidateResult result);  
    }
}
