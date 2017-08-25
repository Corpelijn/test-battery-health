using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatteryHealthService
{
    class ConsoleOutput : OutputInformation
    {

        #region "Fields"



        #endregion

        #region "Constructors"

        public ConsoleOutput() : base()
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
            if (sendBatteryInformation)
            {
                Console.WriteLine(reader.BatteryInfo);
                Console.WriteLine();
            }

            Console.WriteLine("Battery Health: " + reader.Percentage + "%");
        }

        #endregion

        #region "Static Methods"



        #endregion

        #region "Operators"



        #endregion
    }
}
