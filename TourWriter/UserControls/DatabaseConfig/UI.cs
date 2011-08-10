﻿using System;
using System.Collections.Specialized;
using System.Windows.Forms;
using TourWriter.Properties;

namespace TourWriter.UserControls.DatabaseConfig
{
    public partial class Ui : Form
    {
        protected DbConnections Connections;

        public Ui(UserControl defaultControl, DbConnections connections)
        {
            InitializeComponent();
            Icon = Resources.TourWriter16;

            Connections = connections;
            LoadControl(defaultControl);
        }

        /// <summary>
        /// The forms Back button
        /// </summary>
        internal Button BackButton { get { return btnPrev; } }
        /// <summary>
        /// The forms Next button
        /// </summary>
        internal Button NextButton { get { return btnNext; } }
        /// <summary>
        /// The forms Cancel button
        /// </summary>
        internal Button EndButton { get { return btnCancel; } }

        internal UserControl NextControl { get; set; }
        internal UserControl PrevControl { get; set; }
        
        private void LoadControl(UserControl control)
        {
            panel1.Controls.Clear();
            if (control != null)
            {
                (control as UiControlBase).Connections = Connections;
                control.Dock = DockStyle.Fill;
                panel1.Controls.Add(control);
            }
        }

        internal void GoBack()
        {
            GoTo(PrevControl);
        }

        internal void GoNext()
        {
            if (NextControl != null)
                GoTo(NextControl);
            else GoComplete();
        }

        internal void GoCancel()
        {
            DialogResult = DialogResult.Cancel;
        }

        internal void GoComplete()
        {
            var control = panel1.Controls[0] as IConnectionControl;
            if (control.ValidateAndFinalise())
            {
                DialogResult = DialogResult.OK;
            }
        }

        internal void GoTo(UserControl control)
        {
            LoadControl(control);
        }
        
        private void OnPrevClick(object sender, EventArgs e)
        {
            GoBack();
        }

        private void OnNextClick(object sender, EventArgs e)
        {
            GoNext();
        }

        private void OnCancelClick(object sender, EventArgs e)
        {
            GoCancel();
        }
    }
}
