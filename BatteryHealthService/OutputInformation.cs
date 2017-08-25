using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatteryHealthService
{
    abstract class OutputInformation
    {

        #region "Fields"



        #endregion

        #region "Constructors"

        public OutputInformation()
        {

        }

        #endregion

        #region "Properties"



        #endregion

        #region "Methods"



        #endregion

        #region "Abstract/Virtual Methods"

        public abstract void Send(InformationReader reader, bool sendBatteryInformation);

        #endregion

        #region "Inherited Methods"



        #endregion

        #region "Static Methods"



        #endregion

        #region "Operators"



        #endregion
    }
}
