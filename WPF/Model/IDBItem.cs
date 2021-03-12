using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace SmartPert.Model
{
    /// <summary>
    /// Observers of IDBItem
    /// </summary>
    public interface IItemObserver
    {
        /// <summary>
        /// Called when item is updated
        /// </summary>
        /// <param name="item">the item in question</param>
        void OnUpdate(IDBItem item);

        /// <summary>
        /// Called when item is deleted or outdated
        /// </summary>
        /// <param name="item">the item</param>
        void OnDeleted(IDBItem item);
    }

    /// <summary>
    /// Objects inherit from IDBItem if they are updated, inserted, and deleted from the sql database
    /// Created 1/28/2021 by Robert Nelson
    /// Edited 2/22/2021 by Robert Nelson to make more robust (cannot perform actions on deleted items)
    /// </summary>
    public abstract class IDBItem
    {
        private SqlConnection connection;
        private bool isOutdated;    // Out of date object (not tracked by model)
        private bool isDeleted;     // Set when item is gone from database
        protected bool isUpdated;     // Set during updating, if it has updated
        private bool isUpdating;    // Set during an update
        private List<IItemObserver> observers;

        #region Constructor
        /// <summary>
        /// Constructor for item
        /// </summary>
        public IDBItem(IItemObserver observer=null)
        {
            observers = new List<IItemObserver>();
            if (observer != null)
                Subscribe(observer);
        }

        /// <summary>
        /// All children should call this as the last step of their constructor
        /// This adds the item to the DBReader, which maintains the latest update of items
        /// </summary>
        /// <param name="insert">insertion</param>
        /// <param name="track">Track this as the latest object in dbreader</param>
        public void PostInit(bool insert=true, bool track=true)
        {
            if (insert)  // insert first to set primary key
                Insert();
            if (track)
            {
                try
                {
                    DBReader.Instance.TrackItem(this);
                } catch(ArgumentException)
                {
                    throw new DuplicateKeyError("Error creating " + this.ToString() + ", duplicate item exists with same primary key");
                }
            }
            else
                isOutdated = true;
        }
        #endregion

        #region Exceptions
        /// <summary>
        ///  Exception for trying to use deleted items
        /// </summary>
        public class ItemDeletedException : Exception
        {
            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="s">error message</param>
            public ItemDeletedException(string s) : base(s) { }
        }

        /// <summary>
        /// Insertion Exception
        /// </summary>
        public class InsertionError : Exception
        {
            public InsertionError(string message) : base(message)
            {
            }
        }

        /// <summary>
        /// Thrown when a duplicate item exists with the same primary key
        /// </summary>
        public class DuplicateKeyError : Exception
        {
            public DuplicateKeyError(string message) : base(message)
            {
            }
        }

        /// <summary>
        /// Update Exception
        /// </summary>
        public class UpdateError : Exception { }

        /// <summary>
        /// Delete Exception
        /// </summary>
        protected class DeletionError : Exception { }
        #endregion

        #region Properties

        /// <summary>
        /// Has the item been deleted? Can not undelete!
        /// </summary>
        public bool IsDeleted { get => isDeleted;
            set 
            {
                if (!value)
                    throw new Exception("Unable to undo delete of item!");
                isDeleted = true;
                NotifyDelete();
            }
        }

        /// <summary>
        /// Is it outdated?
        /// </summary>
        public bool IsOutdated
        {
            get => isOutdated;
            set
            {
                isOutdated = value;
                NotifyDelete();     // sends the same notification as deleted
            }
        }


        #endregion

        #region Private Methods
        private void NotifyDelete()
        {
            foreach (IItemObserver observer in observers)
                observer.OnDeleted(this);
            observers.Clear();      // No more updates
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Subscribe for updates
        /// </summary>
        /// <param name="observer">observer</param>
        public void Subscribe(IItemObserver observer)
        {
            if (isDeleted || isOutdated)
                throw new ItemDeletedException("Can't subscribe to deleted item!");
            if (!observers.Contains(observer))
                observers.Add(observer);
        }

        /// <summary>
        /// UnSubscribe from updates
        /// </summary>
        /// <param name="observer">observer</param>
        public void UnSubscribe(IItemObserver observer)
        {
            observers.Remove(observer);
        }

        /// <summary>
        /// Attempts to delete item from database
        /// </summary>
        /// <throws>ItemDeletedException, DeletionError</throws>
        public void Delete()
        {
            if (IsDeleted || IsOutdated)
                throw new ItemDeletedException("Unable to delete deleted item");
            PerformDelete();
            IsDeleted = true;
        }

        /// <summary>
        /// Parses in data from the database
        /// </summary>
        /// <param name="reader">data reader</param>
        public void ParseUpdate(SqlDataReader reader)
        {
            isUpdating = true;
            if(PerformParse(reader))
                isUpdated = true;
            isUpdating = false;
        }


        /// <summary>
        /// After a refresh, send out updates to subscribers
        /// </summary>
        public void PostUpdate()
        {
            if(isUpdated)
            {
                NotifyUpdate();
                isUpdated = false;
            }
        }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Notify an update occurred
        /// </summary>
        protected void NotifyUpdate()
        {
            /* Notify newest subscribers first, this is important so the generic model update is sent last! */
            for(int i = observers.Count - 1; i >= 0; i--)
                observers[i].OnUpdate(this);
        }


        /// <summary>
        /// Attempts to Insert item in database
        /// </summary>
        /// <throws>ItemDeletedException, InsertionError</throws>
        protected void Insert()
        {
            if (IsDeleted || IsOutdated)
                throw new ItemDeletedException("Unable to insert deleted item");
            PerformInsert();
        }

        /// <summary>
        /// Updates the database
        /// </summary>
        /// <throws>ItemDeletedException, UpdateError</throws>
        protected void Update()
        {
            if (isUpdating)  // Triggered while updating from database (no update needed)
                return;
            if (IsDeleted || IsOutdated)
                throw new ItemDeletedException("Unable to update deleted item");
            PerformUpdate();
            NotifyUpdate();
        }

        /// <summary>
        /// Open connection with the Default connection string. Use CloseConnection when done.
        /// </summary>
        /// <param name="query">Sql query to generate command</param>
        /// <returns>SqlCommand to add params and/or execute</returns>
        protected SqlCommand OpenConnection(string query)
        {
            connection = new SqlConnection(Properties.Settings.Default.ConnectionString);
            connection.Open();
            return new SqlCommand(query, connection);
        }

        /// <summary>
        /// Executes the query and closes connection (no sql params)
        /// </summary>
        /// <param name="query">The sql statement</param>
        /// <returns>the ExecuteNonQuery result</returns>
        protected int ExecuteSql(string query)
        {
            SqlCommand cmd = OpenConnection(query);
            int result = cmd.ExecuteNonQuery();
            connection.Close();
            return result;
        }

        /// <summary>
        /// Closes sql connection
        /// </summary>
        protected void CloseConnection()
        {
            connection.Close();
        }
        #endregion

        #region Abstract Methods
        abstract protected void PerformUpdate();
        abstract protected int PerformInsert();
        abstract protected void PerformDelete();

        /// <summary>
        /// Parses in data from database
        /// </summary>
        /// <param name="reader">sql data reader (open)</param>
        /// <returns>true if updated</returns>
        abstract public bool PerformParse(SqlDataReader reader);
        #endregion
    }
}
