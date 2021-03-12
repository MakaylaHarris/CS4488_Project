using SmartPert.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SmartPert.View.ViewClasses
{
    /// <summary>
    /// View class for handling a row on the workspace corresponding to a TimedItem
    /// </summary>
    public class RowData
    {
        private string _Name;
        private int _StartDateCol;
        private int _ColSpan;
        private bool _isProject;
        private readonly int endDateSpan;
        private readonly int minEstSpan;
        private readonly int maxEstSpan;
        private readonly int likelyEstSpan;
        private readonly TimedItem timedItem;
        private readonly int subTaskLevel;

        public RowData(string name, int startDateCol, int colSpan, bool isProject, int endDateSpan = -1, int minEstSpan = -1, int maxEstSpan = -1, int likelyEstSpan = -1, 
            TimedItem timedItem = null, int subTaskLevel=0)
        {
            _Name = name;
            _StartDateCol = startDateCol;
            _ColSpan = colSpan;
            _isProject = isProject;
            this.endDateSpan = endDateSpan;
            this.minEstSpan = minEstSpan;
            this.maxEstSpan = maxEstSpan;
            this.likelyEstSpan = likelyEstSpan;
            this.timedItem = timedItem;
            this.subTaskLevel = subTaskLevel;
        }

        public string Name
        {
            get => _Name;
        }

        public int StartDateCol
        {
            get => _StartDateCol;
        }

        public int ColSpan
        {
            get => _ColSpan;
        }

        public bool IsProject
        {
            get => _isProject;
        }
        
        public int EndDateSpan => endDateSpan;

        public int MinEstSpan => minEstSpan;

        public int MaxEstSpan => maxEstSpan;

        public int LikelyEstSpan => likelyEstSpan;

        public TimedItem TimedItem => timedItem;

        public int SubTaskLevel => subTaskLevel;
    }
}
