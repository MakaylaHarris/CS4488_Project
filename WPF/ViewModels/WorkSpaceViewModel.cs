using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using SmartPert.Annotations;
using SmartPert.Model;
using SmartPert.View.ViewClasses;

namespace SmartPert.ViewModels
{
    class WorkSpaceViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<RowData> _rowData;
        private Project _Project;
        private List<string> _headers = new List<string>();
        private String[] _weekDayAbbrev = { "S", "M", "T", "W", "T", "F", "S" };

        /// <summary>
        /// Initializes an instance of the WorkSpaceViewModel class
        /// </summary>
        public WorkSpaceViewModel()
        {
            //Would get the Project we are working with
            this._rowData = new ObservableCollection<RowData>();
            Project p1 = new Project("Makayla", new DateTime(2021, 2, 12, 0, 0, 0, 0), new DateTime(2021, 3, 13, 0, 0, 0, 0), "This is a test project.", new User("Makayla","thommaka@isu.edu", "Pass", "TestUserMakayla"), DateTime.Today, 123);
            _Project = p1;
            _Project.AddTask(new SmartPert.Model.Task("Test1", new DateTime(2021, 2, 12, 0, 0, 0, 0), new DateTime(2021, 2, 15, 0, 0, 0, 0), 0, 0, 0, "This task is cool.", new User("Makayla", "thommaka@isu.edu", "Pass", "TestUserMakayla"), DateTime.Now, p1, 3));
            _Project.AddTask(new SmartPert.Model.Task("Test2", new DateTime(2021, 2, 18, 0, 0, 0, 0), new DateTime(2021, 2, 22, 0, 0, 0, 0), 0, 0, 0, "This task is cool.", new User("Makayla", "thommaka@isu.edu", "Pass", "TestUserMakayla"), DateTime.Now, p1, 3));
            _Project.AddTask(new SmartPert.Model.Task("Test3", new DateTime(2021, 2, 25, 0, 0, 0, 0), new DateTime(2021, 2, 28, 0, 0, 0, 0), 0, 0, 0, "This task is cool.", new User("Makayla", "thommaka@isu.edu", "Pass", "TestUserMakayla"), DateTime.Now, p1, 3));
            _Project.AddTask(new SmartPert.Model.Task("Test4", new DateTime(2021, 2, 28, 0, 0, 0, 0), new DateTime(2021, 3, 13, 0, 0, 0, 0), 0, 0, 0, "This task is cool.", new User("Makayla", "thommaka@isu.edu", "Pass", "TestUserMakayla"), DateTime.Now, p1, 3));
            _headers = GetWeekHeader();
            LoadData();
        }

        #region Properties
        public Project Project
        {
            get { return _Project; }
        }

        public int DaySpan
        {
            get { return (((DateTime)this.Project.EndDate).Date - this.Project.StartDate.Date).Days; }
        }

        public int GridOffset
        {
            get { return (int)Project.StartDate.DayOfWeek; }
        }

        public String[] weekDayAbbrev
        {
            get { return _weekDayAbbrev; }
        }

        public ObservableCollection<RowData> RowData
        {
            get { return _rowData; }
            set
            {
                this._rowData = value;
                OnPropertyChanged("RowData");
                
            }
        }

        public List<string> Headers
        {
            get { return _headers; }
        }

        public bool TodayInProject
        {
            get
            {
                return ( this.Project.StartDate.Date.AddDays(-GridOffset).CompareTo(DateTime.Now) <= 0 && ((DateTime)this.Project.EndDate).CompareTo(DateTime.Now) >= 0 );
            }
        }

        public int TodayCol
        {
            get { if (TodayInProject) { return (DateTime.Now - this.Project.StartDate.AddDays(-GridOffset)).Days + 1; } 
                else { return 0; }
            }
        }

        #endregion

        #region Methods
        /// <summary>
        /// This is the method that creates RowData objects from the Project
        /// </summary>
        public void LoadData()
        {
            RowData num1 = new RowData(Project.Name, GridOffset + 1, DaySpan, true);
            this.RowData.Add(num1);
            for (int i = 0; i < Project.Tasks.Count; i++)
            {
                RowData num2 = new RowData(Project.Tasks[i].Name, (((DateTime)this.Project.Tasks[i].StartDate).Date - ((DateTime)this.Project.StartDate).Date).Days + GridOffset + 1, (((DateTime)this.Project.Tasks[i].EndDate).Date - ((DateTime)this.Project.Tasks[i].StartDate).Date).Days, false);
                this.RowData.Add(num2);
            }
        }

        /// <summary>
        /// Stores a list of Headers for the Dates for the in the format "MMM dd" i.e. Feb 04
        /// </summary>
        /// <returns></returns>
        public List<string> GetWeekHeader()
        {
            List<String> weekHeaders = new List<string>();
            DateTime headerStore = this.Project.StartDate;
            while ((((DateTime)this.Project.EndDate)-headerStore).Days > 0 )
            {
                if (headerStore == Project.StartDate)
                {
                    headerStore = Project.StartDate.AddDays(-(this.GridOffset));
                }
                weekHeaders.Add(String.Format("{0} {1}", headerStore.ToString("MMM"), headerStore.ToString("dd")));
                headerStore = headerStore.AddDays(7);
            }
            return weekHeaders;
        }
        #endregion

        #region INotifyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion 
    }
}
