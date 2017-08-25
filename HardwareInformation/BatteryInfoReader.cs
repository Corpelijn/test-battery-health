using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HardwareInformation
{
    class BatteryInfoReader
    {

        #region "Fields"

        private Thread thread;
        private bool stopThread;

        private float percentage;
        private int timeLeft;
        private BatteryChargeStatus status;
        private PowerLineStatus charger;

        #endregion

        #region "Constructors"

        public BatteryInfoReader()
        {
            
        }

        #endregion

        #region "Properties"

        public float Percentage
        {
            get { return percentage; }
        }

        public int TimeLeft
        {
            get { return timeLeft; }
        }

        public BatteryChargeStatus Status
        {
            get { return status; }
        }

        public PowerLineStatus ChargerStatus
        {
            get { return charger; }
        }

        #endregion

        #region "Methods"

        public void Start()
        {
            if (thread != null && thread.IsAlive)
                return; 

            stopThread = false;

            thread = new Thread(GetInformation);
            thread.Start();
        }

        private void GetInformation()
        {
            DateTime last = DateTime.Now.Subtract(new TimeSpan(0, 2, 0));
            while(!stopThread)
            {
                if (last.AddMinutes(1) <= DateTime.Now)
                {
                    percentage = SystemInformation.PowerStatus.BatteryLifePercent;
                    timeLeft = SystemInformation.PowerStatus.BatteryLifeRemaining;
                    status = SystemInformation.PowerStatus.BatteryChargeStatus;
                    charger = SystemInformation.PowerStatus.PowerLineStatus;

                    last = DateTime.Now;
                }

                Thread.Sleep(50);
            }
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
