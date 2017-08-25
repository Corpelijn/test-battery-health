using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClientBattery
{
    class BatteryInfoRequester
    {

        #region "Fields"

        private Thread thread;
        private bool stopThread;

        private TcpClient client;
        private string hostname;

        #endregion

        #region "Constructors"

        public BatteryInfoRequester()
        {
            try
            {
                //hostname = Microsoft.Win32.RegistryKey
                //    .OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, Microsoft.Win32.RegistryView.Registry64)
                //    .OpenSubKey("SOFTWARE\\Citrix\\ICA\\Session")
                //    .GetValue("ClientName").ToString();
            }
            catch (Exception ex)
            {
                string sSource = "Battery Client";
                string sLog = "Application";
                string sEvent = "Cannot read value from ClientName at HKLM\\SOFTWARE\\Citrix\\ICA\\Session (64-bit registry)";
                if (!EventLog.SourceExists(sSource))
                {
                    EventLog.CreateEventSource(sSource, sLog);
                }

                EventLog.WriteEntry(sSource, sEvent, EventLogEntryType.Information);
            }

            hostname = "lap25828";
        }

        #endregion

        #region "Properties"

        public bool HasHostname
        {
            get { return hostname != "" || hostname != null; }
        }

        public string Hostname
        {
            set { hostname = value; }
        }

        #endregion

        #region "Methods"

        public void Start()
        {
            if (thread != null && thread.IsAlive)
                return;

            stopThread = false;

            thread = new Thread(Requester);
            thread.Start();
        }

        public bool Open()
        {
            if (client != null && client.Connected)
                return true;

            try
            {
                if (hostname == null || hostname == "")
                    return false;

                client = new TcpClient(hostname, 34000);
                return true;
            }
            catch (SocketException)
            {
                Console.WriteLine("Cannot connect to the given remote host");
            }

            return false;
        }

        private void Requester()
        {
            if (!Open())
                return;

            SendMessage("start");

            DateTime lastDataTime = DateTime.Now;

            try
            {
                List<byte> message = new List<byte>();
                bool retrievingMessage = false;
                int messagesize = 0;
                NetworkStream stream = client.GetStream();

                while (!stopThread)
                {
                    if (stream.DataAvailable)
                    {
                        lastDataTime = DateTime.Now;

                        if (!retrievingMessage)
                        {
                            messagesize = stream.ReadByte();
                            retrievingMessage = true;
                            message = new List<byte>();
                        }

                        if (message.Count < messagesize)
                        {
                            byte[] buffer = new byte[client.Available];
                            stream.Read(buffer, 0, buffer.Length);
                            if (message.Count + buffer.Length > messagesize)
                            {
                                int amount = messagesize - message.Count;
                                message.AddRange(buffer.Take(amount));
                                buffer = buffer.Skip(amount).ToArray();
                                ProcessMessage(Encoding.ASCII.GetString(message.ToArray()));

                                message.Clear();
                                messagesize = buffer[0];
                                message.AddRange(buffer.Skip(1));
                            }
                            else
                            {
                                message.AddRange(buffer);
                                if (message.Count == messagesize)
                                {
                                    ProcessMessage(Encoding.ASCII.GetString(message.ToArray()));
                                    message.Clear();
                                    retrievingMessage = false;
                                }
                            }
                        }
                        else
                        {
                            byte[] tmpMessage;
                            while (message.Count > messagesize)
                            {
                                tmpMessage = message.Take(messagesize).ToArray();
                                messagesize = (int)message[messagesize];
                                message.RemoveRange(0, messagesize + 1);

                                ProcessMessage(Encoding.ASCII.GetString(tmpMessage));
                            }

                            if (message.Count == messagesize)
                            {
                                ProcessMessage(Encoding.ASCII.GetString(message.ToArray()));
                                message.Clear();
                                retrievingMessage = false;
                            }
                        }
                    }
                    else if (client != null && client.Connected && lastDataTime.AddMinutes(1) < DateTime.Now)
                    {
                        client = null;
                        thread = new Thread(Requester);
                        thread.Start();
                        return;
                    }

                    Thread.Sleep(10);
                }
            }
            catch (SocketException)
            {
                thread = new Thread(Requester);
                thread.Start();
            }
        }

        private void ProcessMessage(string message)
        {
            string[] info = message.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            BatteryChargeStatus status = (BatteryChargeStatus)Enum.Parse(typeof(BatteryChargeStatus), info[1]);
            PowerLineStatus charger = (PowerLineStatus)Enum.Parse(typeof(PowerLineStatus), info[2]);
            ApplicationStartup.UpdateIcon(int.Parse(info[0]), status, charger);
        }

        public void SendMessage(string message)
        {
            if (!thread.IsAlive)
                return;

            while (client == null)
            {
                if (stopThread)
                    return;
                Thread.Sleep(100);
            }

            List<byte> buffer = new List<byte>(Encoding.ASCII.GetBytes(message));
            buffer.Insert(0, (byte)buffer.Count);
            NetworkStream stream = client.GetStream();
            client.GetStream().Write(buffer.ToArray(), 0, buffer.Count);
        }

        public void Stop()
        {
            stopThread = true;
        }

        #endregion

        #region "Abstract/Virtual Methods"



        #endregion

        #region "Inherited Methods"



        #endregion

        #region "Static Methods"



        #endregion

        #region "Operators"



        #endregion
    }
}
