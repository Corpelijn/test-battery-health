using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HardwareInformation
{
    class NetworkRequester
    {

        #region "Fields"

        private Thread thread;
        private bool stopThread;

        private TcpListener listener;

        #endregion

        #region "Constructors"

        public NetworkRequester()
        {
            
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

            thread = new Thread(AcceptRequests);
            thread.Start();
        }

        private void AcceptRequests()
        {
            listener = new TcpListener(IPAddress.Any, 34000);
            while(!stopThread)
            {
                listener.Start();
                TcpClient client = listener.AcceptTcpClient();
                new Thread(HandleRequest).Start(client);

                Thread.Sleep(50);
            }
        }

        private void HandleRequest(object o_client)
        {
            TcpClient client = o_client as TcpClient;
            NetworkStream stream = client.GetStream();

            BatteryInfoReader info = ApplicationStartup.Current.Battery;
            string message = (int)(info.Percentage * 100) + ";" +
                (int)info.Status + ";" +
                (int)info.ChargerStatus;

            byte[] data = Encoding.ASCII.GetBytes(message);
            stream.Write(data, 0, data.Length);

            stream.Dispose();
            client.Close();
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
