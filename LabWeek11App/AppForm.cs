using System;
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
   }
}
