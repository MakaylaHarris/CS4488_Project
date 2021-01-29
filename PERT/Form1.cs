using PERT.Model;
using PERT.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PERT
{
    public partial class Form1 : Form, IViewModel
    {
        private IModel model;
        public Form1()
        {
            model = new Model.Model(this);
            InitializeComponent();
        }

        public void OnModelUpdate(Project p)
        {
            throw new NotImplementedException();
        }
    }
}
