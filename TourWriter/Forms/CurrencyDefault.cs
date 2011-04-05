using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TourWriter.Global;

namespace TourWriter.Forms
{
    public partial class CurrencyDefault : Form
    {
        public string CurrencyCode
        {
            get { return cmbCurrency.SelectedValue.ToString(); }
        }

        public string LanguageCode
        {
            get { return cmbLanguage.SelectedValue.ToString(); }
        }

        public void Warn()
        {
            if (Visible && !App.IsDebugMode)
            {
                cmbCurrency.Enabled = cmbLanguage.Enabled = false;
                App.ShowInfo("Please contact TourWriter staff to enable changing your base currency.\r\n\r\n" +
                             "If you have existing Itineraries priced in a different default currency, " +
                             "we need to discuss whether these should also convert to the new default currency.\r\n\r\n" +
                             "We can easily enable editing if you wish.");
            }
        }

        public CurrencyDefault()
        {
            InitializeComponent();
        }

        private void CurrencyDefault_Load(object sender, EventArgs e)
        {
            var lang = new Dictionary<string, string> { { "", "-- default computer setting --" } };
            foreach (var l in CultureInfo.GetCultures(CultureTypes.SpecificCultures).ToDictionary(x => x.Name, x => x.EnglishName).OrderBy(x => x.Value))
                lang.Add(l.Key, l.Value);
            cmbLanguage.DataSource = new BindingSource(lang, null);
            cmbLanguage.ValueMember = "Key";
            cmbLanguage.DisplayMember = "Value";
            cmbLanguage.SelectedIndexChanged += delegate { RefreshExample(); };
            cmbLanguage.SelectedValue = !Cache.ToolSet.AppSettings[0].IsLanguageCodeNull() ? Cache.ToolSet.AppSettings[0].LanguageCode.Trim() : "";

            var ccys = new Dictionary<string, string> { { "", "-- default computer setting --" } };
            foreach (var c in Cache.ToolSet.Currency.ToDictionary(x => x.CurrencyCode, x => x.DisplayName))
                ccys.Add(c.Key, c.Value);
            cmbCurrency.DataSource = new BindingSource(ccys, null);
            cmbCurrency.ValueMember = "Key";
            cmbCurrency.DisplayMember = "Value";
            cmbCurrency.SelectedValue = !Cache.ToolSet.AppSettings[0].IsCurrencyCodeNull() ? Cache.ToolSet.AppSettings[0].CurrencyCode.Trim() : "";

            RefreshExample();
            Warn();
        }
            
        private void RefreshExample()
        {
            var lang = cmbLanguage.SelectedValue.ToString() != "" ? cmbLanguage.SelectedValue.ToString() : CultureInfo.CurrentCulture.Name;
            txtDisplay.Text = 12345.6789f.ToString("c", CultureInfo.GetCultureInfo(lang));
        }
        
        private void btnOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }
    }
}
