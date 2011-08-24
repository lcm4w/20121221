using System;
using System.Collections;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using Infragistics.Win.UltraWinTree;
using TourWriter.BusinessLogic;
using TourWriter.Info;

namespace TourWriter.Forms
{
	/// <summary>
	/// Summary description for MenuHelper.
	/// </summary>
	public class MenuHelper
	{
		private readonly UltraTree _menu;		// ref back to the Menu that we are working with
        private readonly MainForm _mainForm;	// ref back to the MainForm

		#region Public Interface
		internal MenuHelper(MainForm mainForm, UltraTree menu)
		{
			_menu = menu;
			_mainForm = mainForm;
		}
		
        
		internal void HandleMenu_Load(string topNodeText)
		{
			Folder_Open(0, _menu.Nodes[0].Nodes);
            SetNodeText(_menu.Nodes[0], topNodeText); 
			_menu.Nodes[0].Enabled = true;
            RefreshSort(_menu);
		}
		
		internal void HandleMenu_LoadFolder(UltraTreeNode folderNode)
		{
			Folder_Open((folderNode.Tag as NavigationTreeItemInfo).ItemID, folderNode.Nodes);	
		}

		
		internal void HandleMenuItem_Create(NavigationTreeItemInfo.ItemTypes itemType)
		{
			// check permissions and create

			if(_menu.Equals(_mainForm.ItineraryMenu) && 
				App.ShowCheckPermission(Services.AppPermissions.Permissions.ItineraryEdit))
			{
				if(itemType == NavigationTreeItemInfo.ItemTypes.Folder)
					Folder_Create();
				else
					Itinerary_Create();
			}
			
			else if(_menu.Equals(_mainForm.SupplierMenu) && 
				App.ShowCheckPermission(Services.AppPermissions.Permissions.SupplierEdit))
			{
				if(itemType == NavigationTreeItemInfo.ItemTypes.Folder)
					Folder_Create();
				else
					Supplier_Create();
			}
			
			else if(_menu.Equals(_mainForm.ContactMenu) && 
				App.ShowCheckPermission(Services.AppPermissions.Permissions.ContactEdit))
			{
				if(itemType == NavigationTreeItemInfo.ItemTypes.Folder)
					Folder_Create();
				else
					Contact_Create();
			}
		}

        internal void HandleMenuItem_Open(SelectedNodesCollection nodes)
        {
            // Isolate the nodes for processing.
            UltraTreeNode[] array = new UltraTreeNode[nodes.Count];
            nodes.CopyTo(array, 0);

            foreach (UltraTreeNode node in array)
                HandleMenuItem_Open(node);
        }
		
		internal void HandleMenuItem_Open(UltraTreeNode node)
		{	
            NavigationTreeItemInfo info = node.Tag as NavigationTreeItemInfo;
            if (info == null)
                return;

			// check permissions and open
            if (_menu.Equals(_mainForm.ItineraryMenu) &&
                App.ShowCheckPermission(Services.AppPermissions.Permissions.ItineraryView))
            {
                if (IsNodeMenuFolder(node))
                    Folder_Open(info.ItemID, node.Nodes);
                else
                    Itinerary_Open(node);
            }
            else if (_menu.Equals(_mainForm.SupplierMenu) &&
                     App.ShowCheckPermission(Services.AppPermissions.Permissions.SupplierView))
            {
                if (IsNodeMenuFolder(node))
                    Folder_Open(info.ItemID, node.Nodes);
                else
                    Supplier_Open(node);
            }
            else if (_menu.Equals(_mainForm.ContactMenu) &&
                     App.ShowCheckPermission(Services.AppPermissions.Permissions.ContactView))
            {
                if (IsNodeMenuFolder(node))
                    Folder_Open(info.ItemID, node.Nodes);
                else
                    Contact_Open(node);
            }
		}

