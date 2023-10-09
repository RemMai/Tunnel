using Common.Libs;
using Common.Libs.Extends;
using Common.Server;
using Common.Server.Model;
using Server.Messengers.SignIn;
using System.Text;
using Common.Libs.AutoInject.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace Server.Service.Messengers
{
    /// <summary>
    /// 加密
    /// </summary>
    [MessengerIdRange((ushort)CryptoMessengerIds.Min, (ushort)CryptoMessengerIds.Max)]   
    [AutoInject(ServiceLifetime.Singleton)]
    public sealed class CryptoMessenger : IMessenger
    {
        private readonly IAsymmetricCrypto asymmetricCrypto;
        private readonly ICryptoFactory cryptoFactory;
        private readonly IClientSignInCaching clientSignInCache;
        private readonly Config config;
        public CryptoMessenger(IAsymmetricCrypto asymmetricCrypto, ICryptoFactory cryptoFactory, IClientSignInCaching clientSignInCache, Config config)
        {
            this.asymmetricCrypto = asymmetricCrypto;
            this.cryptoFactory = cryptoFactory;
            this.clientSignInCache = clientSignInCache;
            this.config = config;
        }

        [MessengerId((ushort)CryptoMessengerIds.Key)]
        public void Key(IConnection connection)
        {
            connection.WriteUTF8(asymmetricCrypto.Key.PublicKey);
        }

        [MessengerId((ushort)CryptoMessengerIds.Set)]
        public void Set(IConnection connection)
        {
            string password;
            if (connection.ReceiveRequestWrap.Payload.Length > 0)
            {
                var memory = asymmetricCrypto.Decode(connection.ReceiveRequestWrap.Payload);
                password = memory.GetUTF8String();
            }
            else
            {
                password = config.EncodePassword;
            }
            if (string.IsNullOrWhiteSpace(password))
            {
                connection.Write(Helper.FalseArray);
                return;
            }

            ISymmetricCrypto encoder = cryptoFactory.CreateSymmetric(password);
            connection.EncodeEnable(encoder);
            connection.Write(Helper.TrueArray);
        }

        [MessengerId((ushort)CryptoMessengerIds.Test)]
        public void Test(IConnection connection)
        {
            if (Logger.Instance.LoggerLevel <= LoggerTypes.DEBUG)
                Logger.Instance.Debug($"encoder test : {connection.Crypto.Decode(connection.ReceiveRequestWrap.Payload).Span.GetString()}");
            connection.Write(Helper.TrueArray);
        }

        [MessengerId((ushort)CryptoMessengerIds.Clear)]
        public void Clear(IConnection connection)
        {
            if (clientSignInCache.Get(connection.ConnectId, out SignInCacheInfo client))
            {
                client.Connection?.EncodeDisable();
            }
            connection.Write(Helper.TrueArray);
        }
    }
}
