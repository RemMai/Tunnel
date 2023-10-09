using Common.ForWard;
using Common.Proxy;
using System;
using Client.Messengers.Signin;
using Common.Libs.AutoInject.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace Client.Service.ForWard
{
    
    [AutoInject(ServiceLifetime.Singleton, typeof(IForwardProxyPlugin))]
    public class ClientForwardProxyPlugin : ForwardProxyPlugin, IForwardProxyPlugin
    {
        public override HttpHeaderCacheInfo Headers { get; set; }
        public override Memory<byte> HeadersBytes { get; set; }

        public ClientForwardProxyPlugin(Common.ForWard.Config config, Config config1, IProxyServer proxyServer, IForwardTargetProvider forwardTargetProvider
            , SignInStateInfo signInStateInfo) : base(config, proxyServer, forwardTargetProvider)
        {
            signInStateInfo.OnChange += (bool state) =>
            {
                Headers = new HttpHeaderCacheInfo
                {
                    Addr =  signInStateInfo.RemoteInfo.Ip,
                    Name = config1.Client.Name,
                    Proxy = Name
                };
            };
        }

    }
}
