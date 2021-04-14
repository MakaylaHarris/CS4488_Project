using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SmartPert.View
{
    /// <summary>
    /// Objects that can be connected
    /// </summary>
    public abstract class Connectable : UserControl
    {
        private List<Anchor> anchors;
        private Canvas canvas;

        #region Properties
        /// <summary>
        /// Connector Visibility
        /// </summary>
        public bool AnchorsVisible
        {
            get => anchors[0].IsVisible;
            set
            {
                System.Windows.Visibility visible = value ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                foreach (Anchor a in anchors)
                    a.Visibility = visible;
            }
        }
        public Canvas Canvas
        {
            get => canvas;
            set
            {
                canvas = value;
                foreach (Anchor a in anchors)
                    a.Canvas = canvas;
            }
        }
        #endregion


        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public Connectable()
        {
            anchors = new List<Anchor>();
            MouseEnter += Connectable_MouseEnter;
            MouseLeave += Connectable_MouseLeave;
        }
        #endregion

        #region Private Methods
        private void Connectable_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if(!IsMouseOver)
                AnchorsVisible = false;
        }

        private void Connectable_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            AnchorsVisible = true;
        }
        #endregion

        #region public Methods
        /// <summary>
        /// Determines if it can connect to target
        /// the default rule is it can't connect to itself or existing connections
        /// </summary>
        /// <param name="target">connectable target</param>
        /// <returns>true if it can</returns>
        virtual public bool CanConnect(Connectable target, Anchor targetAnchor, bool isOrigin)
        {
            if (target == this)
                return false;
            foreach (Anchor anchor in anchors)
                if (anchor.IsConnectedTo(target))
                    return false;
            return true;
        }


        /// <summary>
        /// When a successful connection is made
        /// </summary>
        /// <param name="sender">The anchor sending signal</param>
        /// <param name="target">Connected item</param>
        /// <param name="isReceiver">True if it received the connection, false if it originated it</param>
        public abstract void OnConnect(Anchor sender, Connectable target, bool isReceiver);

        /// <summary>
        /// When a disconnect is made
        /// </summary>
        /// <param name="sender">The anchor sending</param>
        /// <param name="target">Connected item that is disconnecting</param>
        /// <param name="isReceiver">Did not originate from here if true</param>
        public abstract void OnDisconnect(Anchor sender, Connectable target, bool isReceiver);

        /// <summary>
        /// Get the anchors from a connectable item
        /// </summary>
        /// <returns>List of anchors</returns>
        public List<Anchor> GetAnchors() => anchors;

        /// <summary>
        /// Removes all connection lines
        /// </summary>
        public void DisconnectAllLines()
        {
            foreach (Anchor a in GetAnchors())
                a.DisconnectAll();
        }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Add an anchor
        /// </summary>
        /// <param name="a">anchor</param>
        protected void AddAnchor(Anchor a)
        {
            anchors.Add(a);
            if(canvas != null)
                a.Canvas = canvas;
            a.Connectable = this;
        }

        /// <summary>
        /// Call whenever control is moved
        /// </summary>
        protected virtual void OnMove()
        {
            foreach (Anchor a in anchors)
                a.OnMove();
        } 
        #endregion
    }
}
