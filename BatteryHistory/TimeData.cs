using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BatteryHistory
{
    class TimeData
    {

        #region "Fields"

        private bool noData;
        private DateTime time;
        private int percentage;
        private BatteryChargeStatus status;
        private PowerLineStatus charger;

        #endregion

        #region "Constructors"

        public TimeData(DateTime date, string line)
        {
            string[] data = line.Split(new char[] { ';', '=' });
            data[0] = FixNumeric(data[0]);
            time = date.Add(TimeSpan.Parse(data[0]));

            percentage = Convert.ToInt32(data[1]);
            status = (BatteryChargeStatus)Enum.Parse(typeof(BatteryChargeStatus), data[2]);
            charger = (PowerLineStatus)Enum.Parse(typeof(PowerLineStatus), data[3]);
        }

        public TimeData(DateTime time)
        {
            noData = true;
            this.time = time;

            charger = PowerLineStatus.Unknown;
            status = BatteryChargeStatus.Unknown;
        }

        #endregion

        #region "Properties"

        public DateTime Time
        {
            get { return time; }
        }

        public int Percentage
        {
            get { return percentage; }
        }

        public BatteryChargeStatus Status
        {
            get { return status; }
        }

        public PowerLineStatus Charger
        {
            get { return charger; }
        }

        public bool NoData
        {
            get { return noData; }
        }

        #endregion

        #region "Methods"

        private string FixNumeric(string data)
        {
            string newString = "";
            foreach (char c in data)
            {
                if (c >= 48 || c <= 57 || c == ':')
                    newString += c;
                else
                {
                    continue;
                }
            }
            return newString;
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
