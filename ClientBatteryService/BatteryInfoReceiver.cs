using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ClientBatteryService
{
    public partial class BatteryInfoReceiver : ServiceBase
    {
        private TcpClient client;
        private bool stopThread;

        public BatteryInfoReceiver()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            SystemEvents.SessionSwitch += OnSessionSwitch;

            LoginHardware();

            stopThread = false;
            Thread thread = new Thread(ReadIncomingData);
            thread.Start();
        }

        private void OnSessionSwitch(object sender, SessionSwitchEventArgs e)
        {
            if (e.Reason == SessionSwitchReason.SessionLock)
            {
                // Send a stop signal to the current server
                LogoffHardware();
            }
            else if (e.Reason == SessionSwitchReason.SessionUnlock)
            {
                // Send a start signal the a new server
                LoginHardware();
            }
        }

        private void ReadIncomingData()
        {
            while(client == null || !client.Connected)
            {
                if (stopThread)
                    return;
                Thread.Sleep(100);
            }

            List<byte> message = new List<byte>();
            bool retrievingMessage = false;
            int messagesize = 0;
            NetworkStream stream = client.GetStream();

            while(!stopThread)
            {
                if (stream.DataAvailable)
                {
                    if (!retrievingMessage)
                    {
                        messagesize = stream.ReadByte();
                        retrievingMessage = true;
                        message.Clear();
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
                }
            }
        }

        protected override void OnStop()
        {
            stopThread = true;
            LogoffHardware();
        }

        private void LoginHardware()
        {
            // Get the physical hardware id
            string name = Environment.GetEnvironmentVariable("clientname");

            // Connect to the physical OS
            while (client == null || !client.Connected)
            {
                try
                {
                    client = new TcpClient(name, 34000);
                }
                catch(SocketException)
                {
                    continue;
                }

                Thread.Sleep(100);
            }

            // Send the message to start receiving updates
            if (client.Connected)
                SendMessage("start");
        }

        private void LogoffHardware()
        {
            if(client != null && client.Connected)
            {
                SendMessage("stop");
            }
        }

        private void SendMessage(string message)
        {
            List<byte> data = new List<byte>(Encoding.ASCII.GetBytes(message));
            data.Insert(0, (byte)message.Length);
            client.GetStream().Write(data.ToArray(), 0, data.Count);
        }

        private void ProcessMessage(string message)
        {
            System.IO.File.AppendAllText("C:\\test.txt", message + "\n");
        }
    }
}
