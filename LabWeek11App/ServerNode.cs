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
      public int PortNumber { get; set; }
      public IPAddress IPAddress { get; set; }
      private Socket _listener { get; set; }
      private IPEndPoint _localEndPoint { get; set; }



      public ServerNode(int portNumber)
      {
         _observers = new ConcurrentBag<IObserver<string>>();
         PortNumber = portNumber;
      }

      public void SetUpLocalEndPoint()
      {
         IPHostEntry _ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
         IPAddress = _ipHostInfo.AddressList[0];
         _localEndPoint = new IPEndPoint(IPAddress, PortNumber);
      }

      public void StartListening()
      {
         _listener = new Socket(IPAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
         _listener.Bind(_localEndPoint);
         _listener.Listen(10);
      }

      private void ProcessRequests(Socket handler)
      {
         handler.Shutdown(SocketShutdown.Both);
         handler.Close();
      }
      
      public void WaitForConnection()
      {
         do
         {
            ReportMessage("Waiting for a connection...");
            Socket handler = _listener.Accept();
            Task.Factory.StartNew(
               () => ProcessRequests(handler)
            );
         } while (true);
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
