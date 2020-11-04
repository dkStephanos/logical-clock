using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace LabWeek11App
{
   public class LogicalClock : IDisposable
   {
      private System.Timers.Timer _timer;
      private int _interval;
      public long Counter { get; set; }
      public int Step { get; set; }

      public LogicalClock(int interval, int step)
      {
         _interval = interval;
         Step = step;

         _timer = new System.Timers.Timer(_interval);
         _timer.AutoReset = true;
         _timer.Elapsed += OnTick;
      }

      public void OnTick(object sender, ElapsedEventArgs e)
      {
         Counter += Step;
      }

      public void Start()
      {
         _timer.Start();
      }

      public void Stop()
      {
         _timer.Stop();
      }

      public void Dispose()
      {
         Stop();
         _timer.Dispose();
      }
   }
}
