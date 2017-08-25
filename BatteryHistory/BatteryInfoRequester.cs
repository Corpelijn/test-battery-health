using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BatteryHistory
{
    class BatteryInfoRequester
    {

        #region "Fields"

        private Thread thread;
        private bool stopThread;

        private TcpClient client;
        private string hostname;
        private bool dataRead;
        private bool notFound;

        #endregion

        #region "Constructors"

        public BatteryInfoRequester(string host)
        {
            hostname = host;
            dataRead = false;
            notFound = false;
        }

        #endregion

        #region "Properties"

        public string Hostname
        {
            get { return hostname; }
        }

        public bool IsDone
        {
            get { return dataRead; }
        }

        public bool NotFound
        {
            get { return notFound; }
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
                client = new TcpClient(hostname, 34000);
                return true;
            }
            catch (SocketException)
            {
                Console.WriteLine("Cannot connect to the given remote host");
                notFound = true;
            }

            return false;
        }

        private void Requester()
        {
            if (!Open())
            {
                dataRead = true;
                return;
            }

            SendMessage("history");

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
                        if (!retrievingMessage)
                        {
                            messagesize = stream.ReadByte();
                            retrievingMessage = true;
                            message = new List<byte>();
                        }

                        lastDataTime = DateTime.Now;

                        if (message.Count < messagesize || messagesize == 255)
                        {
                            byte[] buffer = new byte[client.Available];
                            stream.Read(buffer, 0, buffer.Length);
                            message.AddRange(buffer);

                            if (message.Count >= 4 && messagesize == 255)
                            {
                                messagesize = BitConverter.ToInt32(message.Take(4).ToArray(), 0);
                                message.RemoveRange(0, 4);
                            }
                        }
                    }
                    if (message.Count >= messagesize && messagesize != 255)
                    {
                        byte[] msg = message.Take(messagesize).ToArray();
                        HistoryManager.INSTANCE.AddHistory(hostname, msg);

                        message.RemoveRange(0, messagesize);

                        retrievingMessage = message.Count != 0;
                        if (retrievingMessage)
                        {
                            messagesize = message[0];
                            message.RemoveAt(0);
                        }
                    }
                    else if(lastDataTime.AddSeconds(1) < DateTime.Now)
                    {
                        if (message.Count >= 4 && messagesize == 255)
                        {
                            messagesize = BitConverter.ToInt32(message.Take(4).ToArray(), 0);
                            message.RemoveRange(0, 4);
                            lastDataTime = DateTime.Now;
                            continue;
                        }
                        break;
                    }

                    Thread.Sleep(10);
                }
            }
            catch (SocketException)
            {
                notFound = true;
                
                //thread = new Thread(Requester);
                //thread.Start();
            }

            dataRead = true;
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
