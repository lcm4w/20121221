namespace TourWriter.Modules.ItineraryModule.RoomTypes
{
    partial class RoomTypesHost
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
            this.roomTypesControl1 = new TourWriter.Modules.ItineraryModule.RoomTypes.RoomTypesControl();
            this.SuspendLayout();
            // 
            // roomTypesControl1
            // 
            this.roomTypesControl1.Location = new System.Drawing.Point(13, 13);
            this.roomTypesControl1.Name = "roomTypesControl1";
            this.roomTypesControl1.Size = new System.Drawing.Size(1036, 506);
            this.roomTypesControl1.TabIndex = 0;
            // 
            // RoomTypesHost
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1061, 523);
            this.Controls.Add(this.roomTypesControl1);
            this.Name = "RoomTypesHost";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Room Types";
            this.ResumeLayout(false);

        }

        #endregion

        private RoomTypesControl roomTypesControl1;
    }
}