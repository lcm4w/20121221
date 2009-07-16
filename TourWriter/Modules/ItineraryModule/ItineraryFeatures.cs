using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Infragistics.Win.UltraWinTree;

namespace TourWriter.Modules.ItineraryModule
{
	/// <summary>
	/// Summary description for ItineraryFeatures.
	/// </summary>
	public class ItineraryFeatures : System.Windows.Forms.Form
	{
		#region Designer
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private Infragistics.Win.UltraWinEditors.UltraOptionSet optType;
		private Infragistics.Win.Misc.UltraButton btnOk;
		private Infragistics.Win.Misc.UltraButton btnCancel;
		private Infragistics.Win.UltraWinTree.UltraTree treeFeatures;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ItineraryFeatures()
		{
			InitializeComponent();
			
			treeFeatures.Nodes.Clear();
			for(int i = 0; i<AllFeatures.Length; i++)
				treeFeatures.Nodes.Add(new UltraTreeNode("", AllFeatures[i]));
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			Infragistics.Win.UltraWinTree.UltraTreeColumnSet ultraTreeColumnSet1 = new Infragistics.Win.UltraWinTree.UltraTreeColumnSet();
			Infragistics.Win.UltraWinTree.UltraTreeNode ultraTreeNode1 = new Infragistics.Win.UltraWinTree.UltraTreeNode();
			Infragistics.Win.UltraWinTree.UltraTreeNode ultraTreeNode2 = new Infragistics.Win.UltraWinTree.UltraTreeNode();
			Infragistics.Win.UltraWinTree.UltraTreeNode ultraTreeNode3 = new Infragistics.Win.UltraWinTree.UltraTreeNode();
			Infragistics.Win.UltraWinTree.UltraTreeNode ultraTreeNode4 = new Infragistics.Win.UltraWinTree.UltraTreeNode();
			Infragistics.Win.UltraWinTree.UltraTreeNode ultraTreeNode5 = new Infragistics.Win.UltraWinTree.UltraTreeNode();
			Infragistics.Win.UltraWinTree.UltraTreeNode ultraTreeNode6 = new Infragistics.Win.UltraWinTree.UltraTreeNode();
			Infragistics.Win.UltraWinTree.UltraTreeNode ultraTreeNode7 = new Infragistics.Win.UltraWinTree.UltraTreeNode();
			Infragistics.Win.UltraWinTree.Override _override1 = new Infragistics.Win.UltraWinTree.Override();
			Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
			Infragistics.Win.ValueListItem valueListItem1 = new Infragistics.Win.ValueListItem();
			Infragistics.Win.ValueListItem valueListItem2 = new Infragistics.Win.ValueListItem();
			Infragistics.Win.ValueListItem valueListItem3 = new Infragistics.Win.ValueListItem();
			Infragistics.Win.ValueListItem valueListItem4 = new Infragistics.Win.ValueListItem();
			this.treeFeatures = new Infragistics.Win.UltraWinTree.UltraTree();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.optType = new Infragistics.Win.UltraWinEditors.UltraOptionSet();
			this.btnOk = new Infragistics.Win.Misc.UltraButton();
			this.btnCancel = new Infragistics.Win.Misc.UltraButton();
			((System.ComponentModel.ISupportInitialize)(this.treeFeatures)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.optType)).BeginInit();
			this.SuspendLayout();
			// 
			// treeFeatures
			// 
			this.treeFeatures.ColumnSettings.RootColumnSet = ultraTreeColumnSet1;
			this.treeFeatures.Indent = 15;
			this.treeFeatures.Location = new System.Drawing.Point(24, 190);
			this.treeFeatures.Name = "treeFeatures";
			this.treeFeatures.NodeConnectorStyle = Infragistics.Win.UltraWinTree.NodeConnectorStyle.None;
			ultraTreeNode1.DataKey = "";
			ultraTreeNode1.Enabled = false;
			ultraTreeNode1.Key = "Details";
			ultraTreeNode1.Text = "Details: general details";
			ultraTreeNode2.DataKey = "";
			ultraTreeNode2.Key = "Bookings";
			ultraTreeNode2.Text = "Bookings: manage bookings";
			ultraTreeNode3.Key = "Clients";
			ultraTreeNode3.Text = "Clients: client in the Itinerary";
			ultraTreeNode4.Key = "Reports";
			ultraTreeNode4.Text = "Reports: general reports for Itinerary";
			ultraTreeNode5.Key = "Publisher";
			ultraTreeNode5.Text = "Publisher: for creating client Itinerary documents";
			ultraTreeNode6.Key = "Planner";
			ultraTreeNode6.Text = "Groups: features for managing group Itineraries";
			ultraTreeNode7.Key = "Messages";
			ultraTreeNode7.Text = "Messages: for viewing the message history";
			this.treeFeatures.Nodes.AddRange(new Infragistics.Win.UltraWinTree.UltraTreeNode[] {
																								   ultraTreeNode1,
																								   ultraTreeNode2,
																								   ultraTreeNode3,
																								   ultraTreeNode4,
																								   ultraTreeNode5,
																								   ultraTreeNode6,
																								   ultraTreeNode7});
			_override1.NodeStyle = Infragistics.Win.UltraWinTree.NodeStyle.CheckBox;
			this.treeFeatures.Override = _override1;
			this.treeFeatures.Size = new System.Drawing.Size(328, 188);
			this.treeFeatures.TabIndex = 15;
			// 
			// label3
			// 
			this.label3.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
			this.label3.Location = new System.Drawing.Point(24, 136);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(344, 36);
			this.label3.TabIndex = 14;
			this.label3.Text = "Designed for managing group tour itineraries, and includes many of the features o" +
				"f the Advanced Itinerary option above.";
			// 
			// label2
			// 
			this.label2.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
			this.label2.Location = new System.Drawing.Point(24, 82);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(344, 36);
			this.label2.TabIndex = 13;
			this.label2.Text = "Includes many advanced features for managing large custom itineraries.";
			// 
			// label1
			// 
			this.label1.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
			this.label1.Location = new System.Drawing.Point(24, 28);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(344, 36);
			this.label1.TabIndex = 12;
			this.label1.Text = "Designed for quick and easy bookings, or when you just want to record a new clien" +
				"ts interest in an itinerary.";
			// 
			// optType
			// 
			this.optType.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
			this.optType.ItemAppearance = appearance1;
			this.optType.ItemOrigin = new System.Drawing.Point(0, -20);
			valueListItem1.DataValue = "Quick";
			valueListItem1.DisplayText = "Quick Booking";
			valueListItem2.DataValue = "Advanced";
			valueListItem2.DisplayText = "Advanced Itinerary";
			valueListItem3.DataValue = "Group";
			valueListItem3.DisplayText = "Group Itinerary";
			valueListItem4.DataValue = "Custom";
			valueListItem4.DisplayText = "Custom";
			this.optType.Items.Add(valueListItem1);
			this.optType.Items.Add(valueListItem2);
			this.optType.Items.Add(valueListItem3);
			this.optType.Items.Add(valueListItem4);
			this.optType.ItemSpacingVertical = 40;
			this.optType.Location = new System.Drawing.Point(16, 12);
			this.optType.Name = "optType";
			this.optType.Size = new System.Drawing.Size(364, 210);
			this.optType.TabIndex = 11;
			this.optType.ValueChanged += new System.EventHandler(this.optType_ValueChanged);
			// 
			// btnOk
			// 
			this.btnOk.Location = new System.Drawing.Point(110, 396);
			this.btnOk.Name = "btnOk";
			this.btnOk.TabIndex = 16;
			this.btnOk.Text = "OK";
			this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(192, 396);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.TabIndex = 17;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// ItineraryFeatures
			// 
			this.AcceptButton = this.btnOk;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(376, 428);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.treeFeatures);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.optType);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "ItineraryFeatures";
			this.Text = "Itinerary Type";
			((System.ComponentModel.ISupportInitialize)(this.treeFeatures)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.optType)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion
		#endregion

		internal static string[] QuickFeatures		= new string[]{"0","1"};
		internal static string[] AdvancedFeatures	= new string[]{"0","1","2","3","4","5"};
		internal static string[] GroupFeatures		= new string[]{"0","1","2","3","4","5","6"};
		internal static string[] AllFeatures = new string[]
			{
				"Details: general details",
				"Bookings: manage bookings",
				"Clients: client in the Itinerary",
				"Reports: general reports for Itinerary",
				"Messages: for viewing the message history",
				"Publisher: for creating client Itinerary documents",
				"Groups: features for managing group Itineraries"
			};

		
		public string[] SelectedFeatures
		{
			set
			{
				optType.Value = null;
			
				if(value.Equals(QuickFeatures))
					optType.Value = "Quick";
				else if(value.Equals(AdvancedFeatures))
					optType.Value = "Advanced";
				else if(value.Equals(GroupFeatures))
					optType.Value = "Group";
				else
				{
					optType.Value = "Custom";
					SetDisplayChoices(value);
				}

			}
			get
			{
				ArrayList ar = new ArrayList();
				foreach(UltraTreeNode node in this.treeFeatures.Nodes)
				{
					if(node.CheckedState == CheckState.Checked)
						ar.Add(node.Index.ToString());
				}
				return ar.ToArray(typeof(string)) as string[];
			}
		}

		private void SetDisplayChoices(string[] choices)
		{
			// Uncheck all nodes.
			foreach(UltraTreeNode node in this.treeFeatures.Nodes)
				node.CheckedState = CheckState.Unchecked;

			// Check chosen nodes.
			for(int i = 0; i<choices.Length; i++)
			{
				int index = int.Parse(choices[i]);
				treeFeatures.Nodes[index].CheckedState = CheckState.Checked;
			}
		}


		private void optType_ValueChanged(object sender, System.EventArgs e)
		{
			switch(optType.Value.ToString())
			{
				case "Quick" :
				{
					SetDisplayChoices(QuickFeatures);
					treeFeatures.Enabled = false;
					break;
				}
				case "Advanced" :
				{
					SetDisplayChoices(AdvancedFeatures);
					treeFeatures.Enabled = false;
					break;
				}
				case "Group" :
				{
					SetDisplayChoices(GroupFeatures);
					treeFeatures.Enabled = false;
					break;
				}
				case "Custom" :
				{
					treeFeatures.Enabled = true;
					break;
				}
			}
		}

		private void btnOk_Click(object sender, System.EventArgs e)
		{
			this.DialogResult = DialogResult.OK;
		}

		private void btnCancel_Click(object sender, System.EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
		}
	}
}
