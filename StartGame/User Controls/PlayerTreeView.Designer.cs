namespace StartGame.User_Controls
{
    partial class PlayerTreeView
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
            this.skillLevel = new System.Windows.Forms.Label();
            this.treeInformation = new System.Windows.Forms.Label();
            this.treeName = new System.Windows.Forms.Label();
            this.treeList = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // skillLevel
            // 
            this.skillLevel.AutoSize = true;
            this.skillLevel.Font = new System.Drawing.Font("Modern No. 20", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.skillLevel.Location = new System.Drawing.Point(6, 222);
            this.skillLevel.Name = "skillLevel";
            this.skillLevel.Size = new System.Drawing.Size(63, 16);
            this.skillLevel.TabIndex = 9;
            this.skillLevel.Text = "skillLevel";
            // 
            // treeInformation
            // 
            this.treeInformation.AutoSize = true;
            this.treeInformation.Font = new System.Drawing.Font("Modern No. 20", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.treeInformation.Location = new System.Drawing.Point(6, 122);
            this.treeInformation.MaximumSize = new System.Drawing.Size(180, 100);
            this.treeInformation.Name = "treeInformation";
            this.treeInformation.Size = new System.Drawing.Size(85, 15);
            this.treeInformation.TabIndex = 8;
            this.treeInformation.Text = "treeInformation";
            // 
            // treeName
            // 
            this.treeName.AutoSize = true;
            this.treeName.Font = new System.Drawing.Font("Modern No. 20", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.treeName.Location = new System.Drawing.Point(6, 98);
            this.treeName.Name = "treeName";
            this.treeName.Size = new System.Drawing.Size(62, 16);
            this.treeName.TabIndex = 7;
            this.treeName.Text = "treeName";
            // 
            // treeList
            // 
            this.treeList.Font = new System.Drawing.Font("Modern No. 20", 8.249999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.treeList.FormattingEnabled = true;
            this.treeList.ItemHeight = 14;
            this.treeList.Location = new System.Drawing.Point(3, 35);
            this.treeList.Name = "treeList";
            this.treeList.Size = new System.Drawing.Size(274, 60);
            this.treeList.TabIndex = 6;
            this.treeList.SelectedIndexChanged += new System.EventHandler(this.TreeList_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Modern No. 20", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(65, 8);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(157, 24);
            this.label2.TabIndex = 5;
            this.label2.Text = "Skills and Titles";
            // 
            // PlayerTreeView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.Controls.Add(this.skillLevel);
            this.Controls.Add(this.treeInformation);
            this.Controls.Add(this.treeName);
            this.Controls.Add(this.treeList);
            this.Controls.Add(this.label2);
            this.Name = "PlayerTreeView";
            this.Size = new System.Drawing.Size(300, 250);
            this.Load += new System.EventHandler(this.PlayerTreeView_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label skillLevel;
        private System.Windows.Forms.Label treeInformation;
        private System.Windows.Forms.Label treeName;
        private System.Windows.Forms.ListBox treeList;
        private System.Windows.Forms.Label label2;
    }
}
