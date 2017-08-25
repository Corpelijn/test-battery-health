using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace BatteryHistory
{
    class HistoryManager
    {

        #region "Fields"

        private static HistoryManager instance = new HistoryManager();

        private Dictionary<string, List<HistoryData>> data;
        private Dictionary<string, List<IPushNewInfo>> listeners;
        private System.Timers.Timer timer1;

        private List<string> toRemove;

        #endregion

        #region "Constructors"

        private HistoryManager()
        {
            data = new Dictionary<string, List<HistoryData>>();
            toRemove = new List<string>();
            listeners = new Dictionary<string, List<IPushNewInfo>>();

            timer1 = new System.Timers.Timer();
            timer1.Interval = 5000;
            timer1.Elapsed += new ElapsedEventHandler(this.UpdateInformation);
            timer1.Start();
        }

        #endregion

        #region "Properties"

        public static HistoryManager INSTANCE
        {
            get { return instance; }
        }

        #endregion

        #region "Methods"

        private void UpdateInformation(object sender, ElapsedEventArgs e)
        {
            foreach (string hostname in data.Keys.ToArray())
            {
                if (toRemove.Contains(hostname.ToUpper()))
                {
                    data.Remove(hostname.ToUpper());
                    toRemove.Remove(hostname.ToUpper());
                    continue;
                }

                BatteryInfoRequester requester = new BatteryInfoRequester(hostname.ToUpper());
                lock (this.data)
                {
                    Reset(hostname.ToUpper());
                    requester.Start();

                    while (!requester.IsDone)
                    {
                        Thread.Sleep(10);
                    }

                    requester.Stop();
                }

                listeners.Where(x => x.Key == hostname.ToUpper()).Select(y => y.Value).FirstOrDefault()?.ForEach(f => f.PushInformation());
            }
        }

        public void Reset(string hostname)
        {
            if (data.Keys.Contains(hostname.ToUpper()))
                data[hostname.ToUpper()].Clear();

            GC.Collect();
        }

        public void AddHistory(string hostname)
        {
            if (!data.Keys.Contains(hostname.ToUpper()))
            {
                data.Add(hostname.ToUpper(), new List<HistoryData>());
            }
        }

        public void AddHistory(string hostname, byte[] msg)
        {
            if (msg.Length == 0)
                return;

            if (!data.Keys.Contains(hostname.ToUpper()))
            {
                data.Add(hostname.ToUpper(), new List<HistoryData>());
            }
            data[hostname.ToUpper()].Add(new HistoryData(msg));
        }

        public TimeData[] GetData(string hostname, DateTime from, DateTime to)
        {
            if (!this.data.Keys.Contains(hostname.ToUpper()))
                return new TimeData[] { };
            List<TimeData> data = new List<TimeData>();
            lock (this.data)
            {
                foreach (HistoryData dat in this.data[hostname.ToUpper()])
                {
                    data.AddRange(dat.GetRange(from, to));
                }
            }

            return data.ToArray();
        }

        public void Remove(string hostname)
        {
            toRemove.Add(hostname.ToUpper());
        }

        public string[] GetDates(string hostname)
        {
            List<string> dates = new List<string>();
            lock (this.data)
            {
                if (!this.data.Keys.Contains(hostname.ToUpper()))
                    return new string[] { "No data found" };
                foreach (HistoryData dat in this.data[hostname.ToUpper()])
                {
                    dates.Add(dat.ToString());
                }
            }

            return dates.ToArray();
        }

        public void AddListener(IPushNewInfo listener, string hostname)
        {
            if (!listeners.Keys.Contains(hostname.ToUpper()))
                listeners.Add(hostname.ToUpper(), new List<IPushNewInfo>());

            listeners[hostname.ToUpper()].Add(listener);
        }

        public void RemoveListener(IPushNewInfo listener, string hostname)
        {
            if (listeners.Keys.Contains(hostname.ToUpper()))
            {
                listeners[hostname.ToUpper()].Remove(listener);

                if (listeners[hostname.ToUpper()].Count == 0)
                    listeners.Remove(hostname.ToUpper());
            }
        }

        #endregion

        #region "Abstract/Virtual Methods"



        #endregion

        #region "Inherited Methods"



        #endregion

        #region "Static Methods"



        #endregion

        #region "Operators"



        #endregion
    }
}
