using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
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

      public LogicalClock Clock { get; set; }



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
         ReportMessage("CONNECTED TO: " + handler.LocalEndPoint);
        
         byte[] buffer = new byte[1024];
         string data;
         string request;
         do
         {
            data = "";
            while (true)
            {
               int bytesRec = handler.Receive(buffer);
               data += Encoding.ASCII.GetString(buffer, 0, bytesRec);
               int index = data.IndexOf("<EOF>");
               if (index > -1)
               {
                  request = data.Substring(0, index);
                  break;
               }
            }
            ReportMessage($"RECEIVED:{request} at {Clock.Counter}");
            // Get clock value from other node and update ours if it is greater
            string[] requestParts = request.Split(':');
            ReportMessage($"Counter received: {requestParts[1]}");
            if (Int32.Parse(requestParts[1]) > Clock.Counter)
            {
               Clock.Counter = Int32.Parse(requestParts[1]);
               ReportMessage($"CLOCK UPDATED to {requestParts[1]}!");
            } else
            {
               ReportMessage("Clock not updated");
            }
         } while (request != "Exit");
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
