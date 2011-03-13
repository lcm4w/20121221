using System.Windows.Forms;
using TourWriter.Modules.Dashboard.Controls;

namespace TourWriter.Modules.Dashboard
{
    public partial class Dashboard : ModuleBase
    {
        public Dashboard()
        {
            _doLicenseCheck = false;
            InitializeComponent();
        }

        #region Override methods
        protected override bool IsDataDirty()
        {
            return false;
        }

        protected override string GetDisplayName()
        {
            return "Dashboard";
        }
        #endregion

        private void Dashboard_Load(object sender, System.EventArgs e)
        {
            var itineraryPaymentsDue = new TabPage();
            itineraryPaymentsDue.Text = "Client Payments Due";
            var c1 = new ItineraryPaymentsDue {Dock = DockStyle.Fill};
            itineraryPaymentsDue.Controls.Add(c1);
            tabControlBottom.TabPages.Add(itineraryPaymentsDue);

            var bookingPaymentsDue = new TabPage();
            bookingPaymentsDue.Text = "Supplier Payments Due";
            var c2 = new BookingPaymentsDue { Dock = DockStyle.Fill };
            bookingPaymentsDue.Controls.Add(c2);
            tabControlLeft.TabPages.Add(bookingPaymentsDue);
            
            var charts = new TabPage();
            charts.Text = "Charts";
            var c3 = new Graph { Dock = DockStyle.Fill };
            charts.Controls.Add(c3);
            tabControlRight.TabPages.Add(charts);
        }
    }
}
