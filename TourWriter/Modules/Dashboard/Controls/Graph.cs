using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
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
select ItineraryID, ArriveDate, DepartDate, ItineraryGrossFinalTotal, ItineraryYield, ItineraryStatusName 
from ItineraryDetail where ArriveDate >= '{0}' and ArriveDate <= '{1}'";

            var now = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0);
            var from = now.AddMonths(-18);
            var to = now.AddMonths(7).AddSeconds(-1);
            var seperate = now.AddMonths(-6);

            // load data and split into previous and current periods
            var ds = DataSetHelper.FillDataset(new DataSet(), string.Format(sql, from.ToString("yyyy-MM-dd HH:mm:ss"), to.ToString("yyyy-MM-dd HH:mm:ss")));
            var table = ds.Tables[0].AsEnumerable();
            var previous = table.Where(x => x.Field<DateTime>("ArriveDate") < seperate);
            var current  = table.Where(x => x.Field<DateTime>("ArriveDate") >= seperate);
            
            // get range of months, with this month in the middle
            var months = Enumerable.Range(1, 12).ToList(); months.AddRange(Enumerable.Range(1, 12)); months.Add(1);
            months = months.Skip(seperate.Month-1).Take(13).ToList();

            for(var i = 0; i<months.Count(); i++)// month in months)
            {
                var month = months[i];
                var monthName = new DateTime(3000, month, 1).ToString("MMM");

                // SUM gross
                chart1.Series["Series1a"].Points.AddY(current.Where(x => x.Field<DateTime>("ArriveDate").Month == month).Select(x => x.Field<decimal?>("ItineraryGrossFinalTotal")).Sum());
                chart1.Series["Series1b"].Points.AddY(previous.Where(x => x.Field<DateTime>("ArriveDate").Month == month).Select(x => x.Field<decimal?>("ItineraryGrossFinalTotal")).Sum());

                // SUM yield
                chart1.Series["Series2a"].Points.AddY(current.Where(x => x.Field<DateTime>("ArriveDate").Month == month).Select(x => x.Field<decimal?>("ItineraryYield")).Sum());
                chart1.Series["Series2b"].Points.AddY(previous.Where(x => x.Field<DateTime>("ArriveDate").Month == month).Select(x => x.Field<decimal?>("ItineraryYield")).Sum());

                chart1.ChartAreas["Default"].AxisX.CustomLabels.Add(i+.5, i+1.5, monthName);//, 1, LabelMarkStyle.None));


                // COUNT
                //chart1.Series["Series1a"].Points.AddY(current.Where(x => x.Field<DateTime>("ArriveDate").Month == m).Count());
                //chart1.Series["Series1b"].Points.AddY(previous.Where(x => x.Field<DateTime>("ArriveDate").Month == m).Count());
            }

            // Set series chart type
            chart1.Series["Series1a"].ChartType = SeriesChartType.Line;
            chart1.Series["Series2a"].ChartType = SeriesChartType.Line;

            // Set point labels
            //chart1.Series["Series1a"].IsValueShownAsLabel = true;
            //chart1.Series["Series2a"].IsValueShownAsLabel = true;

            // X axis margin
            chart1.ChartAreas["Default"].AxisX.IsMarginVisible = false;

            // 3D
            chart1.ChartAreas["Default"].Area3DStyle.Enable3D = false;


            chart1.Series["Series1a"].EmptyPointStyle.Color = Color.Transparent; 
            chart1.Series["Series1a"].ToolTip = "#VALY{C}";
            //chart1.ChartAreas["Default"].AxisX.Interval = 1;
            //chart1.ChartAreas["Default"].AxisX.IntervalType = DateTimeIntervalType.Months;
            //chart1.ChartAreas["Default"].AxisX.LabelStyle.Format = "MMM";

        }
    }
}
