using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Collections.Generic;
using Infragistics.Shared;
using Infragistics.Win;
using Infragistics.Win.UltraWinScrollBar;
using Infragistics.Win.UltraWinStatusBar;
using Infragistics.Win.UltraWinTree;
using TourWriter.Info;
using TourWriter.Info.Services;
using TourWriter.Modules.AdminModule;
using TourWriter.Modules.ContactModule;
using TourWriter.Modules.DataExtract;
using TourWriter.Modules.GeneralReportsModule;
using TourWriter.Modules.ItineraryModule;
using TourWriter.Modules.Search;
using TourWriter.Properties;
using TourWriter.Services.Update;
using TourWriter.Utilities.KeyHook;
using TourWriter.Modules.ContactModule.Email;

namespace TourWriter.Forms
{
    /// <summary>
    /// TourWriter application main user interface form.
    /// </summary>
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            Icon = Properties.Resources.TourWriter16;

            // for keyboard hook
            Disposed += Form_Deactivate;

            LoadLayout();

            // keyboard hook			
            hook = new LocalKeyboardHook();
            hook.KeyboardEvent += Keyboard_Event;
            hook.Attach();

            // initial settings
            App.MainForm = this;
            App.IsDebugMode = false;

            // set focus
            ItineraryMenu.Select();
            if (ItineraryMenu.Nodes.Count > 0)
                ItineraryMenu.Nodes[0].Selected = true;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // set version display text
            var s = AssemblyInfo.FileVersion; 
            StatusBar_VersionText = "Version " + s.Substring(0, s.LastIndexOf('.'));
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            Load_StartPage();
            Application.DoEvents();
            Login();
        }

        private void Login()
        {
            if (Global.Cache.UserSet != null)
                Logout();

            Login login = new Login();
            login.ShowInTaskbar = false; // so can't seperate from parent
            DialogResult loginResult = login.ShowDialog(this);

            switch (loginResult)
            {
                case DialogResult.OK:
                    {
                        ApplicationUpdateService.StartUpdateMonitor();
                        InitialiseMenu(ItineraryMenu);
                        InitialiseMenu(SupplierMenu);
                        InitialiseMenu(ContactMenu);

                        // set connection display text
                        StatusBar_ConnectionText = String.Format(
                            "Connection: {0}\\{1}", App.Servername, Global.Cache.User.UserName);
                        break;
                    }
                case DialogResult.Cancel:
                    {
                        ShowAppClosingPrompt = false;
                        Close();
                        break;
                    }
            }
        }

        #region Menu handling

        internal void InitialiseMenu(UltraTree menu)
        {
            // create the top level node
            NavigationTreeItemInfo info = new NavigationTreeItemInfo(0, menu.Nodes[0].Text, NavigationTreeItemInfo.ItemTypes.Folder, 0, true);
            UltraTreeNode node = MenuHelper.Menu_BuildTopLevelNode(info);

            // use custom sort
            menu.Override.Sort = SortType.Descending; // required to refresh sort, due to Infragistics problem
            menu.Override.SortComparer = new MenuSortComparer();

            // add top level node
            menu.Nodes.Clear();
            menu.Nodes.Add(node);

            // load child nodes from database
            HandleMenu_LoadOnThread(menu);
        }

        private void HandleMenu_LoadOnThread(UltraTree menu)
        {
            MenuHelper m = new MenuHelper(this, menu);
            m.HandleMenu_Load_OnThread();
        }

        private void HandleMenu_LoadChildListAndParentTree(UltraTree menu, UltraTreeNode node)
        {
            MenuHelper m = new MenuHelper(this, menu);
            m.HandleMenu_LoadChildListAndParentTree(node);
        }

        private void HandleMenu_RepositionNodes(UltraTree menu, UltraTreeNode dropNode, SelectedNodesCollection selectedNodes)
        {
            var m = new MenuHelper(this, menu);
            var moveNodes = new UltraTreeNode[selectedNodes.Count];
            selectedNodes.CopyTo(moveNodes, 0);

            if (!MenuHelper.IsNodeMenuFolder(dropNode))
                dropNode = dropNode.Parent;
            HandleMenuItem_Open(menu, dropNode);
            dropNode.Expanded = true;
            menu.ActiveNode = null;
            menu.SelectedNodes.Clear();

            m.HandleMenu_RepositionNodes(dropNode, moveNodes);

            MenuHelper.RefreshSort(menu);
        }

        private void HandleMenuItem_Create(UltraTree menu, NavigationTreeItemInfo.ItemTypes itemType)
        {
            menu.Select();
            menu.SelectedNodes.Clear();

            MenuHelper m = new MenuHelper(this, menu);
            m.HandleMenuItem_Create(itemType);
            m.HandleMenuItem_Open(menu.ActiveNode);
        }

        private void HandleMenuItem_Open(UltraTree menu, UltraTreeNode node)
        {
            MenuHelper m = new MenuHelper(this, menu);
            m.HandleMenuItem_Open(node);
        }

        private void HandleMenuItem_Open(UltraTree menu, SelectedNodesCollection nodes)
        {
            MenuHelper m = new MenuHelper(this, menu);
            m.HandleMenuItem_Open(nodes);

            MenuHelper.RefreshSort(menu);
        }

        private UltraTreeNode[] _menuActionCutNodes;
        private void HandleMenuItem_StartCopyOrCut(SelectedNodesCollection nodes, bool disableSelectedNodes)
        {
            HandleMenuItem_EndCopyOrCut(); // cancel pending

            _menuActionCutNodes = new UltraTreeNode[nodes.Count];
            nodes.CopyTo(_menuActionCutNodes, 0);
            if (disableSelectedNodes)
                foreach (var node in _menuActionCutNodes) node.Enabled = false;
            contextMenuPaste.Text = disableSelectedNodes ? "Paste (cut)" : "Paste (copy)";
            contextMenuPaste.Enabled = true;
            contextMenuCancelPaste.Visible = true;
        }

        private void HandleMenuItem_EndCopyOrCut()
        {
            if (_menuActionCutNodes == null) return;
            foreach (var node in _menuActionCutNodes) node.Enabled = true;
            _menuActionCutNodes = null;
            contextMenuPaste.Text = "Paste";
            contextMenuPaste.Enabled = false;
            contextMenuCancelPaste.Visible = false;
        }

        private void HandleMenuItem_Paste(UltraTree menu, SelectedNodesCollection nodes)
        {
            if (_menuActionCutNodes != null && _menuActionCutNodes.Length > 0)
            {
                var m = new MenuHelper(this, menu);
                var parentNode = nodes[0]; // restrict copy/cut to single target node
                if (!MenuHelper.IsNodeMenuFolder(parentNode))
                    parentNode = parentNode.Parent;
                HandleMenuItem_Open(menu, parentNode);
                parentNode.Expanded = true;
                menu.ActiveNode = null;
                menu.SelectedNodes.Clear();

                var isCopyNotCut = _menuActionCutNodes[0].Enabled;
                if (isCopyNotCut)
                    m.HandleMenuItem_Copy(parentNode, _menuActionCutNodes);
                else
                    m.HandleMenu_RepositionNodes(parentNode, _menuActionCutNodes);

                MenuHelper.RefreshSort(menu);
            }
        }

        private void HandleMenuItem_Rename(UltraTree menu, UltraTreeNode node, string newName)
        {
            MenuHelper m = new MenuHelper(this, menu);
            m.HandleMenuItem_Rename(node, newName);
        }

        private void HandleMenuItem_Delete(UltraTree menu, SelectedNodesCollection selectedNodes)
        {
            if (!App.AskDeleteRows(selectedNodes.Count))
                return;

            // Isolate the nodes before processing.
            UltraTreeNode[] nodes = new UltraTreeNode[selectedNodes.Count];
            selectedNodes.CopyTo(nodes, 0);

            // Close each menu item if it is open for editing.
            foreach (UltraTreeNode node in nodes)
            {
                Form f = ActivateExistingMdiForm(node);
                if (f != null) f.Close();
            }

            // Delete items, and handle data conflict errors.
            MenuHelper m = new MenuHelper(this, menu);
            try
            {
                m.HandleMenuItem_Delete(nodes);
            }

            catch (InvalidOperationException ex)
            {
                App.ShowError(ex.Message);
            }
            catch (SqlException ex)
            {
                if (ex.Message.StartsWith(App.DataErrorPKDeleteConflictText))
                {
                    App.Error("Cannot delete record as it is used elsewhere as a link to another record.", ex);
                }
                else throw;
            }
        }

        #endregion

        #region Menu events

        private Rectangle _dragBoxFromMouseDown;

        private void Menu_AfterSelect(object sender, SelectEventArgs e)
        {
            //if (sender is UltraTree)
            //{
            //    // this keeps selected and active node in sync when dragging nodes
            //    UltraTree ultraTree = sender as UltraTree;
            //    if (e.NewSelections.Count == 1)
            //        ultraTree.ActiveNode = e.NewSelections[0];
            //}
        }

        private void Menu_BeforeExpand(object sender, CancelableNodeEventArgs e)
        {
            if (sender is UltraTree)
            {
                UltraTree menu = sender as UltraTree;
                UltraTreeNode node = e.TreeNode;

                if (MenuHelper.IsNodeMenuFolder(node))
                {
                    if (node.Nodes.Count == 0)
                        HandleMenuItem_Open(menu, node); // load folder items

                    menu.SelectedNodes.Clear();
                    node.Selected = true;
                    menu.ActiveNode = node;
                }
            }
        }

        private void Menu_DoubleClick(object sender, EventArgs e)
        {
            if (sender is UltraTree)
            {
                UltraTree menu = sender as UltraTree;
                if (menu.SelectedNodes.Count > 0)
                {
                    UltraTreeNode node = menu.SelectedNodes[0];
                    node.EndEdit(true);
                    HandleMenuItem_Open(menu, node);
                }
            }
        }

        private void Menu_KeyDown(object sender, KeyEventArgs e)
        {
            if (sender is UltraTree)
            {
                UltraTree menu = sender as UltraTree;

                if (menu.SelectedNodes.Count > 0 && !menu.SelectedNodes[0].IsEditing)
                {
                    switch (e.KeyCode)
                    {
                        case Keys.F5:
                            HandleMenu_LoadOnThread(menu);
                            break;
                        case Keys.Return:
                            HandleMenuItem_Open(menu, menu.SelectedNodes[0]); // only open one item from enter key
                            break;
                        case Keys.Delete:
                            HandleMenuItem_Delete(menu, menu.SelectedNodes);
                            break;
                        case Keys.Left:
                            if (menu.ActiveNode == menu.Nodes.All[0])
                                e.Handled = true; // menu title node, don't collapse it
                            break;
                    }
                }
            }
        }
        
        private void Menu_BeforeLabelEdit(object sender, CancelableNodeEventArgs e)
        {
            if (sender is UltraTree)
            {
                if (IsMdiFormOpen(e.TreeNode))
                {
                    App.ShowError("Item is open in editor, please close first or rename in editor");
                    e.Cancel = true;
                }
            }
        }

        private void Menu_AfterLabelEdit(object sender, NodeEventArgs e)
        {
            if (sender is UltraTree)
            {
                // rename node
                if (e.TreeNode.Text.Trim().Length > 0)
                {
                    UltraTree menu = sender as UltraTree;
                    HandleMenuItem_Rename(menu, e.TreeNode, e.TreeNode.Text);
                }
            }
        }

        private void Menu_MouseDown(object sender, MouseEventArgs e)
        {
            if (sender is UltraTree)
            {
                UltraTree menu = sender as UltraTree;

                UltraTreeNode node = menu.GetNodeFromPoint(e.X, e.Y);
                if (node != null)
                {
                    // prepare for drag-drop
                    if (e.Button == MouseButtons.Left)
                    {
                        _dragBoxFromMouseDown = App.DragDrop_SetDragStartPosition(e.X, e.Y);
                        node.BringIntoView();
                    }
                    // enable select node on right-click
                    else if (e.Button == MouseButtons.Right)
                    {
                        if (!node.Selected)
                        {
                            menu.SelectedNodes.Clear();
                            node.Selected = true;
                        }

                        menu.ActiveNode = node;
                    }
                }
                // not on a node so clear selections, ignoring scrollbars
                else
                {
                    UIElement elem = menu.UIElement.ElementFromPoint(new Point(e.X, e.Y));

                    if (elem != null && // not scroll bar
                        !(elem is ScrollThumbUIElement) &&
                        !(elem is ScrollTrackSubAreaUIElement) &&
                        !(elem is ScrollArrowUIElement))
                    {
                        menu.SelectedNodes.Clear();
                        menu.ActiveNode = null;
                    }

                    _dragBoxFromMouseDown = Rectangle.Empty;
                }
            }
        }

        private void Menu_MouseUp(object sender, MouseEventArgs e)
        {
            if (sender is UltraTree)
                _dragBoxFromMouseDown = Rectangle.Empty;
        }

        private void Menu_MouseMove(object sender, MouseEventArgs e)
        {
            if (sender is UltraTree)
            {
                if (e.Button == MouseButtons.Left)
                {
                    if (_dragBoxFromMouseDown != Rectangle.Empty && !_dragBoxFromMouseDown.Contains(e.X, e.Y))
                    {
                        // start a DragDrop operation

                        UltraTree menu = sender as UltraTree;

                        if (menu == ItineraryMenu)
                            menu.DoDragDrop(menu.SelectedNodes, DragDropEffects.Move);
                        else if (menu == SupplierMenu || menu == ContactMenu)
                            menu.DoDragDrop(menu.SelectedNodes, DragDropEffects.Move | DragDropEffects.Link);
                    }
                }
            }
        }

        private void Menu_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            if (e.EscapePressed)
                e.Action = DragAction.Cancel;
        }

        private void Menu_DragOver(object sender, DragEventArgs e)
        {
            if (sender is UltraTree)
            {
                UltraTree menu = sender as UltraTree;

                Point point = menu.PointToClient(new Point(e.X, e.Y));
                UltraTreeNode dropNode = menu.GetNodeFromPoint(point);

                if (dropNode == null)
                    e.Effect = DragDropEffects.None;
                else
                {
                    e.Effect = DragDropEffects.Move;
                }
            }
        }

        private void Menu_DragDrop(object sender, DragEventArgs e)
        {
            if (sender is UltraTree)
            {
                UltraTree menu = sender as UltraTree;
                Point dropPoint = menu.PointToClient(new Point(e.X, e.Y));
                UltraTreeNode dropNode = menu.GetNodeFromPoint(dropPoint);

                if (dropNode != null)
                {
                    // get drag-drop data
                    object dragData = e.Data.GetData(typeof (SelectedNodesCollection));
                    SelectedNodesCollection selectedNodes = dragData as SelectedNodesCollection;
                    if (selectedNodes == null)
                        return;

                    // use cloned collection
                    selectedNodes = selectedNodes.Clone() as SelectedNodesCollection;
                    if (selectedNodes == null)
                        return;

                    // validate
                    bool isValidMove = false;
                    foreach (UltraTreeNode node in selectedNodes)
                    {
                        if (node != dropNode) // node and drop node can't be same thing
                            isValidMove = true;
                    }
                    if (!isValidMove)
                        return;

                    // ensure item not open in editor
                    foreach (UltraTreeNode node in selectedNodes)
                    {
                        if (IsMdiFormOpen(node))
                        {
                            App.ShowError("Item is open in editor, please close before moving");
                            return;
                        }
                    }

                    // ask before move
                    if (!App.AskYesNo("Are you sure you want to move item(s)?"))
                        return;

                    // reposition nodes
                    if (selectedNodes.Count > 0)
                    {
                        selectedNodes.SortByPosition();
                        dropNode.Expanded = true; // this will load existing childnodes
                        HandleMenu_RepositionNodes(menu, dropNode, selectedNodes);
                    }
                }
            }
        }

        #endregion

        #region Load module forms

        internal void Load_StartPage()
        {
            UltraTreeNode tag = new UltraTreeNode("Welcome to TourWriter");
            LoadMdiForm(typeof(Modules.StartPage.StartMain), tag);
        }

        internal void Load_ItineraryForm(UltraTreeNode formTag)
        {
            LoadMdiForm(typeof(ItineraryMain), formTag);
        }

        internal void Load_SupplierForm(UltraTreeNode formTag)
        {
            LoadMdiForm(typeof(Modules.SupplierModule.SupplierMain), formTag);
        }

        internal void Load_ContactForm(UltraTreeNode formTag)
        {
            NavigationTreeItemInfo info = formTag.Tag as NavigationTreeItemInfo;
            if (info != null)
            {
                ContactMain contact = new ContactMain(info.ItemID);
                contact.HeaderVisible = false;
                contact.Show();
            }
        }

        internal void Load_SetupForm()
        {
            Load_SetupForm("");
        }

        internal void Load_SetupForm(string defaultControl)
        {
            UltraTreeNode tag = new UltraTreeNode("AdminMain");
            AdminMain adminMain = LoadMdiForm(typeof(AdminMain), tag) as AdminMain;
            if (adminMain != null)
                adminMain.LoadNewControl(defaultControl);
        }

        internal void Load_ReportsFormOLD()
        {
            UltraTreeNode tag = new UltraTreeNode("ReportsMainOLD");
            LoadMdiForm(typeof(Modules.ReportsModule.old.GeneralReportsMain), tag);
        }

        internal void Load_ReportsForm()
        {
            UltraTreeNode tag = new UltraTreeNode("ReportMain");
            LoadMdiForm(typeof(GeneralReportsMain), tag);
        }

        internal void Load_SearchForm()
        {
            var type =
                ItineraryMenu.Focused ? NavigationTreeItemInfo.ItemTypes.Itinerary :
                SupplierMenu.Focused ? NavigationTreeItemInfo.ItemTypes.Supplier :
                ContactMenu.Focused ? NavigationTreeItemInfo.ItemTypes.Contact :
                /* default */        NavigationTreeItemInfo.ItemTypes.Itinerary;
            Load_SearchForm(type);
        }

        internal void Load_SearchForm(NavigationTreeItemInfo.ItemTypes menuType)
        {
            UltraTreeNode tag = new UltraTreeNode("SearchMain");
            SearchMain s = LoadMdiForm(typeof(SearchMain), tag) as SearchMain;
            if (s != null)
                s.MenuType = menuType;
        }

        internal void Load_DataExtractForm()
        {
            LoadMdiForm(typeof(DataExtractMain), new UltraTreeNode("DataExtractMain"));
        }

        internal static void Load_AboutForm()
        {
            About f = new About();
            f.ShowDialog();
        }

        internal void Load_TourWriterDataFolder()
        {
            var f = Services.ExternalFilesHelper.GetTourWriterDataFolder();
            try
            {
                Cursor = Cursors.WaitCursor;
                Process.Start(Services.ExternalFilesHelper.GetTourWriterDataFolder());
            }
            catch(Exception ex)
            {
                Cursor = Cursors.Default;
                if (Services.ErrorHelper.IsFileAccessError(ex))
                    App.ShowFileNotFound(f);
                else throw;
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        internal Form LoadMdiForm(string formType, UltraTreeNode formTag)
        {
            Assembly assembly = Assembly.LoadFrom(Assembly.GetExecutingAssembly().CodeBase);
            return LoadMdiForm(assembly.GetType(formType), formTag);
        }

        internal Form LoadMdiForm(Type formType, UltraTreeNode formTag)
        {
            Cursor c = Cursor;
            Cursor = Cursors.WaitCursor;

            Form form = null;
            try
            {
                form = ActivateExistingMdiForm(formTag);
                if (form == null && formType != null)
                {
                    form = (Form)Activator.CreateInstance(formType);
                    form.Tag = formTag; // reference to menu node					
                    form.MdiParent = this;
                    form.AutoScroll = true;
                    form.Show();
                }
            }
            catch (Exception ex)
            {
                App.Error(ex);
            }
            finally
            {
                Cursor = c;
            }
            return form;
        }


        internal UltraTreeNode BuildMenuNode(NavigationTreeItemInfo info)
        {
            return MenuHelper.Menu_BuildNode(info);
        }

        private Form ActivateExistingMdiForm(IKeyedSubObject testNode)
        {
            foreach (Form form in MdiChildren)
            {
                if (form.Tag != null)
                {
                    UltraTreeNode formNode = form.Tag as UltraTreeNode;

                    if (formNode != null && testNode.Key.Equals(formNode.Key))
                    {
                        form.Activate();
                        return form;
                    }
                }
            }
            return null;
        }

        private bool IsMdiFormOpen(UltraTreeNode formTag)
        {
            foreach (Form form in MdiChildren)
            {
                if (form.Tag != null && form.Tag == formTag)
                    return true;
            }
            return false;
        }

        internal UltraTreeNode ActivateMenuNode(UltraTreeNode node)
        {
            ItineraryMenu.SelectedNodes.Clear();
            SupplierMenu.SelectedNodes.Clear();
            treeContacts.SelectedNodes.Clear();

            node.Selected = true;

            if (!node.Selected)
            {
                // path to node is lost (eg. menu refreshed) so reload the node

                UltraTree menu = null;

                if (node.Tag != null)
                {
                    UltraTreeNode foundNode;
                    NavigationTreeItemInfo info = node.Tag as NavigationTreeItemInfo;

                    if (info != null)
                    {
                        if (info.ItemType == NavigationTreeItemInfo.ItemTypes.Itinerary)
                            menu = ItineraryMenu;
                        else if (info.ItemType == NavigationTreeItemInfo.ItemTypes.Supplier)
                            menu = SupplierMenu;
                        else if (info.ItemType == NavigationTreeItemInfo.ItemTypes.Contact)
                            menu = ContactMenu;

                        if (menu != null)
                        {
                            foundNode = menu.GetNodeByKey(node.Key);
                            if (foundNode != null)
                            {
                                foundNode.Text = node.Text;
                                foundNode.Tag = node.Tag;
                                node = foundNode;
                            }
                            else
                            {
                                HandleMenu_LoadChildListAndParentTree(menu, node);
                                node = menu.GetNodeByKey(node.Key);
                            }
                            // Mdi form could have changed node text
                            MenuHelper.RefreshSort(menu);
                        }
                    }
                }
            }
            if (node != null)
            {
                node.Selected = true;
                node.BringIntoView(true);
            }
            return node;
        }

        #endregion

        #region Keyboard hook

        private readonly LocalKeyboardHook hook;

        private void Keyboard_Event(KeyDefinition e)
        {
            if (e.Equals(App.DebugKeyCombination))
            {
                // Show/hide the debug menu
                App.IsDebugMode = !App.IsDebugMode;
                menuDebug.Visible = App.IsDebugMode;
                menuReports.Visible = App.IsDebugMode;
                navPane.Groups["Additional"].Items["Reports"].Visible = App.IsDebugMode;
                return;
            }

            // [CTRL + F]
            if (e.Equals(new KeyDefinition(true, false, false, Keys.F, Utilities.KeyHook.KeyState.KeyDown)))
            {
                Load_SearchForm();
                return;
            }

            // menus
            var menu = GetNavigationMenuTree(currentNavigationMenuType);
            if (menu.Focused)
            {
                // CTRL+X
                if (e.Equals(new KeyDefinition(true, false, false, Keys.X, Utilities.KeyHook.KeyState.KeyDown)))
                {
                    if (!(menu.ActiveNode == menu.Nodes[0]))
                        contextMenuCut.PerformClick();
                    return;
                }

                // CTRL+C
                if (e.Equals(new KeyDefinition(true, false, false, Keys.C, Utilities.KeyHook.KeyState.KeyDown)))
                {
                    if (!(menu.ActiveNode == menu.Nodes[0] || MenuHelper.IsNodeMenuFolder(menu.ActiveNode)))
                        contextMenuCopy.PerformClick();
                    return;
                }

                // CTRL+V
                if (e.Equals(new KeyDefinition(true, false, false, Keys.V, Utilities.KeyHook.KeyState.KeyDown)))
                {
                    if (contextMenuPaste.Enabled)
                        contextMenuPaste.PerformClick();
                    return;
                }

                // ESC
                if (e.Equals(new KeyDefinition(false, false, false, Keys.Escape, Utilities.KeyHook.KeyState.KeyDown)))
                {
                    contextMenuCancelPaste.PerformClick();
                    return;
                }
            }
        }

        #endregion

        #region Helper methods

        private void ultraExplorerBar1_ItemClick(object sender, Infragistics.Win.UltraWinExplorerBar.ItemEventArgs e)
        {
            switch (e.Item.Key)
            {
                case "Reports":
                    Load_ReportsForm();
                    break;
                case "ReportsOLD":
                    Load_ReportsFormOLD();
                    break;
                case "Search":
                    Load_SearchForm();
                    break;
                case "Setup":
                    Load_SetupForm();
                    break;
                case "DataExtract":
                    Load_DataExtractForm();
                    break;
                case "StartPage":
                    Load_StartPage();
                    break;
                case "TourWriterData":
                    Load_TourWriterDataFolder();
                    break;
            }
        }

        internal ProgressBarInfo StatusBar_ProgressBar
        {
            get { return StatusBar.Panels["Progress"].ProgressBarInfo; }
        }

        internal string StatusBar_ConnectionText
        {
            get { return StatusBar.Panels["Connection"].Text; }
            set { StatusBar.Panels["Connection"].Text = value; }
        }

        internal string StatusBar_VersionText
        {
            get { return StatusBar.Panels["Version"].Text; }
            set { StatusBar.Panels["Version"].Text = value; }
        }

        internal bool ShowAppClosingPrompt = true;
        private void MainForm_Closing(object sender, CancelEventArgs e)
        {
            if (e.Cancel != true) // no Mdi forms have cancelled...
            {
                if (ShowAppClosingPrompt)
                {
                    // ask if close application
                    DialogResult result = MessageBox.Show(
                        App.GetResourceString("AskCloseApp"),
                        App.GetResourceString("MessageCaption"),
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.No)
                    {
                        e.Cancel = true;
                        return;
                    }
                }

                if (Global.Cache.IsUserLoggedIn)
                {
                    SaveLayout();
                    Logout();
                }
                Application.Exit(); // to ensure all other forms are closed also
            }
        }

        private void LoadLayout()
        {
            Size = Settings.Default.MainFormSize;
            Location = Settings.Default.MainFormLocation;
            WindowState = Settings.Default.MainFormWindowState;
            pnlMenu.Width = Settings.Default.MainFormMenuWidth;
        }

        private void SaveLayout()
        {
            Settings.Default.MainFormWindowState =
                (WindowState != FormWindowState.Minimized) ? WindowState : FormWindowState.Normal;

            if (WindowState == FormWindowState.Normal)
            {
                Settings.Default.MainFormSize = Size;
                Settings.Default.MainFormLocation = Location;
            }
            else
            {
                Settings.Default.MainFormSize = RestoreBounds.Size;
                Settings.Default.MainFormLocation = RestoreBounds.Location;
            }

            Settings.Default.MainFormMenuWidth = pnlMenu.Width;

            Settings.Default.Save();
        }

        private static void Logout()
        {
            new UserSession().RemoveSession(App.LoginGuid);
        }

        internal UltraTree ItineraryMenu
        {
            get { return treeItineraries; }
        }

        internal UltraTree SupplierMenu
        {
            get { return treeSuppliers; }
        }

        internal UltraTree ContactMenu
        {
            get { return treeContacts; }
        }

        private void Form_Deactivate(object sender, EventArgs e)
        {
            hook.Detach();
        }

        #endregion

        #region Context menu

        private enum NavigationMenus
        {
            Itinerares,
            Suppliers,
            Contacts,
            NULL
        };

        private NavigationMenus currentNavigationMenuType;

        private void SetCurrentNavigationMenuType(object sender)
        {
            currentNavigationMenuType = NavigationMenus.NULL;

            if (sender == contextMenuNavigation)
            {
                // Get navigation type from navigation tree menus right-click.
                if (contextMenuNavigation.SourceControl != null)
                {
                    if (contextMenuNavigation.SourceControl == treeItineraries)
                    {
                        currentNavigationMenuType = NavigationMenus.Itinerares;
                    }
                    else if (contextMenuNavigation.SourceControl == treeSuppliers)
                    {
                        currentNavigationMenuType = NavigationMenus.Suppliers;
                    }
                    else if (contextMenuNavigation.SourceControl == treeContacts)
                    {
                        currentNavigationMenuType = NavigationMenus.Contacts;
                    }
                }

                // Get navigation type from tools menu dropdown.
                else if (contextMenuNavigation.OwnerItem != null)
                {
                    if (contextMenuNavigation.OwnerItem == menuItineraries)
                    {
                        currentNavigationMenuType = NavigationMenus.Itinerares;
                    }
                    else if (contextMenuNavigation.OwnerItem == menuSuppliers)
                    {
                        currentNavigationMenuType = NavigationMenus.Suppliers;
                    }
                    else if (contextMenuNavigation.OwnerItem == menuContacts)
                    {
                        currentNavigationMenuType = NavigationMenus.Contacts;
                    }
                }

                contextMenuEmail.Visible = (currentNavigationMenuType == NavigationMenus.Contacts);
            }
        }

        private UltraTree GetNavigationMenuTree(NavigationMenus navigationMenu)
        {
            switch (navigationMenu)
            {
                case NavigationMenus.Itinerares:
                    return treeItineraries;
                case NavigationMenus.Suppliers:
                    return treeSuppliers;
                case NavigationMenus.Contacts:
                    return treeContacts;
                default:
                    return null;
            }
        }

        private static NavigationTreeItemInfo.ItemTypes? GetNavigationMenuItemType(NavigationMenus navigationMenu)
        {
            switch (navigationMenu)
            {
                case NavigationMenus.Itinerares:
                    return NavigationTreeItemInfo.ItemTypes.Itinerary;
                case NavigationMenus.Suppliers:
                    return NavigationTreeItemInfo.ItemTypes.Supplier;
                case NavigationMenus.Contacts:
                    return NavigationTreeItemInfo.ItemTypes.Contact;
                default:
                    return null;
            }
        }


        private void contextMenuNavigation_LostFocus(object sender, System.EventArgs e)
        {
            HandleMenuItem_EndCopyOrCut();
        }

        private void contextMenu_Opening(object sender, CancelEventArgs e)
        {
            SetCurrentNavigationMenuType(sender);

            UltraTree currentNavigationTree = GetNavigationMenuTree(currentNavigationMenuType);
            if (currentNavigationTree != null)
            {
                // Set enabled controls, depending on navigation item selection.
                bool isTopNode = currentNavigationTree.ActiveNode == currentNavigationTree.Nodes[0];
                bool isValid = !isTopNode && (currentNavigationTree.SelectedNodes.Count > 0);
                contextMenuOpen.Enabled = isValid;
                contextMenuDelete.Enabled = isValid;
                contextMenuRename.Enabled = isValid;
                contextMenuSeparator1.Enabled = isValid;
                contextMenuCopy.Enabled = isValid && !MenuHelper.IsNodeMenuFolder(currentNavigationTree.ActiveNode);
                contextMenuCut.Enabled = isValid;
                contextMenuPaste.Enabled = currentNavigationTree.SelectedNodes.Count > 0 && _menuActionCutNodes != null && _menuActionCutNodes.Length > 0;

                // Set control appearance, depending on selected navigation menu.
                switch (currentNavigationMenuType)
                {
                    case NavigationMenus.Itinerares:
                        contextMenuNew.Text = "New Itinerary";
                        contextMenuNew.Image = Properties.Resources.Image;
                        break;
                    case NavigationMenus.Suppliers:
                        contextMenuNew.Text = "New Supplier";
                        contextMenuNew.Image = Properties.Resources.House;
                        break;
                    case NavigationMenus.Contacts:
                        contextMenuNew.Text = "New Contact";
                        contextMenuNew.Image = Properties.Resources.User;
                        break;
                }
            }
        }

        private void contextMenuOpen_Click(object sender, EventArgs e)
        {
            HandleMenuItem_EndCopyOrCut();
            UltraTree contextTree = GetNavigationMenuTree(currentNavigationMenuType);
            if (contextTree != null)
            {
                SetActiveNavigationMenu(currentNavigationMenuType);
                HandleMenuItem_Open(contextTree, contextTree.SelectedNodes);
            }
        }

        private void contextMenuNew_Click(object sender, EventArgs e)
        {
            HandleMenuItem_EndCopyOrCut();
            UltraTree contextTree = GetNavigationMenuTree(currentNavigationMenuType);
            NavigationTreeItemInfo.ItemTypes? contextType = GetNavigationMenuItemType(currentNavigationMenuType);
            if (contextTree != null && contextType.HasValue)
            {
                SetActiveNavigationMenu(currentNavigationMenuType);
                HandleMenuItem_Create(contextTree, (NavigationTreeItemInfo.ItemTypes)contextType);
            }
        }

        private void contextMenuNewFolder_Click(object sender, EventArgs e)
        {
            HandleMenuItem_EndCopyOrCut();
            UltraTree contextTree = GetNavigationMenuTree(currentNavigationMenuType);
            if (contextTree != null)
            {
                SetActiveNavigationMenu(currentNavigationMenuType);
                HandleMenuItem_Create(contextTree, NavigationTreeItemInfo.ItemTypes.Folder);
            }
        }
        
        private void contextMenuCopy_Click(object sender, EventArgs e)
        {
            HandleMenuItem_EndCopyOrCut();
            var contextTree = GetNavigationMenuTree(currentNavigationMenuType);
            if (contextTree != null)
            {
                SetActiveNavigationMenu(currentNavigationMenuType);
                HandleMenuItem_StartCopyOrCut(contextTree.SelectedNodes, false);
            }
        }

        private void contextMenuCut_Click(object sender, EventArgs e)
        {
            HandleMenuItem_EndCopyOrCut();
            var contextTree = GetNavigationMenuTree(currentNavigationMenuType);
            if (contextTree != null)
            {
                SetActiveNavigationMenu(currentNavigationMenuType);
                HandleMenuItem_StartCopyOrCut(contextTree.SelectedNodes, true);
            }
        }

        private void contextMenuPaste_Click(object sender, EventArgs e)
        {
            UltraTree contextTree = GetNavigationMenuTree(currentNavigationMenuType);
            if (contextTree != null)
            {
                SetActiveNavigationMenu(currentNavigationMenuType);
                HandleMenuItem_Paste(contextTree, contextTree.SelectedNodes);
                HandleMenuItem_EndCopyOrCut();
            }
        }

        private void contextMenuCancelPaste_Click(object sender, EventArgs e)
        {
            HandleMenuItem_EndCopyOrCut();
        }

        private void contextMenuDelete_Click(object sender, EventArgs e)
        {
            HandleMenuItem_EndCopyOrCut();
            UltraTree contextTree = GetNavigationMenuTree(currentNavigationMenuType);
            if (contextTree != null)
            {
                SetActiveNavigationMenu(currentNavigationMenuType);
                HandleMenuItem_Delete(contextTree, contextTree.SelectedNodes);
            }
        }

        private void contextMenuRename_Click(object sender, EventArgs e)
        {
            HandleMenuItem_EndCopyOrCut();
            UltraTree contextTree = GetNavigationMenuTree(currentNavigationMenuType);
            if (contextTree != null)
            {
                SetActiveNavigationMenu(currentNavigationMenuType);
                contextTree.ActiveNode.BeginEdit();
            }
        }

        private void contextMenuSearch_Click(object sender, EventArgs e)
        {
            HandleMenuItem_EndCopyOrCut();
            NavigationTreeItemInfo.ItemTypes? contextType = GetNavigationMenuItemType(currentNavigationMenuType);
            if (contextType.HasValue)
            {
                SetActiveNavigationMenu(currentNavigationMenuType);
                Load_SearchForm((NavigationTreeItemInfo.ItemTypes)contextType);
            }
        }

        private void contextMenuRefresh_Click(object sender, EventArgs e)
        {
            HandleMenuItem_EndCopyOrCut();
            UltraTree contextTree = GetNavigationMenuTree(currentNavigationMenuType);
            if (contextTree != null)
            {
                SetActiveNavigationMenu(currentNavigationMenuType);
                InitialiseMenu(contextTree);
            }
        }

        private void contextMenuEmailCategories_Click(object sender, EventArgs e)
        {
            HandleMenuItem_EndCopyOrCut();
            CategorySelector categorySelector = new CategorySelector();
            categorySelector.ShowDialog();
        }

        private void contextMenuEmailSelected_Click(object sender, EventArgs e)
        {
            HandleMenuItem_EndCopyOrCut();
            List<int> idList = new List<int>();

            foreach (UltraTreeNode node in treeContacts.SelectedNodes)
            {
                NavigationTreeItemInfo info = (NavigationTreeItemInfo)node.Tag;
                if (info.ItemType == NavigationTreeItemInfo.ItemTypes.Folder)
                {
                    NavigationTreeItemInfo[] children = info.GetChildList(info.ItemID, NavigationTreeItemInfo.ItemTypes.Contact);
                    foreach (NavigationTreeItemInfo child in children)
                    {
                        idList.Add(child.ItemID);
                    }
                }
                else
                {
                    idList.Add(info.ItemID);
                }
            }

            DataTable contactTable = BuildContactTable(idList);
            if (contactTable.Rows.Count > 0)
            {
                ContactEmailForm emailForm = new ContactEmailForm(contactTable);
                emailForm.Show();
            }
            else
            {
                App.ShowWarning("Please select one or more contacts/folders.");
            }
        }

        private static DataTable BuildContactTable(IEnumerable<int> contactIdList)
        {
            string contactIds = String.Empty;
            ContactSet.ContactDataTable table = new ContactSet.ContactDataTable();

            foreach (int id in contactIdList)
            {
                contactIds += id + ",";
            }

            if (contactIds.Length > 0)
            {
                // chop the ',' off the end
                contactIds = contactIds.Remove(contactIds.Length - 1);

                using (SqlConnection conn = new SqlConnection(ConnectionString.GetConnectionString_UNSAFE()))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandText = String.Format(
                        "SELECT DISTINCT ContactName,ISNULL(Title, ''),ISNULL(FirstName,''),ISNULL(LastName,''),ISNULL(Email1,'') " +
                        "FROM Contact " +
                        "WHERE ContactID IN ({0}) " +
                        "ORDER BY ContactName", contactIds);
                    cmd.Connection = conn;

                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            ContactSet.ContactRow row = table.NewContactRow();
                            row.ContactName = reader.GetString(0);
                            row.Title = reader.GetString(1);
                            row.FirstName = reader.GetString(2);
                            row.LastName = reader.GetString(3);
                            row.Email1 = reader.GetString(4);

                            table.AddContactRow(row);
                        }
                    }
                }
            }

            return table;
        }

        #endregion

        #region Toolbar menu

        private void menuExit_Click(object sender, EventArgs e)
        {
           Close();
        }

        private void menuHelpContents_Click(object sender, EventArgs e)
        {
            App.ShowHelp();
        }

        private void menuScreenshot_Click(object sender, EventArgs e)
        {
            MessageForm f = new MessageForm("Enter your message here...");
            f.AllowEdit = true;
            f.IncludeScreenshot = true;
            f.ShowDialog();
        }

        private void menuAbout_Click(object sender, EventArgs e)
        {
            Load_AboutForm();
        }

        private void menuUpdate_Click(object sender, EventArgs e)
        {
            Services.Update.ApplicationUpdateService.RunUpdate();
        }

        #region Debug

        private void menuDebugUser_Click(object sender, EventArgs e)
        {
            App.ViewDataSet(Global.Cache.UserSet);
        }

        private void menuDebugTools_Click(object sender, EventArgs e)
        {
            App.ViewDataSet(Global.Cache.ToolSet);
        }

        private void menuDebugAgents_Click(object sender, EventArgs e)
        {
            App.ViewDataSet(Global.Cache.AgentSet);
        }

        #endregion

        private void btnSchema_Click(object sender, EventArgs e)
        {
            string file = App.PromptSaveFile("TourWriter.sql", "", ".sql");

            if (!string.IsNullOrEmpty(file))
            {
                Application.DoEvents();
                Cursor = Cursors.WaitCursor;

                StreamWriter sw = null;
                try
                {
                    string installId = !Global.Cache.ToolSet.AppSettings[0].IsInstallIDNull() ?
                        Global.Cache.ToolSet.AppSettings[0].InstallID.ToString() : "n/a";

                    sw = new FileInfo(file).CreateText();
                    sw.WriteLine(string.Format("/** Schema number: {0} **/", installId));
                    sw.Write(DatabaseHelper.GetSchema());
                }
                finally
                {
                    if(sw != null)
                        sw.Close();

                    Cursor = Cursors.Default;
                }
            }
        }

        #region Additional

        private void menuGeneralReports_Click(object sender, EventArgs e)
        {
            Load_ReportsFormOLD();
        }

        private void menuDataExtraction_Click(object sender, EventArgs e)
        {
            Load_DataExtractForm();
        }

        private void menuSearch_Click(object sender, EventArgs e)
        {
            Load_SearchForm();
        }

        private void menuStartPage_Click(object sender, EventArgs e)
        {
            Load_StartPage();
        }

        private void menuTourWriterData_Click(object sender, EventArgs e)
        {
            Load_TourWriterDataFolder();
        }

        #endregion

        #region New

        private void menuItineraryNew_Click(object sender, EventArgs e)
        {
            SetActiveNavigationMenu(NavigationMenus.Itinerares);
            HandleMenuItem_Create(treeItineraries, NavigationTreeItemInfo.ItemTypes.Itinerary);
        }

        private void menuSupplierNew_Click(object sender, EventArgs e)
        {
            SetActiveNavigationMenu(NavigationMenus.Suppliers);
            HandleMenuItem_Create(treeSuppliers, NavigationTreeItemInfo.ItemTypes.Supplier);
        }

        private void menuContactNew_Click(object sender, EventArgs e)
        {
            SetActiveNavigationMenu(NavigationMenus.Contacts);
            HandleMenuItem_Create(treeContacts, NavigationTreeItemInfo.ItemTypes.Contact);
        }

        #endregion

        private void menuSetup_Click(object sender, EventArgs e)
        {
            Load_SetupForm();
        }

        private void menuReports_Click(object sender, EventArgs e)
        {
            Load_ReportsForm();
        }

        #endregion

        private void SetActiveNavigationMenu(NavigationMenus navigationMenu)
        {
            if (navigationMenu == NavigationMenus.Itinerares)
            {
                navPane.Groups["Itineraries"].EnsureGroupInView();
            }
            else if (navigationMenu == NavigationMenus.Suppliers)
            {
                navPane.Groups["Suppliers"].EnsureGroupInView();
            }
            else if (navigationMenu == NavigationMenus.Contacts)
            {
                navPane.Groups["Contacts"].EnsureGroupInView();
            }

        }

        private void menuCloseAll_Click(object sender, EventArgs e)
        {
            foreach (Form mdiChildForm in MdiChildren)
            {
                mdiChildForm.Close();
            }
        }

        private void menuWindow_DropDownOpening(object sender, EventArgs e)
        {
            // Hide separator if it is the last item
            ToolStripItemCollection items = menuStrip1.MdiWindowListItem.DropDownItems;
            if (items[items.Count - 1] is ToolStripSeparator)
            {
                items.RemoveAt(items.Count - 1);
            }
        }

        internal void MergeToolStrip(ToolStrip toolStrip)
        {
            if(toolStrip != null)
                ToolStripManager.Merge(toolStrip, toolStrip1);
        }

        internal void RevertToolStrip(ToolStrip toolStrip)
        {
            if (toolStrip != null)
                ToolStripManager.RevertMerge(toolStrip1, toolStrip);
        }

        private void menuFeedback_Click(object sender, EventArgs e)
        {
            Process.Start("mailto:feedback@tourwriter.com?subject=Client Feedback");
        }
    }
}
