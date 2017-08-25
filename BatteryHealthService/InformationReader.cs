using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace BatteryHealthService
{
    class InformationReader
    {

        #region "Fields"

        private XElement batteryInfo;
        private float batteryHealth;
        private int fullCap;
        private int lastCap;

        #endregion

        #region "Constructors"

        public InformationReader()
        {

        }

        #endregion

        #region "Properties"

        public float Percentage
        {
            get { return batteryHealth; }
        }

        public string BatteryInfo
        {
            get { return batteryInfo.ToString(); }
        }

        public int FullCapacity
        {
            get { return fullCap; }
        }

        public int LastCapacity
        {
            get { return lastCap; }
        }

        #endregion

        #region "Methods"

        public void Start()
        {
            if (!System.IO.Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\battery"))
                System.IO.Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\battery");

            Process proc = new Process();

            proc.StartInfo.FileName = Environment.SystemDirectory + "\\powercfg.exe";
            proc.StartInfo.Arguments = "-energy -duration 0 -xml -output " + Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\battery\\health.xml";
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.UseShellExecute = false;
            //proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

            proc.Start();
            proc.WaitForExit();

            ParseXML();
        }

        private void ParseXML()
        {
            XElement document = XElement.Parse(File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\battery\\health.xml"));

            XElement batteryInformation = document.Elements().Where(x => x.Name.LocalName == "Troubleshooter" && x.Attribute("guid").Value == "5f159d5d-4dec-4caf-81e5-645d77e05c84").FirstOrDefault();

            XElement details = batteryInformation.Elements().First(x => x.Name.LocalName == "AnalysisLog").Elements().First(x => x.Attribute("guid").Value == "76e4b077-bb50-4000-9563-7f5aa0c9dc26").Elements().First(x => x.Name.LocalName == "Details");

            int designCapacity = Convert.ToInt32(details.Elements().First(x => x.Attribute("guid").Value == "beb3f51a-9d89-42ad-81c4-5f9b7f682fa4").Elements().First(x => x.Name.LocalName == "Value").Value);
            int lastFullCharge = Convert.ToInt32(details.Elements().First(x => x.Attribute("guid").Value == "b42aa79e-8ee8-44ae-8a11-5fe87cf2822b").Elements().First(x => x.Name.LocalName == "Value").Value);

            float batteryHealth = 100f / designCapacity * lastFullCharge;

            this.batteryHealth = batteryHealth;
            this.batteryInfo = batteryInformation;
            this.fullCap = designCapacity;
            this.lastCap = lastFullCharge;
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
