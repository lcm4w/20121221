using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using TourWriter.Modules.DataExtract.UserControls;

namespace TourWriter.Modules.DataExtract
{
    public partial class DataExtractMain : ModuleBase
    {
        public DataExtractMain()
        {
            Icon = Icon.FromHandle(TourWriter.Properties.Resources.PageExcel.GetHicon());

            InitializeComponent();
            displayTypeName = "DataExtract";
            menuStrip1.Visible = false;
            toolStrip1.Visible = false;
        }

        protected override bool IsDataDirty()
        {
            return false;
        }

        protected override string GetDisplayName()
        {
            return "Data Import/Export";
        }

        private void optExport_ValueChanged(object sender, EventArgs e)
        {
            if (optExport.CheckedIndex < 0)
                return;

            // uncheck the selected import item, if there is one
            optImport.CheckedIndex = -1;
			grpMain.Text = optExport.Text;
            LoadControl(optExport.Value.ToString());
            SetFormActiveText();

            // only enable controls if they have permission
            pnlMain.Enabled = CheckPermissions(optExport.Value.ToString());
        }

        private void optImport_ValueChanged(object sender, EventArgs e)
        {
            if (optImport.CheckedIndex < 0)
                return;

            // uncheck the selected export item, if there is one
            optExport.CheckedIndex = -1;
            grpMain.Text = optImport.Text;

            // clear current bindings
            App.ClearBindings(pnlMain);

            // clear current control
            pnlMain.Controls.Clear();

            Import import = new Import(optImport.Value.ToString());
            import.Tag = this;
            import.Dock = DockStyle.Fill;
            pnlMain.Controls.Add(import);

            // only enable controls if they have permission
            pnlMain.Enabled = CheckPermissions(import.Name);
        }

        internal void LoadControl(string controlToLoad)
        {
            // load new control into pnlMain form
            if (controlToLoad.Trim() != "")
            {
                Cursor = Cursors.WaitCursor;

                // clear current bindings
                App.ClearBindings(pnlMain);

                // clear current control
                pnlMain.Controls.Clear();

                // load the controls dynamically based on what was clicked, check the Key property of the node
                try
                {
                    // since the form is not in the current MdiChildren collection, load it up using reflection
                    Assembly asm = Assembly.LoadFrom(Assembly.GetExecutingAssembly().CodeBase);
                    Type t = asm.GetType("TourWriter.Modules.DataExtract.UserControls." + controlToLoad);
                    UserControl uc = (UserControl)Activator.CreateInstance(t);
                    uc.Tag = this;
                    uc.Dock = DockStyle.Fill;
                    pnlMain.Controls.Add(uc);
                }
                catch (Exception ex)
                {
                    App.Error(ex);
                }
                finally
                {
                    Cursor = Cursors.Default;
                }
            }
        }

        private static bool CheckPermissions(string controlName)
        {
            bool hasPermission = true;

            switch (controlName)
            {
                case "ItineraryExport":
                    hasPermission = Services.AppPermissions.UserHasPermission(
                        Services.AppPermissions.Permissions.DataExtractionItineraries);
                    break;

                case "SupplierExport":
                    hasPermission = Services.AppPermissions.UserHasPermission(
                        Services.AppPermissions.Permissions.DataExtractionSuppliers);
                    break;

                case "PaymentsExport":
                    hasPermission = Services.AppPermissions.UserHasPermission(
                        Services.AppPermissions.Permissions.DataExtractionPayments);
                    break;

                case "Import":
                    hasPermission = Services.AppPermissions.UserHasPermission(
                        Services.AppPermissions.Permissions.DataImport);
                    break;

                default:
                    break;
            }

            return hasPermission;
        }


        private void menuClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void menuHelp_Click(object sender, EventArgs e)
        {
            App.ShowHelp();
        }
    }
}