using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientBattery
{
    class TranslationManager
    {
        private static TranslationManager instance;

        private static string cultureName = CultureInfo.InstalledUICulture.TwoLetterISOLanguageName.ToUpper() == "NL" ? "NL" : "EN";

        private Dictionary<string, string> values;

        private TranslationManager()
        {
            values = new Dictionary<string, string>();
            values.Add("NL_BatteryInformation", "Batterij informatie");
            values.Add("EN_BatteryInformation", "Battery information");

            values.Add("NL_Charging", "opladen");
            values.Add("EN_Charging", "charging");

            values.Add("NL_NotCharging", "wordt niet opgeladen");
            values.Add("EN_NotCharging", "not charging");

            values.Add("NL_remaining", "opgeladen");
            values.Add("EN_remaining", "remaining");

            values.Add("NL_FullyCharged", "volledig opgeladen");
            values.Add("EN_FullyCharged", "fully charged");

            values.Add("NL_PluggedIn", "aan netstroom");
            values.Add("EN_PluggedIn", "plugged in");

            values.Add("NL_NoBattery", "Geen batterij gevonden, aan netstroom");
            values.Add("EN_NoBattery", "No battery found, plugged in");

            values.Add("NL_warning1", "Lage batterij spanning!");
            values.Add("EN_warning1", "Low battery power!");

            values.Add("NL_warning2", "Sluit de laptop aan op de netspanning om op te laden");
            values.Add("EN_warning2", "Please connect the laptop to the powergrid to charge the battery");
        }

        public static string Get(string name)
        {
            if (instance == null)
                instance = new TranslationManager();

            string value;
            instance.values.TryGetValue(cultureName + "_" + name, out value);
            if(value == null)
            {
                return "";
            }

            return value;
        }
    }
}
