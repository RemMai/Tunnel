using System;
using System.Threading.Tasks;
using Client.Messengers.Signin;
using client.service.ui.api.Interfaces;
using client.service.ui.api.Models;
using Common.Extensions.AutoInject.Attributes;
using Common.Libs;
using Common.Libs.Extends;
using Common.Proxy;
using Common.Proxy.Enums;
using Common.Server;
using Common.Server.Implementations;
using Common.Server.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Client.Service.Proxy.Implementations
{
    /// <summary>
    /// proxy
    /// </summary>
    [AutoInject(ServiceLifetime.Singleton)]
    public sealed class ServerProxyClientService : IClientService
    {
        private readonly SignInStateInfo signInStateInfo;
        private readonly MessengerSender messengerSender;
        public ServerProxyClientService(SignInStateInfo signInStateInfo, MessengerSender messengerSender)
        {
            this.signInStateInfo = signInStateInfo;
            this.messengerSender = messengerSender;
        }

        public async Task<Common.Proxy.Config> Get(ClientServiceParamsInfo arg)
        {
            MessageResponeInfo resp = await messengerSender.SendReply(new MessageRequestWrap
            {
                Connection = signInStateInfo.Connection,
                MessengerId = (ushort)ProxyMessengerIds.GetFirewall,
            });
            if (resp.Code == MessageResponeCodes.OK)
            {
                return resp.Data.GetUTF8String().DeJson<Common.Proxy.Config>();
            }
            return new Common.Proxy.Config();
        }

        public async Task<bool> Add(ClientServiceParamsInfo arg)
        {
            MessageResponeInfo resp = await messengerSender.SendReply(new MessageRequestWrap
            {
                Connection = signInStateInfo.Connection,
                MessengerId = (ushort)ProxyMessengerIds.AddFirewall,
                Payload = arg.Content.ToUTF8Bytes()
            });
            if (resp.Code == MessageResponeCodes.OK && resp.Data.Span.SequenceEqual(Helper.TrueArray))
            {
                return true;
            }
            return false;
        }

        public async Task<bool> Remove(ClientServiceParamsInfo arg)
        {
            MessageResponeInfo resp = await messengerSender.SendReply(new MessageRequestWrap
            {
                Connection = signInStateInfo.Connection,
                MessengerId = (ushort)ProxyMessengerIds.RemoveFirewall,
                Payload = uint.Parse(arg.Content).ToBytes(),
            });
            if (resp.Code == MessageResponeCodes.OK && resp.Data.Span.SequenceEqual(Helper.TrueArray))
            {
                return true;
            }
            return false;
        }
    }
}
