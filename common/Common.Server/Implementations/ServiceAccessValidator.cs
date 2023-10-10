using Common.Server;
using Common.Server.Interfaces;

namespace Common.Server.Implementations
{
    public class ServiceAccessValidator : IServiceAccessValidator
    {

        public ServiceAccessValidator()
        {
        }

        public virtual bool Validate(ulong connectionid, uint service)
        {
            return false;
        }
        public bool Validate(uint access, uint service)
        {
            return (access & service) == service;
        }
    }
}