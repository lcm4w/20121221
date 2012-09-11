using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using TourWriter.Global;

namespace TourWriter.UserControls.Reports
{
    public partial class OptionEdit : UserControl
    {
        public RdlcFileHelper.SqlExpression SqlExpression { get; private set; }
        private readonly Dictionary<string, object> _sqlParameters;

        public OptionEdit(RdlcFileHelper.SqlExpression sqlExpression, Dictionary<string, object> sqlParameters)
        {
            SqlExpression = sqlExpression;
            _sqlParameters = sqlParameters;
            InitializeComponent();
            LoadControls();
        }

        private void LoadControls()
        {
            var label = new Label();
            var editor = new Control();
            InitialiseControl(ref label, ref editor);

            flowLayoutPanel.Controls.Add(label);
            flowLayoutPanel.Controls.Add(editor);
            Size = flowLayoutPanel.Size;
        }

        private void InitialiseControl(ref Label label, ref Control editor)
        {

            //var rows = table.AsEnumerable().Where(x => x.RowState != DataRowState.Deleted).OrderBy(x => x.Field<string>(displayMember));


            label.AutoSize = true;
            switch (SqlExpression.ParameterName)
            {
                case "@ItineraryID":
                    label.Text = "Itinerary ID";
                    editor = new NumericUpDown { Width = 80, Enabled = false, Maximum = int.MaxValue };
                    (editor as NumericUpDown).Value = Convert.ToInt32(_sqlParameters[SqlExpression.ParameterName]);
                    break;
                case "@SupplierID":
                    label.Text = "Supplier ID";
                    editor = new NumericUpDown { Width = 80, Enabled = false, Maximum = int.MaxValue };
                    (editor as NumericUpDown).Value = Convert.ToInt32(_sqlParameters[SqlExpression.ParameterName]);
                    break;
                case "@StartDate":
                    label.Text = "Start date";
                    editor = new DateTimePicker { Format = DateTimePickerFormat.Short, Width = 80 };
                    if (_sqlParameters.ContainsKey(SqlExpression.ParameterName))
                        editor.Text = _sqlParameters[SqlExpression.ParameterName].ToString();
                    break;
                case "@EndDate":
                    label.Text = "End date";
                    editor = new DateTimePicker { Format = DateTimePickerFormat.Short, Width = 80 };
                    if (_sqlParameters.ContainsKey(SqlExpression.ParameterName))
                        editor.Text = _sqlParameters[SqlExpression.ParameterName].ToString();
                    break;
                case "@ContentTypeID":
                    label.Text = "Content Type";
                    editor = new ComboBox { Width = 120, DropDownStyle = ComboBoxStyle.DropDownList,
                        DataSource = Cache.ToolSet.ContentType, ValueMember = "ContentTypeID", DisplayMember = "ContentTypeName" };
                    break;
                case "@ServiceTypeIDList":
                    label.Text = "Service types";
                    editor = new CheckBoxSet();
                    if (Cache.ToolSet.ServiceType.Count == 0) break;
                    (editor as CheckBoxSet).Initialise(Cache.ToolSet.ServiceType.Where(x => x.RowState != DataRowState.Deleted).OrderBy(x => x.ServiceTypeName).CopyToDataTable(), "ServiceTypeID", "ServiceTypeName", false);
                    (editor as CheckBoxSet).CheckAll(true);
                    break;
                case "@PurchaseLineIDList":
                    label.Text = "Booking lines";
                    editor = new CheckBoxSet();
                    (editor as CheckBoxSet).Initialise(_sqlParameters[SqlExpression.ParameterName], "PurchaseLineID", "PurchaseLineName", false);
                    (editor as CheckBoxSet).CheckAll(true);
                    break;
                case "@AssignedToIDList":
                    label.Text = "Assigned to";
                    editor = new CheckBoxSet();
                    if (Cache.ToolSet.User.Count == 0) break;
                    (editor as CheckBoxSet).Initialise(Cache.ToolSet.User.Where(x => x.RowState != DataRowState.Deleted).OrderBy(x => x.DisplayName).CopyToDataTable(), "UserID", "DisplayName", false);
                    (editor as CheckBoxSet).CheckAll(true);
                    break;
                case "@ItineraryStatusIDList":
                    label.Text = "Itinerary status";
                    editor = new CheckBoxSet();
                    if (Cache.ToolSet.ItineraryStatus.Count == 0) break;
                    (editor as CheckBoxSet).Initialise(Cache.ToolSet.ItineraryStatus.Where(x => x.RowState != DataRowState.Deleted).OrderBy(x => x.ItineraryStatusName).CopyToDataTable(), "ItineraryStatusID", "ItineraryStatusName", true);
                    (editor as CheckBoxSet).CheckAll(true);
                    break;
                case "@RequestStatusIDList":
                    label.Text = "Booking status";
                    editor = new CheckBoxSet();
                    if (Cache.ToolSet.RequestStatus.Count == 0) break;
                    (editor as CheckBoxSet).Initialise(Cache.ToolSet.RequestStatus.Where(x => x.RowState != DataRowState.Deleted).OrderBy(x => x.RequestStatusName).CopyToDataTable(), "RequestStatusID", "RequestStatusName", true);
                    (editor as CheckBoxSet).CheckAll(true);
                    break;
                case "@BranchIDList":
                    label.Text = "Branch";
                    editor = new CheckBoxSet();
                    if (Cache.ToolSet.Branch.Count == 0) break;
                    (editor as CheckBoxSet).Initialise(Cache.ToolSet.Branch.Where(x => x.RowState != DataRowState.Deleted).OrderBy(x => x.BranchName).CopyToDataTable(), "BranchID", "BranchName", true);
                    (editor as CheckBoxSet).CheckAll(true);
                    break;
                case "@CityIDList":
                    label.Text = "City";
                    editor = new CheckBoxSet();
                    if (Cache.ToolSet.City.Count == 0) break;
                    (editor as CheckBoxSet).Initialise(Cache.ToolSet.City.Where(x => x.RowState != DataRowState.Deleted).OrderBy(x => x.CityName).CopyToDataTable(), "CityID", "CityName", true);
                    (editor as CheckBoxSet).CheckAll(true);
                    break;
                case "@DepartmentIDList":
                    label.Text = "Department";
                    editor = new CheckBoxSet();
                    if (Cache.ToolSet.Department.Count == 0) break;
                    (editor as CheckBoxSet).Initialise(Cache.ToolSet.Department.Where(x => x.RowState != DataRowState.Deleted).OrderBy(x => x.DepartmentName).CopyToDataTable(), "DepartmentID", "DepartmentName", true);
                    (editor as CheckBoxSet).CheckAll(true);
                    break;
                case "@UserIDList":
                    label.Text = "Assigned to";
                    editor = new CheckBoxSet();
                    if (Cache.ToolSet.User.Count == 0) break;
                    (editor as CheckBoxSet).Initialise(Cache.ToolSet.User.Where(x => x.RowState != DataRowState.Deleted).OrderBy(x => x.DisplayName).CopyToDataTable(), "UserID", "DisplayName", true);
                    (editor as CheckBoxSet).CheckAll(true);
                    break;
                case "@AgentIDList":
                    label.Text = "Agent";
                    editor = new CheckBoxSet();
                    if (Cache.ToolSet.Agent.Count == 0) break;
                    (editor as CheckBoxSet).Initialise(Cache.ToolSet.Agent.Where(x => x.RowState != DataRowState.Deleted).OrderBy(x => x.AgentName).CopyToDataTable(), "AgentID", "AgentName", true);
                    (editor as CheckBoxSet).CheckAll(true);
                    break;
                case "@ContactCategoryIDList":
                    label.Text = "Contact Category";
                    editor = new CheckBoxSet();
                    if (Cache.ToolSet.ContactCategory.Count == 0) break;
                    (editor as CheckBoxSet).Initialise(Cache.ToolSet.ContactCategory.Where(x => x.RowState != DataRowState.Deleted).OrderBy(x => x.ContactCategoryName).CopyToDataTable(), "ContactCategoryID", "ContactCategoryName", true);
                    (editor as CheckBoxSet).CheckAll(true);
                    break;
                case "@IsLockedAccountingList":
                    label.Text = "Accounting Export";
                    editor = new CheckBoxSet();
                    var t = new DataTable();t.Columns.Add("value");t.Columns.Add("text");
                    t.Rows.Add(0, "Not Exported");t.Rows.Add(1, "Exported");
                    (editor as CheckBoxSet).Initialise(t, "value", "text", true);
                    (editor as CheckBoxSet).CheckAll(true);
                    break;
                default:
                    Enabled = false;
                    throw new ArgumentException("Report sql parameter not recognised: " + SqlExpression.ParameterName);
            }
        }