        internal void HandleMenuItem_Copy(UltraTreeNode targetNode, UltraTreeNode[] sourceNodes)
        {
            if (!IsNodeMenuFolder(targetNode)) targetNode = targetNode.Parent;
            if (targetNode == null) return;

            // check permissions and copy
            foreach (var sourceNode in sourceNodes)
            {
                if (_menu.Equals(_mainForm.ItineraryMenu) &&
                    App.ShowCheckPermission(Services.AppPermissions.Permissions.ItineraryEdit))
                {
                    Itinerary_Copy(targetNode, sourceNode);
                }

                else if (_menu.Equals(_mainForm.SupplierMenu) &&
                         App.ShowCheckPermission(Services.AppPermissions.Permissions.SupplierEdit))
                {
                    Supplier_Copy(targetNode, sourceNode);
                }

                else if (_menu.Equals(_mainForm.ContactMenu) &&
                         App.ShowCheckPermission(Services.AppPermissions.Permissions.ContactEdit))
                {
                    Contact_Copy(targetNode, sourceNode);
                }
            }
		}

		internal void HandleMenuItem_Rename(UltraTreeNode node, string newName)
		{
			// check permissions and rename

			if(_menu.Equals(_mainForm.ItineraryMenu) && 
				App.ShowCheckPermission(Services.AppPermissions.Permissions.ItineraryEdit))
			{
				if(IsNodeMenuFolder(node))
					Folder_Rename(node);
				else
					Itinerary_Rename(node);
			}
			
			else if(_menu.Equals(_mainForm.SupplierMenu) && 
				App.ShowCheckPermission(Services.AppPermissions.Permissions.SupplierEdit))
			{
				if(IsNodeMenuFolder(node))
					Folder_Rename(node);
				else
					Supplier_Rename(node);
			}
			
			else if(_menu.Equals(_mainForm.ContactMenu) && 
				App.ShowCheckPermission(Services.AppPermissions.Permissions.ContactEdit))
			{
				if(IsNodeMenuFolder(node))
					Folder_Rename(node);
				else
					Contact_Rename(node);
			}
		}

        internal void HandleMenuItem_Delete(UltraTreeNode[] nodes)
        {
            for (int i = nodes.Length - 1; i >= 0; i--)
                HandleMenuItem_Delete(nodes[i]);
		}

		internal void HandleMenuItem_Delete(UltraTreeNode node)
		{
			// check permissions and delete

			if(_menu.Equals(_mainForm.ItineraryMenu) && 
				App.ShowCheckPermission(Services.AppPermissions.Permissions.ItineraryDelete))
			{
				if(IsNodeMenuFolder(node))
					Folder_Delete(node);
				else
					Itinerary_Delete(node);
			}
			
			else if(_menu.Equals(_mainForm.SupplierMenu) && 
				App.ShowCheckPermission(Services.AppPermissions.Permissions.SupplierDelete))
			{
				if(IsNodeMenuFolder(node))
					Folder_Delete(node);
				else
					Supplier_Delete(node);
			}
			
			else if(_menu.Equals(_mainForm.ContactMenu) && 
				App.ShowCheckPermission(Services.AppPermissions.Permissions.ContactDelete))
			{
				if(IsNodeMenuFolder(node))
					Folder_Delete(node);
				else
					Contact_Delete(node);
			}
		}

		internal void HandleMenu_LoadChildListAndParentTree(UltraTreeNode node)
		{	
			NavigationTreeItemInfo info = node.Tag as NavigationTreeItemInfo;
            if (info == null)
                return;

			int parentFolderID = info.ParentFolderID;
			NavigationTreeItemInfo.ItemTypes itemType = info.ItemType;

			UltraTreeNode addNode;

			// start at top level
			int folderID = 0; 
			TreeNodesCollection collection = _menu.Nodes[0].Nodes; 

            NavigationTreeItemInfo navigationTree = new NavigationTreeItemInfo();
			foreach(NavigationTreeItemInfo item in navigationTree.GetChildListAndParentTree(parentFolderID, itemType))
			{
				// get collection to add to
				if(folderID != item.ParentFolderID)
				{
					// go to the next level collection
					foreach(UltraTreeNode parentNode in collection)
					{
					    NavigationTreeItemInfo parentInfo = parentNode.Tag as NavigationTreeItemInfo;
                        if (parentInfo != null && parentInfo.ItemID == item.ParentFolderID)
						{
							collection = parentNode.Nodes;
							break;
						}
					}
					folderID = item.ParentFolderID;
				}
				// add node to collection
				addNode = Menu_BuildNode(item);

                try
                {
                    if (!collection.Exists(addNode.Key))
                        collection.Add(addNode);
                }
                catch (ArgumentException ex)
                {
                    if (!ex.Message.Contains("Key already exists"))
                        throw;
                }
			}
		}

