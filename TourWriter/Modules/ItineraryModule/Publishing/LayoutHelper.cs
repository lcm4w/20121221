using System;
using System.Collections;
using System.Data;
using System.Drawing;
using Infragistics.Win;
using Infragistics.Win.UltraWinTree;
using TourWriter.Info;
using TourWriter.Services.IconHelper;

namespace TourWriter.Modules.ItineraryModule.Publishing
{
	public class LayoutHelper
	{
		#region ** FORMATTING NOTES FOR LAYOUT TREE **

		/* 
		 * Example layout...
		 * 
		 * NOTE: Node.Tag used for file name instead of Key which must be unique.
		 * 
		 * -------------------------------------------------------------------
		 * Front section
		 *		File~[filePath]					TAG used for file name and path
		 *	
		 *	Itinerary section : Location		TEXT used for location
		 *	Day~[date]							KEY used for info
		 *		Item~[purchaseItemID]~[date]	KEY used for info
		 *		File~[filePath]					TAG used for file name and path	
		 *				
		 * Back section
		 *		File~[filePath]					TAG used for file name and path
		 * -------------------------------------------------------------------
		 *		
		 * Each File node that is checked is used to create the document.
		 */

		#endregion		

		private readonly UltraTree tree;
		private readonly ItinerarySet itinerarySet;
		private const string BAD_DATE_WARNING = "\r\nDate outside Itinerary start-end dates.";

		public LayoutHelper(ItinerarySet itinerarySet, UltraTree tree)
		{
			this.tree = tree;
			this.itinerarySet = itinerarySet;
		}
		

		/// <summary>
		/// Rebuild the layout tree based on the base itinerary data.
		/// </summary>
		/// <returns></returns>
		public void RebuildLayoutTree()
		{
			AddTopLevelNode();
			AddFrontSectionNode();
			AddDaySections();
			AddBackSectionNode();
		}
		
		private void AddDaySections()
		{
            if (itinerarySet == null)
                return;

			int countDays;
			DateTime date;
			UltraTreeNode node;
			ArrayList newDayNodesList = new ArrayList();

			ClearLocationTexts();

			// Add actual PurchaseItem days: days based on purchase items used
			itinerarySet.PurchaseItem.DefaultView.Sort = "StartDate";
			foreach (DataRowView view in itinerarySet.PurchaseItem.DefaultView)
			{
				ItinerarySet.PurchaseItemRow item = view.Row as ItinerarySet.PurchaseItemRow;
				countDays = !item.IsNumberOfDaysNull() ? Convert.ToInt32(item.NumberOfDays + 0.4) : 1;

				// add each day that item is used for
				for (int i = 0; i < countDays; i++)
				{
					if (item.IsStartDateNull())
						continue;

					date = item.StartDate.AddDays(i).Date;

					node = AddDaySectionNode(date);
					UpdateLocationText(node, item);
					newDayNodesList.Add(node);

					// show as a warning node for now
                    // strip the warning text if it's already there
                    node.Text = node.Text.Replace(BAD_DATE_WARNING, "");
					node.Text += BAD_DATE_WARNING;
					node.Override.NodeAppearance.ForeColor = Color.Red;
				}
			}

			// Add Itinerary days based on itinerary start to end dates
			if (itinerarySet.Itinerary[0].IsArriveDateNull() || itinerarySet.Itinerary[0].IsDepartDateNull())
				return;
			countDays = (itinerarySet.Itinerary[0].DepartDate.Date - itinerarySet.Itinerary[0].ArriveDate.Date).Days;
			for (int i = 0; i <= countDays; i++)
			{
				date = itinerarySet.Itinerary[0].ArriveDate.AddDays(i).Date;

				node = AddDaySectionNode(date);
				newDayNodesList.Add(node);

				// override node font to show as valid itinerary day
				node.Override.NodeAppearance.ForeColor = Color.Black;
				node.Override.NodeAppearance.FontData.Strikeout = DefaultableBoolean.False;
				node.Text = node.Text.Replace(BAD_DATE_WARNING, "");
			}

			RemoveUnusedDayNodes(newDayNodesList, tree.Nodes[0].Nodes);
		}


