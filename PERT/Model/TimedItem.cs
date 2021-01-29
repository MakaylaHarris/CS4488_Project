using System;
using System.Collections.Generic;

namespace PERT.Model
{
    /// <summary>
    /// A named item with a start and end date, not used directly (abstract).
    /// Robert Nelson 1/28/2021
    /// </summary>
    public abstract class TimedItem : IDBItem
    {
        private DateTime startDate;
        private DateTime endDate;
        private string name;
        private string description;
        private uint id;
        protected bool isComplete;
        private List<User> workers;

        #region Properties
        public DateTime StartDate { get => startDate; 
            set { 
                startDate = value;
                Update(); 
            } }

        public DateTime EndDate { get => endDate;
            set
            {
                endDate = value;
                Update();
            } }

        public string Name { get => name;
            set
            {
                name = value;
                Update();
            }
        }

        public string Description { get => description; 
            set
            {
                description = value;
                Update();
            }
        }

        public List<User> Workers { get => workers; }
        public uint Id { get => id; }
        #endregion

        #region Constructor
        public TimedItem(string name, DateTime start, DateTime end, string description = "", int id = -1)
        {
            this.Name = name;
            this.StartDate = start;
            this.EndDate = end;
            this.Description = description;
            if (id >= 0)
            {
                this.id = (uint)id;
            }
            else
            {
                this.id = (uint)Insert();
            }
            isComplete = EndDate != null && EndDate < DateTime.Now;
        }
        #endregion

        #region Worker Methods
        public void AddWorker(User worker)
        {
            workers.Add(worker);
        }

        public void RemoveWorker(User worker)
        {
            workers.Remove(worker);
        }
        #endregion
    }
}
