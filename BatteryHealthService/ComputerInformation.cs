using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BatteryHealthService
{
    class ComputerInformation
    {

        #region "Fields"

        private string name;
        private string vendor;
        private string serviceTag;
        private string machineName;
        private string[] ipAddresses;

        private static ComputerInformation instance;

        #endregion

        #region "Constructors"



        #endregion

        #region "Properties"

        private static ComputerInformation INSTANCE
        {
            get
            {
                if (instance == null)
                {
                    instance = new ComputerInformation();
                    instance.Get();
                }

                return instance;
            }
        }

        public static string Type
        {
            get { return INSTANCE.name; }
        }

        public static string Vendor
        {
            get { return INSTANCE.vendor; }
        }

        public static string ServiceTag
        {
            get { return INSTANCE.serviceTag; }
        }

        public static string MachineName
        {
            get { return INSTANCE.machineName; }
        }

        public static string[] IPAddresses
        {
            get { return INSTANCE.ipAddresses; }
        }

        #endregion

        #region "Methods"

        private void Get()
        {
            string wmiQuery = string.Format("SELECT Name, IdentifyingNumber, Vendor FROM Win32_ComputerSystemProduct");

            ManagementObjectSearcher searcher = new ManagementObjectSearcher(wmiQuery);
            ManagementObjectCollection retObjectCollection = searcher.Get();

            foreach (ManagementObject retObject in retObjectCollection)
            {
                name = retObject["Name"].ToString();
                vendor = retObject["Vendor"].ToString();
                serviceTag = retObject["IdentifyingNumber"].ToString();
            }

            searcher.Dispose();

            machineName = Environment.MachineName;
            ipAddresses = Dns.GetHostEntry(machineName).AddressList.Select(x => x.ToString()).ToArray();
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
