using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Infragistics.Win.Misc;

namespace TourWriter.UserControls
{

    #region Button event delegats

    public delegate void OnAddButtonClickHandler(object sender, EventArgs e);

    public delegate void OnDeleteButtonClickHandler(object sender, EventArgs e);

    public delegate void OnCopyButtonClickHandler(object sender, EventArgs e);

    #endregion

    public partial class GridCaption : UserControl
    {
        #region Public properties

        public event OnAddButtonClickHandler OnAddButtonClick;
        public event OnDeleteButtonClickHandler OnDeleteButtonClick;
        public event OnCopyButtonClickHandler OnCopyButtonClick;

        public GridCaption()
        {
            InitializeComponent();
        }

        public string CaptionText
        {
            set { txtCaption.Text = value; }
            get { return txtCaption.Text; }
        }
        
        public Label Caption
        {
            get { return txtCaption; }
        }


        public string ButtonAddToolTipText
        {
            get { return toolTip1.GetToolTip(btnAdd); }
            set { toolTip1.SetToolTip(btnAdd, value); }
        }

        public string ButtonDeleteToolTipText
        {
            get { return toolTip1.GetToolTip(btnDelete); }
            set { toolTip1.SetToolTip(btnDelete, value); }
        }

        public string ButtonCopyToolTipText
        {
            get { return toolTip1.GetToolTip(btnCopy); }
            set { toolTip1.SetToolTip(btnCopy, value); }
        }
        
        
        public bool ButtonAddEnabled
        {
            get { return btnAdd.Enabled; }
            set { btnAdd.Enabled = value; }
        }
        
        public bool ButtonDeleteEnabled
        {
            get { return btnDelete.Enabled; }
            set { btnDelete.Enabled = value; }
        }
        
        public bool ButtonCopyEnabled
        {
            get { return btnCopy.Enabled; }
            set { btnCopy.Enabled = value; }
        }
        

        public bool ButtonAddVisible
        {
            get { return pnlAdd.Visible; }
            set { pnlAdd.Visible = value; }
        }

        public bool ButtonDeleteVisible
        {
            get { return pnlDelete.Visible; }
            set { pnlDelete.Visible = value; }
        }

        public bool ButtonCopyVisible
        {
            get { return pnlCopy.Visible; }
            set { pnlCopy.Visible = value; }
        }


        public bool EnableDisableButtons
        {
            get { return btnAdd.Enabled && btnCopy.Enabled && btnDelete.Enabled; }
            set
            {
                btnAdd.Enabled = value;
                btnCopy.Enabled = value;
                btnDelete.Enabled = value;
            }
        }

        #endregion

        #region Button event handlers

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (OnAddButtonClick != null)
                OnAddButtonClick(sender, e);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (OnDeleteButtonClick != null)
                OnDeleteButtonClick(sender, e);
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            if (OnCopyButtonClick != null)
                OnCopyButtonClick(sender, e);
        }

        #endregion        
    }
}