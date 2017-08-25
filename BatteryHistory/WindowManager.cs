using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BatteryHistory
{
    class WindowManager : ApplicationContext
    {

        #region "Fields"

        private List<Form> forms;
        private Thread thread;
        private bool stopThread;

        private static WindowManager instance;

        #endregion

        #region "Constructors"

        public WindowManager()
        {
            thread = new Thread(RunningProcess);
            stopThread = false;
            forms = new List<Form>();

            instance = this;
        }

        #endregion

        #region "Properties"

        private static WindowManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new WindowManager();
                }
                return instance;
            }
        }

        #endregion

        #region "Methods"

        public void Begin()
        {
            if (!thread.IsAlive)
            {
                stopThread = false;
                thread.Start();
            }
        }

        private void RunningProcess()
        {
            List<Form> toRemove = new List<Form>();
            while (!stopThread)
            {
                lock (forms)
                {
                    foreach (Form form in forms)
                    {
                        if (form == null)
                            break;
                        // Check if any of tyhe forms has already been closed.
                        // If a window has been closed, destory all the handles to it
                        if (form.Tag != null && form.Tag.ToString() == "Closed")
                        {
                            toRemove.Add(form);
                        }
                    }
                }

                while (toRemove.Count > 0)
                {
                    if (toRemove[0].IsDisposed)
                    {
                        forms.Remove(toRemove[0]);
                        GC.Collect();
                        toRemove.RemoveAt(0);
                    }
                }

                if(forms.Count == 0)
                {
                    StartIntroWindow();
                }

                Application.DoEvents();

                Thread.Sleep(10);
            }
        }

        private void OnClose(object sender, FormClosedEventArgs e)
        {
            Form form = (Form)sender;
            form.Tag = "Closed";
            form.Dispose();
        }

        #endregion

        #region "Abstract/Virtual Methods"



        #endregion

        #region "Inherited Methods"



        #endregion

        #region "Static Methods"

        public static ApplicationContext Start()
        {
            StartIntroWindow();
            Instance.Begin();
            
            return Instance;
        }

        public static void Exit()
        {
            Instance.stopThread = true;
            Environment.Exit(0);
        }

        public static void StartIntroWindow()
        {
            Form1 f1 = new Form1();
            f1.FormClosed += Instance.OnClose;
            f1.Show();
            lock (Instance.forms)
            {
                Instance.forms.Add(f1);
            }
        }

        public static void StartDetailWindow(string hostname, bool destroyData)
        {
            GraphView view = new GraphView(hostname, destroyData);
            view.FormClosed += Instance.OnClose;
            view.Show();
            lock (Instance.forms)
            {
                Instance.forms.Add(view);
            }
        }

        public static void StartMultiview()
        {
            Multiview mv = new Multiview();
            mv.FormClosed += Instance.OnClose;
            mv.Show();
            lock (Instance.forms)
            {
                Instance.forms.Add(mv);
            }
        }

        #endregion

        #region "Operators"



        #endregion
    }
}
