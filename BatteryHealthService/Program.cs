using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BatteryHealthService
{
    static class Program
    {
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            IntPtr ptr = GetConsoleWindow();
            ShowWindow(ptr,SW_HIDE);

            List<OutputInformation> outputTypes = new List<OutputInformation>();
            OutputInformation consoleOutput = null;

            bool GetFullInfo = false;
            int belowMarge = 100;

            string[] argv = Environment.GetCommandLineArgs();
            bool isQuiet = false;
            
            if (argv.Length > 1)
            {
                for (int i = 0; i < argv.Length; i++)
                {
                    switch (argv[i])
                    {
                        case "-b":
                        case "/b":
                        case "--below":
                            belowMarge = (int)Convert.ToSingle(argv[i + 1]);
                            break;
                        case "/oc":
                        case "-oc":
                        case "--console":
                            outputTypes.Add(new ConsoleOutput());
                            break;
                        case "/oe":
                        case "-oe":
                        case "--email":
                            outputTypes.Add(new EmailOutput(argv[i + 1], argv[i + 2], Convert.ToInt32(argv[i + 3])));
                            i += 3;
                            break;
                        case "-ov":
                        case "/ov":
                        case "--eventlog":
                            outputTypes.Add(new EventLogOutput());
                            break;
                        case "-?":
                        case "/?":
                        case "-h":
                        case "/h":
                        case "--help":
                            isQuiet = false;
                            ShowWindow(ptr, SW_SHOW);
                            PrintHelp();
                            Environment.Exit(0);
                            break;
                        case "-i":
                        case "/i":
                        case "--info":
                            GetFullInfo = true;
                            break;
                        case "-q":
                        case "/q":
                        case "--quiet":
                            isQuiet = true;
                            break;
                        case "-ou":
                        case "/ou":
                        case "--ultimo":
                            outputTypes.Add(new UltimoOutput(argv[i + 1], argv[i + 2], argv[i + 3], argv[i + 4], argv[i + 5], argv[i + 6]));
                            i += 6;
                            break;
                    }
                }
            }

            ShowWindow(ptr, isQuiet ? SW_HIDE : SW_SHOW);

            if (outputTypes.Count == 0)
            {
                consoleOutput = new ConsoleOutput();
                outputTypes.Add(consoleOutput);
            }

            if (outputTypes.Count > 0)
            {
                InformationReader reader = new InformationReader();
                reader.Start();

                foreach (OutputInformation output in outputTypes)
                {
                    if (reader.Percentage <= belowMarge)
                    {
                        output.Send(reader, GetFullInfo);
                    }
                }
            }
        }

        private static void PrintHelp()
        {
            Console.WriteLine("Battery Health Information");
            Console.WriteLine();
            Console.WriteLine("Usage: BatteryHealthInfo.exe [options]");
            Console.WriteLine();
            Console.WriteLine("Options:");
            PrintLine("-b, --below <percentage>", "Percentage must be below <percentage> before a message is send");
            PrintLine("-oc, --console", "Prints the battery health and information to the console output");
            PrintLine("-oe, --email <receiver email> <smtp host> <smtp host port>", "Sends an email containing the battery health");
            PrintLine("-ov, --eventlog", "Stores the battery health information in the event viewer");
            PrintLine("-h, --help", "Prints this help message");
            PrintLine("-i, --info", "Include detailed battery information");
            PrintLine("-q, --quiet", "Show no console window");
            PrintLine("-ou, --ultimo <url_webservice> <jobname> <username> <password> <employeeId>\n\t      <skillgroup>", "Push the battery health info into an Ultimo ticket");

            Console.WriteLine();
            Console.WriteLine("[Press any key to quit]");

            Console.ReadKey();
        }

        private static void PrintLine(string input, string description)
        {
            Console.Write(input);
            if (input.Length < 23)
            {
                int left = (24 - input.Length) / 8;
                for (int i = 0; i < left; i++)
                {
                    Console.Write("\t");
                }
                Console.WriteLine(description);
            }
            else
            {
                Console.WriteLine();
                Console.WriteLine("\t\t" + description);
            }
        }
    }
}
