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
        private int id;
        private bool isComplete;
        protected List<User> workers;
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

        public List<User> Workers { get => workers; }
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
        public TimedItem(string name, DateTime start, DateTime? end, string description = "", 
            User creator = null, DateTime? creationTime = null, int id = -1)
        {
            this.name = name;
            startDate = start;
            endDate = end;
            this.description = description;
            this.creator = creator;
            this.creationDate = creationTime != null ? (DateTime) creationTime : DateTime.Now;
            if (id >= 0)
            {
                this.id = (int)id;
            }
            else
            {
                this.id = (int)Insert();
            }
            isComplete = EndDate != null && EndDate < DateTime.Now;
            workers = new List<User>();
        }
        #endregion

        #region Worker Methods
        abstract public void AddWorker(User worker, bool UpdateDB=true);

        abstract public void RemoveWorker(User worker, bool UpdateDB=true);
        #endregion
    }
}
