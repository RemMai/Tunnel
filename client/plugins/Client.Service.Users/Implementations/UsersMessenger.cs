using System;
using Client.Messengers.Clients;
using Client.Messengers.Signin;
using Client.Service.Users.Interfaces;
using Common.Libs;
using Common.Libs.AutoInject.Attributes;
using Common.Libs.Extends;
using Common.Server;
using Common.Server.Attributes;
using Common.Server.Implementations;
using Common.Server.Interfaces;
using Common.Server.Models;
using Common.User;
using Common.User.Enums;
using Common.User.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Client.Service.Users.Implementations
{
    [MessengerIdRange((ushort)UsersMessengerIds.Min, (ushort)UsersMessengerIds.Max)]
    [AutoInject(ServiceLifetime.Singleton)]
    public sealed class UsersMessenger : IMessenger
    {
        private readonly IUserMapInfoCaching userMapInfoCaching;
        private readonly MessengerSender messengerSender;
        private readonly SignInStateInfo signInStateInfo;
        private readonly IClientInfoCaching clientInfoCaching;

        public UsersMessenger(IUserMapInfoCaching userMapInfoCaching, MessengerSender messengerSender, SignInStateInfo signInStateInfo, IClientInfoCaching clientInfoCaching)
        {
            this.userMapInfoCaching = userMapInfoCaching;
            this.messengerSender = messengerSender;
            this.signInStateInfo = signInStateInfo;
            this.clientInfoCaching = clientInfoCaching;
            clientInfoCaching.OnOnline += OnOnline;
        }

        /// <summary>
        /// 向目标客户端登入，在目标客户端进行权限缓存，下次去和目标端通信时，才可能有权限
        /// </summary>
        /// <param name="client"></param>
        private void OnOnline(ClientInfo client)
        {
            messengerSender.SendReply(new MessageRequestWrap
            {
                Connection = signInStateInfo.Connection,
                MessengerId = (ushort)UsersMessengerIds.Info
            }).ContinueWith((resp) =>
            {
                if (resp.Result.Code == MessageResponeCodes.OK)
                {
                    UserInfo user = resp.Result.Data.GetUTF8String().DeJson<UserInfo>();
                    _ = messengerSender.SendOnly(new MessageRequestWrap
                    {
                        Connection = client.Connection,
                        MessengerId = (ushort)UsersMessengerIds.SignIn,
                        Payload = new UserSignInfo { ConnectionId = signInStateInfo.ConnectId, UserId = user.ID }.ToBytes()
                    });
                }
            });
        }


        [MessengerId((ushort)UsersMessengerIds.SignIn)]
        public void SignIn(IConnection connection)
        {
            UserSignInfo userSignInfo = new UserSignInfo();
            userSignInfo.DeBytes(connection.ReceiveRequestWrap.Payload);
            userSignInfo.ConnectionId = connection.FromConnection.ConnectId;

            //去服务器验证登录是否正确，
            _ = messengerSender.SendReply(new MessageRequestWrap
            {
                Connection = signInStateInfo.Connection,
                MessengerId = (ushort)UsersMessengerIds.SignIn,
                Payload = userSignInfo.ToBytes()
            }).ContinueWith(async (resp) =>
            {
                if (resp.Result.Code == MessageResponeCodes.OK && resp.Result.Data.Span.SequenceEqual(Helper.TrueArray))
                {
                    if (clientInfoCaching.Get(connection.FromConnection.ConnectId, out ClientInfo client))
                    {
                        if (userMapInfoCaching.Get(userSignInfo.UserId, out UserMapInfo map) == false)
                        {
                            map = new UserMapInfo { Access = 0, ID = userSignInfo.UserId, ConnectionId = userSignInfo.ConnectionId };
                            await userMapInfoCaching.Add(map);
                        }
                        //更新客户端的权限值
                        client.UserAccess = map.Access;
                        map.ConnectionId = userSignInfo.ConnectionId;
                    }
                }
            });
        }
    }
}