        internal void HandleMenu_RepositionNodes(UltraTreeNode dropNode, UltraTreeNode[] selectedNodes)
		{
			if(!IsNodeMenuFolder(dropNode))
				dropNode = dropNode.Parent;

		    NavigationTreeItemInfo info = dropNode.Tag as NavigationTreeItemInfo;
            if (info == null)
                return;

		    int dropNodeId = info.ItemID;

			// load the folders other nodes
			if(dropNode.Nodes.Count == 0)
				Folder_Open(dropNodeId, dropNode.Nodes);

			// move the node(s)
			UltraTreeNode moveNode = null;
			NavigationTreeItemInfo  moveInfo;
			for (int i = 0; i<= (selectedNodes.Length - 1); i++)
			{
				moveNode = selectedNodes[i];
				moveInfo = moveNode.Tag as NavigationTreeItemInfo;
                if (moveInfo == null)
                    continue;
				 
				if(moveInfo.ParentFolderID == dropNodeId)
					continue; // no change to position
	
				if ((dropNodeId != moveInfo.ItemID && dropNode.FullPath == moveNode.FullPath) ||  
					dropNode.FullPath.IndexOf(moveNode.FullPath) == -1) // not descendant of itself
				{
                    moveNode.Remove();
                    dropNode.Nodes.Add(moveNode);
				    moveNode.Enabled = true;
                    moveNode.Selected = true;
				    _menu.ActiveNode = moveNode;
                    moveInfo.ParentFolderID = dropNodeId;

                    NavigationTreeItemInfo navigationTree = new NavigationTreeItemInfo();
					navigationTree.SetParentFolderID(moveInfo.ItemID, dropNodeId, moveInfo.ItemType);					
				}
			}
		}
		
	
		/// <summary>
		/// Builds the topmost level node which is the container of all other items in a menu.
		/// </summary>
		/// <param name="itemInfo"></param>
		/// <returns></returns>
				
		internal static UltraTreeNode Menu_BuildTopLevelNode(NavigationTreeItemInfo itemInfo)
		{	
			UltraTreeNode node = Menu_BuildNode(itemInfo);
			
			// set additional appearances
			node.Expanded = true;
			node.Override.LabelEdit = Infragistics.Win.DefaultableBoolean.False;	
			node.Override.NodeDoubleClickAction = NodeDoubleClickAction.None;
			node.Override.ShowExpansionIndicator = ShowExpansionIndicator.Never;
						
			return node;
		}
							
		internal static UltraTreeNode Menu_BuildNode(NavigationTreeItemInfo itemInfo)
		{	
			UltraTreeNode node = new UltraTreeNode();
		
			String key = null;
			Image image = null;
			switch(itemInfo.ItemType)
			{
				case NavigationTreeItemInfo.ItemTypes.Folder :
					key = String.Format("f~{0}", itemInfo.ItemID);
                    image = TourWriter.Properties.Resources.Folder;
					node.Override.ShowExpansionIndicator = ShowExpansionIndicator.CheckOnExpand;
					break;
				case NavigationTreeItemInfo.ItemTypes.Itinerary :
					key = String.Format("i~{0}", itemInfo.ItemID);
                    image = TourWriter.Properties.Resources.Image;
					break;
				case NavigationTreeItemInfo.ItemTypes.Supplier :
					key = String.Format("s~{0}", itemInfo.ItemID);
                    image = TourWriter.Properties.Resources.House;
					break;
				case NavigationTreeItemInfo.ItemTypes.Contact :
					key = String.Format("c~{0}", itemInfo.ItemID);
                    image = TourWriter.Properties.Resources.User;
					break;
                case NavigationTreeItemInfo.ItemTypes.PurchaseLine:
                    key = String.Format("i~{0}", itemInfo.ItemID);
                    image = TourWriter.Properties.Resources.Image;
                    break;
			}
	
		
			node.Text = itemInfo.ItemName;
			node.Key = key;
			node.Tag  = itemInfo;
			node.LeftImages.Add(image);

			// strickout text
			if(!itemInfo.IsActive)
				node.Override.NodeAppearance.FontData.Strikeout = Infragistics.Win.DefaultableBoolean.True;
			
			return node;
		}


