namespace StartGame
{
    partial class PlayerView
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
            this.mainPlayerStrip = new System.Windows.Forms.MenuStrip();
            this.baseStatsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.itemsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.spellsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.weaponsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.skillsAndTitlesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mainPlayerStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainPlayerStrip
            // 
            this.mainPlayerStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.baseStatsToolStripMenuItem,
            this.itemsToolStripMenuItem,
            this.statusesToolStripMenuItem,
            this.spellsToolStripMenuItem,
            this.weaponsToolStripMenuItem,
            this.skillsAndTitlesToolStripMenuItem});
            this.mainPlayerStrip.Location = new System.Drawing.Point(0, 0);
            this.mainPlayerStrip.Name = "mainPlayerStrip";
            this.mainPlayerStrip.Size = new System.Drawing.Size(408, 24);
            this.mainPlayerStrip.TabIndex = 43;
            this.mainPlayerStrip.Text = "menuStrip1";
            this.mainPlayerStrip.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.MainPlayerStrip_ItemClicked);
            // 
            // baseStatsToolStripMenuItem
            // 
            this.baseStatsToolStripMenuItem.Name = "baseStatsToolStripMenuItem";
            this.baseStatsToolStripMenuItem.Size = new System.Drawing.Size(71, 20);
            this.baseStatsToolStripMenuItem.Text = "Base Stats";
            this.baseStatsToolStripMenuItem.Click += new System.EventHandler(this.BaseStatsToolStripMenuItem_Click);
            // 
            // itemsToolStripMenuItem
            // 
            this.itemsToolStripMenuItem.Name = "itemsToolStripMenuItem";
            this.itemsToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.itemsToolStripMenuItem.Text = "Items";
            this.itemsToolStripMenuItem.Click += new System.EventHandler(this.ItemsToolStripMenuItem_Click);
            // 
            // statusesToolStripMenuItem
            // 
            this.statusesToolStripMenuItem.Name = "statusesToolStripMenuItem";
            this.statusesToolStripMenuItem.Size = new System.Drawing.Size(62, 20);
            this.statusesToolStripMenuItem.Text = "Statuses";
            this.statusesToolStripMenuItem.Click += new System.EventHandler(this.StatusesToolStripMenuItem_Click);
            // 
            // spellsToolStripMenuItem
            // 
            this.spellsToolStripMenuItem.Name = "spellsToolStripMenuItem";
            this.spellsToolStripMenuItem.Size = new System.Drawing.Size(49, 20);
            this.spellsToolStripMenuItem.Text = "Spells";
            this.spellsToolStripMenuItem.Click += new System.EventHandler(this.SpellsToolStripMenuItem_Click);
            // 
            // weaponsToolStripMenuItem
            // 
            this.weaponsToolStripMenuItem.Name = "weaponsToolStripMenuItem";
            this.weaponsToolStripMenuItem.Size = new System.Drawing.Size(68, 20);
            this.weaponsToolStripMenuItem.Text = "Weapons";
            this.weaponsToolStripMenuItem.Click += new System.EventHandler(this.WeaponsToolStripMenuItem_Click);
            // 
            // skillsAndTitlesToolStripMenuItem
            // 
            this.skillsAndTitlesToolStripMenuItem.Name = "skillsAndTitlesToolStripMenuItem";
            this.skillsAndTitlesToolStripMenuItem.Size = new System.Drawing.Size(99, 20);
            this.skillsAndTitlesToolStripMenuItem.Text = "Skills and Titles";
            this.skillsAndTitlesToolStripMenuItem.Click += new System.EventHandler(this.SkillsAndTitlesToolStripMenuItem_Click);
            // 
            // PlayerView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.Controls.Add(this.mainPlayerStrip);
            this.Name = "PlayerView";
            this.Size = new System.Drawing.Size(408, 350);
            this.mainPlayerStrip.ResumeLayout(false);
            this.mainPlayerStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.MenuStrip mainPlayerStrip;
        private System.Windows.Forms.ToolStripMenuItem baseStatsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem itemsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem statusesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem spellsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem weaponsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem skillsAndTitlesToolStripMenuItem;
    }
}
