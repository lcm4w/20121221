using System.ComponentModel;
using System.Windows.Forms;
using Infragistics.Win.UltraWinEditors;
using TourWriter.UserControls;

namespace TourWriter.Modules.ItineraryModule.Converter
{
	public class ConverterTypes : WizardPage
	{
		#region Designer

		private UltraOptionSet optType;
		private Label label1;
		private Label label2;
		private Label label3;
		private Infragistics.Win.UltraWinTree.UltraTree ultraTree1;
		private IContainer components = null;

		public ConverterTypes()
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
			Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
			Infragistics.Win.ValueListItem valueListItem1 = new Infragistics.Win.ValueListItem();
			Infragistics.Win.ValueListItem valueListItem2 = new Infragistics.Win.ValueListItem();
			Infragistics.Win.ValueListItem valueListItem3 = new Infragistics.Win.ValueListItem();
			Infragistics.Win.UltraWinTree.UltraTreeColumnSet ultraTreeColumnSet1 = new Infragistics.Win.UltraWinTree.UltraTreeColumnSet();
			Infragistics.Win.UltraWinTree.Override _override1 = new Infragistics.Win.UltraWinTree.Override();
			this.optType = new Infragistics.Win.UltraWinEditors.UltraOptionSet();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.ultraTree1 = new Infragistics.Win.UltraWinTree.UltraTree();
			((System.ComponentModel.ISupportInitialize)(this.optType)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.ultraTree1)).BeginInit();
			this.SuspendLayout();
			// 
			// optType
			// 
			this.optType.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
			this.optType.ItemAppearance = appearance1;
			this.optType.ItemOrigin = new System.Drawing.Point(0, -25);
			valueListItem1.DataValue = "Default Item";
			valueListItem1.DisplayText = "Quick Booking";
			valueListItem2.DataValue = "ValueListItem1";
			valueListItem2.DisplayText = "Advanced Itinerary";
			valueListItem3.DataValue = "ValueListItem2";
			valueListItem3.DisplayText = "Group Itinerary";
			this.optType.Items.Add(valueListItem1);
			this.optType.Items.Add(valueListItem2);
			this.optType.Items.Add(valueListItem3);
			this.optType.ItemSpacingVertical = 50;
			this.optType.Location = new System.Drawing.Point(16, 36);
			this.optType.Name = "optType";
			this.optType.Size = new System.Drawing.Size(364, 188);
			this.optType.TabIndex = 3;
			this.optType.ValueChanged += new System.EventHandler(this.optType_ValueChanged);
			// 
			// label1
			// 
			this.label1.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
			this.label1.Location = new System.Drawing.Point(24, 52);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(344, 36);
			this.label1.TabIndex = 7;
			this.label1.Text = "Designed for quick and easy bookings, or when you just want to record a new clien" +
				"ts interest in an itinerary.";
			// 
			// label2
			// 
			this.label2.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
			this.label2.Location = new System.Drawing.Point(24, 116);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(344, 36);
			this.label2.TabIndex = 8;
			this.label2.Text = "Includes many advanced features for managing large custom itineraries.";
			// 
			// label3
			// 
			this.label3.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
			this.label3.Location = new System.Drawing.Point(24, 180);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(344, 36);
			this.label3.TabIndex = 9;
			this.label3.Text = "Designed for managing group tour itineraries, and includes many of the features o" +
				"f the Advanced Itinerary option above.";
			// 
			// ultraTree1
			// 
			this.ultraTree1.ColumnSettings.RootColumnSet = ultraTreeColumnSet1;
			this.ultraTree1.Location = new System.Drawing.Point(12, 232);
			this.ultraTree1.Name = "ultraTree1";
			this.ultraTree1.NodeConnectorStyle = Infragistics.Win.UltraWinTree.NodeConnectorStyle.None;
			_override1.NodeStyle = Infragistics.Win.UltraWinTree.NodeStyle.CheckBox;
			this.ultraTree1.Override = _override1;
			this.ultraTree1.Size = new System.Drawing.Size(368, 264);
			this.ultraTree1.TabIndex = 10;
			// 
			// ConverterTypes
			// 
			this.BackColor = System.Drawing.Color.WhiteSmoke;
			this.Controls.Add(this.ultraTree1);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.optType);
			this.Name = "ConverterTypes";
			this.PageText = "Choose conversion type";
			this.Size = new System.Drawing.Size(392, 504);
			this.Load += new System.EventHandler(this.ConverterTypes_Load);
			this.Controls.SetChildIndex(this.optType, 0);
			this.Controls.SetChildIndex(this.label1, 0);
			this.Controls.SetChildIndex(this.label2, 0);
			this.Controls.SetChildIndex(this.label3, 0);
			this.Controls.SetChildIndex(this.ultraTree1, 0);
			((System.ComponentModel.ISupportInitialize)(this.optType)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.ultraTree1)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private void optType_ValueChanged(object sender, System.EventArgs e)
		{
		}

		#endregion


		private void ConverterTypes_Load(object sender, System.EventArgs e)
		{
			optType.CheckedIndex = 0;
				//(Wizard.Params as ConverterArgs)._ItinerarySet.Itinerary[0].ItineraryTypeID;
		}
	}
}