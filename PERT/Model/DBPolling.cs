using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PERT.Model
{
    class DBPolling
    {
        private SynchronizationContext context;
        private Thread thread;
        private bool finished;
        private int refreshTime;
        DBReader receiver;

        public DBPolling(DBReader receiver)
        {
            context = SynchronizationContext.Current;
            this.receiver = receiver;
            thread = new Thread(new ThreadStart(CheckForUpdates));
            thread.IsBackground = true;
            thread.Priority = ThreadPriority.BelowNormal;
            refreshTime = Properties.Settings.Default.RefreshPeriod;
            thread.Start();
        }
        private bool DBIsUpdated()
        {
            // Todo: an actual check of the database, this just makes it update every time
            return true;
        }


        private void CheckForUpdates()
        {
            while (!finished)
            {
                if (DBIsUpdated())
                {
                    this.context.Post(new SendOrPostCallback(Handler), null);
                }
                Thread.Sleep(refreshTime);
            }
        }

        private void Handler(object state)
        {
            this.receiver.OnDBUpdate();
        }
        
    }
}
