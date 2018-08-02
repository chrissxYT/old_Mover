using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;
using CThreading;

namespace Mover
{
    class Reflector
    {
        public volatile List<string> movingFiles = new List<string>();
        public volatile List<string> movingDrives = new List<string>();
        public volatile List<string> movedDrives = new List<string>();
        public volatile List<Thread> backThreads = new List<Thread>();

        public volatile int i = 0;
        public volatile bool blockmoveadd = false;
        public volatile bool blocklog = false;
        public volatile bool blockthreads = false;
        public volatile bool blockconsole = false;
        public volatile bool blockmove = false;
        public volatile bool run = true;
        public volatile bool stop = false;
        public volatile int currentX = 0, currentY = 0;
        
        public volatile Logger logger;
        public volatile UpdateManager updateManager;
        public volatile Config config;
        private volatile GUI gui;
        public volatile LegacyThreadManager threadManager = new LegacyThreadManager(int.MaxValue, 1000*5);

        public Reflector()
        {
            gui = new GUI(this);
            logger = new Logger(this);
            updateManager = new UpdateManager(this);
            gui.Show();
        }

        public void AddMove(string name)
        {
            BackgroundWorker w = new BackgroundWorker();
            w.RunWorkerCompleted += new RunWorkerCompletedEventHandler((object sender, RunWorkerCompletedEventArgs args) =>
            {
                gui.AddMove(name);
                gui.Update();
            });
            w.RunWorkerAsync();
        }

        public void UpdatePer(string name, int per)
        {
            BackgroundWorker w = new BackgroundWorker();
            w.RunWorkerCompleted += new RunWorkerCompletedEventHandler((object sender, RunWorkerCompletedEventArgs args) =>
            {
                gui.UpdatePer(name, per);
                gui.Update();
            });
            w.RunWorkerAsync();
        }

        public void RemoveMove(string name)
        {
            BackgroundWorker w = new BackgroundWorker();
            w.RunWorkerCompleted += new RunWorkerCompletedEventHandler((object sender, RunWorkerCompletedEventArgs args) =>
            {
                gui.RemoveMove(name);
                gui.Update();
            });
            w.RunWorkerAsync();
        }

        public void IncreasePos()
        {
            if (currentX + MovingDialog.Size.Width < Screen.AllScreens[0].Bounds.Width)
            {
                currentX += MovingDialog.Size.Width;
            }
            else if (currentY + MovingDialog.Size.Height < Screen.AllScreens[0].Bounds.Height)
            {
                currentX = 0;
                currentY += MovingDialog.Size.Height;
            }
            else
            {
                currentX = 0;
                currentY = 0;
            }
        }
    }
}