using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace SmartPert.View
{

    /// <summary>
    /// Class to detect hit test elements in a visual tree
    /// Created 3/2/2021 by Robert Nelson
    /// </summary>
    public class CtrlHitTest
    {
        private List<DependencyObject> items;
        private Type filter;
        private Visual canvas;

        public CtrlHitTest(Type filter, Visual canvas)
        {
            this.filter = filter;
            items = new List<DependencyObject>();
            this.canvas = canvas;
        }

        public List<DependencyObject> Run(Point pt, DependencyObject exclude = null)
        {
            items.Clear();
            VisualTreeHelper.HitTest(canvas, new HitTestFilterCallback(MyHitTestFilter), new HitTestResultCallback(MyHitTestResult), new PointHitTestParameters(pt));
            if (exclude != null)
                items.Remove(exclude);
            return items;
        }

        // Filter the hit test values for each object in the enumeration.
        public HitTestFilterBehavior MyHitTestFilter(DependencyObject o)
        {
            // Test for the object value you want to filter, must be a subclass of filter.
            if (o.GetType() != filter && !o.GetType().IsSubclassOf(filter))
            {
                // Visual object is NOT part of hit test results enumeration.
                return HitTestFilterBehavior.ContinueSkipSelf;
            }
            else
            {
                items.Add(o);
                // Visual object is part of hit test results enumeration.
                return HitTestFilterBehavior.Continue;
            }
        }

        // Return the result of the hit test to the callback.
        public HitTestResultBehavior MyHitTestResult(HitTestResult result)
        {
            //items.Add(result.VisualHit);

            // Set the behavior to return visuals at all z-order levels.
            return HitTestResultBehavior.Continue;
        }

    }
}
