namespace Client.Service.Vea.Platforms
{
    public interface IVeaPlatform
    {
        public bool Run();
        public void Kill();
        public void AddRoute(VeaLanIPAddress[] ip);
        public void DelRoute(VeaLanIPAddress[] ip);
    }
}