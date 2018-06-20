namespace StartGame.User_Controls
{
    partial class PlayerStatusView
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
            this.statusDescription = new System.Windows.Forms.Label();
            this.statusTitle = new System.Windows.Forms.Label();
            this.statusList = new System.Windows.Forms.ListBox();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // statusDescription
            // 
            this.statusDescription.AutoSize = true;
            this.statusDescription.Location = new System.Drawing.Point(110, 49);
            this.statusDescription.MaximumSize = new System.Drawing.Size(190, 100);
            this.statusDescription.Name = "statusDescription";
            this.statusDescription.Size = new System.Drawing.Size(88, 13);
            this.statusDescription.TabIndex = 7;
            this.statusDescription.Text = "statusDescription";
            // 
            // statusTitle
            // 
            this.statusTitle.AutoSize = true;
            this.statusTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.statusTitle.Location = new System.Drawing.Point(107, 32);
            this.statusTitle.Name = "statusTitle";
            this.statusTitle.Size = new System.Drawing.Size(66, 13);
            this.statusTitle.TabIndex = 6;
            this.statusTitle.Text = "statusTitle";
            this.statusTitle.Click += new System.EventHandler(this.StatusTitle_Click);
            // 
            // statusList
            // 
            this.statusList.FormattingEnabled = true;
            this.statusList.Location = new System.Drawing.Point(3, 32);
            this.statusList.Name = "statusList";
            this.statusList.Size = new System.Drawing.Size(98, 199);
            this.statusList.TabIndex = 5;
            this.statusList.SelectedIndexChanged += new System.EventHandler(this.StatusList_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.label3.Location = new System.Drawing.Point(95, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(73, 20);
            this.label3.TabIndex = 4;
            this.label3.Text = "Statuses";
            // 
            // PlayerStatusView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.Controls.Add(this.statusDescription);
            this.Controls.Add(this.statusTitle);
            this.Controls.Add(this.statusList);
            this.Controls.Add(this.label3);
            this.Name = "PlayerStatusView";
            this.Size = new System.Drawing.Size(300, 250);
            this.Load += new System.EventHandler(this.PlayerStatusView_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label statusDescription;
        private System.Windows.Forms.Label statusTitle;
        private System.Windows.Forms.ListBox statusList;
        private System.Windows.Forms.Label label3;
    }
}
