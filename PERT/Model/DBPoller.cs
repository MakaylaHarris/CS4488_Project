using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PERT.Model
{
    /// <summary>
    /// Polling class that checks for updates from the database
    /// Created 1/28/2021 by Robert Nelson
    /// </summary>
    class DBPoller
    {
        private SynchronizationContext context;
        private Thread thread;
        private bool finished;
        private int refreshTime;
        private DateTime lastUpdated;
        DBReader receiver;

        public DBPoller(DBReader receiver)
        {
            context = SynchronizationContext.Current;
            this.receiver = receiver;
            thread = new Thread(new ThreadStart(CheckForUpdates));
            thread.IsBackground = true;
            thread.Priority = ThreadPriority.BelowNormal;
            refreshTime = Properties.Settings.Default.RefreshPeriod;
            thread.Start();
        }

        private void Handler(object state)
        {
            this.receiver.OnDBUpdate();
        }

        #region Worker Thread
        private bool DBIsUpdated()
        {
            // Todo: an actual check of the database, this just makes it update every time
            // Make sure to use the lastUpdated variable
            return true;
        }

        /// <summary>
        /// Worker thread function that continuously checks for updates.
        /// </summary>
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
        #endregion

        
    }
}
