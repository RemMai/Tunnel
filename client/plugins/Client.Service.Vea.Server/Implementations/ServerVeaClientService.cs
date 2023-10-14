using System;
using System.Threading.Tasks;
using Client.Messengers.Signin;
using Client.Service.Ui.api.Interfaces;
using Client.Service.Ui.api.Models;
using Client.Service.Vea.Server.Models;
using Common.Extensions.AutoInject.Attributes;
using Common.Libs;
using Common.Libs.Extends;
using Common.Server;
using Common.Server.Implementations;
using Common.Server.Models;
using Common.Vea;
using Common.Vea.Enums;
using Common.Vea.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Client.Service.Vea.Server.Implementations
{
    [AutoInject(ServiceLifetime.Singleton)]
    public sealed class ServerVeaClientService : IClientService
    {
        private readonly MessengerSender messengerSender;
        private readonly SignInStateInfo signInStateInfo;

        public ServerVeaClientService(MessengerSender messengerSender, SignInStateInfo signInStateInfo)
        {
            this.messengerSender = messengerSender;
            this.signInStateInfo = signInStateInfo;
        }

        public async Task<DHCPInfo> List(ClientServiceParamsInfo arg)
        {
            MessageResponeInfo resp = await messengerSender.SendReply(new MessageRequestWrap
            {
                Connection = signInStateInfo.Connection,
                MessengerId = (ushort)VeaSocks5MessengerIds.Network,
            });
            if (resp.Code == MessageResponeCodes.OK)
            {
                return resp.Data.GetUTF8String().DeJson<DHCPInfo>();
            }

            return null;
        }

        public async Task<bool> AddNetwork(ClientServiceParamsInfo arg)
        {
            MessageResponeInfo resp = await messengerSender.SendReply(new MessageRequestWrap
            {
                Connection = signInStateInfo.Connection,
                MessengerId = (ushort)VeaSocks5MessengerIds.AddNetwork,
                Payload = BitConverter.GetBytes(uint.Parse(arg.Content))
            });
            if (resp.Code == MessageResponeCodes.OK)
            {
                return resp.Data.Span.SequenceEqual(Helper.TrueArray);
            }

            return false;
        }

        public async Task<bool> ModifyIP(ClientServiceParamsInfo arg)
        {
            ModifyIPModel model = arg.Content.DeJson<ModifyIPModel>();
            MessageResponeInfo resp = await messengerSender.SendReply(new MessageRequestWrap
            {
                Connection = signInStateInfo.Connection,
                MessengerId = (ushort)VeaSocks5MessengerIds.ModifyIP,
                Payload = model.ToBytes()
            });
            if (resp.Code == MessageResponeCodes.OK)
            {
                return resp.Data.Span.SequenceEqual(Helper.TrueArray);
            }

            return false;
        }

        public async Task<bool> DeleteIP(ClientServiceParamsInfo arg)
        {
            MessageResponeInfo resp = await messengerSender.SendReply(new MessageRequestWrap
            {
                Connection = signInStateInfo.Connection,
                MessengerId = (ushort)VeaSocks5MessengerIds.DeleteIP,
                Payload = BitConverter.GetBytes(ulong.Parse(arg.Content))
            });
            if (resp.Code == MessageResponeCodes.OK)
            {
                return resp.Data.Span.SequenceEqual(Helper.TrueArray);
            }

            return false;
        }
    }
}