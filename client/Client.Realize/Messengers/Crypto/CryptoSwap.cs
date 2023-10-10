using Common.Libs;
using Common.Libs.Extends;
using System;
using System.Threading.Tasks;
using Common.Extensions.AutoInject.Attributes;
using Common.Server.Implementations;
using Common.Server.Interfaces;
using Common.Server.Models;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Client.Realize.Messengers.Crypto
{
    /// <summary>
    /// 秘钥交换
    /// </summary>
    [AutoInject(ServiceLifetime.Singleton)]
    public sealed class CryptoSwap
    {
        private readonly MessengerSender messengerSender;
        private readonly ICryptoFactory cryptoFactory;

        public CryptoSwap(MessengerSender messengerSender, ICryptoFactory cryptoFactory)
        {
            this.messengerSender = messengerSender;
            this.cryptoFactory = cryptoFactory;
        }

        /// <summary>
        /// 交换秘钥
        /// </summary>
        /// <param name="tcp"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<ICrypto> Swap(IConnection tcp, string password)
        {
            try
            {
                byte[] encodedData = Helper.EmptyArray;
                if (string.IsNullOrWhiteSpace(password))
                {
                    MessageResponeInfo publicKeyResponse = await messengerSender.SendReply(new MessageRequestWrap
                    {
                        Connection = tcp,
                        Encode = false,
                        MessengerId = (ushort)CryptoMessengerIds.Key,
                    }).ConfigureAwait(false);
                    if (publicKeyResponse.Code != MessageResponeCodes.OK)
                    {
                        return null;
                    }

                    string publicKey = publicKeyResponse.Data.GetUTF8String();

                    IAsymmetricCrypto encoder = cryptoFactory.CreateAsymmetric(new RsaKey { PublicKey = publicKey, PrivateKey = string.Empty });
                    password = StringHelper.RandomPasswordStringMd5();
                    encodedData = encoder.Encode(password.ToUTF8Bytes());
                    encoder.Dispose();
                }

                ICrypto crypto = cryptoFactory.CreateSymmetric(password);
                if (tcp != null)
                {
                    MessageResponeInfo setResponse = await messengerSender.SendReply(new MessageRequestWrap
                    {
                        Connection = tcp,
                        Encode = false,
                        MessengerId = (ushort)CryptoMessengerIds.Set,
                        Payload = encodedData
                    }).ConfigureAwait(false);
                    if (setResponse.Code != MessageResponeCodes.OK || setResponse.Data.Span.SequenceEqual(Helper.TrueArray) == false)
                    {
                        return null;
                    }
                }

                return crypto;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message + "\r\n" + ex.StackTrace);
                return null;
            }
        }

        /// <summary>
        /// 测试加密
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public async Task<bool> Test(IConnection connection)
        {
            MessageResponeInfo resp = await messengerSender.SendReply(new MessageRequestWrap
            {
                Connection = connection,
                MessengerId = (ushort)CryptoMessengerIds.Test,
                Payload = connection.Crypto.Encode("test".ToBytes())
            }).ConfigureAwait(false);

            return resp.Code == MessageResponeCodes.OK;
        }
    }
}
