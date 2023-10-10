using System;

namespace Common.Server.Implementations
{
    public sealed class TokenBucketRatelimit
    {
        double ticks = 1000.0d * TimeSpan.TicksPerMillisecond;
        TokenBucketRateInfo info;

        public TokenBucketRatelimit(int rate)
        {
            info = new TokenBucketRateInfo { Rate = rate, CurrentRate = 0, Token = rate / ticks, LastTime = GetTime() };
        }

        public void ChangeRate(int rate)
        {
            info.Rate = rate;
            info.Token = rate / ticks;
        }

        public int Try(int num)
        {
            if (info.Rate == 0)
            {
                return num;
            }
            AddToken(info);
            //消耗掉能消耗的
            int canEat = Math.Min(num, (int)info.CurrentRate);
            if (canEat >= num)
            {
                info.CurrentRate -= canEat;
            }
            return canEat;
        }

        private void AddToken(TokenBucketRateInfo info)
        {
            long time = GetTime();
            long times = (time - info.LastTime);

            info.CurrentRate = Math.Min(info.CurrentRate + times * info.Token, info.Rate);

            info.LastTime = time;
        }

        private long GetTime()
        {
            return DateTime.UtcNow.Ticks;
        }

        class TokenBucketRateInfo
        {
            public double Rate { get; set; }
            public double CurrentRate { get; set; }
            public double Token { get; set; }
            public long LastTime { get; set; }

        }
    }
}