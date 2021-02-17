using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SmartPert.View.ViewClasses
{
    class RowData
    {
        private string _Name;
        private int _StartDateCol;
        private int _ColSpan;
        private bool _isProject;

        public RowData(string name, int startDateCol, int endDateCol, bool isProject)
        {
            _Name = name;
            _StartDateCol = startDateCol;
            _ColSpan = endDateCol;
            _isProject = isProject;
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
    }
}
