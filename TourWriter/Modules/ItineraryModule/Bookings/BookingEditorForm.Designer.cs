namespace TourWriter.Modules.ItineraryModule.Bookings
{
    partial class BookingEditorForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BookingEditorForm));
            this.bookingsNavigator = new System.Windows.Forms.BindingNavigator(this.components);
            this.bindingNavigatorCountItem = new System.Windows.Forms.ToolStripLabel();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.bindingNavigatorMoveFirstItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorMovePreviousItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.bindingNavigatorPositionItem = new System.Windows.Forms.ToolStripTextBox();
            this.bindingNavigatorSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.bindingNavigatorMoveNextItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorMoveLastItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.cmbBookings = new System.Windows.Forms.ToolStripComboBox();
            this.btnDeleteLine = new System.Windows.Forms.ToolStripButton();
            this.btnAddLine = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.btnCopy = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.btnSendBookings = new System.Windows.Forms.ToolStripSplitButton();
            this.btnBookThis = new System.Windows.Forms.ToolStripMenuItem();
            this.btnBookAll = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnClose = new System.Windows.Forms.Button();
            this.bookingsBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.bookingEditor11 = new TourWriter.Modules.ItineraryModule.Bookings.BookingEditor();
            ((System.ComponentModel.ISupportInitialize)(this.bookingsNavigator)).BeginInit();
            this.bookingsNavigator.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bookingsBindingSource)).BeginInit();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // bookingsNavigator
            // 
            this.bookingsNavigator.AddNewItem = null;
            this.bookingsNavigator.CountItem = this.bindingNavigatorCountItem;
            this.bookingsNavigator.DeleteItem = null;
            this.bookingsNavigator.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.bookingsNavigator.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.bindingNavigatorMoveFirstItem,
            this.bindingNavigatorMovePreviousItem,
            this.bindingNavigatorSeparator,
            this.bindingNavigatorPositionItem,
            this.bindingNavigatorCountItem,
            this.bindingNavigatorSeparator1,
            this.bindingNavigatorMoveNextItem,
            this.bindingNavigatorMoveLastItem,
            this.bindingNavigatorSeparator2,
            this.cmbBookings,
            this.btnDeleteLine,
            this.btnAddLine,
            this.toolStripSeparator2,
            this.btnCopy,
            this.toolStripSeparator3,
            this.btnSendBookings,
            this.toolStripSeparator1});
            this.bookingsNavigator.Location = new System.Drawing.Point(0, 0);
            this.bookingsNavigator.MoveFirstItem = this.bindingNavigatorMoveFirstItem;
            this.bookingsNavigator.MoveLastItem = this.bindingNavigatorMoveLastItem;
            this.bookingsNavigator.MoveNextItem = this.bindingNavigatorMoveNextItem;
            this.bookingsNavigator.MovePreviousItem = this.bindingNavigatorMovePreviousItem;
            this.bookingsNavigator.Name = "bookingsNavigator";
            this.bookingsNavigator.PositionItem = this.bindingNavigatorPositionItem;
            this.bookingsNavigator.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.bookingsNavigator.Size = new System.Drawing.Size(555, 25);
            this.bookingsNavigator.TabIndex = 0;
            this.bookingsNavigator.Text = "bindingNavigator1";
            // 
            // bindingNavigatorCountItem
            // 
            this.bindingNavigatorCountItem.Name = "bindingNavigatorCountItem";
            this.bindingNavigatorCountItem.Size = new System.Drawing.Size(35, 22);
            this.bindingNavigatorCountItem.Text = "of {0}";
            this.bindingNavigatorCountItem.ToolTipText = "Total number of items";
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(56, 22);
            this.toolStripLabel1.Text = "Bookings";
            // 
            // bindingNavigatorMoveFirstItem
            // 
            this.bindingNavigatorMoveFirstItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMoveFirstItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMoveFirstItem.Image")));
            this.bindingNavigatorMoveFirstItem.Name = "bindingNavigatorMoveFirstItem";
            this.bindingNavigatorMoveFirstItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMoveFirstItem.Size = new System.Drawing.Size(23, 22);
            this.bindingNavigatorMoveFirstItem.Text = "Move first";
            // 
            // bindingNavigatorMovePreviousItem
            // 
            this.bindingNavigatorMovePreviousItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMovePreviousItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMovePreviousItem.Image")));
            this.bindingNavigatorMovePreviousItem.Name = "bindingNavigatorMovePreviousItem";
            this.bindingNavigatorMovePreviousItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMovePreviousItem.Size = new System.Drawing.Size(23, 22);
            this.bindingNavigatorMovePreviousItem.Text = "Move previous";
            // 
            // bindingNavigatorSeparator
            // 
            this.bindingNavigatorSeparator.Name = "bindingNavigatorSeparator";
            this.bindingNavigatorSeparator.Size = new System.Drawing.Size(6, 25);
            // 
            // bindingNavigatorPositionItem
            // 
            this.bindingNavigatorPositionItem.AccessibleName = "Position";
            this.bindingNavigatorPositionItem.AutoSize = false;
            this.bindingNavigatorPositionItem.Name = "bindingNavigatorPositionItem";
            this.bindingNavigatorPositionItem.Size = new System.Drawing.Size(30, 21);
            this.bindingNavigatorPositionItem.Text = "0";
            this.bindingNavigatorPositionItem.ToolTipText = "Current position";
            // 
            // bindingNavigatorSeparator1
            // 
            this.bindingNavigatorSeparator1.Name = "bindingNavigatorSeparator1";
            this.bindingNavigatorSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // bindingNavigatorMoveNextItem
            // 
            this.bindingNavigatorMoveNextItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMoveNextItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMoveNextItem.Image")));
            this.bindingNavigatorMoveNextItem.Name = "bindingNavigatorMoveNextItem";
            this.bindingNavigatorMoveNextItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMoveNextItem.Size = new System.Drawing.Size(23, 22);
            this.bindingNavigatorMoveNextItem.Text = "Move next";
            // 
            // bindingNavigatorMoveLastItem
            // 
            this.bindingNavigatorMoveLastItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMoveLastItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMoveLastItem.Image")));
            this.bindingNavigatorMoveLastItem.Name = "bindingNavigatorMoveLastItem";
            this.bindingNavigatorMoveLastItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMoveLastItem.Size = new System.Drawing.Size(23, 22);
            this.bindingNavigatorMoveLastItem.Text = "Move last";
            // 
            // bindingNavigatorSeparator2
            // 
            this.bindingNavigatorSeparator2.Name = "bindingNavigatorSeparator2";
            this.bindingNavigatorSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // cmbBookings
            // 
            this.cmbBookings.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBookings.Name = "cmbBookings";
            this.cmbBookings.Size = new System.Drawing.Size(150, 25);
            // 
            // btnDeleteLine
            // 
            this.btnDeleteLine.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.btnDeleteLine.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnDeleteLine.Image = ((System.Drawing.Image)(resources.GetObject("btnDeleteLine.Image")));
            this.btnDeleteLine.Name = "btnDeleteLine";
            this.btnDeleteLine.RightToLeftAutoMirrorImage = true;
            this.btnDeleteLine.Size = new System.Drawing.Size(23, 22);
            this.btnDeleteLine.Text = "Delete";
            this.btnDeleteLine.ToolTipText = "Delete this booking";
            this.btnDeleteLine.Click += new System.EventHandler(this.btnDeleteLine_Click);
            // 
            // btnAddLine
            // 
            this.btnAddLine.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.btnAddLine.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnAddLine.Image = ((System.Drawing.Image)(resources.GetObject("btnAddLine.Image")));
            this.btnAddLine.Name = "btnAddLine";
            this.btnAddLine.RightToLeftAutoMirrorImage = true;
            this.btnAddLine.Size = new System.Drawing.Size(23, 22);
            this.btnAddLine.Text = "Add new";
            this.btnAddLine.ToolTipText = "Add new booking";
            this.btnAddLine.Click += new System.EventHandler(this.btnAddLine_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // btnCopy
            // 
            this.btnCopy.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.btnCopy.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnCopy.Image = global::TourWriter.Properties.Resources.PageCopy;
            this.btnCopy.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(23, 22);
            this.btnCopy.Text = "toolStripButton1";
            this.btnCopy.ToolTipText = "Copy this booking and related items";
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // btnSendBookings
            // 
            this.btnSendBookings.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.btnSendBookings.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnSendBookings.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnBookThis,
            this.btnBookAll});
            this.btnSendBookings.Image = ((System.Drawing.Image)(resources.GetObject("btnSendBookings.Image")));
            this.btnSendBookings.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSendBookings.Name = "btnSendBookings";
            this.btnSendBookings.Size = new System.Drawing.Size(99, 19);
            this.btnSendBookings.Text = "Email booking";
            this.btnSendBookings.ToolTipText = "Send booking request";
            this.btnSendBookings.ButtonClick += new System.EventHandler(this.btnSendBookings_ButtonClick);
            // 
            // btnBookThis
            // 
            this.btnBookThis.Name = "btnBookThis";
            this.btnBookThis.Size = new System.Drawing.Size(132, 22);
            this.btnBookThis.Text = "Book this...";
            this.btnBookThis.Click += new System.EventHandler(this.btnBookThis_Click);
            // 
            // btnBookAll
            // 
            this.btnBookAll.Name = "btnBookAll";
            this.btnBookAll.Size = new System.Drawing.Size(132, 22);
            this.btnBookAll.Text = "Book all...";
            this.btnBookAll.Click += new System.EventHandler(this.btnBookAll_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Location = new System.Drawing.Point(473, 5);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 3;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.btnClose);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 480);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(555, 35);
            this.panel2.TabIndex = 5;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.bookingEditor11);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 25);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(4);
            this.panel1.Size = new System.Drawing.Size(555, 455);
            this.panel1.TabIndex = 6;
            // 
            // bookingEditor11
            // 
            this.bookingEditor11.BackColor = System.Drawing.Color.WhiteSmoke;
            this.bookingEditor11.BindingSource = null;
            this.bookingEditor11.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.bookingEditor11.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bookingEditor11.ItineraryMain = null;
            this.bookingEditor11.Location = new System.Drawing.Point(4, 4);
            this.bookingEditor11.MinimumSize = new System.Drawing.Size(452, 450);
            this.bookingEditor11.Name = "bookingEditor11";
            this.bookingEditor11.Size = new System.Drawing.Size(547, 450);
            this.bookingEditor11.TabIndex = 1;
            // 
            // BookingEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(555, 515);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.bookingsNavigator);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(563, 549);
            this.Name = "BookingEditorForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "TourWriter bookings";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.BookingsManager_FormClosing);
            this.Load += new System.EventHandler(this.BookingEditorForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.bookingsNavigator)).EndInit();
            this.bookingsNavigator.ResumeLayout(false);
            this.bookingsNavigator.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bookingsBindingSource)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.BindingNavigator bookingsNavigator;
        private System.Windows.Forms.ToolStripButton btnAddLine;
        private System.Windows.Forms.ToolStripLabel bindingNavigatorCountItem;
        private System.Windows.Forms.ToolStripButton btnDeleteLine;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMoveFirstItem;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMovePreviousItem;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator;
        private System.Windows.Forms.ToolStripTextBox bindingNavigatorPositionItem;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator1;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMoveNextItem;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMoveLastItem;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator2;
        private System.Windows.Forms.ToolStripComboBox cmbBookings;
        private System.Windows.Forms.ToolStripSplitButton btnSendBookings;
        private System.Windows.Forms.ToolStripMenuItem btnBookThis;
        private System.Windows.Forms.ToolStripMenuItem btnBookAll;
        private System.Windows.Forms.BindingSource bookingsBindingSource;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel1;
        private BookingEditor bookingEditor11;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton btnCopy;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
    }
}