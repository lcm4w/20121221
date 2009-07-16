using System;
using System.Data;
using System.Collections;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.UltraWinTree;

namespace TourWriter.UserControls
{
	/// <summary>
	/// A control for displaying a Value/Display pair item list with a checkbox in each row.
	/// </summary>
	public class CheckBoxSet : UserControl
	{
		#region Designer
		private UltraTree tree;
		private System.ComponentModel.IContainer components = null;
		
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

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			Infragistics.Win.UltraWinTree.UltraTreeColumnSet ultraTreeColumnSet1 = new Infragistics.Win.UltraWinTree.UltraTreeColumnSet();
			Infragistics.Win.UltraWinTree.Override _override1 = new Infragistics.Win.UltraWinTree.Override();
			this.tree = new Infragistics.Win.UltraWinTree.UltraTree();
			((System.ComponentModel.ISupportInitialize)(this.tree)).BeginInit();
			this.SuspendLayout();
			// 
			// tree
			// 
			this.tree.ColumnSettings.RootColumnSet = ultraTreeColumnSet1;
			this.tree.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tree.Location = new System.Drawing.Point(0, 0);
			this.tree.Name = "tree";
			_override1.NodeStyle = Infragistics.Win.UltraWinTree.NodeStyle.CheckBox;
			this.tree.Override = _override1;
			this.tree.Size = new System.Drawing.Size(150, 150);
			this.tree.TabIndex = 0;
			this.tree.AfterCheck += new Infragistics.Win.UltraWinTree.AfterNodeChangedEventHandler(this.tree_AfterCheck);
			this.tree.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tree_MouseDown);
			// 
			// CheckBoxSet
			// 
			this.BackColor = System.Drawing.Color.WhiteSmoke;
			this.Controls.Add(this.tree);
			this.Name = "CheckBoxSet";
			((System.ComponentModel.ISupportInitialize)(this.tree)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion
		#endregion		
		private const string NULLDATA_DISPLAYTEXT = "<unassigned>";

		public bool ShowCheckAll = true;
        public bool IncludeNullValueRow { get; private set; }

		public CheckBoxSet()
		{
			InitializeComponent();
			
			tree.Override.NodeStyle = NodeStyle.CheckBox;			
			tree.Override.ActiveNodeAppearance.BackColor = System.Drawing.Color.Transparent;
			tree.Override.ActiveNodeAppearance.ForeColor = System.Drawing.Color.Black;			
			tree.Override.NodeDoubleClickAction = NodeDoubleClickAction.None;
		}
		
		/// <summary>
		/// Populates the control with a list of data, and sets its parameters.
		/// </summary>
		/// <param name="dataSource">The list that this control will use to get its items</param>
		/// <param name="valueMember">The property to use for the actual value for the items in the control</param>
		/// <param name="displayMember">The property to display for the items in the control</param>
		/// <param name="includeNullValueRow">Determines whether a row will be added for NULL (unassigned) value (only supports DataTable as the data source, otherwise throw exception).</param>
		public void Initialise(object dataSource, string valueMember, string displayMember, bool includeNullValueRow)
		{
			if(!(dataSource is DataTable))
				throw new ArgumentException("Only DataTable type of DataSource is supported when including null value row");
			
            tree.Nodes.Clear();
			UltraTreeNode node;	
			TreeNodesCollection nodes;

			if(ShowCheckAll)
			{
				node = new UltraTreeNode("wer")
				           {
				               Override = {ShowExpansionIndicator = ShowExpansionIndicator.Never},
				               Expanded = true,
				               CheckedState = CheckState.Checked
				           };
			    tree.Nodes.Add(node);			
				nodes = node.Nodes;
			}
			else nodes = tree.Nodes;
				
			// add row for unassigned (NULL) values
			if(includeNullValueRow)
			{
				node = new UltraTreeNode(null) {Text = NULLDATA_DISPLAYTEXT, Tag = "NULL"};
			    nodes.Add(node);
                IncludeNullValueRow = true;
			}

			// add data rows
            var table = dataSource as DataTable;
			foreach(DataRow dr in table.Rows)
			{
                // don't add rows that have been soft deleted
                if (table.Columns.Contains("IsDeleted") &&
                    (dr["IsDeleted"] != DBNull.Value && (bool)dr["IsDeleted"]))
                {
                    continue;
                }
                node = new UltraTreeNode(null) {Text = dr[displayMember].ToString(), Tag = dr[valueMember]};
			    nodes.Add(node);
			}
		}

		/// <summary>
		/// Checks or unchecks all the checkboxs of the items in the list.
		/// </summary>
		/// <param name="checkState">if set to <c>true</c> [check state].</param>
		public void CheckAll(bool checkState)
		{			
			TreeNodesCollection nodes = ShowCheckAll ? tree.Nodes[0].Nodes : tree.Nodes;
			CheckState chkState = checkState ? CheckState.Checked : CheckState.Unchecked; 

			foreach(UltraTreeNode node in nodes)
				node.CheckedState = chkState;

			if(ShowCheckAll)
				tree.Nodes[0].Text = checkState ? "Select none" : "Select all";
		}

		/// <summary>
		/// Gets the selected key values in comma separated string, ready for an SQL 'IN' clause.
		/// Example: NULL,1,2,3
		/// </summary>
		/// <returns>Comma separated string of key values</returns>
		public string GetResultAsCsvString()
		{
			string s = "";
			TreeNodesCollection nodes = ShowCheckAll && tree.Nodes.Count == 1 ? tree.Nodes[0].Nodes : tree.Nodes;

			foreach(UltraTreeNode node in nodes)
				if(node.CheckedState == CheckState.Checked)
					s += node.Tag + ",";
			
			return s.TrimEnd(',');
		}

		/// <summary>
		/// Gets the selected key/value pairs in a HashTable, possibly including the key 'key = "NULL"'.
		/// Example keys: NULL,1,2,3
		/// </summary>
		/// <returns>HashTable of key/value pairs, possibly including the key 'key = "NULL"'.</returns>
		public Hashtable GetResultAsHashTable()
		{
			var dictionary = new Hashtable();
			TreeNodesCollection nodes = ShowCheckAll ? tree.Nodes[0].Nodes : tree.Nodes;

			foreach(UltraTreeNode node in nodes)
				if(node.CheckedState == CheckState.Checked)
					dictionary.Add(node.Tag.ToString(), node.Text);

			return dictionary;
		}

		
		private void tree_AfterCheck(object sender, NodeEventArgs e)
		{
			// Handle the select all node.
			if(e.TreeNode == tree.Nodes[0])
				CheckAll(e.TreeNode.CheckedState == CheckState.Checked);		
		}

		private void tree_MouseDown(object sender, MouseEventArgs e)
		{	
			// Toggle checkbox even if a nodes text-area is selected.
			if(e.Button == MouseButtons.Left)
			{
				UIElement elem = tree.UIElement.ElementFromPoint(new System.Drawing.Point( e.X, e.Y ) );
				
				if(elem != null && elem is NodeTextUIElement)
				{
					var node = (UltraTreeNode)elem.GetContext(typeof(UltraTreeNode));
					CheckState chk = node.CheckedState == CheckState.Checked ? CheckState.Unchecked : CheckState.Checked;
					node.CheckedState = chk;					
				}
			}		
		}		
	}
}
