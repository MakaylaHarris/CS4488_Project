using PERT.Model;
using PERT.View;
using System;
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
            // todo
            Console.WriteLine("Updated");
        }
    }
}
