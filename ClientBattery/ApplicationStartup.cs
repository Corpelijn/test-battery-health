using ClientBattery.Properties;
using IconParsing;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClientBattery
{
    class ApplicationStartup : ApplicationContext
    {
        private NotifyIcon trayIcon;

        private BatteryInfoRequester requester;

        private static ApplicationStartup instance;

        private ImageParse parse;

        private BatteryChargeStatus currentState;
        public string currentText;
        public string warningText;
        public Image baseImage;

        public ApplicationStartup()
        {
            instance = this;

            requester = new BatteryInfoRequester();
            string[] argv = Environment.GetCommandLineArgs();
            if (argv.Length > 1)
            {
                requester.Hostname = argv[1];
            }

            if (!requester.HasHostname || !requester.Open())
            {
                Environment.Exit(0);
                return;
            }
            requester.Start();

            SystemEvents.SessionSwitch += OnSessionSwitch;

            parse = new ImageParse(0, Resources.battery);

            // Set up a timer to trigger every minute to call the garbage collector  
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = 60000;
            timer.Elapsed += OnCleanup;
            timer.Start();

            // Initialize Tray Icon
            trayIcon = new NotifyIcon()
            {
                Icon = parse.GetIcon(8),
                Text = TranslationManager.Get("BatteryInformation"),
                //ContextMenu = new ContextMenu(new MenuItem[] {
                //    new MenuItem("Exit", Exit)
                //}),
                Visible = true
            };
            trayIcon.Click += TrayIcon_Click;
            trayIcon.Visible = false;
            trayIcon.Visible = true;
        }

        private void OnCleanup(object sender, System.Timers.ElapsedEventArgs e)
        {
            GC.Collect();
        }

        private void TrayIcon_Click(object sender, EventArgs e)
        {
            Info i = new Info();
            i.ShowDialog();

            i.Dispose();
        }

        private void OnSessionSwitch(object sender, SessionSwitchEventArgs e)
        {
            if (e.Reason == SessionSwitchReason.SessionLock || e.Reason == SessionSwitchReason.SessionLogoff)
            {
                trayIcon.Visible = false;

                requester.SendMessage("stop");
                requester.Stop();

                Application.Exit();
            }
        }

        void Exit(object sender, EventArgs e)
        {
            try
            {
                ManagementScope scope = new ManagementScope(@"root\ccm");

            }
            catch (System.Management.ManagementException ex)
            {
                MessageBox.Show("Failed to connect\n" + ex.Message);
            }
        }

        public static void UpdateIcon(int percentage, BatteryChargeStatus status, PowerLineStatus charger)
        {
            int index = (int)(percentage / 11.5f);

            if (charger == PowerLineStatus.Online)
            {
                index += 13;
            }

            if ((status & BatteryChargeStatus.Low) == BatteryChargeStatus.Low && charger == PowerLineStatus.Offline)
                index = 9;
            else if ((status & BatteryChargeStatus.Critical) == BatteryChargeStatus.Critical && charger == PowerLineStatus.Offline)
                index = 10;
            else if ((status & BatteryChargeStatus.NoSystemBattery) == BatteryChargeStatus.NoSystemBattery)
                index = 11;

            try
            {
                instance.trayIcon.Icon = instance.parse.GetIcon(index);
            }
            catch (Exception)
            {

            }

            string text = percentage + "% " + TranslationManager.Get("remaining");
            string msg = "";
            if (percentage == 100 && charger == PowerLineStatus.Online)
                msg = TranslationManager.Get("FullyCharged");
            else if (charger == PowerLineStatus.Online)
            {
                msg = TranslationManager.Get("PluggedIn");

                if ((status & BatteryChargeStatus.Charging) == BatteryChargeStatus.Charging)
                    msg += ", " + TranslationManager.Get("Charging");
                else
                    msg += ", " + TranslationManager.Get("NotCharging");
            }

            if ((status & BatteryChargeStatus.NoSystemBattery) == BatteryChargeStatus.NoSystemBattery)
            {
                instance.currentText = TranslationManager.Get("NoBattery");
            }
            else
            {
                instance.currentText = text + (msg == "" ? "" : (" (" + msg + ")"));
            }
            instance.trayIcon.Text = instance.currentText;

            if (instance.currentState != status && ((status & BatteryChargeStatus.Low) == BatteryChargeStatus.Low ||
                (status & BatteryChargeStatus.Critical) == BatteryChargeStatus.Critical) && (status & BatteryChargeStatus.Charging) != BatteryChargeStatus.Charging)
            {
                instance.trayIcon.ShowBalloonTip(3000, TranslationManager.Get("warning1"), TranslationManager.Get("warning2"), ToolTipIcon.Warning);

            }
            if (((status & BatteryChargeStatus.Low) == BatteryChargeStatus.Low || (status & BatteryChargeStatus.Critical) == BatteryChargeStatus.Critical)
                && charger == PowerLineStatus.Offline)
            {
                instance.warningText = TranslationManager.Get("warning1") + "\n" + TranslationManager.Get("warning2");
            }
            else
                instance.warningText = "";

            try
            {
                instance.baseImage = instance.parse.GetBitmap(index);
            }
            catch (Exception)
            {

            }

            instance.currentState = status;

            GC.Collect();
        }

        public static ApplicationStartup Get()
        {
            return instance;
        }
    }
}
