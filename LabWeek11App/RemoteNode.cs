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
   }
}
