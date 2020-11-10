using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LabWeek11App
{
   public partial class AppForm : Form
   {
      private ServerNode _localServer;
      private ConcurrentDictionary<int, RemoteNode> _remoteServers = new ConcurrentDictionary<int, RemoteNode>();

      public AppForm()
      {
         InitializeComponent();
      }

      private void CmdBox_KeyPress(object sender, KeyPressEventArgs e)
      {
         if (e.KeyChar == (char)Keys.Enter)
         {
            ProcessCommand(CmdBox.Text);
            CmdBox.Text = "";
         }
      }

      private void ProcessCommand(string commandText)
      {
         // commandText ::= <command> <parameters>
         var tokens = commandText.Split(' ');
         switch (tokens[0])
         {
            case "set": // e.g. set 5001
               ProcessSet(tokens[1]);
               break;
            case "connect":
               ProcessConnect(tokens[1]);
               break;
            case "send":
               ProcessSend(tokens[1]);
               break;
            case "create_clock":
               ProcessCreateClock(tokens[1]);
               break;
            case "clock":
               ProcessClock();
               break;
         }
      }

      private void ProcessSet(string parameters)
      {
         // parameters ::= <port>
         var port = Int32.Parse(parameters);
         _localServer = new ServerNode(port);
         _localServer.Subscribe(new StringObserver(OutputBox));
         _localServer.SetUpLocalEndPoint();
         _localServer.ReportMessage("\nIP Address: " + _localServer.IPAddress);
         _localServer.StartListening();

         Task.Factory.StartNew(
            () => _localServer.WaitForConnection()
         );

         OutputBox.Text += "Port: " + port;
      }

      private void ProcessConnect(string parameters)
      {
         RemoteNode _remoteServer = new RemoteNode(_localServer);
         _remoteServer.ConnectToRemoteEndPoint(_localServer.IPAddress, Int32.Parse(parameters));
         _remoteServers[Int32.Parse(parameters)] = _remoteServer;
      }

      private void ProcessSend(string paramters)
      {
         int port = Int32.Parse(paramters.Split('|')[0]);

         _localServer.Clock.IncrementCounter();
         string message = "Clock:" + _localServer.Clock.Counter + ": " + paramters.Split('|')[1];

         foreach(var node in _remoteServers)
         {
            if (node.Key == port) node.Value.SendRequest(message);
         }
      }

      private void ProcessCreateClock(string parameters)
      {
         int interval = Int32.Parse(parameters.Split('|')[0]);
         int step = Int32.Parse(parameters.Split('|')[1]);

         _localServer.Clock = new LogicalClock(interval, step);
         _localServer.Clock.Start();
         _localServer.ReportMessage("Started Clock!");
      }

      private void ProcessClock()
      {
         _localServer.ReportMessage("Clock current counter: " + _localServer.Clock.Counter);
      }
   }
}
