using SmartPert.Command;
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
    public abstract class TimedItem : IDBItem
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
                    AfterStartDateChanged(startDate);
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
                    AfterEndDateChanged(EndDate);
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
                    AfterLikelyDurationChanged(mostLikelyDuration);
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
                    AfterMaxDurationChanged(maxDuration);
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
                    AfterMinDurationChanged(value);
                    Update();
                }
            }
        }

        public DateTime LikelyDate { get => StartDate.AddDays(LikelyDuration); set => LikelyDuration = (value - StartDate).Days; }

        public DateTime MinEstDate { get => StartDate.AddDays(MinDuration); set => MinDuration = (value - StartDate).Days; }
        public DateTime MaxEstDate { get => StartDate.AddDays(MaxDuration); set => MaxDuration = (value - StartDate).Days; }
        #endregion

        #region Property Callbacks
        protected virtual void AfterMaxDurationChanged(int newValue) { }
        protected virtual void AfterLikelyDurationChanged(int newValue) { }
        protected virtual void AfterMinDurationChanged(int newValue) { }
        protected virtual void AfterStartDateChanged(DateTime newValue) { }
        protected virtual void AfterEndDateChanged(DateTime? newValue) { }

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
        /// Get Task by name
        /// </summary>
        /// <param name="name">string name of task</param>
        /// <returns>task if found, or null</returns>
        public Task GetTask(string name)
        {
            foreach (Task t in tasks)
                if (t.Name == name)
                    return t;
            return null;
        }

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
                StartDate = startDate.AddDays(days);
                
                if (endDate != null)
                    EndDate = ((DateTime)endDate).AddDays(days);
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

        #region Child Methods
        /// <summary>
        /// Updates based on all of child's properties
        /// </summary>
        /// <param name="child">child</param>
        public void OnChild_Change(TimedItem child)
        {
            OnChild_CompletedDateChange(child.EndDate);
            OnChild_MaxEstDateChange(child.MaxEstDate);
            OnChild_LikelyDateChange(child.LikelyDate);
            OnChild_MinEstDateChange(child.MinEstDate);
            OnChild_StartDateChange(child.StartDate);
        }
        /// <summary>
        /// When a child task has a start date change, shifts the parent start date if the child's start date is earlier.
        /// </summary>
        /// <param name="newStart">the new date</param>
        public void OnChild_StartDateChange(DateTime newStart)
        {
            if (newStart < StartDate)
            {
                int days = (StartDate - newStart).Days;
                if (GetType() == typeof(Task))
                    new EditTaskCmd(this as Task, Name, newStart, EndDate, LikelyDuration + days, MaxDuration + days, MinDuration + days, Description).Run();
                else if (GetType() == typeof(Project))
                    new EditProjectCmd(this as Project, Name, newStart, EndDate, Description, LikelyDuration + days, MaxDuration + days, MinDuration + days).Run();
            }
        }

        /// <summary>
        /// Shifts the likely date if the child's likely date exceeds its parent
        /// </summary>
        /// <param name="newLikelyDate">date</param>
        public void OnChild_LikelyDateChange(DateTime newLikelyDate)
        {
            if (newLikelyDate > LikelyDate)
            {
                int newLikely = (newLikelyDate - StartDate).Days;
                if (GetType() == typeof(Task))
                    new EditTaskCmd(this as Task, Name, startDate, EndDate, newLikely, MaxDuration, MinDuration, Description).Run();
                else if (GetType() == typeof(Project))
                    new EditProjectCmd(this as Project, Name, startDate, EndDate, Description, newLikely, MaxDuration, MinDuration).Run();

            }
        }

        /// <summary>
        /// Shifts the max estimated date if the child's maximum exceeds its parent
        /// </summary>
        /// <param name="newMaxDate">New maximum estimated date</param>
        public void OnChild_MaxEstDateChange(DateTime newMaxDate)
        {
            if (newMaxDate > MaxEstDate)
            {
                int newMax = (newMaxDate - StartDate).Days;
                if (GetType() == typeof(Task))
                    new EditTaskCmd(this as Task, Name, startDate, EndDate, LikelyDuration, newMax, MinDuration, Description).Run();
                else if (GetType() == typeof(Project))
                    new EditProjectCmd(this as Project, Name, startDate, EndDate, Description, LikelyDuration, newMax, MinDuration).Run();

            }
        }

        /// <summary>
        /// Shifts the Min estimated date if the child's exceeds this
        /// </summary>
        /// <param name="newMinDate">date</param>
        public void OnChild_MinEstDateChange(DateTime newMinDate)
        {
            if (newMinDate > MinEstDate)
            {
                int newMin = (newMinDate - StartDate).Days;
                if (GetType() == typeof(Task))
                    new EditTaskCmd(this as Task, Name, startDate, EndDate, LikelyDuration, MaxDuration, newMin, Description).Run();
                else if (GetType() == typeof(Project))
                    new EditProjectCmd(this as Project, Name, startDate, EndDate, Description, LikelyDuration, MaxDuration, newMin).Run();

            }
        }

        /// <summary>
        /// Shifts the completed end date if the child's exceeds this
        /// </summary>
        /// <param name="completedDate">Child's end date</param>
        public void OnChild_CompletedDateChange(DateTime? completedDate)
        {
            if (completedDate != null && EndDate != null && completedDate > EndDate)
            {
                if (GetType() == typeof(Task))
                    new EditTaskCmd(this as Task, Name, startDate, completedDate, LikelyDuration, MaxDuration, MinDuration, Description).Run();
                else if (GetType() == typeof(Project))
                    new EditProjectCmd(this as Project, Name, startDate, completedDate, Description, LikelyDuration, MaxDuration, MinDuration).Run();

            }
        }
        #endregion
    }
}
