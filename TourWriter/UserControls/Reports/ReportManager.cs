﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace TourWriter.UserControls.Reports
{
    public partial class ReportManager : UserControl
    {
        public Dictionary<string, object> DefaultParameters { get; set; }

        public ReportManager()
        {
            InitializeComponent();
            DefaultParameters = new Dictionary<string, object>();
        }

        public void FilterReportExplorer(string categoryName)
        {
            reportExplorer.FilterReportTree(categoryName);
        }

        public void RefreshReportExplorer()
        {
            reportExplorer.RebuildReportTree();
        }

        private void OpenReport(string reportName, string reportFile)
        {
            var reportViewer = new ViewerControl(reportName, reportFile, DefaultParameters)
                                   {Dock = DockStyle.Fill};
            reportViewer.ViewerControlClosed += viewerControl_ReportHostClosed;

            // create a new tab and add the ReportHost control to it
            var key = Guid.NewGuid().ToString();
            reportTabControl.Tabs.Add(key, reportName);
            reportTabControl.Tabs[key].TabPage.Controls.Add(reportViewer);
            reportTabControl.SelectedTab = reportTabControl.Tabs[key];
        }

        private void reportExplorer_ReportDoubleClick(object sender, ReportDoubleClickEventArgs e)
        {
            if (File.Exists(e.ReportPath))
                OpenReport(e.ReportName, e.ReportPath);
            else
                App.ShowError("Report file not found: " + e.ReportPath);
        }

        private void viewerControl_ReportHostClosed(object sender, EventArgs e)
        {
            // look for the tab which contains this control, and remove it
            for (int i = reportTabControl.Tabs.Count - 1; i >= 0; i--)
            {
                var tab = reportTabControl.Tabs[i];

                if (tab.TabPage.Controls.Contains((Control)sender))
                {
                    reportTabControl.Tabs.Remove(tab);
                    break;
                }
            }
        }
    }
}