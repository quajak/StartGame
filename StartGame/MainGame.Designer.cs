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
            this.playerAttack = new System.Windows.Forms.Button();
            this.playerActionPoints = new System.Windows.Forms.Label();
            this.playerAttackType = new System.Windows.Forms.Label();
            this.playerAttackRange = new System.Windows.Forms.Label();
            this.playerAttackDamage = new System.Windows.Forms.Label();
            this.playerName = new System.Windows.Forms.Label();
            this.playerInfo = new System.Windows.Forms.Panel();
            this.enemyAttackType = new System.Windows.Forms.Label();
            this.enemyAttackRange = new System.Windows.Forms.Label();
            this.enemyAttackDamage = new System.Windows.Forms.Label();
            this.enemyName = new System.Windows.Forms.Label();
            this.spellList = new System.Windows.Forms.ListBox();
            this.spellInfo = new System.Windows.Forms.Panel();
            this.nextAction = new System.Windows.Forms.Button();
            this.playerWeaponList = new System.Windows.Forms.ListBox();
            this.playerPossibleWeaponName = new System.Windows.Forms.Label();
            this.playerPossibleAttackRange = new System.Windows.Forms.Label();
            this.playerPossibleWeaponDamage = new System.Windows.Forms.Label();
            this.playerPossibleWeaponType = new System.Windows.Forms.Label();
            this.changeWeapon = new System.Windows.Forms.Button();
            this.playerHealth = new System.Windows.Forms.Label();
            this.enemyHealth = new System.Windows.Forms.Label();
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
            // enemyInfo
            // 
            this.enemyInfo.Controls.Add(this.playerHealth);
            this.enemyInfo.Controls.Add(this.playerAttack);
            this.enemyInfo.Controls.Add(this.playerActionPoints);
            this.enemyInfo.Controls.Add(this.playerAttackType);
            this.enemyInfo.Controls.Add(this.playerAttackRange);
            this.enemyInfo.Controls.Add(this.playerAttackDamage);
            this.enemyInfo.Controls.Add(this.playerName);
            this.enemyInfo.Location = new System.Drawing.Point(824, 13);
            this.enemyInfo.Name = "enemyInfo";
            this.enemyInfo.Size = new System.Drawing.Size(202, 212);
            this.enemyInfo.TabIndex = 3;
            // 
            // playerAttack
            // 
            this.playerAttack.Location = new System.Drawing.Point(3, 173);
            this.playerAttack.Name = "playerAttack";
            this.playerAttack.Size = new System.Drawing.Size(75, 36);
            this.playerAttack.TabIndex = 7;
            this.playerAttack.Text = "Attack";
            this.playerAttack.UseVisualStyleBackColor = true;
            this.playerAttack.Click += new System.EventHandler(this.PlayerAttack_Click);
            // 
            // playerActionPoints
            // 
            this.playerActionPoints.AutoSize = true;
            this.playerActionPoints.Location = new System.Drawing.Point(3, 56);
            this.playerActionPoints.Name = "playerActionPoints";
            this.playerActionPoints.Size = new System.Drawing.Size(94, 13);
            this.playerActionPoints.TabIndex = 6;
            this.playerActionPoints.Text = "playerActionPoints";
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
            // playerAttackRange
            // 
            this.playerAttackRange.AutoSize = true;
            this.playerAttackRange.Location = new System.Drawing.Point(4, 30);
            this.playerAttackRange.Name = "playerAttackRange";
            this.playerAttackRange.Size = new System.Drawing.Size(101, 13);
            this.playerAttackRange.TabIndex = 4;
            this.playerAttackRange.Text = "playerAtttackRange";
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
            // playerWeaponList
            // 
            this.playerWeaponList.FormattingEnabled = true;
            this.playerWeaponList.Location = new System.Drawing.Point(825, 231);
            this.playerWeaponList.Name = "playerWeaponList";
            this.playerWeaponList.Size = new System.Drawing.Size(199, 95);
            this.playerWeaponList.TabIndex = 10;
            this.playerWeaponList.SelectedIndexChanged += new System.EventHandler(this.PlayerWeaponList_SelectedIndexChanged);
            // 
            // playerPossibleWeaponName
            // 
            this.playerPossibleWeaponName.AutoSize = true;
            this.playerPossibleWeaponName.Location = new System.Drawing.Point(824, 333);
            this.playerPossibleWeaponName.Name = "playerPossibleWeaponName";
            this.playerPossibleWeaponName.Size = new System.Drawing.Size(73, 13);
            this.playerPossibleWeaponName.TabIndex = 11;
            this.playerPossibleWeaponName.Text = "weaponName";
            // 
            // playerPossibleAttackRange
            // 
            this.playerPossibleAttackRange.AutoSize = true;
            this.playerPossibleAttackRange.Location = new System.Drawing.Point(825, 363);
            this.playerPossibleAttackRange.Name = "playerPossibleAttackRange";
            this.playerPossibleAttackRange.Size = new System.Drawing.Size(77, 13);
            this.playerPossibleAttackRange.TabIndex = 12;
            this.playerPossibleAttackRange.Text = "weaponRange";
            // 
            // playerPossibleWeaponDamage
            // 
            this.playerPossibleWeaponDamage.AutoSize = true;
            this.playerPossibleWeaponDamage.Location = new System.Drawing.Point(824, 350);
            this.playerPossibleWeaponDamage.Name = "playerPossibleWeaponDamage";
            this.playerPossibleWeaponDamage.Size = new System.Drawing.Size(85, 13);
            this.playerPossibleWeaponDamage.TabIndex = 13;
            this.playerPossibleWeaponDamage.Text = "weaponDamage";
            // 
            // playerPossibleWeaponType
            // 
            this.playerPossibleWeaponType.AutoSize = true;
            this.playerPossibleWeaponType.Location = new System.Drawing.Point(824, 376);
            this.playerPossibleWeaponType.Name = "playerPossibleWeaponType";
            this.playerPossibleWeaponType.Size = new System.Drawing.Size(69, 13);
            this.playerPossibleWeaponType.TabIndex = 14;
            this.playerPossibleWeaponType.Text = "weaponType";
            // 
            // changeWeapon
            // 
            this.changeWeapon.Location = new System.Drawing.Point(951, 333);
            this.changeWeapon.Name = "changeWeapon";
            this.changeWeapon.Size = new System.Drawing.Size(75, 43);
            this.changeWeapon.TabIndex = 15;
            this.changeWeapon.Text = "Use";
            this.changeWeapon.UseVisualStyleBackColor = true;
            this.changeWeapon.Click += new System.EventHandler(this.ChangeWeapon_Click);
            // 
            // playerHealth
            // 
            this.playerHealth.AutoSize = true;
            this.playerHealth.Location = new System.Drawing.Point(4, 69);
            this.playerHealth.Name = "playerHealth";
            this.playerHealth.Size = new System.Drawing.Size(66, 13);
            this.playerHealth.TabIndex = 8;
            this.playerHealth.Text = "playerHealth";
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
            // MainGameWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1241, 639);
            this.Controls.Add(this.changeWeapon);
            this.Controls.Add(this.playerPossibleWeaponType);
            this.Controls.Add(this.playerPossibleWeaponDamage);
            this.Controls.Add(this.playerPossibleAttackRange);
            this.Controls.Add(this.playerPossibleWeaponName);
            this.Controls.Add(this.playerWeaponList);
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
            this.PerformLayout();

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
        private System.Windows.Forms.ListBox playerWeaponList;
        private System.Windows.Forms.Label playerPossibleWeaponName;
        private System.Windows.Forms.Label playerPossibleAttackRange;
        private System.Windows.Forms.Label playerPossibleWeaponDamage;
        private System.Windows.Forms.Label playerPossibleWeaponType;
        private System.Windows.Forms.Button changeWeapon;
        private System.Windows.Forms.Label playerActionPoints;
        private System.Windows.Forms.Button playerAttack;
        private System.Windows.Forms.Label playerHealth;
        private System.Windows.Forms.Label enemyHealth;
    }
}

