using System;
using System.Net;
using System.Net.Sockets;
using Common.Libs;
using Common.Libs.Extends;

namespace Common.Proxy
{

    public sealed class UdpToken
    {
        public ConnectionKeyUdp Key { get; set; }
        public Socket TargetSocket { get; set; }
        public ProxyInfo Data { get; set; }
        public byte[] PoolBuffer { get; set; }
        public long LastTime { get; set; } = DateTimeHelper.GetTimeStamp();
        public EndPoint TempRemoteEP = new IPEndPoint(IPAddress.Any, IPEndPoint.MinPort);
        public EndPoint TargetEP = new IPEndPoint(IPAddress.Any, IPEndPoint.MinPort);
        public void Clear()
        {
            TargetSocket?.SafeClose();
            PoolBuffer = Helper.EmptyArray;
            GC.Collect();
            GC.SuppressFinalize(this);
        }
        public void Update()
        {
            LastTime = DateTimeHelper.GetTimeStamp();
        }
    }
}