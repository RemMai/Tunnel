using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Common.Libs
{
    public sealed class Logger
    {
        private static readonly Lazy<Logger> lazy = new(() => new Logger());
        public static Logger Instance => lazy.Value;

        private readonly ConcurrentQueue<LoggerModel> queue = new();
        public Action<LoggerModel> OnLogger { get; set; } = _ => { };

        public int PaddingWidth { get; set; } = 50;
#if DEBUG
        public LoggerTypes LoggerLevel { get; set; } = LoggerTypes.DEBUG;
#else
        public LoggerTypes LoggerLevel { get; set; } = LoggerTypes.WARNING;
#endif

        private Logger()
        {
            new Thread(start: () =>
                {
                    while (true)
                    {
                        while (!queue.IsEmpty)
                        {
                            if (queue.TryDequeue(out LoggerModel model))
                            {
                                OnLogger?.Invoke(model);
                            }
                        }
                    }
                })
                { IsBackground = true }.Start();
        }


        private int lockNum = 0;

        public void Lock()
        {
            Interlocked.Increment(ref lockNum);
        }

        public void UnLock()
        {
            Interlocked.Decrement(ref lockNum);
        }

        public void Debug(string content, params object[] args)
        {
            if (args is { Length: > 0 })
            {
                content = string.Format(content, args);
            }

            Enqueue(new LoggerModel { Type = LoggerTypes.DEBUG, Content = content });
        }

        public void Info(string content, params object[] args)
        {
            if (args is { Length: > 0 })
            {
                content = string.Format(content, args);
            }

            Enqueue(new LoggerModel { Type = LoggerTypes.INFO, Content = content });
        }

        public void Warning(string content, params object[] args)
        {
            if (args is { Length: > 0 })
            {
                content = string.Format(content, args);
            }

            Enqueue(new LoggerModel { Type = LoggerTypes.WARNING, Content = content });
        }

        public void Error(string content, params object[] args)
        {
            if (args is { Length: > 0 })
            {
                content = string.Format(content, args);
            }

            Enqueue(new LoggerModel { Type = LoggerTypes.ERROR, Content = content });
        }

        public void Error(Exception ex)
        {
            Enqueue(new LoggerModel { Type = LoggerTypes.ERROR, Content = ex + "" });
        }

        public void FATAL(string content, params object[] args)
        {
            if (args is { Length: > 0 })
            {
                content = string.Format(content, args);
            }

            Enqueue(new LoggerModel { Type = LoggerTypes.FATAL, Content = content });
        }

        public void Fatal(Exception ex)
        {
            Enqueue(new LoggerModel { Type = LoggerTypes.FATAL, Content = ex + "" });
        }

        private void Enqueue(LoggerModel model)
        {
            queue.Enqueue(model);
        }
    }

    public sealed class LoggerModel
    {
        public LoggerTypes Type { get; set; } = LoggerTypes.INFO;
        public DateTime Time { get; set; } = DateTime.Now;
        public string Content { get; set; } = string.Empty;
    }

    public enum LoggerTypes : byte
    {
        DEBUG = 0,
        INFO = 1,
        WARNING = 2,
        ERROR = 3,
        FATAL = 4,
    }
}