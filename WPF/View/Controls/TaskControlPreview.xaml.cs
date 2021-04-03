using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SmartPert.View.Controls
{
    /// <summary>
    /// Interaction logic for TaskControlPreview.xaml
    /// </summary>
    public partial class TaskControlPreview : UserControl
    {
        private readonly Canvas canvas;
        private readonly Point initial;
        private Point initialOnCanvas;
        private readonly TaskControl original;
        private Point location;

        public Point Location
        {
            get => location;
            set
            {
                location = value;
                double x = initial.X - location.X;
                double y = initial.Y - location.Y;
                Canvas.SetLeft(this, initialOnCanvas.X - x);
                Canvas.SetTop(this, initialOnCanvas.Y - y);
            }
        }

        public Canvas Canvas
        {
            get => canvas;
        }

        public TaskControlPreview(TaskControl control, Canvas canvas, Point initial)
        {
            InitializeComponent();
            this.initial = initial;
            this.original = control;
            this.canvas= canvas;
            initialOnCanvas = original.TransformToAncestor(canvas).Transform(new Point(0, 0));
            this.Width = original.Width;
            canvas.Children.Add(this);
            Location = initialOnCanvas;
            double margin = CreatePreviewOfRect(control.MinRect, MinRect, 0);
            //CreatePreviewOfRect(control.LikelyRect, LikelyRect, margin);
            //CreatePreviewOfRect(control.MaxRect, MaxRect, margin);
        }

        private double CreatePreviewOfRect(Rectangle originalRect, Rectangle preview, double margin)
        {
            preview.Fill = originalRect.Fill;
            preview.Stroke = originalRect.Stroke;
            //Point p = originalRect.TransformToAncestor(original).Transform(new Point(0, 0));
            //if (margin != 0)
            //{
            //    preview.RenderTransform = (Transform) originalRect.TransformToAncestor(original);
            //    //preview.Margin = new Thickness(p.X, 0, 0, 0);
            //    preview.Width = originalRect.ActualWidth;
            //}
            //else    // First Rect, transform to 0
            //{
                preview.Width = original.ActualWidth;   // Ensure we take up the entire control space
            //}
            preview.Height = originalRect.ActualHeight;
            preview.StrokeThickness = originalRect.StrokeThickness;
            return 0;
        }

    }
}
