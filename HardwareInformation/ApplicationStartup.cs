using HardwareInformation.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HardwareInformation
{
    class ApplicationStartup : ApplicationContext
    {
        private NotifyIcon trayIcon;

        private BatteryInfoReader reader;
        private NetworkRequester sender;

        private static ApplicationStartup instance;

        public static ApplicationStartup Current
        {
            get { return instance; }
        }

        public BatteryInfoReader Battery
        {
            get { return reader; }
        }

        public ApplicationStartup()
        {
            instance = this;

            reader = new BatteryInfoReader();
            reader.Start();
            sender = new NetworkRequester();
            sender.Start();

            // Initialize Tray Icon
            trayIcon = new NotifyIcon()
            {
                Icon = Icon.FromHandle(Resources.battery_default.GetHicon()),
                Text = "Battery information",
                ContextMenu = new ContextMenu(new MenuItem[] {
                    new MenuItem("Exit", Exit),
                    new MenuItem("Status", Status)
                }),
                Visible = true
            };
        }

        void Exit(object sender, EventArgs e)
        {
            // Hide tray icon, otherwise it will remain shown until user mouses over it
            trayIcon.Visible = false;

            reader.Stop();
            this.sender.Stop();

            Application.Exit();
        }

        void Status(object sender, EventArgs e)
        {
            MessageBox.Show(
                "Charge:\t" + reader.Percentage * 100 + "%\n" +
                "Time Left:\t" + TimeSpan.FromMinutes(reader.TimeLeft).ToString() + "\n" +
                "Status:\t" + reader.Status.ToString() + "\n" +
                "Charger:\t" + reader.ChargerStatus.ToString()
                );
        }
    }
}
