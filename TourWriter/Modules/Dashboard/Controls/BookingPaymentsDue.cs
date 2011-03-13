using System;
using System.Data;
using System.Windows.Forms;
using TourWriter.Info.Services;

namespace TourWriter.Modules.Dashboard.Controls
{
    public partial class BookingPaymentsDue : UserControl
    {
        public BookingPaymentsDue()
        {
            InitializeComponent();
        }

        private void BookingPaymentsDue_Load(object sender, EventArgs e)
        {
            var sql = @"
select top(100) purchaseitemstartdate, suppliername, requeststatusname, email, phone
from PurchaseItemDetail
where requeststatusname is not null
and requeststatusname <> 'Confirmed'
and purchaseitemstartdate > getdate()
--and purchaseitemstartdate < getdate()
order by purchaseitemstartdate desc
";
            DataSet ds = new DataSet();
            DataSetHelper.FillDataset(ds, sql);
            dataExtractGrid1.DataSource = ds;
        }
    }
}
