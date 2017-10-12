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
            this.gameBoard = new System.Windows.Forms.PictureBox();
            this.troopList = new System.Windows.Forms.ListBox();
            this.enemyInfo = new System.Windows.Forms.Panel();
            this.playerName = new System.Windows.Forms.Label();
            this.playerInfo = new System.Windows.Forms.Panel();
            this.enemyName = new System.Windows.Forms.Label();
            this.spellList = new System.Windows.Forms.ListBox();
            this.spellInfo = new System.Windows.Forms.Panel();
            this.nextAction = new System.Windows.Forms.Button();
            this.enemyAttackDamage = new System.Windows.Forms.Label();
            this.playerAttackDamage = new System.Windows.Forms.Label();
            this.enemyAttackRange = new System.Windows.Forms.Label();
            this.playerAttackRange = new System.Windows.Forms.Label();
            this.enemyAttackType = new System.Windows.Forms.Label();
            this.playerAttackType = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.gameBoard)).BeginInit();
            this.enemyInfo.SuspendLayout();
            this.playerInfo.SuspendLayout();
            this.SuspendLayout();
            // 
            // gameBoard
            // 
            this.gameBoard.Location = new System.Drawing.Point(13, 13);
            this.gameBoard.Name = "gameBoard";
            this.gameBoard.Size = new System.Drawing.Size(620, 620);
            this.gameBoard.TabIndex = 1;
            this.gameBoard.TabStop = false;
            // 
            // troopList
            // 
            this.troopList.FormattingEnabled = true;
            this.troopList.Location = new System.Drawing.Point(641, 13);
            this.troopList.Name = "troopList";
            this.troopList.Size = new System.Drawing.Size(178, 212);
            this.troopList.TabIndex = 2;
            this.troopList.SelectedIndexChanged += new System.EventHandler(this.PlayerList_SelectedIndexChanged);
            // 
            // enemyInfo
            // 
            this.enemyInfo.Controls.Add(this.playerAttackType);
            this.enemyInfo.Controls.Add(this.playerAttackRange);
            this.enemyInfo.Controls.Add(this.playerAttackDamage);
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
            // playerInfo
            // 
            this.playerInfo.Controls.Add(this.enemyAttackType);
            this.playerInfo.Controls.Add(this.enemyAttackRange);
            this.playerInfo.Controls.Add(this.enemyAttackDamage);
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
            // enemyAttackDamage
            // 
            this.enemyAttackDamage.AutoSize = true;
            this.enemyAttackDamage.Location = new System.Drawing.Point(6, 18);
            this.enemyAttackDamage.Name = "enemyAttackDamage";
            this.enemyAttackDamage.Size = new System.Drawing.Size(77, 13);
            this.enemyAttackDamage.TabIndex = 2;
            this.enemyAttackDamage.Text = "attackDamage";
            // 
            // playerAttackDamage
            // 
            this.playerAttackDamage.AutoSize = true;
            this.playerAttackDamage.Location = new System.Drawing.Point(4, 17);
            this.playerAttackDamage.Name = "playerAttackDamage";
            this.playerAttackDamage.Size = new System.Drawing.Size(77, 13);
            this.playerAttackDamage.TabIndex = 3;
            this.playerAttackDamage.Text = "attackDamage";
            // 
            // enemyAttackRange
            // 
            this.enemyAttackRange.AutoSize = true;
            this.enemyAttackRange.Location = new System.Drawing.Point(9, 35);
            this.enemyAttackRange.Name = "enemyAttackRange";
            this.enemyAttackRange.Size = new System.Drawing.Size(101, 13);
            this.enemyAttackRange.TabIndex = 3;
            this.enemyAttackRange.Text = "enemyAttackRange";
            // 
            // playerAttackRange
            // 
            this.playerAttackRange.AutoSize = true;
            this.playerAttackRange.Location = new System.Drawing.Point(4, 30);
            this.playerAttackRange.Name = "playerAttackRange";
            this.playerAttackRange.Size = new System.Drawing.Size(101, 13);
            this.playerAttackRange.TabIndex = 4;
            this.playerAttackRange.Text = "playerAtttackRange";
            // 
            // enemyAttackType
            // 
            this.enemyAttackType.AutoSize = true;
            this.enemyAttackType.Location = new System.Drawing.Point(9, 48);
            this.enemyAttackType.Name = "enemyAttackType";
            this.enemyAttackType.Size = new System.Drawing.Size(93, 13);
            this.enemyAttackType.TabIndex = 10;
            this.enemyAttackType.Text = "enemyAttackType";
            // 
            // playerAttackType
            // 
            this.playerAttackType.AutoSize = true;
            this.playerAttackType.Location = new System.Drawing.Point(4, 43);
            this.playerAttackType.Name = "playerAttackType";
            this.playerAttackType.Size = new System.Drawing.Size(90, 13);
            this.playerAttackType.TabIndex = 5;
            this.playerAttackType.Text = "playerAttackType";
            // 
            // MainGameWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1241, 639);
            this.Controls.Add(this.nextAction);
            this.Controls.Add(this.spellInfo);
            this.Controls.Add(this.spellList);
            this.Controls.Add(this.playerInfo);
            this.Controls.Add(this.enemyInfo);
            this.Controls.Add(this.troopList);
            this.Controls.Add(this.gameBoard);
            this.Name = "MainGameWindow";
            this.Text = "Start Game";
            this.Load += new System.EventHandler(this.MainGameWindow_Load);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MainGameWindow_MouseMove);
            ((System.ComponentModel.ISupportInitialize)(this.gameBoard)).EndInit();
            this.enemyInfo.ResumeLayout(false);
            this.enemyInfo.PerformLayout();
            this.playerInfo.ResumeLayout(false);
            this.playerInfo.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.PictureBox gameBoard;
        private System.Windows.Forms.ListBox troopList;
        private System.Windows.Forms.Panel enemyInfo;
        private System.Windows.Forms.Panel playerInfo;
        private System.Windows.Forms.ListBox spellList;
        private System.Windows.Forms.Panel spellInfo;
        private System.Windows.Forms.Label playerName;
        private System.Windows.Forms.Button nextAction;
        private System.Windows.Forms.Label enemyName;
        private System.Windows.Forms.Label playerAttackType;
        private System.Windows.Forms.Label playerAttackRange;
        private System.Windows.Forms.Label playerAttackDamage;
        private System.Windows.Forms.Label enemyAttackType;
        private System.Windows.Forms.Label enemyAttackRange;
        private System.Windows.Forms.Label enemyAttackDamage;
    }
}

