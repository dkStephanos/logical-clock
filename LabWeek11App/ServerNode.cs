using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace LabWeek11App
{
   public class ServerNode : IObservable<string>
   {
      private readonly ConcurrentBag<IObserver<string>> _observers;
      int PortNumber { get; set; }
      IPAddress IPAddress { get; set; }
      private Socket _listener { get; set; }
      private IPEndPoint _localEndPoint { get; set; }



      public ServerNode(int portNumber)
      {
         _observers = new ConcurrentBag<IObserver<string>>();
         PortNumber = portNumber;
      }

      public void SetUpLocalEndPoint()
      {
         string strHostName = Dns.GetHostName();
         IPHostEntry ipHostEntry = Dns.GetHostEntry(strHostName);
         IPAddress IPAddress = ipHostEntry.AddressList[4];
         _localEndPoint = new IPEndPoint(IPAddress, PortNumber);
      }

      public void StartListening()
      {
         _listener = new Socket(IPAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
         _listener.Connect(_localEndPoint);
      }

      private void ProcessRequests(Socket handler)
      {
         handler.Shutdown(SocketShutdown.Both);
         handler.Close();
      }
      
      public void WaitForConnection()
      {
         ReportMessage("Waiting for a connection...");
         Socket handler = _listener.Accept();
         Task.Factory.StartNew(
            () => HandleRequest(handler)
         );
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
