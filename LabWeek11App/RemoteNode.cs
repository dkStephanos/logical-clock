using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace LabWeek11App
{
   class RemoteNode
   {
      private Socket _remoteSocket;
      public IPEndPoint RemoteEndPoint { get; set; }
      public ServerNode LocalServer { get; set; }

      public RemoteNode(ServerNode localServer)
      {
         LocalServer = localServer;
         _remoteSocket = null;
         RemoteEndPoint = null;
      }

      public void ConnectToRemoteEndPoint(IPAddress serverIpAddress, int serverPort)
      {
         RemoteEndPoint = new IPEndPoint(serverIpAddress, serverPort);
         _remoteSocket = new Socket(serverIpAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
         _remoteSocket.Connect(RemoteEndPoint);
         LocalServer.ReportMessage("Set up connection to: " + serverIpAddress + ":" + serverPort);
      }
      public void SendRequest(string request)
      {
         LocalServer.ReportMessage($"SENDING: {request}");
         byte[] msg = Encoding.ASCII.GetBytes(request + "<EOF>");
         _remoteSocket.Send(msg);
      }
   }
}
