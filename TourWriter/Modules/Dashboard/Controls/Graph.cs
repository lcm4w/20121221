using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using TourWriter.Info.Services;

namespace TourWriter.Modules.Dashboard.Controls
{
    public partial class Graph : UserControl
    {
        public Graph()
        {
            InitializeComponent();
        }

        private void Graph_Load(object sender, EventArgs e)
        {
            var sql = @"
select ItineraryID, ArriveDate, DepartDate, ItineraryGross, ItineraryYield, ItineraryStatusName from ItineraryDetail where ArriveDate >= '{0}' and ArriveDate <= '{1}'";

            var mid = DateTime.Parse(DateTime.Now.AddMonths(-6).ToString("yyyy-MM-dd 00:00:00"));
            var ds = DataSetHelper.FillDataset(new DataSet(), string.Format(sql, mid.AddYears(-1).ToString("yyyy-MM-dd 00:00:00"), mid.AddYears(1).ToString("yyyy-MM-dd 23:59:59")));

            var table = ds.Tables[0].AsEnumerable();
            var previous = table.Where(x => x.Field<DateTime>("ArriveDate") < mid);
            var current  = table.Where(x => x.Field<DateTime>("ArriveDate") >= mid);

            var test = Enumerable.Range(0, 12).Select(mid.AddMonths).Select(x => x.Month);

            foreach(var month in Enumerable.Range(mid.Month, mid.AddYears(1).Month))
            {
                var m = month;

                // SUM
                chart1.Series["Series1a"].Points.AddY(current.Where(x => x.Field<DateTime>("ArriveDate").Month == m).Select(x => x.Field<decimal?>("ItineraryGross")).Sum());
                chart1.Series["Series1b"].Points.AddY(previous.Where(x => x.Field<DateTime>("ArriveDate").Month == m).Select(x => x.Field<decimal?>("ItineraryGross")).Sum());

                chart1.Series["Series2a"].Points.AddY(current.Where(x => x.Field<DateTime>("ArriveDate").Month == m).Select(x => x.Field<decimal?>("ItineraryYield")).Sum());
                chart1.Series["Series2b"].Points.AddY(previous.Where(x => x.Field<DateTime>("ArriveDate").Month == m).Select(x => x.Field<decimal?>("ItineraryYield")).Sum());

                // COUNT
                //chart1.Series["Series1a"].Points.AddY(current.Where(x => x.Field<DateTime>("ArriveDate").Month == m).Count());
                //chart1.Series["Series1b"].Points.AddY(previous.Where(x => x.Field<DateTime>("ArriveDate").Month == m).Count());
            }

            // Set series chart type
            chart1.Series["Series1a"].ChartType = SeriesChartType.Spline;
            chart1.Series["Series2a"].ChartType = SeriesChartType.Spline;

            // Set point labels
            chart1.Series["Series1a"].IsValueShownAsLabel = true;
            chart1.Series["Series2a"].IsValueShownAsLabel = true;

            // X axis margin
            chart1.ChartAreas["Default"].AxisX.IsMarginVisible = false;

            // 3D
            chart1.ChartAreas["Default"].Area3DStyle.Enable3D = false;
            
        }
    }
}
