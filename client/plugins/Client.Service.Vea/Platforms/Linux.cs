using Common.Libs;
using Common.Libs.Extends;
using System;
using System.Buffers.Binary;
using System.Diagnostics;
using System.Linq;
using Client.Service.Vea.Models;
using Serilog;

namespace Client.Service.Vea.Platforms
{
    public sealed class Linux : IVeaPlatform
    {
        string interfaceLinux = string.Empty;
        Process tun2SocksProcess;
        const string VeaName = "p2p-tunnel";

        private readonly Models.Config config;
        public Linux(Models.Config config)
        {
            this.config = config;
        }

        public bool Run()
        {
            Command.Linux(string.Empty, new string[] {
                $"ip tuntap add mode tun dev {VeaName}",
                $"ip addr add {config.IP}/24 dev {VeaName}",
                $"ip link set dev {VeaName} up"
            });

            string str = Command.Linux(string.Empty, new string[] { $"ifconfig" });
            if (str.Contains(VeaName) == false)
            {
                string msg = Command.Linux(string.Empty, new string[] { $"ip tuntap add mode tun dev {VeaName}" });
                Log.Error(msg);
                return false;
            }

            interfaceLinux = GetLinuxInterfaceNum();
            try
            {
                string command = $" -device {VeaName} -proxy socks5://127.0.0.1:{config.ListenPort} -interface {interfaceLinux} -loglevel silent";
                if (Logger.Instance.LoggerLevel <= LoggerTypes.DEBUG)
                {
                    Log.Warning($"vea linux ->exec:{command}");
                }
                tun2SocksProcess = Command.Execute("./tun2socks-linux", command);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return false;
            }

            return string.IsNullOrWhiteSpace(interfaceLinux) == false;
        }
        public void Kill()
        {
            if (tun2SocksProcess != null)
            {
                try
                {
                    tun2SocksProcess.Kill();
                }
                catch (Exception)
                {
                }
                tun2SocksProcess = null;
            }

            Command.Linux(string.Empty, new string[] { $"ip tuntap del mode tun dev {VeaName}" });
        }
        public void AddRoute(VeaLanIPAddress[] ip)
        {
            string[] commands = ip.Where(c => c.IPAddress > 0).Select(item =>
            {
                return $"ip route add {string.Join(".", BinaryPrimitives.ReverseEndianness(item.IPAddress).ToBytes())}/{item.MaskLength} via {config.IP} dev {VeaName} metric 1 ";
            }).ToArray();
            if (commands.Length > 0)
            {
                if (Logger.Instance.LoggerLevel <= LoggerTypes.DEBUG)
                {
                    Log.Warning($"vea linux ->add route:{string.Join(Environment.NewLine, commands)}");
                }
                Command.Linux(string.Empty, commands);
            }
        }
        public void DelRoute(VeaLanIPAddress[] ip)
        {
            string[] commands = ip.Select(item =>
            {
                return $"ip route del {string.Join(".", BinaryPrimitives.ReverseEndianness(item.IPAddress).ToBytes())}/{item.MaskLength}";
            }).ToArray();

            if (Logger.Instance.LoggerLevel <= LoggerTypes.DEBUG)
            {
                Log.Warning($"vea linux ->del route:{string.Join(Environment.NewLine, commands)}");
            }
            Command.Linux(string.Empty, commands);
        }

        private string GetLinuxInterfaceNum()
        {
            string output = Command.Linux(string.Empty, new string[] { "ip route" });
            foreach (var item in output.Split(Environment.NewLine))
            {
                if (item.StartsWith("default via"))
                {
                    var strs = item.Split(Helper.SeparatorCharSpace);
                    for (int i = 0; i < strs.Length; i++)
                    {
                        if (strs[i] == "dev")
                        {
                            return strs[i + 1];
                        }
                    }
                }
            }
            return string.Empty;
        }
    }
}
