using Common.Libs;
using Common.Libs.Extends;
using System;
using System.Buffers.Binary;
using System.Diagnostics;
using System.Linq;
using System.Net;
using Client.Service.Vea.Models;
using Serilog;

namespace Client.Service.Vea.Platforms
{
    public sealed class Windows : IVeaPlatform
    {
        int interfaceNumber;
        Process tun2SocksProcess;
        const string VeaName = "p2p-tunnel";

        private readonly Models.Config config;

        public Windows(Models.Config config)
        {
            this.config = config;
        }

        public bool Run()
        {
            string command = $" -device {VeaName} -proxy socks5://127.0.0.1:{config.ListenPort} -loglevel silent";
            if (Logger.Instance.LoggerLevel <= LoggerTypes.DEBUG)
            {
                Log.Warning($"vea windows ->exec:{command}");
            }

            tun2SocksProcess = Command.Execute("tun2socks-windows.exe", command);
            try
            {
                for (int i = 0; i < 10; i++)
                {
                    System.Threading.Thread.Sleep(1000);
                    if (tun2SocksProcess.HasExited)
                    {
                        break;
                    }

                    if (GetWindowsHasInterface(VeaName) == false)
                    {
                        if (Logger.Instance.LoggerLevel <= LoggerTypes.DEBUG)
                        {
                            Log.Warning($"vea windows ->interface not dound");
                        }

                        continue;
                    }

                    interfaceNumber = GetWindowsInterfaceNum();
                    if (interfaceNumber == 0)
                    {
                        if (Logger.Instance.LoggerLevel <= LoggerTypes.DEBUG)
                        {
                            Log.Warning($"vea windows ->interface num not dound");
                        }

                        continue;
                    }

                    Command.Windows(string.Empty,
                        new string[]
                        {
                            $"netsh interface ip set address name=\"{VeaName}\" source=static addr={config.IP} mask=255.255.255.0 gateway=none"
                        });
                    for (int k = 0; k < 5; k++)
                    {
                        System.Threading.Thread.Sleep(500);
                        if (GetWindowsHasIp(config.IP) == false)
                        {
                            if (Logger.Instance.LoggerLevel <= LoggerTypes.DEBUG)
                            {
                                Log.Error($"vea windows ->set ip fail");
                            }

                            continue;
                        }

                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message + "\r\n" + ex.StackTrace);
            }

            if (interfaceNumber <= 0)
            {
                string msg = Command.Execute("tun2socks-windows.exe", command, Array.Empty<string>());
                Log.Error(msg);
            }

            return interfaceNumber > 0;
        }

        public void Kill()
        {
            interfaceNumber = 0;
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

            foreach (var item in Process.GetProcesses().Where(c => c.ProcessName.Contains("tun2socks")))
            {
                try
                {
                    item.Kill();
                }
                catch (Exception)
                {
                }
            }

            ;
        }

        public void AddRoute(VeaLanIPAddress[] ip)
        {
            if (interfaceNumber > 0)
            {
                string[] commands = ip.Where(c => c.IPAddress > 0).Select(item =>
                {
                    byte[] maskArr = BitConverter.GetBytes(BinaryPrimitives.ReverseEndianness(item.MaskValue));
                    return
                        $"route add {string.Join(".", BinaryPrimitives.ReverseEndianness(item.IPAddress).ToBytes())} mask {string.Join(".", maskArr)} {config.IP} metric 5 if {interfaceNumber}";
                }).ToArray();
                if (commands.Length > 0)
                {
                    if (Logger.Instance.LoggerLevel <= LoggerTypes.DEBUG)
                    {
                        Log.Warning(
                            $"vea windows ->add route:{string.Join(Environment.NewLine, commands)}");
                    }

                    Command.Windows(string.Empty, commands);
                }
            }
        }

        public void DelRoute(VeaLanIPAddress[] ip)
        {
            if (interfaceNumber > 0)
            {
                string[] commands = ip.Select(item =>
                        $"route delete {string.Join(".", BinaryPrimitives.ReverseEndianness(item.IPAddress).ToBytes())}")
                    .ToArray();
                if (Logger.Instance.LoggerLevel <= LoggerTypes.DEBUG)
                {
                    Log.Warning($"vea windows ->del route:{string.Join(Environment.NewLine, commands)}");
                }

                Command.Windows(string.Empty, commands.ToArray());
            }
        }


        private int GetWindowsInterfaceNum()
        {
            string output = Command.Windows(string.Empty, new string[] { "route print" });
            foreach (var item in output.Split(Environment.NewLine))
            {
                if (item.Contains("WireGuard Tunnel"))
                {
                    return int.Parse(item.Substring(0, item.IndexOf('.')).Trim());
                }
            }

            return 0;
        }

        private bool GetWindowsHasInterface(string name)
        {
            string output = Command.Windows(string.Empty, new string[] { $"ipconfig | findstr \"{name}\"" });
            return string.IsNullOrWhiteSpace(output) == false;
        }

        private bool GetWindowsHasIp(IPAddress ip)
        {
            string output = Command.Windows(string.Empty, new string[] { $"ipconfig | findstr \"{ip}\"" });
            return string.IsNullOrWhiteSpace(output) == false;
        }
    }
}