using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace BatteryHealthService
{
    class EmailOutput : OutputInformation
    {

        #region "Fields"

        private string mailAddress;
        private string smtpHost;
        private int smtpPort;

        #endregion

        #region "Constructors"

        public EmailOutput(string mailAddress, string smtpHost, int smtpPort) : base()
        {
            this.mailAddress = mailAddress;
            this.smtpHost = smtpHost;
            this.smtpPort = smtpPort;
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
            MailMessage mail = new MailMessage("BatteryInfoLaptops@lzr.nl", mailAddress);
            SmtpClient client = new SmtpClient(smtpHost, smtpPort);

            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = true;

            mail.Subject = Environment.MachineName + ": Bad battery health";
            mail.Body += "The following computer may need a replacement battery because of a low health:\r\n\r\n";
            mail.Body += "Hostname:\t\t\t" + ComputerInformation.MachineName + "\r\n";
            mail.Body += "IP Address(es):\t\t\t" + String.Join("\r\n\t\t\t\t", ComputerInformation.IPAddresses) + "\r\n\r\n";
            mail.Body += "Vendor:\t\t\t" + ComputerInformation.Vendor + "\r\n" ;
            mail.Body += "Type:\t\t\t\t" + ComputerInformation.Type + "\r\n";
            mail.Body += "Service Tag:\t\t\t" + ComputerInformation.ServiceTag + "\r\n\r\n";
            mail.Body += "Design capacity:\t\t" + reader.FullCapacity + " mWh\r\n";
            mail.Body += "Last Full Charge capacity:\t" + reader.LastCapacity + " mWh\r\n";
            mail.Body += "Full Charge percentage:\t" + reader.Percentage + "%";

            if (sendBatteryInformation)
            {
                MemoryStream stream = new MemoryStream(Encoding.ASCII.GetBytes(reader.BatteryInfo));
                mail.Attachments.Add(new Attachment(stream, "batteryInformation.xml"));
            }

            client.Send(mail);
        }

        #endregion

        #region "Static Methods"



        #endregion

        #region "Operators"



        #endregion
    }
}
