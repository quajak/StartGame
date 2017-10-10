namespace StartGame
{
    partial class TroopViewer
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
            this.troopList = new System.Windows.Forms.ListBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.formViewer = new System.Windows.Forms.PictureBox();
            this.name = new System.Windows.Forms.Label();
            this.troopType = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.formViewer)).BeginInit();
            this.SuspendLayout();
            // 
            // troopList
            // 
            this.troopList.FormattingEnabled = true;
            this.troopList.Location = new System.Drawing.Point(12, 12);
            this.troopList.Name = "troopList";
            this.troopList.Size = new System.Drawing.Size(120, 238);
            this.troopList.TabIndex = 0;
            this.troopList.SelectedIndexChanged += new System.EventHandler(this.TroopList_SelectedIndexChanged);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.troopType);
            this.panel1.Controls.Add(this.formViewer);
            this.panel1.Controls.Add(this.name);
            this.panel1.Location = new System.Drawing.Point(139, 13);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(297, 237);
            this.panel1.TabIndex = 1;
            // 
            // formViewer
            // 
            this.formViewer.Location = new System.Drawing.Point(190, 16);
            this.formViewer.Name = "formViewer";
            this.formViewer.Size = new System.Drawing.Size(90, 90);
            this.formViewer.TabIndex = 1;
            this.formViewer.TabStop = false;
            // 
            // name
            // 
            this.name.AutoSize = true;
            this.name.Location = new System.Drawing.Point(3, 16);
            this.name.Name = "name";
            this.name.Size = new System.Drawing.Size(39, 13);
            this.name.TabIndex = 0;
            this.name.Text = "_name";
            // 
            // troopType
            // 
            this.troopType.AutoSize = true;
            this.troopType.Location = new System.Drawing.Point(4, 33);
            this.troopType.Name = "troopType";
            this.troopType.Size = new System.Drawing.Size(33, 13);
            this.troopType.TabIndex = 2;
            this.troopType.Text = "_type";
            // 
            // TroopViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(448, 260);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.troopList);
            this.Name = "TroopViewer";
            this.Text = "TroopViewer";
            this.Load += new System.EventHandler(this.TroopViewer_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.formViewer)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox troopList;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox formViewer;
        private System.Windows.Forms.Label name;
        private System.Windows.Forms.Label troopType;
    }
}