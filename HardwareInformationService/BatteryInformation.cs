using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HardwareInformationService
{
    public partial class BatteryInformation : ServiceBase
    {
        private int percentage;
        private BatteryChargeStatus status;
        private PowerLineStatus charger;
        private int lastPercentage;
        private BatteryChargeStatus lastStatus;
        private PowerLineStatus lastCharger;

        private TcpListener listener;
        private ClientHandler client;

        private List<ClientHandler> knownClients;

        private static BatteryInformation instance;

        public BatteryInformation()
        {
            InitializeComponent();

            Log.Create();

            knownClients = new List<ClientHandler>();

            // Init a system event to check for sleep mode or other
            SystemEvents.PowerModeChanged += OnPowerModeChanged;

            instance = this;
        }

        private void OnPowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {
            if (e.Mode == PowerModes.Suspend)
                client.SendToClient("stop");
        }

        protected override void OnStart(string[] args)
        {
            // Set up a timer to trigger every minute.  
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = 1000;
            timer.Elapsed += new System.Timers.ElapsedEventHandler(this.OnBatteryInfoUpdate);
            timer.Start();

            System.Timers.Timer timer1 = new System.Timers.Timer();
            timer1.Interval = 10000;
            timer1.Elapsed += new System.Timers.ElapsedEventHandler(this.SendClientUpdate);
            timer1.Start();

            OnBatteryInfoUpdate(null, null);
            SendClientUpdate(null, null);

            listener = new TcpListener(IPAddress.Any, 34000);
            listener.Start();
            listener.BeginAcceptTcpClient(new AsyncCallback(OnClientConnect), listener);
        }

        protected override void OnStop()
        {
            listener.Stop();
            Log.Close();
        }

        public void SendClientUpdate(object sender, System.Timers.ElapsedEventArgs args)
        {
            Log.AddHistory(percentage + ";" + status + ";" + charger);

            if (client != null)
                client.SendToClient(percentage + ";" + status + ";" + charger);
        }

        public void OnBatteryInfoUpdate(object sender, System.Timers.ElapsedEventArgs args)
        {
            lastPercentage = percentage;
            lastStatus = status;
            lastCharger = charger;
            percentage = (int)(SystemInformation.PowerStatus.BatteryLifePercent * 100);
            status = SystemInformation.PowerStatus.BatteryChargeStatus;
            charger = SystemInformation.PowerStatus.PowerLineStatus;



            if (client != null && (percentage != lastPercentage || status != lastStatus || charger != lastCharger))
            {
                client.SendToClient(percentage + ";" + status + ";" + charger);
            }
        }

        public void OnClientConnect(IAsyncResult iar)
        {
            TcpListener listener = iar.AsyncState as TcpListener;
            TcpClient client;
            try
            {
                client = listener.EndAcceptTcpClient(iar);
                ClientHandler c_handler = new ClientHandler(client);
                c_handler.Start();
                knownClients.Add(c_handler);

                // Start listening again
                listener.BeginAcceptTcpClient(new AsyncCallback(OnClientConnect), listener);
            }
            catch (SocketException ex)
            {
                Log.Write("There was an error while accepting a connection with a client: " + ex.Message);
            }
            catch (ObjectDisposedException)
            {
                return;
            }
        }

        public static void SetConnectedClient(ClientHandler handler)
        {
            if (instance.client != null)
            {
                instance.client.Terminate();
            }
            instance.client = handler;
        }

        public static void StopConnectedClient(ClientHandler client)
        {
            if (instance.client == client)
            {
                instance.client = null;
                instance.knownClients.Remove(client);
            }
        }
    }
}
