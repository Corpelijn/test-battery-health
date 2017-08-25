using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatteryHistory
{
    class HistoryData
    {
        #region "Fields"

        private List<TimeData> timeData;
        private DateTime date;

        #endregion

        #region "Constructors"

        public HistoryData(byte[] data)
        {
            timeData = new List<TimeData>();

            if (data.Length == 0)
                return;

            Parse(data);
            FillHoles();
        }

        #endregion

        #region "Properties"



        #endregion

        #region "Methods"

        private void FillHoles()
        {
            TimeSpan time = new TimeSpan(0, 0, 0);

            while (time <= new TimeSpan(23, 59, 0))
            {
                TimeData data = timeData.FirstOrDefault(f => f.Time == date.Add(time));
                if (data == null)
                {
                    timeData.Add(new TimeData(date.Add(time)));
                }

                time = time.Add(new TimeSpan(0, 1, 0));
            }

            timeData = timeData.OrderBy(x => x.Time).ToList();
        }

        private void Parse(byte[] data)
        {
            date = DateTime.FromBinary(BitConverter.ToInt64(data, 0));

            Console.WriteLine(date);

            MemoryStream stream = new MemoryStream(data.Skip(8).ToArray());

            StreamReader reader = new StreamReader(stream);

            while(!reader.EndOfStream)
            {
                timeData.Add(new TimeData(date, reader.ReadLine()));
            }

            reader.Close();
            reader.Dispose();

            stream.Close();
            stream.Dispose();
        }

        public TimeData[] GetRange(DateTime from, DateTime to)
        {
            return timeData.Where(x => x.Time >= from && x.Time <= to).ToArray();
        }

        #endregion

        #region "Abstract/Virtual Methods"



        #endregion

        #region "Inherited Methods"

        public override string ToString()
        {
            return date.ToString("dd-MM-yyyy");
        }

        #endregion

        #region "Static Methods"



        #endregion

        #region "Operators"



        #endregion
    }
}
