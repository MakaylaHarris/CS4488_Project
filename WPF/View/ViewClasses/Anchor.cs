using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SmartPert.View
{

    /// <summary>
    /// Anchor for connectors to connect to
    /// Created 3/2/2021 by Robert Nelson
    /// </summary>
    public class Anchor : Button
    {
        private List<Connector> connectors;
        private Connectable connectable;
        private Canvas canvas;

        #region Properties
        public Point Point { get =>
                TransformToAncestor(Canvas as Visual).Transform(new Point(ActualWidth / 2, ActualHeight / 2));
        }
        public Canvas Canvas { get => canvas;
            set {
                canvas = value;
            }
        }
        public Connectable Connectable { get => connectable;
            set {
                connectable = value;
            } }
        #endregion

        public Anchor()
        {
            Content = "o";
            Background = Brushes.Transparent;
            BorderThickness = new Thickness(0);
            Click += StartConnect;
        }

        #region Public Methods
        public bool IsConnectedTo(Connectable connectable)
        {
            if (connectors != null)
                foreach (Connector connector in connectors)
                    if (connector.GetConnected(this) == connectable)
                        return true;
            return false;
        }

        /// <summary>
        /// Connects the connector, (Does not check if it can!)
        /// </summary>
        /// <param name="connector">connector</param>
        /// <param name="connected">the target connectable</param>
        /// <param name="isReceiver">If this object received the connection (did not start it)</param>
        public void Connect(Connector connector, Connectable connected, bool isReceiver)
        {
            if (connectors == null)
                connectors = new List<Connector>();
            connectors.Add(connector);
            Connectable.OnConnect(this, connected, isReceiver);
        }

        /// <summary>
        /// Connects this anchor to the receiver anchor
        /// </summary>
        /// <param name="anchor">the anchor to connect to</param>
        /// <param name="isReceiver">Whether this anchor is the receiver of connection</param>
        public void Connect(Anchor anchor, bool isReceiver = false)
        {
            Connector connector;
            if (!isReceiver)
                connector = new Connector(Canvas, this, anchor);
            else
                connector = new Connector(Canvas, anchor, this);
            if (connectors == null)
                connectors = new List<Connector>();
            connectors.Add(connector);
            if (anchor.connectors == null)
                anchor.connectors = new List<Connector>();
            anchor.connectors.Add(connector);
        }


        /// <summary>
        /// Disconnects the connection
        /// </summary>
        /// <param name="connector">connector</param>
        /// <param name="connected">connectable that's disconnecting</param>
        /// <param name="isReceiver">is receiver</param>
        public void Disconnect(Connector connector, Connectable connected, bool isReceiver)
        {
            if (connectors != null)
                connectors.Remove(connector);
            Connectable.OnDisconnect(this, connected, isReceiver);
        }

        /// <summary>
        /// Disconnect silently
        /// </summary>
        /// <param name="connector">connector to drop</param>
        public void Disconnect(Connector connector)
        {
            if (connectors != null)
                connectors.Remove(connector);
        }

        public void DisconnectAll()
        {
            if (connectors != null)
                while (connectors.Count > 0)
                    connectors[0].Disconnect(false);
        }

        /// <summary>
        /// Determines if two anchors can be connected
        /// </summary>
        /// <param name="anchor">anchor</param>
        /// <param name="anchorIsReceiver">if the parameter anchor is receiver</param>
        /// <returns>true if they can connect</returns>
        public virtual bool CanConnect(Anchor anchor, bool anchorIsReceiver)
        {
            return Connectable.CanConnect(anchor.Connectable, anchor, anchorIsReceiver);
        }

        /// <summary>
        /// When moving, Update the connection position
        /// </summary>
        public void OnMove()
        {
            //Point = point;
            if (connectors != null)
                foreach (Connector c in connectors)
                    c.OnAnchorMove(this, Point);
        }
        #endregion

        #region Private Methods
        private void StartConnect(object sender, RoutedEventArgs e)
        {
            Connector con = new Connector(Canvas) { Anchor1 = this };
            con.StartConnecting(this);
        }
        #endregion

    }

    /// <summary>
    /// Anchor that only receives connections
    /// </summary>
    public class ReceiverAnchor : Anchor 
    {
        public override bool CanConnect(Anchor anchor, bool anchorIsReceiver)
        {
            if (anchorIsReceiver && anchor.GetType() != typeof(SenderAnchor))
                return false;
            return base.CanConnect(anchor, anchorIsReceiver);
        }
    }

    /// <summary>
    /// Anchor that only sends out connections
    /// </summary>
    public class SenderAnchor : Anchor
    {
        public override bool CanConnect(Anchor anchor, bool anchorIsReceiver)
        {
            if (!anchorIsReceiver && anchor.GetType() != typeof(ReceiverAnchor))
                return false;
            return base.CanConnect(anchor, anchorIsReceiver);
        }
    }
}
