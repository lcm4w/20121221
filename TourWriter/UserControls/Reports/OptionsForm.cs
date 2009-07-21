using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using TourWriter.Properties;

namespace TourWriter.UserControls.Reports
{
    public partial class OptionsForm : Form
    {
        private string reportFile;
        private readonly RdlcFileHelper rdlcHelper;
        private readonly Dictionary<string, object> defaultParameters;

        public OptionsForm(string reportFile, Dictionary<string, object> defaultParameters)
        {
            InitializeComponent();
            Icon = Resources.TourWriter16;

            rdlcHelper = new RdlcFileHelper(reportFile);
            this.reportFile = reportFile;
            this.defaultParameters = defaultParameters;
            LoadParameterOptions();
        }

        private void OptionsForm_Load(object sender, EventArgs e)
        {
            LoadLayoutOptions();
        }

        private void LoadParameterOptions()
        {
            foreach (RdlcFileHelper.SqlExpression sqlExpression in rdlcHelper.GetSqlExpressions())
            {
                var editor = new OptionEdit(sqlExpression, defaultParameters);
                pnlLayout.Controls.Add(editor);
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (!ValidateEditorControls())
            {
                DialogResult = DialogResult.None;
                return;
            }
            SaveLayoutOptions();
        }

        private bool ValidateEditorControls()
        {
            string msgs = "";
            foreach(var c in pnlLayout.Controls)
            {
                var s = "";
                var editor = c as OptionEdit;
                if (editor != null && !editor.ValidateEditorValue(ref s))
                    msgs += s + "\r\n";
            }
            if (!string.IsNullOrEmpty(msgs))
            {
                App.ShowError(msgs);
                return false;
            }
            return true;
        }

        /// <summary>
        ///  Process report options, updating the default params list and returning the processed sql statement
        /// </summary>
        /// <returns></returns>
        public string ProcessOptions()
        {
            // get default sql statement
            bool exists = false;
            var sql = rdlcHelper.GetSql(ref exists);
            if (!exists) throw new ArgumentException("Report section not found: SQL statement");

            // get option parameters
            foreach(var c in pnlLayout.Controls)
            {
                var editor = c as OptionEdit;
                if (editor != null)
                {
                    var value = editor.GetEditorValue();

                    // update default params list
                    if (defaultParameters.ContainsKey(editor.SqlExpression.ParameterName))
                        defaultParameters[editor.SqlExpression.ParameterName] = value;

                    // inject sql params
                    var stringValue = value.ToString();
                    if (value.GetType() == typeof(DateTime))
                    {
                        if (editor.SqlExpression.ParameterName == "@EndDate")
                            value = ((DateTime) value).Date.AddDays(1);
                        stringValue = string.Format("'{0}'", ((DateTime)value).ToString("yyyy-MM-dd 00:00:00:000"));
                    }
                    sql = new Regex(editor.SqlExpression.ParameterName).Replace(sql, stringValue);
                }
            }
            return sql;
        }

        private void LoadLayoutOptions()
        {
            var exists = false;

            txtTopMargin.Value = rdlcHelper.GetTopMargin(ref exists);
            txtTopMargin.Enabled = exists;

            txtBottomMargin.Value = rdlcHelper.GetBottomMargin(ref exists);
            txtBottomMargin.Enabled = exists;

            txtLeftMargin.Value = rdlcHelper.GetLeftMargin(ref exists);
            txtLeftMargin.Enabled = exists;

            txtRightMargin.Value = rdlcHelper.GetRightMargin(ref exists);
            txtRightMargin.Enabled = exists;

            txtSpacing.Value = rdlcHelper.GetSpacing(ref exists);
            txtSpacing.Enabled = lblSpacing1.Enabled = exists;
        }

        private void SaveLayoutOptions()
        {
            // use new file helper to ensure we only write the changes to settings
            var rdlc = new RdlcFileHelper(reportFile);
            bool exists = false, edit = false;

            if (txtTopMargin.Enabled && rdlc.GetTopMargin(ref exists) != (double)txtTopMargin.Value)
            { rdlc.SetTopMargin((double)txtTopMargin.Value); edit = true; }

            if (txtBottomMargin.Enabled && rdlc.GetBottomMargin(ref exists) != (double)txtBottomMargin.Value)
            { rdlc.SetBottomMargin((double)txtBottomMargin.Value); edit = true; }

            if (txtLeftMargin.Enabled && rdlc.GetLeftMargin(ref exists) != (double)txtLeftMargin.Value)
            { rdlc.SetLeftMargin((double)txtLeftMargin.Value); edit = true; }

            if (txtRightMargin.Enabled && rdlc.GetRightMargin(ref exists) != (double)txtRightMargin.Value)
            { rdlc.SetRightMargin((double)txtRightMargin.Value); edit = true; }

            if (txtSpacing.Enabled && rdlc.GetSpacing(ref exists) != (double)txtSpacing.Value)
            { rdlc.SetSpacing((double)txtSpacing.Value); edit = true; }

            if (edit) rdlc.WriteFile();
        }
    }
}