        internal static UltraTreeNode Menu_FindNode(string key)
        {
            if (key.StartsWith("i~")) return App.MainForm.ItineraryMenu.GetNodeByKey(key);
            if (key.StartsWith("s~")) return App.MainForm.SupplierMenu.GetNodeByKey(key);
            if (key.StartsWith("c~")) return App.MainForm.ContactMenu.GetNodeByKey(key);
            return null;
        }

        delegate void SetRefreshSortDelegate(UltraTree menu);
        internal static void RefreshSort(UltraTree menu)
        {
            if (!menu.InvokeRequired)
            {
                // Infragistics bug means have to re-apply sort comparer when doing refresh
                menu.Override.Sort = SortType.Ascending;
                menu.Override.SortComparer = new MenuSortComparer();
                menu.RefreshSort();      
            }
            else
            {
                menu.Invoke(new SetRefreshSortDelegate(RefreshSort), new object[] { menu });
            }
        }

	    internal static bool IsNodeMenuFolder(UltraTreeNode node)
		{
			return (node.Tag as NavigationTreeItemInfo).ItemType == NavigationTreeItemInfo.ItemTypes.Folder;
		}

		internal static NavigationTreeItemInfo.ItemTypes GetMenuItem_TypeName(UltraTree menu)
		{
			if(menu.Equals(App.MainForm.ItineraryMenu))
				return NavigationTreeItemInfo.ItemTypes.Itinerary;
			if(menu.Equals(App.MainForm.SupplierMenu))
				return NavigationTreeItemInfo.ItemTypes.Supplier;
			if(menu.Equals(App.MainForm.ContactMenu))
				return NavigationTreeItemInfo.ItemTypes.Contact;
			return 
				NavigationTreeItemInfo.ItemTypes.Folder;
		}

		#endregion

		#region Itinerary Items
		private void Itinerary_Create()
		{
			UltraTreeNode positionNode = _menu.ActiveNode;

			string newName = "New Itinerary";
			DateTime arriveDate = DateTime.Parse(DateTime.Now.ToShortDateString());
			int parentFolderID = GetParentFolderID_ByPosition(positionNode);

			// create new itinerary
			Itinerary i = new Itinerary();
			int itineraryID = i.Create(newName, arriveDate, parentFolderID, TourWriter.Global.Cache.User.UserID);

			// create menu info				
			NavigationTreeItemInfo info = new NavigationTreeItemInfo(
				itineraryID, newName, NavigationTreeItemInfo.ItemTypes.Itinerary, parentFolderID, true);

			// create menu node
			UltraTreeNode node = Menu_BuildNode(info);
            node.Visible = false; // hide until itin opens, to save double sorting if name changes (db can set name when created)
			PlaceNewNode_ByPosition_AndSelect(positionNode, node);
		}

		private void Itinerary_Open(UltraTreeNode node)
		{
			_mainForm.Load_ItineraryForm(node);			
		}

		private void Itinerary_Delete(UltraTreeNode node)
		{			
			Itinerary i = new Itinerary();
			if(i.Delete((node.Tag as NavigationTreeItemInfo).ItemID))
				node.Remove();				
		}

        private void Itinerary_Copy(UltraTreeNode targetNode, UltraTreeNode sourceNode)
		{
            var info = (NavigationTreeItemInfo)sourceNode.Tag;
            var newItineraryName = sourceNode.Text + " - Copy";
			var copyItineraryId = info.ItemID;

			// create itinerary copy
            var i = new Itinerary();
            var newItinerary = UserControls.CopyHelper.CopyAndSaveItinerary(copyItineraryId, newItineraryName);
            var newItineraryId = newItinerary.Itinerary[0].ItineraryID;
            info.SetParentFolderID(newItineraryId, ((NavigationTreeItemInfo)targetNode.Tag).ItemID, NavigationTreeItemInfo.ItemTypes.Itinerary);

			// update info with new args
			info = info.Copy();
            info.ItemID = newItineraryId;
			info.ItemName = newItineraryName;

            // create node
            sourceNode.Selected = false;
            var newNode = Menu_BuildNode(info);
            AddNodeAndSelect(targetNode.Nodes, newNode);
		}