        /// <summary>
        /// Returns the value of the editor control
        /// </summary>
        /// <returns></returns>
        public object GetEditorValue()
        {
            if (flowLayoutPanel.Controls.Count == 0) return "";
            var editor = flowLayoutPanel.Controls[1];

            if (editor is NumericUpDown)
                return (editor as NumericUpDown).Value;

            if (editor is ComboBox)
                return (editor as ComboBox).SelectedValue ?? 0;

            if (editor is DateTimePicker)
                return (editor as DateTimePicker).Value.Date;

            if (editor is CheckBoxSet)
            {
                var result = (editor as CheckBoxSet).GetResultAsCsvString();
                if (result.ToLower().IndexOf("null") > -1)
                    return string.Format("({0}) OR {1} IS NULL", result, SqlExpression.ColumnName);
                if (result == "")
                    return string.Format("({0})", "NULL");
                return string.Format("({0})", result);
            }
            throw new ArgumentException("Report option editor control type not recognised for parameter: " + SqlExpression.ParameterName);
        }

        public bool ValidateEditorValue(ref string msg)
        {
            msg = "";
            if (flowLayoutPanel.Controls.Count == 0) return false;

            var label = flowLayoutPanel.Controls[0];
            var editor = flowLayoutPanel.Controls[1];

            if (editor is CheckBoxSet)
                if (string.IsNullOrEmpty((editor as CheckBoxSet).GetResultAsCsvString()))
                    msg = label.Text + " requires at least one value";

            var isValid = string.IsNullOrEmpty(msg);
            label.ForeColor = isValid ? System.Drawing.Color.Black : System.Drawing.Color.Red;
            return isValid;
        }
    }
}