using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using TourWriter.Properties;

namespace TourWriter.UserControls.Reports
{
    public partial class OptionsForm : Form
    {
        public bool LayoutRefreshRequired;
        private RdlcFileHelper _rdlcHelper;
        private readonly string _reportFile;
        private readonly Dictionary<string, object> _defaultParams;

        public OptionsForm(string reportFile, Dictionary<string, object> reportParams)
        {
            InitializeComponent();
            Icon = Resources.TourWriter16;

            _rdlcHelper = new RdlcFileHelper(reportFile);
            _reportFile = reportFile;
            _defaultParams = reportParams;
            LoadEditorControls();
        }

        private void OptionsForm_Load(object sender, EventArgs e)
        {
            LoadLayoutOptions();
        }

        private void LoadEditorControls()
        {
            var added = new List<string>();
            foreach (var sqlExpression in _rdlcHelper.GetSqlExpressions())
            {
                if (added.Contains(sqlExpression.ParameterName)) continue;
                added.Add(sqlExpression.ParameterName);

                OptionEdit editor;
                try
                {
                    editor = new OptionEdit(sqlExpression, _defaultParams);
                }
                catch (KeyNotFoundException ex)
                {
                    throw new KeyNotFoundException(string.Format(
                        "Sql parameter '{0}' not found for report file '{1}'", sqlExpression.ParameterName, _reportFile), ex);
                }
                pnlLayout.Controls.Add(editor);
            }
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

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (!ValidateEditorControls())
            {
                DialogResult = DialogResult.None;
                return;
            }
            SaveLayoutOptions();
        }

        /// <summary>
        ///  Process report options.
        /// </summary>
        /// <returns></returns>
        public void ProcessOptions(ref Dictionary<string, object> reportParams, ref Dictionary<string, string> dataSources)
        {
            var exists = false;
            reportParams = _defaultParams;
            dataSources = _rdlcHelper.GetDataSourcesNameAndSql(ref exists);

            foreach(var editControl in pnlLayout.Controls)
            {
                var editor = editControl as OptionEdit;
                if (editor == null) continue;

                // get editor value
                var value = editor.GetEditorValue();

                // inject value into default params list
                if (_defaultParams.ContainsKey(editor.SqlExpression.ParameterName))
                    _defaultParams[editor.SqlExpression.ParameterName] = value;

                // apply sql formatting to value
                var stringValue = value.ToString();
                if (value.GetType() == typeof(DateTime))
                {
                    if (editor.SqlExpression.ParameterName == "@EndDate")
                        value = ((DateTime) value).Date.AddDays(1);
                    stringValue = string.Format("'{0}'", ((DateTime)value).ToString("yyyy-MM-dd 00:00:00:000"));
                }

                // inject value into datasources sql statements
                var dsNames = new string[dataSources.Count];
                dataSources.Keys.CopyTo(dsNames, 0);
                foreach (var dsName in dsNames)
                {
                    var dsSql = dataSources[dsName];
                    dsSql = new Regex(editor.SqlExpression.ParameterName).Replace(dsSql, stringValue);
                    dataSources[dsName] = dsSql;
                }
            }
        }

        private void LoadLayoutOptions()
        {
            var exists = false;

            txtTopMargin.Value = _rdlcHelper.GetTopMargin(ref exists);
            txtTopMargin.Enabled = exists;

            txtBottomMargin.Value = _rdlcHelper.GetBottomMargin(ref exists);
            txtBottomMargin.Enabled = exists;

            txtLeftMargin.Value = _rdlcHelper.GetLeftMargin(ref exists);
            txtLeftMargin.Enabled = exists;

            txtRightMargin.Value = _rdlcHelper.GetRightMargin(ref exists);
            txtRightMargin.Enabled = exists;

            txtSpacing.Value = _rdlcHelper.GetSpacing(ref exists);
            txtSpacing.Enabled = lblSpacing1.Enabled = exists;
        }

        private void SaveLayoutOptions()
        {
            // use new file helper to ensure we only write the changes to settings
            var rdlc = new RdlcFileHelper(_reportFile);
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

            if (edit)
            {
                rdlc.WriteFile();
                _rdlcHelper = new RdlcFileHelper(_reportFile); // reset
                LayoutRefreshRequired = true;
            }
        }
    }
}
