using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SmartPert.View.ViewClasses
{
    /// <summary>
    /// A preview for moving task control
    /// Created 3/9/2021 by Robert Nelson
    /// </summary>
    public class TaskControlPreview
    {
        private readonly Canvas canvas;
        private readonly Point initial;
        private readonly Rectangle rect1;
        private readonly Rectangle rect2;
        private readonly Rectangle rect3;
        private Point location;

        public Point Location { get => location; 
            set {
                location = value;
                double x = initial.X - location.X;
                double y = initial.Y - location.Y;
                UpdateRect(rect1, x, y);
                UpdateRect(rect2, x, y);
                UpdateRect(rect3, x, y);
            } }

        public TaskControlPreview(Canvas canvas, Rectangle rect1, Rectangle rect2, Rectangle rect3, Point initial)
        {
            this.canvas = canvas;
            this.initial = initial;
            this.rect1 = CreatePreviewOfRect(rect1);
            this.rect2 = CreatePreviewOfRect(rect2);
            this.rect3 = CreatePreviewOfRect(rect3);
        }

        private Rectangle CreatePreviewOfRect(Rectangle r)
        {
            Rectangle ret = new Rectangle();
            ret.Fill = r.Fill;
            ret.Stroke = r.Stroke;
            ret.Fill.Opacity = ret.Fill.Opacity * 2;
            ret.Width = r.Width;
            ret.Height = r.Height;
            ret.StrokeThickness = r.StrokeThickness;
            Point dest = r.TransformToAncestor(canvas).Transform(new Point(0, 0));
            canvas.Children.Add(ret);
            Canvas.SetLeft(ret, dest.X);
            Canvas.SetTop(ret, dest.Y);
            ret.RenderTransform = new TranslateTransform();
            return ret;
        }

        private void UpdateRect(Rectangle rectangle, double x, double y)
        {
            TranslateTransform transform = rectangle.RenderTransform as TranslateTransform;
            transform.X = x;
            transform.Y = y;
        }
    }
}
