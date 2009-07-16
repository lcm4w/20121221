using System;
using System.Data;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.UltraWinTree;
using TourWriter.Info;

namespace TourWriter.Modules.ItineraryModule.Publishing
{
	/// <summary>
	/// Summary description for PublisherFileChooser.
	/// </summary>
	public class PublisherFileChooser : System.Windows.Forms.Form
	{
		#region Designer

		private Infragistics.Win.UltraWinToolTip.UltraToolTipManager tooltip;
		private Infragistics.Win.Misc.UltraButton btnOk;
		private Infragistics.Win.Misc.UltraButton btnCancel;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.LinkLabel btnBrowse;
		private System.Windows.Forms.Label label5;
		private Infragistics.Win.UltraWinTree.UltraTree treeFiles;
		private Infragistics.Win.UltraWinEditors.UltraComboEditor cmbSections;
		internal System.Windows.Forms.ImageList imageList1;
		private Infragistics.Win.UltraWinTree.UltraTree treeBookings;
		private System.Windows.Forms.Label label2;
		private System.ComponentModel.IContainer components;

		public PublisherFileChooser()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
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
			this.components = new System.ComponentModel.Container();
			Infragistics.Win.UltraWinTree.UltraTreeColumnSet ultraTreeColumnSet1 = new Infragistics.Win.UltraWinTree.UltraTreeColumnSet();
			Infragistics.Win.UltraWinTree.Override _override1 = new Infragistics.Win.UltraWinTree.Override();
			Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Add files that are not in the list", Infragistics.Win.ToolTipImage.Default, null, Infragistics.Win.DefaultableBoolean.Default);
			Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo2 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Selected files will be added to this section", Infragistics.Win.ToolTipImage.Default, null, Infragistics.Win.DefaultableBoolean.Default);
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(PublisherFileChooser));
			Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
			Infragistics.Win.UltraWinTree.UltraTreeColumnSet ultraTreeColumnSet2 = new Infragistics.Win.UltraWinTree.UltraTreeColumnSet();
			Infragistics.Win.UltraWinTree.Override _override2 = new Infragistics.Win.UltraWinTree.Override();
			this.treeFiles = new Infragistics.Win.UltraWinTree.UltraTree();
			this.tooltip = new Infragistics.Win.UltraWinToolTip.UltraToolTipManager(this.components);
			this.btnBrowse = new System.Windows.Forms.LinkLabel();
			this.label1 = new System.Windows.Forms.Label();
			this.cmbSections = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
			this.btnOk = new Infragistics.Win.Misc.UltraButton();
			this.btnCancel = new Infragistics.Win.Misc.UltraButton();
			this.label5 = new System.Windows.Forms.Label();
			this.imageList1 = new System.Windows.Forms.ImageList(this.components);
			this.treeBookings = new Infragistics.Win.UltraWinTree.UltraTree();
			this.label2 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.treeFiles)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cmbSections)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.treeBookings)).BeginInit();
			this.SuspendLayout();
			// 
			// treeFiles
			// 
			this.treeFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.treeFiles.ColumnSettings.RootColumnSet = ultraTreeColumnSet1;
			this.treeFiles.Location = new System.Drawing.Point(8, 160);
			this.treeFiles.Name = "treeFiles";
			this.treeFiles.NodeConnectorStyle = Infragistics.Win.UltraWinTree.NodeConnectorStyle.None;
			_override1.SelectionType = Infragistics.Win.UltraWinTree.SelectType.ExtendedAutoDrag;
			_override1.TipStyleNode = Infragistics.Win.UltraWinTree.TipStyleNode.Hide;
			this.treeFiles.Override = _override1;
			this.treeFiles.Size = new System.Drawing.Size(456, 200);
			this.treeFiles.TabIndex = 0;
			this.treeFiles.AfterSelect += new Infragistics.Win.UltraWinTree.AfterNodeSelectEventHandler(this.treeFileList_AfterSelect);
			this.treeFiles.DoubleClick += new System.EventHandler(this.treeFileList_DoubleClick);
			// 
			// btnBrowse
			// 
			this.btnBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnBrowse.Location = new System.Drawing.Point(380, 140);
			this.btnBrowse.Name = "btnBrowse";
			this.btnBrowse.Size = new System.Drawing.Size(84, 23);
			this.btnBrowse.TabIndex = 13;
			this.btnBrowse.TabStop = true;
			this.btnBrowse.Text = "Browse for files";
			this.btnBrowse.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			ultraToolTipInfo1.ToolTipText = "Add files that are not in the list";
			this.tooltip.SetUltraToolTip(this.btnBrowse, ultraToolTipInfo1);
			this.btnBrowse.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.btnBrowse_LinkClicked);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(96, 23);
			this.label1.TabIndex = 4;
			this.label1.Text = "Select the files for";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			ultraToolTipInfo2.ToolTipText = "Selected files will be added to this section";
			this.tooltip.SetUltraToolTip(this.label1, ultraToolTipInfo2);
			// 
			// cmbSections
			// 
			this.cmbSections.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.cmbSections.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
			this.cmbSections.Location = new System.Drawing.Point(100, 8);
			this.cmbSections.Name = "cmbSections";
			this.cmbSections.Size = new System.Drawing.Size(364, 21);
			this.cmbSections.TabIndex = 3;
			// 
			// btnOk
			// 
			this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnOk.Enabled = false;
			this.btnOk.Location = new System.Drawing.Point(296, 364);
			this.btnOk.Name = "btnOk";
			this.btnOk.TabIndex = 1;
			this.btnOk.Text = "OK";
			this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(388, 364);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.TabIndex = 2;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// label5
			// 
			this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.label5.Location = new System.Drawing.Point(8, 140);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(372, 23);
			this.label5.TabIndex = 15;
			this.label5.Text = "Select the files to be added to the Publisher layout";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// imageList1
			// 
			this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
			this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
			this.imageList1.TransparentColor = System.Drawing.Color.Silver;
			// 
			// treeBookings
			// 
			this.treeBookings.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			appearance1.BackColor = System.Drawing.SystemColors.ControlLight;
			this.treeBookings.Appearance = appearance1;
			this.treeBookings.ColumnSettings.RootColumnSet = ultraTreeColumnSet2;
			this.treeBookings.Location = new System.Drawing.Point(8, 52);
			this.treeBookings.Name = "treeBookings";
			this.treeBookings.NodeConnectorStyle = Infragistics.Win.UltraWinTree.NodeConnectorStyle.None;
			_override2.BorderStyleNode = Infragistics.Win.UIElementBorderStyle.None;
			this.treeBookings.Override = _override2;
			this.treeBookings.Size = new System.Drawing.Size(456, 84);
			this.treeBookings.TabIndex = 16;
			this.treeBookings.BeforeActivate += new Infragistics.Win.UltraWinTree.BeforeNodeChangedEventHandler(this.treeBookings_BeforeActivate);
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.label2.Location = new System.Drawing.Point(8, 32);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(456, 23);
			this.label2.TabIndex = 17;
			this.label2.Text = "Supplier bookings for selected day";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// PublisherFileChooser
			// 
			this.AcceptButton = this.btnOk;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.BackColor = System.Drawing.Color.WhiteSmoke;
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(472, 394);
			this.Controls.Add(this.treeBookings);
			this.Controls.Add(this.treeFiles);
			this.Controls.Add(this.btnBrowse);
			this.Controls.Add(this.cmbSections);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label2);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.Name = "PublisherFileChooser";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Choose publisher files";
			this.Load += new System.EventHandler(this.PublisherFileChooser_Load);
			((System.ComponentModel.ISupportInitialize)(this.treeFiles)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cmbSections)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.treeBookings)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion
		#endregion

		private readonly ItinerarySet itinerarySet;
		private readonly ArrayList publisherSectionNodes;
		private readonly string defaultPublisherSection;
		
		public PublisherFileChooser(ItinerarySet itinerarySet, ArrayList publisherSectionNodes)
		{
			this.itinerarySet = itinerarySet;
			this.publisherSectionNodes = publisherSectionNodes;
			InitializeComponent();
		}

		public PublisherFileChooser(ItinerarySet itinerarySet, ArrayList publisherSectionNodes, string defaultPublisherSection)
		{
			this.itinerarySet = itinerarySet;
			this.publisherSectionNodes = publisherSectionNodes;
			this.defaultPublisherSection = defaultPublisherSection;
			InitializeComponent();
		}

		
		private void PublisherFileChooser_Load(object sender, System.EventArgs e)
		{
			// set up combo list
			foreach(UltraTreeNode node in publisherSectionNodes)
				cmbSections.Items.Add(node.Key, node.Text);
			cmbSections.SelectionChanged +=new EventHandler(cmbSections_SelectionChanged);
			
			// set initial combo selection
			if (defaultPublisherSection != null)
				cmbSections.Value = this.defaultPublisherSection;
			else if (cmbSections.Items.Count > 1)
				cmbSections.SelectedIndex = 1;
			else if (cmbSections.Items.Count > 0)
				cmbSections.SelectedIndex = 0;
		}
		
		public ArrayList GetSelectedFiles()
		{
			ArrayList list = new ArrayList();
			
			foreach(UltraTreeNode node in treeFiles.SelectedNodes)
				list.Add(node);

			//foreach(UltraTreeNode node in treeFileList.Nodes)
			//	if(node.CheckedState == CheckState.Checked)
			//		list.Add(node);
			return list;
		}

		public string GetSelectedSectionKey()
		{
			return cmbSections.Value.ToString();
		}

		private void ClearAutoFiles()
		{
			// clear all auto-added files, but keep any manually added files.
			ArrayList a = new ArrayList();
			foreach(UltraTreeNode node in treeFiles.Nodes)
				if(node.Text.StartsWith(FileBuilderAdvanced.FileNodeTag))
					a.Add(node.Clone());
			treeFiles.Nodes.Clear();
			treeFiles.Nodes.AddRange((UltraTreeNode[])a.ToArray(typeof(UltraTreeNode)));
		}
		
		private void AutoFillListsForDay(DateTime date)
		{
			string nodetext;
			int nodeInsertPosition = 0;

			// add supplier files
			foreach (ItinerarySet.PurchaseItemRow item in itinerarySet.PurchaseItem)
			{
				// don't show an item that has no start date (eg booking fee)
				if (item.RowState == DataRowState.Deleted || item.IsStartDateNull())
					continue;

				DateTime itemEndDate = 
					item.StartDate.AddDays(!item.IsNumberOfDaysNull() ? item.NumberOfDays : 1);

				// does it span the choosen date?
				if (date.Date < item.StartDate.Date || date.Date > itemEndDate.Date)
					continue;

				ItinerarySet.PurchaseLineRow line =
					itinerarySet.PurchaseLine.FindByPurchaseLineID(item.PurchaseLineID);

				// get array of supplier texts
				string filter = "SupplierID = " + line.SupplierID.ToString();
				ItinerarySet.SupplierTextRow[] docs = (ItinerarySet.SupplierTextRow[])
					itinerarySet.SupplierText.Select(filter);

				// add booking node
				AddBookingNode(String.Format("{0}, {1}",
					!line.IsPurchaseLineNameNull() ? line.PurchaseLineName : "",
					!item.IsPurchaseItemNameNull() ? item.PurchaseItemName : "" ));

				// add files nodes
				foreach (ItinerarySet.SupplierTextRow row in docs)
				{
					if (!row.IsFileNameNull() && row.FileName.Length > 0)
					{
						nodetext = string.Format(
							"{0}: {1}  ({2})",
							!line.IsPurchaseLineNameNull() ? line.PurchaseLineName : "",
							row.SupplierTextName,
							row.FileName);

						// don't add node twice
						foreach (UltraTreeNode foundNode in this.treeFiles.Nodes)
							if (foundNode.Text == nodetext)
								continue;

						LayoutHelper.AddFileNode(treeFiles, nodeInsertPosition++, row.FileName, nodetext);
					}
				}
			}
		}

		private void AddBookingNode(string text)
		{
			treeBookings.Nodes.Add(null, text);//.Enabled = false;
		}


		private void cmbSections_SelectionChanged(object sender, EventArgs e)
		{
			treeFiles.SuspendLayout();
			treeBookings.SuspendLayout();

			// clear lists
			treeBookings.Nodes.Clear();
			treeFiles.SelectedNodes.Clear();
			ClearAutoFiles();

			// populate lists
			if (cmbSections.Value.ToString().StartsWith(FileBuilderAdvanced.DaysSectionNodeKey))
				AutoFillListsForDay(LayoutHelper.GetDaySectionDate(cmbSections.Value.ToString()));

			treeFiles.ResumeLayout();
			treeBookings.ResumeLayout();
		}

		private void btnBrowse_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
            string fileFilter = "Word objects (*.doc;*.docx;*.rtf;*.jpg;*.jpeg;*.gif;*.png;*.bmp)|" +
                "*.doc;*.docx;*.rtf;*.jpg;*.jpeg;*.gif;*.png;*.bmp|All files (*.*)|*.*";
            string[] files = App.SelectExternalFiles(true, "Choose files", fileFilter, 1);

            if (files != null)
            {
                foreach (string file in files)
                {
                    int i = file.LastIndexOf("\\") + 1;
                    string nodetext = string.Format(
                        "{0}: {1}  ({2})",
                        FileBuilderAdvanced.FileNodeTag,
                        string.Format(file.Substring(i, file.Length - i)),
                        file);

                    LayoutHelper.AddFileNode(treeFiles, treeFiles.Nodes.Count, file, nodetext);
                }
            }
		}

		private void btnOk_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.OK;
		
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
		}

		private void treeFileList_AfterSelect(object sender, SelectEventArgs e)
		{
			btnOk.Enabled = treeFiles.SelectedNodes.Count > 0;
		}

		private void treeFileList_DoubleClick(object sender, EventArgs e)
		{
			UltraTree treeControl = sender as UltraTree;
			Point point = treeControl.PointToClient( Control.MousePosition );
			UltraTreeUIElement mainElement = treeControl.UIElement;
			UltraTreeNode doubleClickedNode = null;
			if ( mainElement != null )
			{
				UIElement elementAtPoint = mainElement.ElementFromPoint( point );

				while ( elementAtPoint != null )
				{
					NodeSelectableAreaUIElement nodeElement = elementAtPoint as 
						NodeSelectableAreaUIElement;
					if ( nodeElement != null )
					{
						doubleClickedNode = nodeElement.Node;
						break;
					}
					elementAtPoint = elementAtPoint.Parent;
				}
				if(doubleClickedNode != null)
				{
					btnOk.PerformClick();
				}
			}

		}

		private void treeBookings_BeforeActivate(object sender, CancelableNodeEventArgs e)
		{
			e.Cancel = true;
		}
		
	}
}