		private UltraTreeNode AddTopLevelNode()
		{
			if (!tree.Nodes.Exists("Itinerary"))
				tree.Nodes.Add("Itinerary", "");

			UltraTreeNode node;
			if(tree.Nodes.Exists("Itinerary"))
			{
				node = tree.Nodes["Itinerary"];
			}
			else
			{
				node = tree.Nodes.Add("Itinerary", "");
			}
			node.Override.ShowExpansionIndicator = ShowExpansionIndicator.Never;
			return node;
		}

		private UltraTreeNode AddFrontSectionNode()
		{
			UltraTreeNode node;
			if(tree.Nodes[0].Nodes.Exists(FileBuilderAdvanced.FrontSectionNodeKey))
			{
				node = tree.Nodes[0].Nodes[FileBuilderAdvanced.FrontSectionNodeKey];
			}
			else
			{
				node = tree.Nodes[0].Nodes.Insert(
					0, FileBuilderAdvanced.FrontSectionNodeKey, "Front section");
			}
            RefreshFileIconsForSectionNode(node);
            //node.Override.ShowExpansionIndicator = ShowExpansionIndicator.Always;
			return node;
		}
		
		private UltraTreeNode AddDaySectionNode(DateTime date)
		{
			UltraTreeNode node = null;

		    string key = String.Format("{0}~{1}",
		                               FileBuilderAdvanced.DaysSectionNodeKey, date.ToShortDateString());
			
			// check for existing node
            if (tree.Nodes[0].Nodes.Exists(key))
            {
                node = tree.Nodes[0].Nodes[key];
            }
            else
            {
                foreach (var n in tree.Nodes[0].Nodes)
                    if (IsDaySectionNode(n) && GetDaySectionDate(n) == date)
                    {
                        node = n;
                        break;
                    }
            }

		    if (node == null) // add new node, sorting already taken care of
			{
			    string text = String.Format("{0} :", date.ToShortDateString());
				node = tree.Nodes[0].Nodes.Add(key, text);
			}
			// set appearance
			RefreshFileIconsForSectionNode(node);
            node.Override.TipStyleNode = TipStyleNode.Show;
            //node.Override.ShowExpansionIndicator = ShowExpansionIndicator.Always;
			return node;
		}

		private UltraTreeNode AddBackSectionNode()
		{
			UltraTreeNode node;
			if(tree.Nodes[0].Nodes.Exists(FileBuilderAdvanced.BackSectionNodeKey))
			{
				node = tree.Nodes[0].Nodes[FileBuilderAdvanced.BackSectionNodeKey];
			}
			else 
			{
				node = tree.Nodes[0].Nodes.Insert(
					tree.Nodes[0].Nodes.Count, FileBuilderAdvanced.BackSectionNodeKey, "Back section");	
			}
            RefreshFileIconsForSectionNode(node);
            //node.Override.ShowExpansionIndicator = ShowExpansionIndicator.Always;
			return node;
		}
		
		internal static UltraTreeNode AddFileNode(
			UltraTree tree, int nodeInsertPosition, string filename, string nodetext)
		{
			UltraTreeNode node;
			
			// Add the node with tag because key could throw 'key exists' error when adding 
			// file to the layout grid when the file is already used in the layout.
			// Node tag = 'File~[FileName]'
			node = tree.Nodes.Insert(nodeInsertPosition, null, nodetext);
			node.Tag = FileBuilderAdvanced.FileNodeTag + "~" + filename;
			node.Override.NodeAppearance.Image = Bitmap.FromHicon(
				IconReader.GetFileIcon(filename, IconReader.IconSize.Small, false).Handle);

			return node;
		}
		
		
		internal static bool IsAnySectionNode(UltraTreeNode node)
		{
			return
				node.Key.StartsWith(FileBuilderAdvanced.FrontSectionNodeKey) ||
				node.Key.StartsWith(FileBuilderAdvanced.DaysSectionNodeKey) ||
				node.Key.StartsWith(FileBuilderAdvanced.BackSectionNodeKey);
		}

