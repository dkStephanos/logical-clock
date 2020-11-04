using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;

namespace LabWeek11App
{
   public class ServerNode : IObservable<string>
   {
      private readonly ConcurrentBag<IObserver<string>> _observers;
      int PortNumber { get; set; }
      IPAddress IPAddress { get; set; }
      private SocketAddress _listener { get; set; }
      private IPEndPoint _localEndPoint { get; set; }



      public ServerNode(int portNumber)
      {
         _observers = new ConcurrentBag<IObserver<string>>();
         PortNumber = portNumber;
      }

      public IDisposable Subscribe(IObserver<string> observer)
      {
         if (!_observers.Contains(observer))
            _observers.Add(observer);

         return new MessageUnsubscriber(_observers, observer);
      }

      public void ReportMessage(string message)
      {
         foreach (var observer in _observers)
         {
            observer.OnNext(message);
         }
      }
   }
}
