namespace StartGame.World
{
    partial class MissionResult
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
            this.nextMission = new System.Windows.Forms.Button();
            this.levelUpButton = new System.Windows.Forms.Button();
            this.lootList = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.playerView = new StartGame.PlayerView();
            this.gainLoot = new System.Windows.Forms.Button();
            this.gainAllLoot = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // nextMission
            // 
            this.nextMission.Location = new System.Drawing.Point(478, 379);
            this.nextMission.Name = "nextMission";
            this.nextMission.Size = new System.Drawing.Size(81, 38);
            this.nextMission.TabIndex = 0;
            this.nextMission.Text = "Next Mission";
            this.nextMission.UseVisualStyleBackColor = true;
            this.nextMission.Click += new System.EventHandler(this.NextMission_Click);
            // 
            // levelUpButton
            // 
            this.levelUpButton.Location = new System.Drawing.Point(12, 12);
            this.levelUpButton.Name = "levelUpButton";
            this.levelUpButton.Size = new System.Drawing.Size(120, 23);
            this.levelUpButton.TabIndex = 1;
            this.levelUpButton.Text = "Level Up";
            this.levelUpButton.UseVisualStyleBackColor = true;
            this.levelUpButton.Click += new System.EventHandler(this.LevelUp_Click);
            // 
            // lootList
            // 
            this.lootList.FormattingEnabled = true;
            this.lootList.Location = new System.Drawing.Point(12, 58);
            this.lootList.Name = "lootList";
            this.lootList.Size = new System.Drawing.Size(120, 95);
            this.lootList.TabIndex = 2;
            this.lootList.SelectedIndexChanged += new System.EventHandler(this.LootList_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(65, 39);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(28, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Loot";
            // 
            // playerView
            // 
            this.playerView.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.playerView.Location = new System.Drawing.Point(151, 12);
            this.playerView.Name = "playerView";
            this.playerView.Size = new System.Drawing.Size(408, 350);
            this.playerView.TabIndex = 10;
            // 
            // gainLoot
            // 
            this.gainLoot.Location = new System.Drawing.Point(12, 160);
            this.gainLoot.Name = "gainLoot";
            this.gainLoot.Size = new System.Drawing.Size(120, 23);
            this.gainLoot.TabIndex = 4;
            this.gainLoot.Text = "Gain Loot";
            this.gainLoot.UseVisualStyleBackColor = true;
            this.gainLoot.Click += new System.EventHandler(this.GainLoot_Click);
            // 
            // gainAllLoot
            // 
            this.gainAllLoot.Location = new System.Drawing.Point(12, 190);
            this.gainAllLoot.Name = "gainAllLoot";
            this.gainAllLoot.Size = new System.Drawing.Size(120, 23);
            this.gainAllLoot.TabIndex = 11;
            this.gainAllLoot.Text = "Gain All Loot";
            this.gainAllLoot.UseVisualStyleBackColor = true;
            this.gainAllLoot.Click += new System.EventHandler(this.GainAllLoot_Click);
            // 
            // MissionResult
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(589, 429);
            this.Controls.Add(this.gainAllLoot);
            this.Controls.Add(this.playerView);
            this.Controls.Add(this.gainLoot);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lootList);
            this.Controls.Add(this.levelUpButton);
            this.Controls.Add(this.nextMission);
            this.Name = "MissionResult";
            this.Text = "Mission Result";
            this.Load += new System.EventHandler(this.WorldView_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button nextMission;
        private System.Windows.Forms.Button levelUpButton;
        private System.Windows.Forms.ListBox lootList;
        private System.Windows.Forms.Label label1;
        private PlayerView playerView;
        private System.Windows.Forms.Button gainLoot;
        private System.Windows.Forms.Button gainAllLoot;
    }
}