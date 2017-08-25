using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HardwareInformationService
{
    public class ClientHandler
    {

        #region "Fields"

        private TcpClient client;
        private NetworkStream stream;
        private Thread thread;

        private bool stopThread;

        #endregion

        #region "Constructors"

        public ClientHandler(TcpClient client)
        {
            this.client = client;
            this.stream = client.GetStream();
        }

        #endregion

        #region "Properties"



        #endregion

        #region "Methods"

        public void Start()
        {
            if (thread != null && thread.IsAlive)
                return;

            stopThread = false;

            thread = new Thread(ReadFromClient);
            thread.Start();
        }

        private void ReadFromClient()
        {
            int messagesize = 0;
            bool retrievingMessage = false;
            List<byte> message = null;

            while (!stopThread)
            {
                if (!client.Connected)
                    break;

                if (stream.DataAvailable)
                {
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
                }

                Thread.Sleep(100);
            }
        }

        private void ProcessMessage(string message)
        {
            switch (message)
            {
                case "start":
                    BatteryInformation.SetConnectedClient(this);
                    break;
                case "stop":
                    BatteryInformation.StopConnectedClient(this);
                    break;
                case "history":
                    SendClientHistory();
                    break;
            }
        }

        public void Terminate()
        {
            stopThread = true;
            stream.Close();
            client.Close();
        }

        public void SendToClient(string message)
        {
            int length = message.Length;
            List<byte> data = new List<byte>(Encoding.ASCII.GetBytes(message));
            data.Insert(0, (byte)length);

            if (!client.Connected)
            {
                BatteryInformation.StopConnectedClient(this);
                return;
            }

            stream.Write(data.ToArray(), 0, data.Count);
        }

        private void SendClientHistory()
        {
            byte[] data = Log.GetHistory();

            if (!client.Connected)
            {
                return;
            }

            stream.Write(data, 0, data.Length);

            Terminate();
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
