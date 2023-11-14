// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Windows.Forms;

namespace Revit.SDK.Samples.InPlaceMembers.CS
{
    /// <summary>
    ///     Main form class, use to diaplay the preview picture box and property grid.
    /// </summary>
    public partial class InPlaceMembersForm : Form, IMessageFilter
    {
        /// <summary>
        ///     window message key number
        /// </summary>
        private const int WM_KEYDOWN = 0X0100;

        /// <summary>
        ///     Graphics data
        /// </summary>
        private readonly GraphicsData m_graphicsData;

        /// <summary>
        ///     Properties instance
        /// </summary>
        private readonly Properties m_instanceProperties;

        /// <summary>
        ///     constructor
        /// </summary>
        /// <param name="p"></param>
        /// <param name="graphicsData"></param>
        public InPlaceMembersForm(Properties p, GraphicsData graphicsData)
        {
            m_instanceProperties = p;
            m_graphicsData = graphicsData;
            Application.AddMessageFilter(this);
            InitializeComponent();
        }

        /// <summary>
        ///     implement IMessageFilter interface
        /// </summary>
        /// <param name="m">The message to be dispatched.</param>
        /// <returns>
        ///     true to filter the message and stop it from being dispatched;
        ///     false to allow the message to continue to the next filter or control.
        /// </returns>
        public bool PreFilterMessage(ref Message m)
        {
            if (!modelPictureBox.Focused) return false;

            if (m.Msg == WM_KEYDOWN)
            {
                var k = (Keys)(int)m.WParam;
                var e = new KeyEventArgs(k);
                switch (e.KeyCode)
                {
                    case Keys.Left:
                        m_graphicsData.RotateY(true);
                        break;
                    case Keys.Right:
                        m_graphicsData.RotateY(false);
                        break;
                    case Keys.Up:
                        m_graphicsData.RotateX(true);
                        break;
                    case Keys.Down:
                        m_graphicsData.RotateX(false);
                        break;
                    case Keys.PageUp:
                        m_graphicsData.RotateZ(true);
                        break;
                    case Keys.PageDown:
                        m_graphicsData.RotateZ(false);
                        break;
                    case Keys.S:
                        modelPictureBox.MoveY(true);
                        break;
                    case Keys.W:
                        modelPictureBox.MoveY(false);
                        break;
                    case Keys.A:
                        modelPictureBox.MoveX(true);
                        break;
                    case Keys.D:
                        modelPictureBox.MoveX(false);
                        break;
                    case Keys.Home:
                        modelPictureBox.Scale(true);
                        break;
                    case Keys.End:
                        modelPictureBox.Scale(false);
                        break;
                }

                return true;
            }

            return false;
        }

        /// <summary>
        ///     load event handle
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InPlaceMembersForm_Load(object sender, EventArgs e)
        {
            instancePropertyGrid.SelectedObject = m_instanceProperties;
            modelPictureBox.DataSource = m_graphicsData;
        }

        /// <summary>
        ///     ok button event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OKbutton_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        ///     Cancel button event handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