		internal static bool IsFileNode(UltraTreeNode node)
		{
			return 
				node.Tag != null && 
				node.Tag.ToString().Split('~')[0] == FileBuilderAdvanced.FileNodeTag;
		}

		internal static string GetFileNodeFileName(UltraTreeNode node)
		{
			return Services.ExternalFilesHelper.ConvertToAbsolutePath(node.Tag.ToString().Split('~')[1].Trim());
		}

		internal static DateTime GetDaySectionDate(UltraTreeNode node)
		{
			return GetDaySectionDate(node.Key);
		}

		internal static DateTime GetDaySectionDate(string key)
		{
			return DateTime.Parse(key.Split('~')[1].Trim());
		}

		internal static string GetDaySectionLocation(UltraTreeNode node)
		{
			return node.Text.Split(':')[1].Trim();
		}

		private void SetDaySectionLocation(UltraTreeNode node, string location)
		{
            UltraTreeNode previousDayNode = node.GetSibling(NodePosition.Previous);

			// insert at start if it is the same location as previous day
			if (previousDayNode != null && previousDayNode.Text.IndexOf(location) > -1)
			{
				int i = node.Text.IndexOf(':') + 1;
				location = " " + location + (node.Text.EndsWith(":") ? "" : ", ");
				node.Text = node.Text.Insert(i, location);
			}
			// add to end of locatons
			else
				node.Text += (node.Text.Replace(BAD_DATE_WARNING, "").EndsWith(":") ? " " : ", ") + location;
		}

		private void ClearDaySectionLocation(UltraTreeNode node)
		{
			if (node.Text.IndexOf(':') != -1)
				node.Text = node.Text.Substring(0, node.Text.IndexOf(':') + 1);
		}

		internal static void RefreshSort(UltraTree publisherLayoutTree)
		{
			// add custom sort class
			// BUG: Infragistics bug means have to re-apply sort comparer when doing refresh
			publisherLayoutTree.Nodes[0].Nodes.Override.Sort = SortType.Ascending;
			publisherLayoutTree.Nodes[0].Nodes.Override.SortComparer = new LayoutSortComparer();
			publisherLayoutTree.RefreshSort();
		}


		private void ClearLocationTexts()
		{
			foreach (UltraTreeNode node in tree.Nodes[0].Nodes)
				if (node.Key.StartsWith(FileBuilderAdvanced.DaysSectionNodeKey))
					ClearDaySectionLocation(node);
		}

		private void RefreshFileIconsForSectionNode(UltraTreeNode layoutSectionNode)
		{
			foreach(UltraTreeNode fileNode in layoutSectionNode.Nodes)
			{
				if (IsFileNode(fileNode))
				{
					try
					{
						fileNode.Override.NodeAppearance.Image = Bitmap.FromHicon(
							IconReader.GetFileIcon(
							GetFileNodeFileName(fileNode), IconReader.IconSize.Small, false).Handle);
					}
					catch
					{}
				}
			}
		}

		private void UpdateLocationText(UltraTreeNode node, ItinerarySet.PurchaseItemRow item)
		{
			ItinerarySet.SupplierLookupRow supplier = itinerarySet.SupplierLookup.FindBySupplierID(
				(item.GetParentRow("PurchaseLinePurchaseItem") as ItinerarySet.PurchaseLineRow).SupplierID);

            // get city name
		    string location = "";
            if(supplier != null && !supplier.IsCityIDNull())
            {
                ToolSet.CityRow city = TourWriter.Global.Cache.ToolSet.City.FindByCityID(supplier.CityID);
                if (city != null)
                    location = city.CityName;
            }

			// does this city already exist
			if (node.Text.IndexOf(location) > -1)
				return;

			SetDaySectionLocation(node, location);
		}

		private void RemoveUnusedDayNodes(ArrayList newDayNodesList, TreeNodesCollection collection)
		{
			for (int i = collection.Count - 1; i >= 0; i--)
			{
				UltraTreeNode node = tree.Nodes[0].Nodes[i];
				if (node.Key.StartsWith(FileBuilderAdvanced.DaysSectionNodeKey))
					if (!newDayNodesList.Contains(node))
						collection.Remove(node);
			}
		}
	}
}
