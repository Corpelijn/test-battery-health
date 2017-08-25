using BatteryHealth.Ultimo;
using System;
using System.Windows.Forms;
using System.Xml.Linq;

namespace BatteryHealthService
{
    class UltimoOutput : OutputInformation
    {

        #region "Fields"

        private string url;
        private string jobName;
        private string username;
        private string password;
        private string employeeId;
        private string skillgroup;

        #endregion

        #region "Constructors"

        public UltimoOutput(string url, string jobName, string username, string password, string employeeId, string skillgroup)
        {
            this.url = url;
            this.jobName = jobName;
            this.username = username;
            this.password = password;
            this.employeeId = employeeId;
            this.skillgroup = skillgroup;
        }

        #endregion

        #region "Properties"



        #endregion

        #region "Methods"

        private XElement CreateElement(string name, params object[] attributes)
        {
            XElement newObject = new XElement(name);
            for (int i = 0; i < attributes.Length; i += 2)
            {
                newObject.Add(new XAttribute(attributes[i].ToString(), attributes[i + 1]));
            }

            return newObject;
        }

        private XElement CreateElement(string name, XElement parent, params object[] attributes)
        {
            XElement newObject = new XElement(name);
            for (int i = 0; i < attributes.Length; i += 2)
            {
                newObject.Add(new XAttribute(attributes[i].ToString(), attributes[i + 1]));
            }

            parent.Add(newObject);

            return newObject;
        }

        private string GetNumeric(string value)
        {
            string output = "";

            foreach (char c in value)
            {
                if (c >= 48 && c <= 57)
                    output += c;
            }

            return output;
        }

        private void ExportConfigFile()
        {
            if(!System.IO.File.Exists(Environment.GetCommandLineArgs()[0] + ".config"))
            {
                System.IO.File.WriteAllText(Environment.GetCommandLineArgs()[0] + ".config" ,BatteryHealth.Properties.Resources.App);
            }
        }

        #endregion

        #region "Abstract/Virtual Methods"



        #endregion

        #region "Inherited Methods"

        public override void Send(InformationReader reader, bool sendBatteryInformation)
        {
            ExportConfigFile();

            SoapConnectorClient client = new SoapConnectorClient("BasicHttpBinding_SoapConnector");
            client.Endpoint.Address = new System.ServiceModel.EndpointAddress(url);

            string description = "";
            description += "<table width=\"80%\">";
            description += "<tr><td>Hostname:</td><td>" + ComputerInformation.MachineName + "</td></tr>";
            description += "<tr><td>IP Address(es):</td><td>" + String.Join("</td></tr><tr><td></td><td>", ComputerInformation.IPAddresses) + "</td></tr>";
            description += "<tr><td colspan=\"2\"></td></tr>";
            description += "<tr><td>Vendor:</td><td>" + ComputerInformation.Vendor + "</td></tr>";
            description += "<tr><td>Type:</td><td>" + ComputerInformation.Type + "</td></tr>";
            description += "<tr><td>Service Tag:</td><td>" + ComputerInformation.ServiceTag + "</td></tr>";
            description += "<tr><td colspan=\"2\"></td></tr>";
            description += "<tr><td>Design capacity:</td><td>" + reader.FullCapacity + " mWh</td></tr>";
            description += "<tr><td>Last Full Charge capacity:</td><td>" + reader.LastCapacity + " mWh</td></tr>";
            description += "<tr><td>Full Charge percentage:</td><td>" + reader.Percentage + "%</td></tr>";
            description += "</table>";

            if (sendBatteryInformation)
            {
                description += "<br/><br/>Extra information:<br/>";
                description += "<textarea width=\"50%\" height=\"300px\">" + reader.BatteryInfo + "<textarea>";
            }

            XElement data_object = CreateElement("Data");
            XElement obj = CreateElement("Object", data_object, "Type", "Job", "Action", "InsertOrUpdate");

            CreateElement("Property", obj, "Name", "Context", "Value", "128");
            CreateElement("Property", obj, "Name", "SkillCategory", "Value", skillgroup);
            CreateElement("Property", obj, "Name", "Status", "Value", "4");
            CreateElement("Property", obj, "Name", "Description", "Value", ComputerInformation.MachineName + ": Bad battery health");
            CreateElement("Property", obj, "Name", "ReportText", "Value", description);
            CreateElement("Property", obj, "Name", "SupportLine", "Value", "FirstLine");
            CreateElement("Property", obj, "Name", "Equipment", "Value", GetNumeric(ComputerInformation.MachineName));
            CreateElement("Property", obj, "Name", "ReportForeignKeyEmployee", "Value", employeeId);
            CreateElement("Property", obj, "Name", "_TELEFOONNUMMER", "Value", "9");

            string xml = data_object.ToString();

            client.ImportData(jobName, username, password, xml);
            client.Close();
        }

        #endregion

        #region "Static Methods"



        #endregion

        #region "Operators"



        #endregion
    }
}
