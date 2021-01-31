using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading;

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
        private long lastVersion;
        DBReader receiver;

        public DBPoller(DBReader receiver)
        {
            lastVersion = -1;
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
            SqlConnection connection = new SqlConnection(Properties.Settings.Default.ConnectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "dbo.TRACKING_VERSION";
            var tmp = command.Parameters.Add("@ret_value", SqlDbType.BigInt);
            tmp.Direction = ParameterDirection.Output;
            command.ExecuteNonQuery();
            long newVersion = (long) tmp.Value;
            bool result = newVersion > lastVersion;
            connection.Close();
            lastVersion = newVersion;
            return result;
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
