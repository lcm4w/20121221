using System;
using System.ComponentModel;

namespace TourWriter.UserControls
{
	public class WizardPageEventArgs : CancelEventArgs
	{
		WizardPage newPage = null;

		public WizardPage NewPage
		{
			get { return newPage; }
			set { newPage = value; }
		}
	}

	public delegate void WizardPageEventHandler(object sender, WizardPageEventArgs e);
}
