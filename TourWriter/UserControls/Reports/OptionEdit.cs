using System;
using System.Collections.Generic;
using System.Windows.Forms;
using TourWriter.Global;

namespace TourWriter.UserControls.Reports
{
    public partial class OptionEdit : UserControl
    {
        public RdlcFileHelper.SqlExpression SqlExpression { get; private set; }
        private readonly Dictionary<string, object> _defaultParameters;

        public OptionEdit(RdlcFileHelper.SqlExpression sqlExpression, Dictionary<string, object> defaultParameters)
        {
            SqlExpression = sqlExpression;
            _defaultParameters = defaultParameters;
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
            label.AutoSize = true;
            switch (SqlExpression.ParameterName)
            {
                case "@ItineraryID":
                    label.Text = "Itinerary ID";
                    editor = new NumericUpDown { Width = 80, Enabled = false, Maximum = int.MaxValue };
                    (editor as NumericUpDown).Value = Convert.ToInt32(_defaultParameters[SqlExpression.ParameterName]);
                    break;
                case "@SupplierID":
                    label.Text = "Supplier ID";
                    editor = new NumericUpDown { Width = 80, Enabled = false, Maximum = int.MaxValue };
                    (editor as NumericUpDown).Value = Convert.ToInt32(_defaultParameters[SqlExpression.ParameterName]);
                    break;
                case "@StartDate":
                    label.Text = "Start date";
                    editor = new DateTimePicker { Format = DateTimePickerFormat.Short, Width = 80 };
                    if (_defaultParameters.ContainsKey(SqlExpression.ParameterName))
                        editor.Text = _defaultParameters[SqlExpression.ParameterName].ToString();
                    break;
                case "@EndDate":
                    label.Text = "End date";
                    editor = new DateTimePicker { Format = DateTimePickerFormat.Short, Width = 80 };
                    if (_defaultParameters.ContainsKey(SqlExpression.ParameterName))
                        editor.Text = _defaultParameters[SqlExpression.ParameterName].ToString();
                    break;
                case "@ContentTypeID":
                    label.Text = "Content Type";
                    editor = new ComboBox { Width = 120, DropDownStyle = ComboBoxStyle.DropDownList, 
                        DataSource = Cache.ToolSet.ContentType, ValueMember = "ContentTypeID", DisplayMember = "ContentTypeName" };
                    break;
                case "@ServiceTypeIDList":
                    label.Text = "Service types";
                    editor = new CheckBoxSet();
                    (editor as CheckBoxSet).Initialise(Cache.ToolSet.ServiceType, "ServiceTypeID", "ServiceTypeName", false);
                    (editor as CheckBoxSet).CheckAll(true);
                    break;
                case "@PurchaseLineIDList":
                    label.Text = "Booking lines";
                    editor = new CheckBoxSet();
                    (editor as CheckBoxSet).Initialise(
                        _defaultParameters[SqlExpression.ParameterName], "PurchaseLineID", "PurchaseLineName", false);
                    (editor as CheckBoxSet).CheckAll(true);
                    break;
                case "@AssignedToIDList":
                    label.Text = "Assigned to";
                    editor = new CheckBoxSet();
                    (editor as CheckBoxSet).Initialise(Cache.ToolSet.User, "UserID", "DisplayName", false);
                    (editor as CheckBoxSet).CheckAll(true);
                    break;
                case "@ItineraryStatusIDList":
                    label.Text = "Itinerary status";
                    editor = new CheckBoxSet();
                    (editor as CheckBoxSet).Initialise(
                        Cache.ToolSet.ItineraryStatus, "ItineraryStatusID", "ItineraryStatusName", true);
                    (editor as CheckBoxSet).CheckAll(true);
                    break;
                case "@RequestStatusIDList":
                    label.Text = "Booking status";
                    editor = new CheckBoxSet();
                    (editor as CheckBoxSet).Initialise(
                        Cache.ToolSet.RequestStatus, "RequestStatusID", "RequestStatusName", true);
                    (editor as CheckBoxSet).CheckAll(true);
                    break;
                case "@BranchIDList":
                    label.Text = "Branch";
                    editor = new CheckBoxSet();
                    (editor as CheckBoxSet).Initialise(Cache.ToolSet.Branch, "BranchID", "BranchName", true);
                    (editor as CheckBoxSet).CheckAll(true);
                    break;
                case "@DepartmentIDList":
                    label.Text = "Department";
                    editor = new CheckBoxSet();
                    (editor as CheckBoxSet).Initialise(Cache.ToolSet.Department, "DepartmentID", "DepartmentName", true);
                    (editor as CheckBoxSet).CheckAll(true);
                    break;
                case "@UserIDList":
                    label.Text = "Assigned to";
                    editor = new CheckBoxSet();
                    (editor as CheckBoxSet).Initialise(Cache.ToolSet.User, "UserID", "DisplayName", true);
                    (editor as CheckBoxSet).CheckAll(true);
                    break;
                case "@AgentIDList":
                    label.Text = "Agent";
                    editor = new CheckBoxSet();
                    (editor as CheckBoxSet).Initialise(Cache.ToolSet.Agent, "AgentID", "AgentName", true);
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