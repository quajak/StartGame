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
            this.playerInfo = new System.Windows.Forms.Panel();
            this.enemyDefense = new System.Windows.Forms.Label();
            this.enemyHeight = new System.Windows.Forms.Label();
            this.enemyPosition = new System.Windows.Forms.Label();
            this.enemyHealth = new System.Windows.Forms.Label();
            this.enemyAttackType = new System.Windows.Forms.Label();
            this.enemyAttackRange = new System.Windows.Forms.Label();
            this.enemyAttackDamage = new System.Windows.Forms.Label();
            this.enemyName = new System.Windows.Forms.Label();
            this.nextAction = new System.Windows.Forms.Button();
            this.showHeightDifference = new System.Windows.Forms.CheckBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.fieldHeight = new System.Windows.Forms.Label();
            this.fieldPosition = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.levelUpButton = new System.Windows.Forms.Button();
            this.console = new System.Windows.Forms.TextBox();
            this.ShowBlockedFields = new System.Windows.Forms.Button();
            this.enemyMovement = new System.Windows.Forms.CheckBox();
            this.debugButton = new System.Windows.Forms.Button();
            this.playerView = new StartGame.PlayerView();
            ((System.ComponentModel.ISupportInitialize)(this.gameBoard)).BeginInit();
            this.playerInfo.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // gameBoard
            // 
            this.gameBoard.Location = new System.Drawing.Point(13, 13);
            this.gameBoard.Name = "gameBoard";
            this.gameBoard.Size = new System.Drawing.Size(620, 620);
            this.gameBoard.TabIndex = 1;
            this.gameBoard.TabStop = false;
            this.gameBoard.Click += new System.EventHandler(this.GameBoard_Click);
            this.gameBoard.MouseClick += new System.Windows.Forms.MouseEventHandler(this.GameBoard_MouseClick);
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
            // playerInfo
            // 
            this.playerInfo.Controls.Add(this.enemyDefense);
            this.playerInfo.Controls.Add(this.enemyHeight);
            this.playerInfo.Controls.Add(this.enemyPosition);
            this.playerInfo.Controls.Add(this.enemyHealth);
            this.playerInfo.Controls.Add(this.enemyAttackType);
            this.playerInfo.Controls.Add(this.enemyAttackRange);
            this.playerInfo.Controls.Add(this.enemyAttackDamage);
            this.playerInfo.Controls.Add(this.enemyName);
            this.playerInfo.Location = new System.Drawing.Point(641, 231);
            this.playerInfo.Name = "playerInfo";
            this.playerInfo.Size = new System.Drawing.Size(178, 182);
            this.playerInfo.TabIndex = 6;
            // 
            // enemyDefense
            // 
            this.enemyDefense.AutoSize = true;
            this.enemyDefense.Location = new System.Drawing.Point(3, 92);
            this.enemyDefense.Name = "enemyDefense";
            this.enemyDefense.Size = new System.Drawing.Size(78, 13);
            this.enemyDefense.TabIndex = 13;
            this.enemyDefense.Text = "enemyDefense";
            // 
            // enemyHeight
            // 
            this.enemyHeight.AutoSize = true;
            this.enemyHeight.Location = new System.Drawing.Point(3, 79);
            this.enemyHeight.Name = "enemyHeight";
            this.enemyHeight.Size = new System.Drawing.Size(69, 13);
            this.enemyHeight.TabIndex = 12;
            this.enemyHeight.Text = "enemyHeight";
            // 
            // enemyPosition
            // 
            this.enemyPosition.AutoSize = true;
            this.enemyPosition.Location = new System.Drawing.Point(3, 66);
            this.enemyPosition.Name = "enemyPosition";
            this.enemyPosition.Size = new System.Drawing.Size(75, 13);
            this.enemyPosition.TabIndex = 11;
            this.enemyPosition.Text = "enemyPosition";
            // 
            // enemyHealth
            // 
            this.enemyHealth.AutoSize = true;
            this.enemyHealth.Location = new System.Drawing.Point(3, 53);
            this.enemyHealth.Name = "enemyHealth";
            this.enemyHealth.Size = new System.Drawing.Size(69, 13);
            this.enemyHealth.TabIndex = 9;
            this.enemyHealth.Text = "enemyHealth";
            // 
            // enemyAttackType
            // 
            this.enemyAttackType.AutoSize = true;
            this.enemyAttackType.Location = new System.Drawing.Point(3, 40);
            this.enemyAttackType.Name = "enemyAttackType";
            this.enemyAttackType.Size = new System.Drawing.Size(93, 13);
            this.enemyAttackType.TabIndex = 10;
            this.enemyAttackType.Text = "enemyAttackType";
            // 
            // enemyAttackRange
            // 
            this.enemyAttackRange.AutoSize = true;
            this.enemyAttackRange.Location = new System.Drawing.Point(3, 27);
            this.enemyAttackRange.Name = "enemyAttackRange";
            this.enemyAttackRange.Size = new System.Drawing.Size(101, 13);
            this.enemyAttackRange.TabIndex = 3;
            this.enemyAttackRange.Text = "enemyAttackRange";
            // 
            // enemyAttackDamage
            // 
            this.enemyAttackDamage.AutoSize = true;
            this.enemyAttackDamage.Location = new System.Drawing.Point(3, 14);
            this.enemyAttackDamage.Name = "enemyAttackDamage";
            this.enemyAttackDamage.Size = new System.Drawing.Size(77, 13);
            this.enemyAttackDamage.TabIndex = 2;
            this.enemyAttackDamage.Text = "attackDamage";
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
            // nextAction
            // 
            this.nextAction.Location = new System.Drawing.Point(1148, 569);
            this.nextAction.Name = "nextAction";
            this.nextAction.Size = new System.Drawing.Size(90, 56);
            this.nextAction.TabIndex = 9;
            this.nextAction.Text = "Next Action";
            this.nextAction.UseVisualStyleBackColor = true;
            this.nextAction.Click += new System.EventHandler(this.NextAction_Click);
            // 
            // showHeightDifference
            // 
            this.showHeightDifference.AutoSize = true;
            this.showHeightDifference.Location = new System.Drawing.Point(640, 598);
            this.showHeightDifference.Name = "showHeightDifference";
            this.showHeightDifference.Size = new System.Drawing.Size(135, 17);
            this.showHeightDifference.TabIndex = 17;
            this.showHeightDifference.Text = "Show height difference";
            this.showHeightDifference.UseVisualStyleBackColor = true;
            this.showHeightDifference.CheckedChanged += new System.EventHandler(this.ShowHeightDifference_CheckedChanged);
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.fieldHeight);
            this.panel1.Controls.Add(this.fieldPosition);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(643, 418);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(176, 67);
            this.panel1.TabIndex = 19;
            // 
            // fieldHeight
            // 
            this.fieldHeight.AutoSize = true;
            this.fieldHeight.Location = new System.Drawing.Point(4, 45);
            this.fieldHeight.Name = "fieldHeight";
            this.fieldHeight.Size = new System.Drawing.Size(57, 13);
            this.fieldHeight.TabIndex = 2;
            this.fieldHeight.Text = "fieldHeight";
            // 
            // fieldPosition
            // 
            this.fieldPosition.AutoSize = true;
            this.fieldPosition.Location = new System.Drawing.Point(4, 32);
            this.fieldPosition.Name = "fieldPosition";
            this.fieldPosition.Size = new System.Drawing.Size(63, 13);
            this.fieldPosition.TabIndex = 1;
            this.fieldPosition.Text = "fieldPosition";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.25F);
            this.label1.Location = new System.Drawing.Point(55, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "Field Info";
            // 
            // levelUpButton
            // 
            this.levelUpButton.Location = new System.Drawing.Point(1065, 583);
            this.levelUpButton.Name = "levelUpButton";
            this.levelUpButton.Size = new System.Drawing.Size(77, 28);
            this.levelUpButton.TabIndex = 20;
            this.levelUpButton.Text = "Level Up";
            this.levelUpButton.UseVisualStyleBackColor = true;
            this.levelUpButton.Click += new System.EventHandler(this.LevelUpButton_Click);
            // 
            // console
            // 
            this.console.BackColor = System.Drawing.SystemColors.Control;
            this.console.Location = new System.Drawing.Point(1026, 406);
            this.console.Multiline = true;
            this.console.Name = "console";
            this.console.ReadOnly = true;
            this.console.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.console.Size = new System.Drawing.Size(212, 147);
            this.console.TabIndex = 22;
            // 
            // ShowBlockedFields
            // 
            this.ShowBlockedFields.Location = new System.Drawing.Point(640, 491);
            this.ShowBlockedFields.Name = "ShowBlockedFields";
            this.ShowBlockedFields.Size = new System.Drawing.Size(135, 23);
            this.ShowBlockedFields.TabIndex = 23;
            this.ShowBlockedFields.Text = "Show blocked fields";
            this.ShowBlockedFields.UseVisualStyleBackColor = true;
            this.ShowBlockedFields.Click += new System.EventHandler(this.ShowBlockedFields_Click);
            // 
            // enemyMovement
            // 
            this.enemyMovement.AutoSize = true;
            this.enemyMovement.Location = new System.Drawing.Point(640, 575);
            this.enemyMovement.Name = "enemyMovement";
            this.enemyMovement.Size = new System.Drawing.Size(147, 17);
            this.enemyMovement.TabIndex = 24;
            this.enemyMovement.Text = "All enemies move at once";
            this.enemyMovement.UseVisualStyleBackColor = true;
            // 
            // debugButton
            // 
            this.debugButton.Location = new System.Drawing.Point(1036, 583);
            this.debugButton.Name = "debugButton";
            this.debugButton.Size = new System.Drawing.Size(23, 28);
            this.debugButton.TabIndex = 26;
            this.debugButton.Text = "D";
            this.debugButton.UseVisualStyleBackColor = true;
            this.debugButton.Click += new System.EventHandler(this.DebugButton_Click);
            // 
            // playerView
            // 
            this.playerView.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.playerView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.playerView.Location = new System.Drawing.Point(825, 12);
            this.playerView.Name = "playerView";
            this.playerView.Size = new System.Drawing.Size(413, 270);
            this.playerView.TabIndex = 27;
            this.playerView.Load += new System.EventHandler(this.PlayerView1_Load);
            // 
            // MainGameWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1250, 637);
            this.Controls.Add(this.playerView);
            this.Controls.Add(this.debugButton);
            this.Controls.Add(this.enemyMovement);
            this.Controls.Add(this.ShowBlockedFields);
            this.Controls.Add(this.console);
            this.Controls.Add(this.levelUpButton);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.showHeightDifference);
            this.Controls.Add(this.nextAction);
            this.Controls.Add(this.playerInfo);
            this.Controls.Add(this.troopList);
            this.Controls.Add(this.gameBoard);
            this.Name = "MainGameWindow";
            this.Text = "Start Game";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainGameWindow_FormClosing);
            this.Load += new System.EventHandler(this.MainGameWindow_Load);
            this.Shown += new System.EventHandler(this.MainGameWindow_Shown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MainGameWindow_MouseMove);
            ((System.ComponentModel.ISupportInitialize)(this.gameBoard)).EndInit();
            this.playerInfo.ResumeLayout(false);
            this.playerInfo.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.PictureBox gameBoard;
        private System.Windows.Forms.ListBox troopList;
        private System.Windows.Forms.Panel playerInfo;
        private System.Windows.Forms.Button nextAction;
        private System.Windows.Forms.Label enemyName;
        private System.Windows.Forms.Label enemyAttackType;
        private System.Windows.Forms.Label enemyAttackRange;
        private System.Windows.Forms.Label enemyAttackDamage;
        private System.Windows.Forms.Label enemyHealth;
        private System.Windows.Forms.Label enemyPosition;
        private System.Windows.Forms.Label enemyHeight;
        private System.Windows.Forms.CheckBox showHeightDifference;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label fieldHeight;
        private System.Windows.Forms.Label fieldPosition;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label enemyDefense;
        private System.Windows.Forms.Button levelUpButton;
        private System.Windows.Forms.TextBox console;
        private System.Windows.Forms.Button ShowBlockedFields;
        private System.Windows.Forms.CheckBox enemyMovement;
        private System.Windows.Forms.Button debugButton;
        private PlayerView playerView;
    }
}

