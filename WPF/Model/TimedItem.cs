using System;
using System.Collections.Generic;

namespace SmartPert.Model
{
    /// <summary>
    /// A named item with a start and end date, not used directly (abstract).
    /// Robert Nelson 1/28/2021
    /// </summary>
    public abstract class TimedItem : IDBItem, IEquatable<TimedItem>
    {
        private DateTime startDate;
        private DateTime? endDate;
        private string name;
        private string description;
        protected int id;
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
            this.id = id;
            isComplete = EndDate != null && EndDate < DateTime.Now;
            workers = new List<User>();
        }

        public TimedItem(TimedItem item, int id = -1)
        {
            name = item.Name;
            startDate = item.startDate;
            endDate = item.endDate;
            description = item.description;
            creator = item.creator;
            creationDate = item.creationDate;
            id = (id == -1) ? item.Id : id;
            isComplete = EndDate != null && EndDate < DateTime.Now;
            workers = new List<User>();
        }
        #endregion

        #region ID stuff
        public void UpdateId(int id) => this.id = id;

        /// <summary>
        /// Equals override that uses ids
        /// </summary>
        /// <param name="obj">object</param>
        /// <returns>true if same type and ids equal</returns>
        public bool Equals(TimedItem obj)
        {
            return obj != null && id == obj.Id;
        }

        /// <summary>
        /// Equals override that uses ids
        ///  Created 2/16/2021 by Robert Nelson
        /// </summary>
        /// <param name="obj">object</param>
        /// <returns>true if same type and ids equal</returns>
        public override bool Equals(object obj) => obj.GetType().Equals(GetType()) && Equals((TimedItem) obj);

        /// <summary>
        /// Gets the id
        /// </summary>
        /// <returns>id</returns>
        public override int GetHashCode() => id;
        #endregion

        #region Worker Methods
        abstract public bool AddWorker(User worker, bool UpdateDB=true);

        abstract public bool RemoveWorker(User worker, bool UpdateDB=true);
        #endregion
    }
}
