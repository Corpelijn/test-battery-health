using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatteryHealthService
{
    class EventLogOutput : OutputInformation
    {

        #region "Fields"



        #endregion

        #region "Constructors"

        public EventLogOutput()
        {

        }

        #endregion

        #region "Properties"



        #endregion

        #region "Methods"



        #endregion

        #region "Abstract/Virtual Methods"



        #endregion

        #region "Inherited Methods"

        public override void Send(InformationReader reader, bool sendBatteryInformation)
        {
            string sSource = "Battery Health Information";
            string sLog = "Application";
            string sEvent = "";
            sEvent += "Hostname:\t\t" + ComputerInformation.MachineName + "\r\n";
            sEvent += "IP Address(es):\t\t" + String.Join("\r\n\t\t\t", ComputerInformation.IPAddresses) + "\r\n\r\n";
            sEvent += "Vendor:\t\t\t" + ComputerInformation.Vendor + "\r\n";
            sEvent += "Type:\t\t\t" + ComputerInformation.Type + "\r\n";
            sEvent += "Service Tag:\t\t" + ComputerInformation.ServiceTag + "\r\n\r\n";
            sEvent += "Design capacity:\t\t" + reader.FullCapacity + " mWh\r\n";
            sEvent += "Last Full Charge capacity:\t" + reader.LastCapacity + " mWh\r\n";
            sEvent += "Full Charge percentage:\t" + reader.Percentage + "%\r\n\r\n\r\n";
            
            if(sendBatteryInformation)
            {
                sEvent += "Details: \r\n";
                sEvent += reader.BatteryInfo;
            }


            if (!EventLog.SourceExists(sSource))
            {
                EventLog.CreateEventSource(sSource, sLog);
            }

            EventLog.WriteEntry(sSource, sEvent, EventLogEntryType.Information);
        }

        #endregion

        #region "Static Methods"



        #endregion

        #region "Operators"



        #endregion
    }
}
