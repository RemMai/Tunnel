using System.Runtime.InteropServices;
using Common.Libs.AutoInject.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace Client.Service.Vea.Platforms
{
    [AutoInject(ServiceLifetime.Singleton, typeof(IVeaPlatform))]
    public class VeaPlatform : IVeaPlatform
    {
        private IVeaPlatform veaPlatform { get; set; }

        private VeaPlatform(Config config)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                veaPlatform = new Windows(config);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                veaPlatform = new MacOs(config);
            }
            else
            {
                veaPlatform = new Linux(config);
            }
        }


        public bool Run() => veaPlatform.Run();
        public void Kill() => veaPlatform.Kill();
        public void AddRoute(VeaLanIPAddress[] ip) => veaPlatform.AddRoute(ip);
        public void DelRoute(VeaLanIPAddress[] ip) => veaPlatform.DelRoute(ip);
    }
}