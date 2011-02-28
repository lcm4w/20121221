namespace TourWriter.Modules.Dashboard
{
    partial class Dashboard
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            Infragistics.Win.UltraWinDock.DockAreaPane dockAreaPane1 = new Infragistics.Win.UltraWinDock.DockAreaPane(Infragistics.Win.UltraWinDock.DockedLocation.DockedLeft, new System.Guid("a72ee027-46ac-424b-ade0-52e408fe5079"));
            Infragistics.Win.UltraWinDock.DockableControlPane dockableControlPane1 = new Infragistics.Win.UltraWinDock.DockableControlPane(new System.Guid("e0e284e2-64dc-41f8-b0fb-048655212adc"), new System.Guid("00000000-0000-0000-0000-000000000000"), -1, new System.Guid("a72ee027-46ac-424b-ade0-52e408fe5079"), -1);
            Infragistics.Win.UltraWinDock.DockAreaPane dockAreaPane2 = new Infragistics.Win.UltraWinDock.DockAreaPane(Infragistics.Win.UltraWinDock.DockedLocation.DockedLeft, new System.Guid("dd69a0dd-15bb-421c-89e2-b8978d5fae08"));
            Infragistics.Win.UltraWinDock.DockableControlPane dockableControlPane2 = new Infragistics.Win.UltraWinDock.DockableControlPane(new System.Guid("b88bcbf6-27b8-4796-b41a-55018bd8101b"), new System.Guid("00000000-0000-0000-0000-000000000000"), -1, new System.Guid("dd69a0dd-15bb-421c-89e2-b8978d5fae08"), -1);
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.ultraDockManager1 = new Infragistics.Win.UltraWinDock.UltraDockManager(this.components);
            this._ModuleBaseUnpinnedTabAreaLeft = new Infragistics.Win.UltraWinDock.UnpinnedTabArea();
            this._ModuleBaseUnpinnedTabAreaRight = new Infragistics.Win.UltraWinDock.UnpinnedTabArea();
            this._ModuleBaseUnpinnedTabAreaTop = new Infragistics.Win.UltraWinDock.UnpinnedTabArea();
            this._ModuleBaseUnpinnedTabAreaBottom = new Infragistics.Win.UltraWinDock.UnpinnedTabArea();
            this._ModuleBaseAutoHideControl = new Infragistics.Win.UltraWinDock.AutoHideControl();
            this.windowDockingArea1 = new Infragistics.Win.UltraWinDock.WindowDockingArea();
            this.dockableWindow1 = new Infragistics.Win.UltraWinDock.DockableWindow();
            this.windowDockingArea2 = new Infragistics.Win.UltraWinDock.WindowDockingArea();
            this.dockableWindow2 = new Infragistics.Win.UltraWinDock.DockableWindow();
            ((System.ComponentModel.ISupportInitialize)(this.ultraDockManager1)).BeginInit();
            this.windowDockingArea1.SuspendLayout();
            this.dockableWindow1.SuspendLayout();
            this.windowDockingArea2.SuspendLayout();
            this.dockableWindow2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel2
            // 
            this.panel2.Location = new System.Drawing.Point(0, 20);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(95, 519);
            this.panel2.TabIndex = 0;
            // 
            // panel3
            // 
            this.panel3.Location = new System.Drawing.Point(0, 20);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(95, 519);
            this.panel3.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(200, 40);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(722, 499);
            this.panel1.TabIndex = 77;
            // 
            // ultraDockManager1
            // 
            this.ultraDockManager1.CaptionStyle = Infragistics.Win.UltraWinDock.CaptionStyle.VisualStudio2008;
            this.ultraDockManager1.CompressUnpinnedTabs = false;
            this.ultraDockManager1.DefaultPaneSettings.AllowFloating = Infragistics.Win.DefaultableBoolean.False;
            this.ultraDockManager1.DefaultPaneSettings.CanDisplayAsMdiChild = Infragistics.Win.DefaultableBoolean.True;
            dockAreaPane1.ChildPaneStyle = Infragistics.Win.UltraWinDock.ChildPaneStyle.TabGroup;
            dockAreaPane1.DockedBefore = new System.Guid("dd69a0dd-15bb-421c-89e2-b8978d5fae08");
            dockableControlPane1.Control = this.panel2;
            dockableControlPane1.OriginalControlBounds = new System.Drawing.Rectangle(191, 68, 200, 100);
            dockableControlPane1.Size = new System.Drawing.Size(100, 100);
            dockableControlPane1.Text = "panel2";
            dockAreaPane1.Panes.AddRange(new Infragistics.Win.UltraWinDock.DockablePaneBase[] {
            dockableControlPane1});
            dockAreaPane1.Size = new System.Drawing.Size(95, 539);
            dockAreaPane2.ChildPaneStyle = Infragistics.Win.UltraWinDock.ChildPaneStyle.TabGroup;
            dockableControlPane2.Control = this.panel3;
            dockableControlPane2.OriginalControlBounds = new System.Drawing.Rectangle(358, 101, 200, 100);
            dockableControlPane2.Size = new System.Drawing.Size(100, 100);
            dockableControlPane2.Text = "panel3";
            dockAreaPane2.Panes.AddRange(new Infragistics.Win.UltraWinDock.DockablePaneBase[] {
            dockableControlPane2});
            dockAreaPane2.Size = new System.Drawing.Size(95, 539);
            this.ultraDockManager1.DockAreas.AddRange(new Infragistics.Win.UltraWinDock.DockAreaPane[] {
            dockAreaPane1,
            dockAreaPane2});
            this.ultraDockManager1.DragWindowStyle = Infragistics.Win.UltraWinDock.DragWindowStyle.OutlineWithIndicators;
            this.ultraDockManager1.HostControl = this;
            this.ultraDockManager1.LayoutStyle = Infragistics.Win.UltraWinDock.DockAreaLayoutStyle.FillContainer;
            this.ultraDockManager1.ShowCloseButton = false;
            this.ultraDockManager1.ShowPinButton = false;
            this.ultraDockManager1.UnpinnedTabStyle = Infragistics.Win.UltraWinTabs.TabStyle.VisualStudio;
            this.ultraDockManager1.WindowStyle = Infragistics.Win.UltraWinDock.WindowStyle.VisualStudio2008;
            // 
            // _ModuleBaseUnpinnedTabAreaLeft
            // 
            this._ModuleBaseUnpinnedTabAreaLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this._ModuleBaseUnpinnedTabAreaLeft.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._ModuleBaseUnpinnedTabAreaLeft.Location = new System.Drawing.Point(0, 0);
            this._ModuleBaseUnpinnedTabAreaLeft.Name = "_ModuleBaseUnpinnedTabAreaLeft";
            this._ModuleBaseUnpinnedTabAreaLeft.Owner = this.ultraDockManager1;
            this._ModuleBaseUnpinnedTabAreaLeft.Size = new System.Drawing.Size(0, 539);
            this._ModuleBaseUnpinnedTabAreaLeft.TabIndex = 78;
            // 
            // _ModuleBaseUnpinnedTabAreaRight
            // 
            this._ModuleBaseUnpinnedTabAreaRight.Dock = System.Windows.Forms.DockStyle.Right;
            this._ModuleBaseUnpinnedTabAreaRight.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._ModuleBaseUnpinnedTabAreaRight.Location = new System.Drawing.Point(922, 0);
            this._ModuleBaseUnpinnedTabAreaRight.Name = "_ModuleBaseUnpinnedTabAreaRight";
            this._ModuleBaseUnpinnedTabAreaRight.Owner = this.ultraDockManager1;
            this._ModuleBaseUnpinnedTabAreaRight.Size = new System.Drawing.Size(0, 539);
            this._ModuleBaseUnpinnedTabAreaRight.TabIndex = 79;
            // 
            // _ModuleBaseUnpinnedTabAreaTop
            // 
            this._ModuleBaseUnpinnedTabAreaTop.Dock = System.Windows.Forms.DockStyle.Top;
            this._ModuleBaseUnpinnedTabAreaTop.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._ModuleBaseUnpinnedTabAreaTop.Location = new System.Drawing.Point(0, 0);
            this._ModuleBaseUnpinnedTabAreaTop.Name = "_ModuleBaseUnpinnedTabAreaTop";
            this._ModuleBaseUnpinnedTabAreaTop.Owner = this.ultraDockManager1;
            this._ModuleBaseUnpinnedTabAreaTop.Size = new System.Drawing.Size(922, 0);
            this._ModuleBaseUnpinnedTabAreaTop.TabIndex = 80;
            // 
            // _ModuleBaseUnpinnedTabAreaBottom
            // 
            this._ModuleBaseUnpinnedTabAreaBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this._ModuleBaseUnpinnedTabAreaBottom.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._ModuleBaseUnpinnedTabAreaBottom.Location = new System.Drawing.Point(0, 539);
            this._ModuleBaseUnpinnedTabAreaBottom.Name = "_ModuleBaseUnpinnedTabAreaBottom";
            this._ModuleBaseUnpinnedTabAreaBottom.Owner = this.ultraDockManager1;
            this._ModuleBaseUnpinnedTabAreaBottom.Size = new System.Drawing.Size(922, 0);
            this._ModuleBaseUnpinnedTabAreaBottom.TabIndex = 81;
            // 
            // _ModuleBaseAutoHideControl
            // 
            this._ModuleBaseAutoHideControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._ModuleBaseAutoHideControl.Location = new System.Drawing.Point(0, 0);
            this._ModuleBaseAutoHideControl.Name = "_ModuleBaseAutoHideControl";
            this._ModuleBaseAutoHideControl.Owner = this.ultraDockManager1;
            this._ModuleBaseAutoHideControl.Size = new System.Drawing.Size(0, 0);
            this._ModuleBaseAutoHideControl.TabIndex = 82;
            // 
            // windowDockingArea1
            // 
            this.windowDockingArea1.Controls.Add(this.dockableWindow2);
            this.windowDockingArea1.Dock = System.Windows.Forms.DockStyle.Left;
            this.windowDockingArea1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.windowDockingArea1.Location = new System.Drawing.Point(0, 0);
            this.windowDockingArea1.Name = "windowDockingArea1";
            this.windowDockingArea1.Owner = this.ultraDockManager1;
            this.windowDockingArea1.Size = new System.Drawing.Size(100, 539);
            this.windowDockingArea1.TabIndex = 83;
            // 
            // dockableWindow1
            // 
            this.dockableWindow1.Controls.Add(this.panel3);
            this.dockableWindow1.Location = new System.Drawing.Point(0, 0);
            this.dockableWindow1.Name = "dockableWindow1";
            this.dockableWindow1.Owner = this.ultraDockManager1;
            this.dockableWindow1.Size = new System.Drawing.Size(95, 539);
            this.dockableWindow1.TabIndex = 86;
            // 
            // windowDockingArea2
            // 
            this.windowDockingArea2.Controls.Add(this.dockableWindow1);
            this.windowDockingArea2.Dock = System.Windows.Forms.DockStyle.Left;
            this.windowDockingArea2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.windowDockingArea2.Location = new System.Drawing.Point(100, 0);
            this.windowDockingArea2.Name = "windowDockingArea2";
            this.windowDockingArea2.Owner = this.ultraDockManager1;
            this.windowDockingArea2.Size = new System.Drawing.Size(100, 539);
            this.windowDockingArea2.TabIndex = 84;
            // 
            // dockableWindow2
            // 
            this.dockableWindow2.Controls.Add(this.panel2);
            this.dockableWindow2.Location = new System.Drawing.Point(0, 0);
            this.dockableWindow2.Name = "dockableWindow2";
            this.dockableWindow2.Owner = this.ultraDockManager1;
            this.dockableWindow2.Size = new System.Drawing.Size(95, 539);
            this.dockableWindow2.TabIndex = 85;
            // 
            // Dashboard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(922, 539);
            this.Controls.Add(this._ModuleBaseAutoHideControl);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this._ModuleBaseUnpinnedTabAreaLeft);
            this.Controls.Add(this._ModuleBaseUnpinnedTabAreaTop);
            this.Controls.Add(this._ModuleBaseUnpinnedTabAreaBottom);
            this.Controls.Add(this._ModuleBaseUnpinnedTabAreaRight);
            this.Controls.Add(this.windowDockingArea2);
            this.Controls.Add(this.windowDockingArea1);
            this.HeaderVisible = true;
            this.Name = "Dashboard";
            this.Text = "Dashboard";
            this.Controls.SetChildIndex(this.windowDockingArea1, 0);
            this.Controls.SetChildIndex(this.windowDockingArea2, 0);
            this.Controls.SetChildIndex(this._ModuleBaseUnpinnedTabAreaRight, 0);
            this.Controls.SetChildIndex(this._ModuleBaseUnpinnedTabAreaBottom, 0);
            this.Controls.SetChildIndex(this._ModuleBaseUnpinnedTabAreaTop, 0);
            this.Controls.SetChildIndex(this._ModuleBaseUnpinnedTabAreaLeft, 0);
            this.Controls.SetChildIndex(this.panel1, 0);
            this.Controls.SetChildIndex(this._ModuleBaseAutoHideControl, 0);
            ((System.ComponentModel.ISupportInitialize)(this.ultraDockManager1)).EndInit();
            this.windowDockingArea1.ResumeLayout(false);
            this.dockableWindow1.ResumeLayout(false);
            this.windowDockingArea2.ResumeLayout(false);
            this.dockableWindow2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private Infragistics.Win.UltraWinDock.UltraDockManager ultraDockManager1;
        private Infragistics.Win.UltraWinDock.UnpinnedTabArea _ModuleBaseUnpinnedTabAreaLeft;
        private Infragistics.Win.UltraWinDock.UnpinnedTabArea _ModuleBaseUnpinnedTabAreaRight;
        private Infragistics.Win.UltraWinDock.UnpinnedTabArea _ModuleBaseUnpinnedTabAreaTop;
        private Infragistics.Win.UltraWinDock.UnpinnedTabArea _ModuleBaseUnpinnedTabAreaBottom;
        private Infragistics.Win.UltraWinDock.AutoHideControl _ModuleBaseAutoHideControl;
        private System.Windows.Forms.Panel panel2;
        private Infragistics.Win.UltraWinDock.WindowDockingArea windowDockingArea1;
        private Infragistics.Win.UltraWinDock.DockableWindow dockableWindow1;
        private System.Windows.Forms.Panel panel3;
        private Infragistics.Win.UltraWinDock.WindowDockingArea windowDockingArea2;
        private Infragistics.Win.UltraWinDock.DockableWindow dockableWindow2;
    }
}
