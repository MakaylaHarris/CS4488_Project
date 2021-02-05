using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading;

namespace WPF.Model
{
    /// <summary>
    /// Polling class that checks for updates from the database
    /// Created 1/28/2021 by Robert Nelson
    /// </summary>
    class DBPoller
    {
        #region private members
        private SynchronizationContext context;
        private Thread thread;
        private bool running;
        private int refreshTime;
        private long lastVersion;
        private DBReader receiver;
        #endregion

        /// <summary>
        /// Polling thread running
        /// </summary>
        public bool Running { get => running; }

        public DBPoller(DBReader receiver)
        {
            lastVersion = -1;
            running = false;
            context = SynchronizationContext.Current;
            this.receiver = receiver;
            thread = new Thread(new ThreadStart(CheckForUpdates));
            thread.IsBackground = true;
            thread.Priority = ThreadPriority.BelowNormal;
            refreshTime = Properties.Settings.Default.RefreshPeriod;
        }

        #region Public Methods
        /// <summary>
        /// Starts the poller
        /// </summary>
        public void Start()
        {
            if (!running)
            {
                running = true;
                thread.Start();
            }
        }

        /// <summary>
        /// Resets the last version number which will trigger an update if running
        /// </summary>
        public void Reset()
        {
            lastVersion = -1;
        }

        /// <summary>
        /// Stops the polling thread
        /// </summary>
        public void Stop()
        {
            running = false;
            Reset();
        }
        #endregion

        #region Worker Thread
        private void Handler(object state)
        {
            receiver.OnDBUpdate();
        }

        /// <summary>
        /// Checks if the Database is updated since last call
        /// </summary>
        /// <returns>true if updated</returns>
        private bool DBIsUpdated()
        {
            SqlConnection connection = new SqlConnection(Properties.Settings.Default.ConnectionString);
            Console.Write(connection.ConnectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "dbo.TRACKING_VERSION";
            var tmp = command.Parameters.Add("@ret_value", SqlDbType.BigInt);
            tmp.Direction = ParameterDirection.Output;
            command.ExecuteNonQuery();
            long newVersion = (long)tmp.Value;
            connection.Close();
            bool result = newVersion > lastVersion;
            lastVersion = newVersion;
            return result;
        }

        /// <summary>
        /// Worker thread function that continuously checks for updates.
        /// </summary>
        private void CheckForUpdates()
        {
            try
            {
                while (running)
                {
                    if (DBIsUpdated())
                    {
                        context.Post(new SendOrPostCallback(Handler), null);
                    }
                    Thread.Sleep(refreshTime);
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e);
                context.Post(new SendOrPostCallback(Handler), e);
            }
            running = false;
        }
        #endregion


    }
}
