namespace StartGame
{
    partial class MainGameWindow
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
            this.troopCreator = new System.Windows.Forms.PictureBox();
            this.gameBoard = new System.Windows.Forms.PictureBox();
            this.playerList = new System.Windows.Forms.ListBox();
            this.enemyInfo = new System.Windows.Forms.Panel();
            this.playerName = new System.Windows.Forms.Label();
            this.spawnTroop = new System.Windows.Forms.Button();
            this.playerInfo = new System.Windows.Forms.Panel();
            this.enemyName = new System.Windows.Forms.Label();
            this.spellList = new System.Windows.Forms.ListBox();
            this.activateSpell = new System.Windows.Forms.Button();
            this.spellInfo = new System.Windows.Forms.Panel();
            this.nextAction = new System.Windows.Forms.Button();
            this.showTroopTypes = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.troopCreator)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gameBoard)).BeginInit();
            this.enemyInfo.SuspendLayout();
            this.playerInfo.SuspendLayout();
            this.SuspendLayout();
            // 
            // troopCreator
            // 
            this.troopCreator.Location = new System.Drawing.Point(823, 231);
            this.troopCreator.Name = "troopCreator";
            this.troopCreator.Size = new System.Drawing.Size(202, 180);
            this.troopCreator.TabIndex = 0;
            this.troopCreator.TabStop = false;
            this.troopCreator.Click += new System.EventHandler(this.TroopCreator_Click);
            this.troopCreator.MouseClick += new System.Windows.Forms.MouseEventHandler(this.TroopCreator_MouseClick);
            // 
            // gameBoard
            // 
            this.gameBoard.Location = new System.Drawing.Point(13, 13);
            this.gameBoard.Name = "gameBoard";
            this.gameBoard.Size = new System.Drawing.Size(620, 620);
            this.gameBoard.TabIndex = 1;
            this.gameBoard.TabStop = false;
            // 
            // playerList
            // 
            this.playerList.FormattingEnabled = true;
            this.playerList.Location = new System.Drawing.Point(641, 13);
            this.playerList.Name = "playerList";
            this.playerList.Size = new System.Drawing.Size(178, 212);
            this.playerList.TabIndex = 2;
            this.playerList.SelectedIndexChanged += new System.EventHandler(this.PlayerList_SelectedIndexChanged);
            // 
            // enemyInfo
            // 
            this.enemyInfo.Controls.Add(this.playerName);
            this.enemyInfo.Location = new System.Drawing.Point(824, 13);
            this.enemyInfo.Name = "enemyInfo";
            this.enemyInfo.Size = new System.Drawing.Size(202, 212);
            this.enemyInfo.TabIndex = 3;
            // 
            // playerName
            // 
            this.playerName.AutoSize = true;
            this.playerName.Location = new System.Drawing.Point(4, 4);
            this.playerName.Name = "playerName";
            this.playerName.Size = new System.Drawing.Size(69, 13);
            this.playerName.TabIndex = 0;
            this.playerName.Text = "_playerName";
            // 
            // spawnTroop
            // 
            this.spawnTroop.Enabled = false;
            this.spawnTroop.Location = new System.Drawing.Point(825, 419);
            this.spawnTroop.Name = "spawnTroop";
            this.spawnTroop.Size = new System.Drawing.Size(201, 23);
            this.spawnTroop.TabIndex = 4;
            this.spawnTroop.Text = "Spawn Troop";
            this.spawnTroop.UseVisualStyleBackColor = true;
            this.spawnTroop.Click += new System.EventHandler(this.SpawnTroop_Click);
            this.spawnTroop.MouseClick += new System.Windows.Forms.MouseEventHandler(this.SpawnTroop_MouseClick);
            // 
            // playerInfo
            // 
            this.playerInfo.Controls.Add(this.enemyName);
            this.playerInfo.Location = new System.Drawing.Point(641, 231);
            this.playerInfo.Name = "playerInfo";
            this.playerInfo.Size = new System.Drawing.Size(178, 182);
            this.playerInfo.TabIndex = 6;
            // 
            // enemyName
            // 
            this.enemyName.AutoSize = true;
            this.enemyName.Location = new System.Drawing.Point(3, 1);
            this.enemyName.Name = "enemyName";
            this.enemyName.Size = new System.Drawing.Size(72, 13);
            this.enemyName.TabIndex = 1;
            this.enemyName.Text = "_enemyName";
            // 
            // spellList
            // 
            this.spellList.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.spellList.FormattingEnabled = true;
            this.spellList.Location = new System.Drawing.Point(1030, 13);
            this.spellList.Name = "spellList";
            this.spellList.Size = new System.Drawing.Size(202, 212);
            this.spellList.TabIndex = 7;
            // 
            // activateSpell
            // 
            this.activateSpell.Enabled = false;
            this.activateSpell.Location = new System.Drawing.Point(1030, 419);
            this.activateSpell.Name = "activateSpell";
            this.activateSpell.Size = new System.Drawing.Size(202, 23);
            this.activateSpell.TabIndex = 8;
            this.activateSpell.Text = "Cast Spell";
            this.activateSpell.UseVisualStyleBackColor = true;
            // 
            // spellInfo
            // 
            this.spellInfo.Location = new System.Drawing.Point(1030, 232);
            this.spellInfo.Name = "spellInfo";
            this.spellInfo.Size = new System.Drawing.Size(202, 182);
            this.spellInfo.TabIndex = 7;
            // 
            // nextAction
            // 
            this.nextAction.Location = new System.Drawing.Point(1142, 571);
            this.nextAction.Name = "nextAction";
            this.nextAction.Size = new System.Drawing.Size(90, 56);
            this.nextAction.TabIndex = 9;
            this.nextAction.Text = "Next Action";
            this.nextAction.UseVisualStyleBackColor = true;
            this.nextAction.Click += new System.EventHandler(this.NextAction_Click);
            // 
            // showTroopTypes
            // 
            this.showTroopTypes.Location = new System.Drawing.Point(825, 448);
            this.showTroopTypes.Name = "showTroopTypes";
            this.showTroopTypes.Size = new System.Drawing.Size(201, 23);
            this.showTroopTypes.TabIndex = 10;
            this.showTroopTypes.Text = "Troop Types";
            this.showTroopTypes.UseVisualStyleBackColor = true;
            this.showTroopTypes.Click += new System.EventHandler(this.ShowTroopTypes_Click);
            // 
            // MainGameWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1241, 639);
            this.Controls.Add(this.showTroopTypes);
            this.Controls.Add(this.nextAction);
            this.Controls.Add(this.spellInfo);
            this.Controls.Add(this.activateSpell);
            this.Controls.Add(this.spellList);
            this.Controls.Add(this.playerInfo);
            this.Controls.Add(this.spawnTroop);
            this.Controls.Add(this.enemyInfo);
            this.Controls.Add(this.playerList);
            this.Controls.Add(this.gameBoard);
            this.Controls.Add(this.troopCreator);
            this.Name = "MainGameWindow";
            this.Text = "Start Game";
            this.Load += new System.EventHandler(this.MainGameWindow_Load);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MainGameWindow_MouseMove);
            ((System.ComponentModel.ISupportInitialize)(this.troopCreator)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gameBoard)).EndInit();
            this.enemyInfo.ResumeLayout(false);
            this.enemyInfo.PerformLayout();
            this.playerInfo.ResumeLayout(false);
            this.playerInfo.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox troopCreator;
        private System.Windows.Forms.PictureBox gameBoard;
        private System.Windows.Forms.ListBox playerList;
        private System.Windows.Forms.Panel enemyInfo;
        private System.Windows.Forms.Button spawnTroop;
        private System.Windows.Forms.Panel playerInfo;
        private System.Windows.Forms.ListBox spellList;
        private System.Windows.Forms.Button activateSpell;
        private System.Windows.Forms.Panel spellInfo;
        private System.Windows.Forms.Label playerName;
        private System.Windows.Forms.Button nextAction;
        private System.Windows.Forms.Button showTroopTypes;
        private System.Windows.Forms.Label enemyName;
    }
}

