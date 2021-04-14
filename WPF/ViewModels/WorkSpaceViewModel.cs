using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;
using SmartPert.Annotations;
using SmartPert.Model;
using SmartPert.View;
using SmartPert.View.Pages;
using SmartPert.View.ViewClasses;

namespace SmartPert.ViewModels
{
    /// <summary>
    /// The interaction logic for the workspace view
    /// Implemented by: Makayla Linnastruth
    /// </summary>
    public class WorkSpaceViewModel : INotifyPropertyChanged, IViewModel
    {
        private ObservableCollection<RowData> _rowData;
        private ObservableCollection<ToolTipData> _tooltipData;
        private ToolTipProjectData _projectTooltip;
        private Project _Project;
        private WorkSpace workspace;
        private List<string> _headers = new List<string>();
        private String[] _weekDayAbbrev = { "S", "M", "T", "W", "T", "F", "S" };

        /// <summary>
        /// Initializes an instance of the WorkSpaceViewModel class
        /// </summary>
        public WorkSpaceViewModel(WorkSpace workSpace)
        {
            this.workspace = workSpace;
            //Would get the Project we are working with
            this._rowData = new ObservableCollection<RowData>();
            Model.Model model = Model.Model.GetInstance(this);
            _Project = model.GetProject();
            this._tooltipData = new ObservableCollection<ToolTipData>();
            _headers = GetWeekHeader();
            LoadData();
            LoadToolTipData();
            LoadProjectData();
        }

        #region Properties
        public Project Project
        {
            get { return _Project; }
        }

        public int DaySpan
        {
            get {
                int days = (((DateTime)this.Project.CalculateLastProjectDate()).Date - this.Project.StartDate.Date).Days;
                if (days <= 0)      /* Ensure natural number for span */
                    return 1;
                return days;
            }
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

        /// <summary>
        /// Property for Task Tooltip collection.
        /// Created 2/25/2021 by Tyler Kness-Miller
        /// </summary>
        public ObservableCollection<ToolTipData> TooltipData
        {
            get { return _tooltipData; }
            set
            {
                this._tooltipData = value;
                OnPropertyChanged("TooltipData");

            }
        }

        /// <summary>
        /// Property for Project Tooltip.
        /// Created 2/25/2021 by Tyler Kness-Miller
        /// </summary>
        public ToolTipProjectData ProjectTooltip
        {
            get { return _projectTooltip; }
            set { this._projectTooltip = value; }
        }

        public List<string> Headers
        {
            get { return _headers; }
        }

        public bool TodayInProject
        {
            get
            {
                DateTime min = this.Project.StartDate.Date.AddDays(-GridOffset);
                DateTime max = Project.StartDate.AddDays(DaySpan);
                return (min.CompareTo(DateTime.Now) <= 0 && max.CompareTo(DateTime.Now) >= 0 );
            }
        }

        public int TodayCol
        {
            get { if (TodayInProject) { return (DateTime.Now - this.Project.StartDate.AddDays(-GridOffset)).Days; }
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
            RowData num1 = new RowData(Project.Name, GridOffset, DaySpan, true);
            this.RowData.Add(num1);
            foreach (Model.Task task in Project.SortedTasks)
            {
                RowData num2 = new RowData(task.Name,
                    startDateCol: (((DateTime)task.StartDate).Date - ((DateTime)this.Project.StartDate).Date).Days + GridOffset,
                    colSpan: (((DateTime)task.CalculateLastTaskDate()).Date - ((DateTime)task.StartDate).Date).Days,
                    isProject: false,
                    endDateSpan: task.EndDate != null ? ((DateTime)task.EndDate - task.StartDate).Days : -1,
                    minEstSpan: task.MinDuration,
                    maxEstSpan: task.MaxDuration - task.LikelyDuration,
                    likelyEstSpan: task.LikelyDuration - task.MinDuration,
                    timedItem: task,
                    subTaskLevel: GetSubLevel(task)
                    );
                this.RowData.Add(num2);
            }
        }

        private int GetSubLevel(Task t)
        {
            if (t.ParentTask == null)
                return 0;
            return GetSubLevel(t.ParentTask) + 1;
        }

        /// <summary>
        /// A method that created ToolTip data classes that hold data for each task and adds them to a collection. Collection indexes are the same for this and RowData starting at index 1.
        /// Created 2/25/2021 by Tyler Kness-Miller
        /// </summary>
        public void LoadToolTipData()
        {
            for(int i = 0; i < Project.Tasks.Count; i++)
            {
                //Task could also have been a reference to System.Threading.Task, so below declaration was necessary.
                SmartPert.Model.Task t = Project.SortedTasks[i];

                ToolTipData data = new ToolTipData(t.Name, t.StartDate, t.EndDate, t.LikelyDuration, t.MaxDuration, t.MinDuration, t.Description);
                this.TooltipData.Add(data);
            }
        }

        /// <summary>
        /// A method that sets the Project data for  its tooltip.
        /// Created 2/25/2021 by Tyler Kness-Miller
        /// </summary>
        public void LoadProjectData()
        {
            ProjectTooltip = new ToolTipProjectData(Project.Name, Project.StartDate, Project.EndDate, Project.Description);
        }

        /// <summary>
        /// Stores a list of Headers for the Dates for the in the format "MMM dd" i.e. Feb 04
        /// </summary>
        /// <returns></returns>
        public List<string> GetWeekHeader()
        {
            List<String> weekHeaders = new List<string>();
            DateTime headerStore = this.Project.StartDate.AddDays(-GridOffset);
            for(int i = 0, max = (DaySpan + 7 - 1 + GridOffset) / 7; i < max; i++)
            {
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

        public void OnDisconnect()
        {
            // do nothing
        }

        public void OnModelUpdate(Project p)
        {
            _Project = p;
            _headers = GetWeekHeader();
            this.RowData.Clear();
            TooltipData.Clear();
            LoadData();
            LoadToolTipData();
            LoadProjectData();
            workspace.OnWorkspaceModelUpdate(this);
        }
        #endregion
    }
}
