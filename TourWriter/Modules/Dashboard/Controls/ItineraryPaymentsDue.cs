using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TourWriter.Info.Services;

namespace TourWriter.Modules.Dashboard.Controls
{
    public partial class ItineraryPaymentsDue : UserControl
    {
        public ItineraryPaymentsDue()
        {
            InitializeComponent();
        }

        private void ItineraryPaymentsDue_Load(object sender, EventArgs e)
        {
            var sql = @"
select top(10) 
    ItineraryName, ArriveDate, ItineraryStatusName, AssignedTo, ItineraryGrossFinalTotal, ItineraryBalanceDueDate, ItineraryTotalOutstanding
from ItineraryDetail
order by ItineraryBalanceDueDate desc
";
            DataSet ds = new DataSet();
            DataSetHelper.FillDataset(ds, sql);
            this.dataExtractGrid1.DataSource = ds;

        }
    }
}
