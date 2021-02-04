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
        private SynchronizationContext context;
        private Thread thread;
        private bool running;
        private int refreshTime;
        private long lastVersion;
        private DBReader receiver;

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
        public void Start()
        {
            if (!running)
            {
                running = true;
                thread.Start();
            }
        }

        public void Reset()
        {
            lastVersion = -1;
        }

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
