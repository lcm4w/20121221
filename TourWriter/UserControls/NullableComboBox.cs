using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace TourWriter.UserControls
{
    /// <summary>
    /// ComboBox that handles null values. 
    /// Databind the list datasource before the selected value datasource 
    /// to ensure null values are shown blank at startup. 
    /// DEL or BACK keys set the combobox value to null.
    /// </summary>
    public class NullableComboBox : ComboBox
    {
        private bool allowNull = true;

        [Browsable(true)]
        [Category("Behavior")]
        [Description("Indicates whether to accept NULL values on DEL or BACK keys")]
        [DefaultValue(true)]
        public bool AllowNull
        {
            get { return allowNull; }
            set { allowNull = value; }
        }

        /// <summary>
        /// Set value to null on DEL or BACK keys
        /// </summary>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (AllowNull && (e.KeyCode == Keys.Delete || e.KeyCode == Keys.Back))
            {
                SelectedValue = DBNull.Value;
                SelectedIndex = -1; // clear the display text
            }
            base.OnKeyDown(e);
        }
    }
}