	    private void Itinerary_Rename(UltraTreeNode node)
		{				
			int id = (node.Tag as NavigationTreeItemInfo).ItemID;
			Itinerary i = new Itinerary();
			i.Rename(id, node.Text);
			node.BringIntoView();
		
			// update tag info
			(node.Tag as NavigationTreeItemInfo).ItemName = node.Text;
		}
		
		#endregion

		#region Supplier Items
		private void Supplier_Create()
		{
			UltraTreeNode positionNode = _menu.ActiveNode;

			string newName = "New Supplier";
			int parentFolderID = GetParentFolderID_ByPosition(positionNode);

			// create new supplier
			Supplier s = new Supplier();
			int supplierID = s.Create(newName, parentFolderID, TourWriter.Global.Cache.User.UserID);

			// create menu info				
			NavigationTreeItemInfo info = new NavigationTreeItemInfo(
				supplierID, newName, NavigationTreeItemInfo.ItemTypes.Supplier, parentFolderID, true);

			// create menu node
			UltraTreeNode node = Menu_BuildNode(info);
			PlaceNewNode_ByPosition_AndSelect(positionNode, node);
		}

		private void Supplier_Open(UltraTreeNode node)
		{
			_mainForm.Load_SupplierForm(node);			
		}

		private void Supplier_Delete(UltraTreeNode node)
		{
            var s = new Supplier();
		    var id = (node.Tag as NavigationTreeItemInfo).ItemID;
            try
            {
                if (s.Delete(id)) node.Remove();
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                if (ex.Message.Contains(App.DataErrorPkDeleteConflictText))
                {
                    var ds = s.GetSupplierSet(id);
                    ds.Supplier[0].IsDeleted = true;
                    if (App.DataSet_AskSaveDeleteConstraints(ds))
                    {
                        s.SaveSupplierSet(ds);
                        node.Remove();
                    }
                }
            }
		}

        private void Supplier_Copy(UltraTreeNode targetNode, UltraTreeNode sourceNode)
		{		
            var info = (NavigationTreeItemInfo)sourceNode.Tag;
            var newSupplierName = sourceNode.Text + " - Copy";
            var copySupplierId = info.ItemID;

            // create itinerary copy
            var i = new Supplier();
            var newSupplier = UserControls.CopyHelper.CopyAndSaveSupplier(copySupplierId, newSupplierName);
            var newSupplierId = newSupplier.Supplier[0].SupplierID;
            info.SetParentFolderID(newSupplierId, ((NavigationTreeItemInfo)targetNode.Tag).ItemID, NavigationTreeItemInfo.ItemTypes.Supplier);

            // update info with new args
            info = info.Copy();
            info.ItemID = newSupplierId;
            info.ItemName = newSupplierName;

            // create node
            var newNode = Menu_BuildNode(info);
            AddNodeAndSelect(targetNode.Nodes, newNode);
            sourceNode.Selected = false;
		}

	    private void Supplier_Rename(UltraTreeNode node)
		{				
			int id = (node.Tag as NavigationTreeItemInfo).ItemID;
			Supplier s = new Supplier();
			s.Rename(id, node.Text);
			node.BringIntoView();
		
			// update tag info
			(node.Tag as NavigationTreeItemInfo).ItemName = node.Text;
		}
		
		#endregion

		#region Contact Items
		private void Contact_Create()
		{
			UltraTreeNode positionNode = _menu.ActiveNode;

			string newName = "New Contact";
			int parentFolderID = GetParentFolderID_ByPosition(positionNode);

			// create new itinerary
			Contact c = new Contact();
			int contactID = c.Create(newName, parentFolderID, TourWriter.Global.Cache.User.UserID);

			// create menu info				
			NavigationTreeItemInfo info = new NavigationTreeItemInfo(
				contactID, newName, NavigationTreeItemInfo.ItemTypes.Contact, parentFolderID, true);

			// create menu node
			UltraTreeNode node = Menu_BuildNode(info);
			PlaceNewNode_ByPosition_AndSelect(positionNode, node);
		}

		private void Contact_Open(UltraTreeNode node)
		{
			_mainForm.Load_ContactForm(node);			
		}

