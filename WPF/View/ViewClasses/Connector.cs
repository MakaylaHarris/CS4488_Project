using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SmartPert.View
{
    /// <summary>
    /// Connector class joins two anchors together
    /// Created 3/2/2021 by Robert Nelson
    /// </summary>
    public class Connector
    {
        private bool isConnecting;
        private bool anchor2IsReceiver;
        private Line line;
        private Anchor anchor1;
        private Anchor anchor2;
        private Canvas canvas;
        private List<Line> path;
        private double thickness;
        private Brush brush;

        #region Properties
        public Anchor Anchor1
        {
            get => anchor1;
            set
            {
                anchor1 = value;
                if (anchor1 != null)
                {
                    if(line != null)
                    {
                        Point p = anchor1.Point;
                        line.X1 = p.X;
                        line.Y1 = p.Y;
                    }
                }
            }
        }
        public Anchor Anchor2
        {
            get => anchor2; set
            {
                anchor2 = value;
                if (anchor2 != null && line != null)
                {
                    Point p = anchor2.Point;
                    line.X2 = p.X;
                    line.Y2 = p.Y;
                }
            }
        }

        public Canvas Canvas { get => canvas; }

        public double Thickness { get => thickness; set => thickness = value; }

        public Brush Brush { get => brush; set => brush = value; }
        #endregion

        #region Constructor
        public Connector(Canvas canvas)
        {
            thickness = 2;
            anchor2IsReceiver = true;
            brush = ((SolidColorBrush)Application.Current.FindResource("SecondaryHueMidBrush"));
            this.canvas = canvas;
            path = new List<Line>();
        }

        /// <summary>
        /// This Constructor is for existing connections (Doesn't call Connect on connectable items)
        /// </summary>
        /// <param name="canvas">canvas</param>
        /// <param name="anchor1">origin anchor</param>
        /// <param name="anchor2">receiver anchor</param>
        public Connector(Canvas canvas, Anchor anchor1, Anchor anchor2)
        {
            thickness = 2;
            brush = ((SolidColorBrush)Application.Current.FindResource("SecondaryHueMidBrush"));
            this.canvas = canvas;
            anchor2IsReceiver = true;
            path = new List<Line>();
            line = init_line();
            Anchor1 = anchor1;
            Anchor2 = anchor2;
            CreatePathFromLine(line);
        }

        private Line init_line()
        {
            Line line = new Line();
            line.Stroke = Brush;
            line.StrokeThickness = Thickness;
            line.MouseUp += Line_MouseUp;
            return line;
        }
        #endregion

        #region Private Methods
        private void AddToPath(double x1, double y1, double x2, double y2)
        {
            Line line = init_line();
            line.X1 = x1;
            line.Y1 = y1;
            line.X2 = x2;
            line.Y2 = y2;
            path.Add(line);
            Canvas.Children.Add(line);
        }

        private void ClearLines()
        {
            // Clean up anything old
            foreach (Line l in path)
                Canvas.Children.Remove(l);
            path.Clear();
        }

        private void CreatePathFromLine(Line line)
        {
            ClearLines();
            // Vertical line
            AddToPath(line.X1, line.Y1, line.X1, line.Y2);
            // Horizontal line
            AddToPath(line.X1, line.Y2, line.X2, line.Y2);
        }

        private void Line_MouseUp(object sender, MouseButtonEventArgs e)
        {
            // Most likely the canvas was deactivated and lost mouse tracking if we're connecting
            if (isConnecting)
            {
                return;
            }
            else
            {
                Disconnect();
                //// Find closest endpoint
                //Point end1, end2, point;
                //point = e.GetPosition(Canvas);
                //end1 = Anchor1.Point;
                //end2 = Anchor2.Point;
                //if (Distance(point, end1) > Distance(point, end2))
                //    StartConnecting(Anchor1, anchor2IsReceiver);
                //else
                //    StartConnecting(Anchor2, !anchor2IsReceiver);
            }
        }

        private double Distance(Point p1, Point p2)
        {

            return Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));

        }

        private Anchor GetClosestConnectableAnchor(List<Anchor> anchors, Point p, Anchor origin)
        {
            Anchor best = null;
            double min = 0;
            double dist;
            foreach (Anchor anchor in anchors)
            {
                if (origin.CanConnect(anchor, true) && anchor.CanConnect(origin, false))
                {
                    Point point = anchor.Point;
                    // distance
                    dist = Distance(p, point);
                    if (best == null || dist < min)
                    {
                        best = anchor;
                        min = dist;
                    }
                }
            }
            return best;
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            Point p = Mouse.GetPosition(Canvas);
            line.X2 = p.X;
            line.Y2 = p.Y;
        }

        private void TryConnectAnchor()
        {
            Canvas.ReleaseMouseCapture();
            if (!isConnecting)
                throw new InvalidOperationException("Can't finish connection if it's not connecting");
            CtrlHitTest test = new CtrlHitTest(typeof(Connectable), Canvas);
            Point p = Mouse.GetPosition(Canvas);
            List<DependencyObject> hits = test.Run(p, Anchor1.Connectable as DependencyObject);
             if (hits.Count > 0)
            {
                Connectable c = hits[0] as Connectable;
                Anchor2 = GetClosestConnectableAnchor(c.GetAnchors(), p, Anchor1);
            }
            OnConnectingFinish();
        }

        // Finish up after attempting connection
        private void OnConnectingFinish()
        {
            // If we are connected, send events to the anchors
            if (IsConnected())
            {
                Anchor2.Connect(this, Anchor1.Connectable, anchor2IsReceiver);
                Anchor1.Connect(this, Anchor2.Connectable, !anchor2IsReceiver);
                CreatePathFromLine(line);
                if (Anchor2 != null)
                    Anchor2.AfterConnect(this, Anchor1.Connectable, anchor2IsReceiver);
                if (Anchor1 != null)
                    Anchor1.AfterConnect(this, Anchor2.Connectable, !anchor2IsReceiver);
            }
            Canvas.Children.Remove(line);
            // Canvas events cleanup
            Canvas.ReleaseMouseCapture();
            Canvas.MouseMove -= Canvas_MouseMove;
            Canvas.MouseUp -= Canvas_MouseUp;
            isConnecting = false;
        }

        private void Canvas_MouseUp(object sender, MouseEventArgs e)
        {
            TryConnectAnchor();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Disconnects the connection
        /// </summary>
        /// <returns>true if a disconnection occurred (was not already disconnected)</returns>
        public bool Disconnect(bool notify=true)
        {
            // If connected then disconnect
            if (IsConnected())
            {
                Anchor1.Disconnect(this);
                Anchor2.Disconnect(this);
                if(notify)
                {
                    Anchor1.AfterDisconnect(this, Anchor2.Connectable, !anchor2IsReceiver);
                    Anchor2.AfterDisconnect(this, Anchor1.Connectable, anchor2IsReceiver);
                }
                ClearLines();
                Anchor1 = Anchor2 = null;
                return true;
            }
            else if (isConnecting)
                OnConnectingFinish();
            return false;
        }

        /// <summary>
        /// Is this connection live?
        /// </summary>
        /// <returns>true if connected to both anchors</returns>
        public bool IsConnected()
        {
            return Anchor1 != null && Anchor2 != null;
        }

        /// <summary>
        /// Gets the connected item
        /// </summary>
        /// <param name="from">The origin item</param>
        /// <returns>IConnectable</returns>
        public Connectable GetConnected(Anchor from)
        {
            if (!IsConnected())
                return null;
            else if (from == Anchor1)
                return Anchor2.Connectable;
            else
                return Anchor1.Connectable;
        }


        /// <summary>
        /// Start connecting to the anchor
        /// </summary>
        /// <param name="start">the starting anchor</param>
        /// <param name="isOrigin">treat starting anchor as origin (not receiver)</param>
        public void StartConnecting(Anchor start, bool isOrigin = true)
        {
            Disconnect();
            line = init_line();
            Canvas.Children.Add(line);
            Anchor1 = start;
            isConnecting = true;
            anchor2IsReceiver = isOrigin;
            Canvas.MouseMove += Canvas_MouseMove;
            Canvas.MouseUp += Canvas_MouseUp;
            Canvas.CaptureMouse();
        }

        /// <summary>
        /// Adjusts connector when connected anchor is moved
        /// </summary>
        /// <param name="anchor">anchor</param>
        public void OnAnchorMove(Anchor anchor, Point p)
        {
            if (anchor == Anchor1)
            {
                Anchor1 = anchor;
            }
            else
            {
                Anchor2 = anchor;
            }
        }
        #endregion
    }
}
