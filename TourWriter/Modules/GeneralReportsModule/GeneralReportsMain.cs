using System;
using System.Data;
using System.Windows.Forms;
using TourWriter.Global;
using TourWriter.Properties;
using Cursors=System.Windows.Forms.Cursors;

namespace TourWriter.Modules.GeneralReportsModule
{
    public partial class GeneralReportsMain : ModuleBase
    {
        public GeneralReportsMain()
        {
            InitializeComponent();

            menuStrip1.Visible = false;
            toolStrip1.Visible = false;

            Icon = System.Drawing.Icon.FromHandle(Resources.Report.GetHicon());
            displayTypeName = "Reports";

            InitializeReportMaster();
        }

        private void ReportMain_Load(object sender, EventArgs e)
        {
            foreach (DataTable dt in Cache.ToolSet.Tables)
            {
                dt.ColumnChanged += DataSet_ColumnChanged;
                dt.RowDeleted += DataSet_RowDeleted;
            }
        }

        private void InitializeReportMaster()
        {
            reportControl.PoplulateReportExplorer(UserControls.Reports.ExplorerControl.ReportCategory.General);
        }

        private void RefreshData()
        {
            bool hasChanges = Cache.ToolSet.HasChanges();
            if (hasChanges)
            {
                hasChanges = App.AskYesNo(App.GetResourceString("AskDoSave"));
            }
            Cache.RefreshToolSet(hasChanges);
            reportControl.RefreshReportExplorer();
            reportControl.PoplulateReportExplorer(UserControls.Reports.ExplorerControl.ReportCategory.General);

            SetDataCleanName();
        }

        protected override void SaveDataChanges()
        {
            Cursor c = Cursor;
            Cursor = Cursors.WaitCursor;

            try
            {
                CommitOpenEdits();

                if (Cache.ToolSet != null && Cache.ToolSet.HasChanges())
                {
                    Cache.SaveToolSet();
                    reportControl.RefreshReportExplorer();
                    reportControl.PoplulateReportExplorer(UserControls.Reports.ExplorerControl.ReportCategory.General);
                }

                Text = Text.TrimStart('*');
            }
            finally
            {
                Cursor = c;
            }
        }

        protected override void CancelDataChanges()
        {
            
        }

        protected override void CommitOpenEdits()
        {
            if (Cache.ToolSet != null)
            {
                foreach (DataTable dt in Cache.ToolSet.Tables)
                    foreach (DataRow dr in dt.Rows)
                        dr.EndEdit();
            }
        }

        protected override bool IsDataDirty()
        {
            CommitOpenEdits();
            return (Cache.ToolSet != null && Cache.ToolSet.HasChanges());
        }

        protected override string GetDisplayName()
        {
            return "Reports";
        }

        #region Events

        private void menuHelp_Click(object sender, EventArgs e)
        {

        }

        private void menuClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void menuSave_Click(object sender, EventArgs e)
        {
            SaveDataChanges();
        }

        private void menuRefresh_Click(object sender, EventArgs e)
        {
            RefreshData();
        }

        private void DataSet_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            SetDataDirtyName();
        }

        private void DataSet_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            SetDataDirtyName();
        }

        #endregion
    }
}