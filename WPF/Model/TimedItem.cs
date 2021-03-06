using System;
using System.Collections.Generic;

namespace SmartPert.Model
{
    /// <summary>
    /// A named item with a start and end date, not used directly (abstract).
    /// Robert Nelson 1/28/2021
    /// </summary>
    public abstract class TimedItem : IDBItem
    {
        private DateTime startDate;
        private DateTime? endDate;
        private string name;
        private string description;
        protected int id;
        private bool isComplete;
        protected HashSet<User> workers;
        protected User creator;
        protected DateTime creationDate;

        #region Properties
        public User Creator { get => creator; }
        public DateTime CreationDate { get => startDate; }
        public DateTime StartDate
        {
            get => startDate;
            set
            {
                if(startDate != value)
                {
                    startDate = value;
                    Update();
                }
            }
        }

        public DateTime? EndDate
        {
            get => endDate;
            set
            {
                if(endDate != value)
                {
                    endDate = value;
                    if (endDate == null)
                        isComplete = false;
                    else
                        isComplete = true;
                    Update();
                }
            }
        }

        public string Name
        {
            get => name;
            set
            {
                if(name != value)
                {
                    name = value;
                    Update();
                }
            }
        }

        public string Description
        {
            get => description;
            set
            {
                if(description != value)
                {
                    description = value;
                    Update();
                }
            }
        }

        public HashSet<User> Workers { get => workers; }
        public int Id { get => id; }
        public bool IsComplete
        {
            get => isComplete;
            set
            {
                if (value)
                {
                    if (endDate == null)
                        EndDate = DateTime.Now;
                }
                else if (endDate != null)
                    EndDate = null;
                isComplete = value;
            }
        }
        #endregion

        #region Constructor
        public TimedItem(string name, DateTime start, DateTime? end, string description = "", int id = -1, IItemObserver observer=null) :base(observer)
        {
            this.name = name;
            startDate = start;
            endDate = end;
            this.description = description;
            this.id = id;
            isComplete = EndDate != null && EndDate < DateTime.Now;
            creator = Model.Instance.GetCurrentUser();
            workers = new HashSet<User>();
        }
        #endregion

        #region ID stuff
        /// <summary>
        /// Gets the id
        /// </summary>
        /// <returns>id</returns>
        public override int GetHashCode() => id;
        #endregion

        /// <summary>
        /// To string
        /// </summary>
        /// <returns>Name</returns>
        public override string ToString() => Name;

        #region Worker Methods
        abstract public bool AddWorker(User worker);

        abstract public bool RemoveWorker(User worker);


        /// <summary>
        /// Used by DBReader to update workers
        /// </summary>
        /// <param name="updatedWorkers">the updated set</param>
        public void DB_UpdateWorkers(HashSet<User> updatedWorkers)
        {
            if(updatedWorkers == null)
            {
                workers.Clear();
                isUpdated = true;
            }
            else if(!workers.SetEquals(updatedWorkers))
            {
                workers = updatedWorkers;
                isUpdated = true;
            }
        }
        #endregion
    }
}
