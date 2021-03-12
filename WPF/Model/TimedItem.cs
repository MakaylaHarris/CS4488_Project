using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace SmartPert.Model
{
    /// <summary>
    /// A named item with a start and end date, not used directly (abstract).
    /// Robert Nelson 1/28/2021
    /// Updated 3/8/2021 pulled up mostLikelyDuration, maxDuration, minDuration, and tasks.
    /// </summary>
    public abstract class TimedItem : IDBItem, IItemObserver
    {
        protected DateTime startDate;
        protected DateTime? endDate;
        protected string name;
        protected string description;
        protected int id;
        private bool isComplete;
        protected HashSet<User> workers;
        protected User creator;
        protected DateTime creationDate;
        protected HashSet<Task> tasks;
        protected int mostLikelyDuration;
        protected int maxDuration;
        protected int minDuration;

        #region Properties
        public User Creator { get => creator; }
        public DateTime CreationDate { get => startDate; }
        public virtual DateTime StartDate
        {
            get => startDate;
            set
            {
                if (startDate != value)
                {
                    if (endDate != null && startDate > endDate)
                        throw new ArgumentOutOfRangeException("StartDate", "Start date must be before end date");
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
                if (endDate != value)
                {
                    if (value != null && startDate > value)
                        throw new ArgumentOutOfRangeException("EndDate", "Start date must be before end date");
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
                if (name != value)
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
                if (description != value)
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
                isComplete = EndDate != null;
            }
        }
        public HashSet<Task> Tasks { get => tasks; }

        public int LikelyDuration
        {
            get => mostLikelyDuration;
            set
            {
                if(mostLikelyDuration != value)
                {
                    if (value < 0)
                        value = 0;
                    if (value < minDuration)
                        minDuration = value;
                    else if (value > MaxDuration)
                        maxDuration = value;
                    mostLikelyDuration = value;
                    Update();
                }
            }
        }

        public int MaxDuration { get => maxDuration; 
            set {
                if(maxDuration != value)
                {
                    if (value < 0)
                        value = 0;
                    if (value < mostLikelyDuration)
                        LikelyDuration = value;
                    maxDuration = value;
                    Update();
                }
            } 
        }

        public int MinDuration
        {
            get => minDuration; set
            {
                if (minDuration != value)
                {
                    if (value < 0)
                        value = 0;
                    if (value > LikelyDuration)
                        LikelyDuration = value;
                    minDuration = value;
                    Update();
                }
            }
        }

        public DateTime LikelyDate { get => StartDate.AddDays(LikelyDuration); set => LikelyDuration = (value - StartDate).Days; }

        public DateTime MinEstDate { get => StartDate.AddDays(MinDuration); set => MinDuration = (value - StartDate).Days; }
        public DateTime MaxEstDate { get => StartDate.AddDays(MaxDuration); set => MaxDuration = (value - StartDate).Days; }
        #endregion

        #region Constructor
        public TimedItem(string name, DateTime start, DateTime? end, string description = "", int id = -1, IItemObserver observer=null,
            int duration = 0, int maxDuration = 0, int minDuration = 0) 
            :base(observer)
        {
            this.name = name;
            startDate = start;
            endDate = end;
            this.description = description;
            this.id = id;
            isComplete = EndDate != null && EndDate < DateTime.Now;
            creator = Model.Instance.GetCurrentUser();
            workers = new HashSet<User>();
            tasks = new HashSet<Task>();
            if (duration == 0)
                mostLikelyDuration = 1;
            else
                mostLikelyDuration = duration;
            if (maxDuration <= 0)
                this.maxDuration = mostLikelyDuration;
            else
            {
                if (maxDuration < mostLikelyDuration)
                    throw new ArgumentOutOfRangeException("MaxDuration", maxDuration, "Must be greater than likely duration!");
                this.maxDuration = maxDuration;
            }
            if (minDuration <= 0)
                this.minDuration = mostLikelyDuration;
            else
            {
                if (minDuration > mostLikelyDuration)
                    throw new ArgumentOutOfRangeException("MinDuration", minDuration, "Must be less than likely duration!");
                this.minDuration = minDuration;
            }

        }
        #endregion

        #region Public Methods


        #region ID stuff
        public override bool Equals(object obj)
        {
            return obj != null && GetType() == obj.GetType() && ((TimedItem)obj).id == id;
        }

        /// <summary>
        /// Gets the task by id
        /// </summary>
        /// <param name="id">task id</param>
        /// <returns></returns>
        public Task GetTaskById(int id)
        {
            foreach (Task task in Tasks)
                if (task.id == id)
                    return task;
            return null;
        }

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

        /// <summary>
        /// Shifts the task start and end by an amount
        /// </summary>
        /// <param name="days">number of days to shift</param>
        public virtual void Shift(int days)
        {
            if(days != 0)
            {
                startDate = startDate.AddDays(days);
                if (endDate != null)
                    endDate = ((DateTime)endDate).AddDays(days);
                Update();
            }
        }

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

        /// <summary>
        /// When a subtask is updated it calls this, increasing this item's estimates as needed to match up with the subtask's estimates
        /// </summary>
        /// <param name="item">subtask</param>
        public virtual void OnUpdate(IDBItem item)
        {
            // Recalculate durations, only shifting upward as needed
            TimedItem sub = item as TimedItem;
            if (sub.MaxEstDate > MaxEstDate)
                MaxEstDate = sub.MaxEstDate;
            if (sub.LikelyDate > LikelyDate)
                LikelyDate = sub.LikelyDate;
            if (sub.MinEstDate > MinEstDate)
                MinEstDate = sub.MinEstDate;
            if (sub.StartDate < StartDate)
                StartDate = sub.StartDate;
        }

        /// <summary>
        /// When one of it's children is deleted
        /// </summary>
        /// <param name="item">task</param>
        public void OnDeleted(IDBItem item)
        {
            Task task = item as Task;
            if (tasks.Remove(task))
                NotifyUpdate();
        }

        /// <summary>
        /// Parses the TimedItem data
        /// </summary>
        /// <param name="reader">Sql Data reader</param>
        /// <returns>true if updated</returns>
        public override bool PerformParse(SqlDataReader reader)
        {
            string name = (string)reader["Name"];
            DateTime start = (DateTime)reader["StartDate"];
            DateTime? end = DBFunctions.DateCast(reader, "EndDate");
            int likely = (int)reader["MostLikelyEstDuration"];
            int max = DBFunctions.IntCast(reader, "MaxEstDuration", likely);
            int min = DBFunctions.IntCast(reader, "MinEstDuration", likely);
            string desc = DBFunctions.StringCast(reader, "Description");
            if(name != Name || start != StartDate || end != EndDate || likely != LikelyDuration || max != MaxDuration|| min != MinDuration || desc != Description)
            {
                this.name = name;
                startDate = start;
                endDate = end;
                mostLikelyDuration = likely;
                maxDuration = max;
                minDuration = min;
                description = desc;
                return true;
            }
            return false;
        }
        #endregion
    }
}
