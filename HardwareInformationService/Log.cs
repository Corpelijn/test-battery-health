using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardwareInformationService
{
    class Log
    {
        private static StreamWriter logfile;
        private static StreamWriter history;

        private static DateTime lastLogTime = new DateTime(1900, 1, 1);

        public static void Create()
        {
            if (!Directory.Exists("C:\\Battery"))
                Directory.CreateDirectory("C:\\Battery");

            logfile = new StreamWriter("C:\\Battery\\batteryerrors.log");
            history = new StreamWriter(new FileStream("C:\\Battery\\history" + DateTime.Now.ToString("ddMMyyyy") + ".batt", FileMode.Append, FileAccess.Write, FileShare.Read));
        }

        public static void Write(string message)
        {
            logfile.WriteLine(DateTime.Now.ToString() + ": " + message);
            logfile.Flush();
        }

        public static void AddHistory(string values)
        {
            if (lastLogTime.AddMinutes(1) <= DateTime.Now)
            {
                CheckHistoryFiles();

                lock (history)
                {
                    history.WriteLine(DateTime.Now.ToString("HH:mm") + "=" + values);
                    history.Flush();
                    lastLogTime = DateTime.Now;
                }

                GC.Collect();
            }
        }

        public static void Close()
        {
            logfile.Close();
            history.Close();
        }

        public static byte[] GetHistory()
        {
            // Get the files containing the history
            string[] files = System.IO.Directory.GetFiles("C:\\Battery", "*.batt");

            List<byte> allData = new List<byte>();

            lock (history)
            {
                history.Flush();
                history.Close();

                foreach (string file in files)
                {
                    try
                    {
                        FileStream filestream;
                        filestream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read);
                        byte[] data = new byte[filestream.Length];
                        filestream.Read(data, 0, data.Length);
                        filestream.Close();
                        filestream.Dispose();

                        int size = data.Length;


                        // Tell the client we have more than 255 bytes of data
                        allData.Add(255);

                        // Tell the data size
                        allData.AddRange(BitConverter.GetBytes(size + 8));

                        // Tell the date of the data
                        DateTime date = DateTime.ParseExact(System.IO.Path.GetFileNameWithoutExtension(file).Replace("history", ""), "ddMMyyyy", CultureInfo.InvariantCulture);
                        allData.AddRange(BitConverter.GetBytes(date.Ticks));

                        // Add the data
                        allData.AddRange(data);

                    }
                    catch (IOException ex)
                    {
                        Write(ex.Message);
                    }
                }

                history = new StreamWriter(new FileStream("C:\\Battery\\history" + DateTime.Now.ToString("ddMMyyyy") + ".batt", FileMode.Append, FileAccess.Write, FileShare.Read));
            }

            return allData.ToArray();
        }

        private static void CheckHistoryFiles()
        {
            if (!File.Exists("C:\\Battery\\history" + DateTime.Now.ToString("ddMMyyyy") + ".batt"))
            {
                history.Flush();
                history.Close();
                history = new StreamWriter(new FileStream("C:\\Battery\\history" + DateTime.Now.ToString("ddMMyyyy") + ".batt", FileMode.Append, FileAccess.Write, FileShare.Read));
            }

            string[] files = Directory.GetFiles("C:\\Battery", "*.batt");
            foreach (string file in files)
            {
                string name = Path.GetFileNameWithoutExtension(file);
                DateTime date = DateTime.ParseExact(name.Replace("history", ""), "ddMMyyyy", CultureInfo.InvariantCulture);
                if (date.AddDays(14) < DateTime.Now)
                {
                    File.Delete(file);
                }
            }


        }
    }
}