		private void Contact_Delete(UltraTreeNode node)
        {
            var c = new Contact();
            var id = (node.Tag as NavigationTreeItemInfo).ItemID;
            try
            {
    			if (c.Delete(id)) node.Remove();
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                if (ex.Message.Contains(App.DataErrorPkDeleteConflictText))
                {
                    var ds = c.GetContactSet(id);
                    ds.Contact[0].IsDeleted = true;
                    if (App.DataSet_AskSaveDeleteConstraints(ds))
                    {
                        c.SaveContactSet(ds);
                        node.Remove();
                    }
                }
            }
		}

        private void Contact_Copy(UltraTreeNode targetNode, UltraTreeNode sourceNode)
		{	
            var info = (NavigationTreeItemInfo)sourceNode.Tag;
            var newContactName = sourceNode.Text + " - Copy";
            var copyContactId = info.ItemID;

            // create itinerary copy
            var i = new Contact();
            var newContact = UserControls.CopyHelper.CopyAndSaveContact(copyContactId, newContactName);
            var newContactId = newContact.Contact[0].ContactID;
            info.SetParentFolderID(newContactId, ((NavigationTreeItemInfo)targetNode.Tag).ItemID, NavigationTreeItemInfo.ItemTypes.Contact);

            // update info with new args
            info = info.Copy();
            info.ItemID = newContactId;
            info.ItemName = newContactName;

            // create node
            var newNode = Menu_BuildNode(info);
            AddNodeAndSelect(targetNode.Nodes, newNode);
            sourceNode.Selected = false;
		}

	    private void Contact_Rename(UltraTreeNode node)
		{				
			int id = (node.Tag as NavigationTreeItemInfo).ItemID;
			Contact c = new Contact();
			c.Rename(id, node.Text);
			node.BringIntoView();
		
			// update tag info
			(node.Tag as NavigationTreeItemInfo).ItemName = node.Text;
		}
		
		#endregion

		#region Folder Items
		private void Folder_Create()
		{
			UltraTreeNode positionNode = _menu.ActiveNode;

			string newName     = "New Folder";
			int parentFolderID = GetParentFolderID_ByPosition(positionNode);
			int menuTypeID     = GetFolderMenuType();

			// create new folder
			Folder f = new Folder();
			int folderID = f.NewFolder(newName, parentFolderID, menuTypeID, TourWriter.Global.Cache.User.UserID);

			// create item info
			NavigationTreeItemInfo info = new NavigationTreeItemInfo(
				folderID, newName, NavigationTreeItemInfo.ItemTypes.Folder, parentFolderID, true);

			// create item node
			UltraTreeNode node = Menu_BuildNode(info);
			PlaceNewNode_ByPosition_AndSelect(positionNode, node);
		}

		private void Folder_Open(int parentFolderID, TreeNodesCollection siblingCollection)
		{
            NavigationTreeItemInfo[] itemList = new NavigationTreeItemInfo().GetChildList(parentFolderID, GetMenuItem_TypeName(_menu));

			UltraTreeNode[] nodeList = new UltraTreeNode[itemList.Length];
			for(int i=0; i<itemList.Length; i++)
				nodeList[i] = Menu_BuildNode(itemList[i]);
            
            Folder_Populate(_menu, siblingCollection, nodeList);
		}

        private void Folder_Populate(UltraTree tree, TreeNodesCollection collection, UltraTreeNode[] nodesToAdd)
        {
            tree.SuspendLayout();
			tree.BeginUpdate();
			collection.Clear();

            try
            {
                collection.AddRange(nodesToAdd);
            }
            catch(ArgumentException ex)
            {
                if(ex.Message.Contains("Key already exists"))
                {
                    // Node already exists in another folder of this tree. 
                    // Caused by another user moving the node since this tree was last refreshed.

                    // Add nodes one-by-one and reposition existing node(s).
                    foreach(UltraTreeNode node in nodesToAdd)
                    {
                        UltraTreeNode n = tree.GetNodeByKey(node.Key);
                        if(n == null)
                            collection.Add(node);
                        else
                            n.Reposition(collection);
                    }
                }
                else throw;
            }
            tree.Invoke((MethodInvoker) (() =>
                                             {
                                                 tree.EndUpdate();
                                                 tree.ResumeLayout(false);
                                             }));
        }

