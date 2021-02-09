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
        protected bool isComplete;
        protected List<User> workers;

        #region Properties
        public DateTime StartDate
        {
            get => startDate;
            set
            {
                startDate = value;
                Update();
            }
        }

        public DateTime? EndDate
        {
            get => endDate;
            set
            {
                endDate = value;
                Update();
            }
        }

        public string Name
        {
            get => name;
            set
            {
                name = value;
                Update();
            }
        }

        public string Description
        {
            get => description;
            set
            {
                description = value;
                Update();
            }
        }

        public List<User> Workers { get => workers; }
        public int Id { get => id; }
        #endregion

        #region Constructor
        public TimedItem(string name, DateTime start, DateTime? end, string description = "", int id = -1)
        {
            this.name = name;
            startDate = start;
            endDate = end;
            this.description = description;
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
