using System.ComponentModel;
using TourWriter.UserControls;

namespace TourWriter.Modules.ItineraryModule.Converter
{
	public class ConverterOptions : WizardPage
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private Infragistics.Win.UltraWinEditors.UltraCheckEditor ultraCheckEditor1;
		private Infragistics.Win.UltraWinEditors.UltraCheckEditor ultraCheckEditor2;
		private Infragistics.Win.UltraWinEditors.UltraCheckEditor ultraCheckEditor3;
		#region Designer

		private IContainer components = null;

		public ConverterOptions()
		{
			// This call is required by the Windows Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		#region Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.ultraCheckEditor1 = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
			this.ultraCheckEditor2 = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
			this.ultraCheckEditor3 = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
			this.label1.Location = new System.Drawing.Point(28, 52);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(348, 52);
			this.label1.TabIndex = 6;
			this.label1.Text = "There are unsaved changes to the Itineary that you are about to convert. Tick thi" +
				"s option to have these changes saved before the conversion takes place. If you c" +
				"hoose not to tick this option, the outstanding changes will be lost.";
			// 
			// label2
			// 
			this.label2.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
			this.label2.Location = new System.Drawing.Point(28, 132);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(348, 56);
			this.label2.TabIndex = 7;
			this.label2.Text = "The Publishing feature allows you to create advanced client documents for the iti" +
				"nerary, including supplier descriptions and directions. You can choose to turn o" +
				"n or off this feature at a later time, using this conversion wizard. ";
			// 
			// label3
			// 
			this.label3.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
			this.label3.Location = new System.Drawing.Point(28, 216);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(344, 60);
			this.label3.TabIndex = 8;
			this.label3.Text = "The Price Adjustment feature allows you to adjust the final client price of the i" +
				"tinerary based on the margins for each service type. You can choose to turn on o" +
				"r off this feature at a later time, using this conversion wizard. ";
			// 
			// ultraCheckEditor1
			// 
			this.ultraCheckEditor1.Location = new System.Drawing.Point(12, 32);
			this.ultraCheckEditor1.Name = "ultraCheckEditor1";
			this.ultraCheckEditor1.Size = new System.Drawing.Size(364, 20);
			this.ultraCheckEditor1.TabIndex = 9;
			this.ultraCheckEditor1.Text = "Save changes";
			// 
			// ultraCheckEditor2
			// 
			this.ultraCheckEditor2.Location = new System.Drawing.Point(12, 112);
			this.ultraCheckEditor2.Name = "ultraCheckEditor2";
			this.ultraCheckEditor2.Size = new System.Drawing.Size(364, 20);
			this.ultraCheckEditor2.TabIndex = 10;
			this.ultraCheckEditor2.Text = "Include Itinerary Publishing feature.";
			// 
			// ultraCheckEditor3
			// 
			this.ultraCheckEditor3.Location = new System.Drawing.Point(12, 196);
			this.ultraCheckEditor3.Name = "ultraCheckEditor3";
			this.ultraCheckEditor3.Size = new System.Drawing.Size(360, 20);
			this.ultraCheckEditor3.TabIndex = 11;
			this.ultraCheckEditor3.Text = "Include Final Price Adjustment feature.";
			// 
			// ConverterOptions
			// 
			this.BackColor = System.Drawing.Color.WhiteSmoke;
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.ultraCheckEditor1);
			this.Controls.Add(this.ultraCheckEditor2);
			this.Controls.Add(this.ultraCheckEditor3);
			this.Name = "ConverterOptions";
			this.PageText = "Choose conversion options";
			this.Size = new System.Drawing.Size(392, 280);
			this.Load += new System.EventHandler(this.ConverterOptions_Load);
			this.Controls.SetChildIndex(this.ultraCheckEditor3, 0);
			this.Controls.SetChildIndex(this.ultraCheckEditor2, 0);
			this.Controls.SetChildIndex(this.ultraCheckEditor1, 0);
			this.Controls.SetChildIndex(this.label1, 0);
			this.Controls.SetChildIndex(this.label2, 0);
			this.Controls.SetChildIndex(this.label3, 0);
			this.ResumeLayout(false);

		}

		#endregion

		private void ConverterOptions_Load(object sender, System.EventArgs e)
		{
			Wizard.ButtonFinish.Enabled = false;
		}

		#endregion
	}
}