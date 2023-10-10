using System;
using System.Threading.Tasks;
using Client.Messengers.Signin;
using client.service.ui.api.Interfaces;
using Common.Extensions.AutoInject.Attributes;
using Common.Libs;
using Common.Libs.Extends;
using Common.Server;
using Common.Server.Implementations;
using Common.Server.Models;
using Common.Vea;
using Common.Vea.Enums;
using Microsoft.Extensions.DependencyInjection;

namespace Client.Service.Vea.Server.Implementations
{
    [AutoInject(ServiceLifetime.Singleton)]
    public sealed class ServerVeaConfigure : IClientConfigure
    {
        private readonly MessengerSender messengerSender;
        private readonly SignInStateInfo signInStateInfo;

        public ServerVeaConfigure(MessengerSender messengerSender, SignInStateInfo signInStateInfo)
        {
            this.messengerSender = messengerSender;
            this.signInStateInfo = signInStateInfo;
        }

        public string Name => "服务端网卡管理";
        public string Author => "snltty";
        public string Desc => string.Empty;
        public bool Enable => true;

        /// <summary>
        /// 加载
        /// </summary>
        /// <returns></returns>
        public async Task<string> Load()
        {
            MessageResponeInfo resp = await messengerSender.SendReply(new MessageRequestWrap
            {
                MessengerId = (ushort)VeaSocks5MessengerIds.GetSetting,
                Connection = signInStateInfo.Connection,
            }).ConfigureAwait(false);
            if (resp.Code == MessageResponeCodes.OK)
            {
                return resp.Data.GetUTF8String();
            }

            return string.Empty;
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="jsonStr"></param>
        /// <returns></returns>
        public async Task<bool> Save(string jsonStr)
        {
            MessageResponeInfo resp = await messengerSender.SendReply(new MessageRequestWrap
            {
                MessengerId = (ushort)VeaSocks5MessengerIds.Setting,
                Connection = signInStateInfo.Connection,
                Payload = jsonStr.ToUTF8Bytes()
            }).ConfigureAwait(false);
            return resp.Code == MessageResponeCodes.OK && resp.Data.Span.SequenceEqual(Helper.TrueArray);
        }
    }
}