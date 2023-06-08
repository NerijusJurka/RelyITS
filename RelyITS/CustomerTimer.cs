using System;
using System.Threading;

namespace RelyITS
{
    public class ConsumerTimer
    {
        private Timer timer;
        private TimeSpan targetTime;
        private Consumer consumer;

        public ConsumerTimer(Consumer consumer)
        {
            this.consumer = consumer;
        }
        public void Start()
        {
            var dailyTime = DateTime.Now.AddSeconds(10).ToString("HH:mm:ss"); //Used for testing
            //var dailyTime = "13:00:00"; // Time to execute code
            var timeParts = dailyTime.Split(new char[] { ':' });

            var now = DateTime.Now;
            var targetDateTime = new DateTime(now.Year, now.Month, now.Day,
                int.Parse(timeParts[0]), int.Parse(timeParts[1]), int.Parse(timeParts[2]));

            if (targetDateTime < now)
            {
                targetDateTime = targetDateTime.AddDays(1);
            }

            targetTime = targetDateTime.TimeOfDay;
            var timeUntilTarget = targetDateTime - now;

            timer = new Timer(TimerCallback, null, timeUntilTarget, TimeSpan.FromDays(1));
        }
        public void Stop()
        {
            timer?.Dispose();
        }
        private void TimerCallback(object state)
        {
            consumer.ConsumeQueue();
        }
    }
}
