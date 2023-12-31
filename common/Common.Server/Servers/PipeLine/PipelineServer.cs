﻿using System;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;

namespace Common.Server.Servers.pipeLine
{
    /// <summary>
    /// 具名管道服务端
    /// </summary>
    public sealed class PipelineServer : IDisposable
    {
        private NamedPipeServerStream Server { get; set; }
        private StreamWriter Writer { get; set; }
        private StreamReader Reader { get; set; }
        private Func<string, string> Action { get; set; }
        private string PipeName { get; set; }
        private static int _maxNumberAcceptedClients = 5;

        /// <summary>
        /// 初始化函数
        /// </summary>
        /// <param name="pipeName">管道名称</param>
        /// <param name="action"></param>
        public PipelineServer(string pipeName, Func<string, string> action)
        {
            Server = new NamedPipeServerStream(pipeName, PipeDirection.InOut, 254);
            Writer = new StreamWriter(Server);
            Reader = new StreamReader(Server);
            Action = action;
            PipeName = pipeName;
        }

        public void BeginAccept()
        {
            IAsyncResult result = Server.BeginWaitForConnection(ProcessAccept, null);
            if (result.CompletedSynchronously)
            {
                ProcessAccept(result);
            }
        }

        private void ProcessAccept(IAsyncResult result)
        {
            Server.EndWaitForConnection(result);

            Interlocked.Decrement(ref _maxNumberAcceptedClients);
            if (_maxNumberAcceptedClients > 0)
            {
                var server = new PipelineServer(PipeName, Action);
                server.BeginAccept();
            }

            Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                        string msg = await Reader.ReadLineAsync().ConfigureAwait(false);
                        string res = Action(msg);
                        await Writer.WriteLineAsync(res).ConfigureAwait(false);
                        await Writer.FlushAsync().ConfigureAwait(false);
                    }
                    catch (Exception)
                    {
                        Server.Disconnect();
                        break;
                    }
                }

                BeginAccept();
            });
        }

        public void Dispose()
        {
            Server.Dispose();
            Server = null;
            Writer = null;
            Reader = null;
            Action = null;
        }
    }
}