		private void Folder_Delete(UltraTreeNode node)
        {
            // Ensure child nodes are loaded...
            if(node.Nodes.Count == 0) 
                HandleMenu_LoadFolder(node); 

            // ... and don't delete if contains child nodes
            if (node.Nodes.Count > 0)
                throw new InvalidOperationException("Cannot delete folder as it is not empty, delete the folder contents first.");

		    Folder f = new Folder();
			if(f.DeleteFolder((node.Tag as NavigationTreeItemInfo).ItemID))
				node.Remove();
		}

		private void Folder_Rename(UltraTreeNode node)
		{					
			int id = (node.Tag as NavigationTreeItemInfo).ItemID;
			Folder f = new Folder();
			f.SetFolderName(id, node.Text);
			node.BringIntoView();
		
			// update tag info
			(node.Tag as NavigationTreeItemInfo).ItemName = node.Text;
		}
				
		#endregion

		#region Helper methods

        delegate void SetNodeTextDelegate(UltraTreeNode node, string text);
		private void SetNodeText(UltraTreeNode node, string text)
		{
			if (!_menu.InvokeRequired)
			{
				node.Text = text;
			}
			else
			{
                _menu.Invoke(new SetNodeTextDelegate(SetNodeText), new object[] { node, text });
			}
		}

		private int GetFolderMenuType()
		{
			if(_menu.Equals(_mainForm.ContactMenu))
				return 1;
			if(_menu.Equals(_mainForm.SupplierMenu))
				return 2;
			if(_menu.Equals(_mainForm.ItineraryMenu))
				return 3;

			throw new ArgumentException("Menu tree named " + _menu.Name + " not found", "tree");
		}

		private int GetParentFolderID_ByPosition(UltraTreeNode positionNode)
		{
			if(positionNode != null)
			{
				NavigationTreeItemInfo info = positionNode.Tag as NavigationTreeItemInfo;

				if(info.ItemType == NavigationTreeItemInfo.ItemTypes.Folder)
					return info.ItemID;
				else
					return info.ParentFolderID;
			}
			return 0;
		}

		private void PlaceNewNode_ByPosition_AndSelect(UltraTreeNode positionNode, UltraTreeNode newNode)
		{
			UltraTreeNode folder;

			// find the collection to add new node to
			if(IsNodeMenuFolder(positionNode))
				folder = positionNode;
			else if(positionNode.Parent != null)
				folder = positionNode.Parent;
			else
				folder = _menu.Nodes[0];
			
			TreeNodesCollection collection = folder.Nodes;

			// expand collection to show nodes
			if(collection.Count == 0)
				Folder_Open(((NavigationTreeItemInfo)folder.Tag).ItemID, collection);
			
			// BUG having problems with node in edit mode showing in wrong place.
			//folder.Expanded = false;

			// ensure node not already existing
			int i = collection.IndexOf(newNode.Key);
			if(i >= 0)
				collection.RemoveAt(i);
			
			// add node at the end of the collection and select it
            AddNodeAndSelect(collection, newNode);
		}

        private void AddNodeAndSelect(TreeNodesCollection collection, UltraTreeNode node)
        {
            // add node at the end of the collection and select it
            try
            {
                if (!collection.Exists(node.Key))
                    collection.Add(node);
                node.Selected = true;
                node.BringIntoView();
                _menu.ActiveNode = node;
            }
            catch (ArgumentException ex)
            {
                if (!ex.Message.Contains("Key already exists"))
                    throw;
            }
        }
		#endregion
	}
	class MenuSortComparer : IComparer
	{
		int IComparer.Compare(object x, object y)
		{
			UltraTreeNode n1 = null;
			UltraTreeNode n2 = null;
			NavigationTreeItemInfo i1 = null;
			NavigationTreeItemInfo i2 = null;

			try
			{
				n1 = x as UltraTreeNode;
				n2 = y as UltraTreeNode;
				i1 = n1.Tag as NavigationTreeItemInfo;
				i2 = n2.Tag as NavigationTreeItemInfo;

				if (i1.ItemType == i2.ItemType)
					return String.Compare(n1.Text, n2.Text);
				else if (i1.ItemType == NavigationTreeItemInfo.ItemTypes.Folder)
					return -1;
				else if (i2.ItemType == NavigationTreeItemInfo.ItemTypes.Folder)
					return 1;
				else
					return 0;
			}
			finally
			{
				n1 = null;
				n2 = null;
				i1 = null;
				i2 = null;
			}
		}
	}
}
