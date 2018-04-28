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
            this.playerXP = new System.Windows.Forms.Label();
            this.playerLevel = new System.Windows.Forms.Label();
            this.playerVitatlity = new System.Windows.Forms.Label();
            this.playerEndurance = new System.Windows.Forms.Label();
            this.playerAgility = new System.Windows.Forms.Label();
            this.playerStrength = new System.Windows.Forms.Label();
            this.playerDefense = new System.Windows.Forms.Label();
            this.playerHeight = new System.Windows.Forms.Label();
            this.playerWeaponAttacks = new System.Windows.Forms.Label();
            this.playerHealth = new System.Windows.Forms.Label();
            this.playerActionPoints = new System.Windows.Forms.Label();
            this.playerAttackType = new System.Windows.Forms.Label();
            this.playerAttackRange = new System.Windows.Forms.Label();
            this.playerAttackDamage = new System.Windows.Forms.Label();
            this.playerName = new System.Windows.Forms.Label();
            this.playerInfo = new System.Windows.Forms.Panel();
            this.enemyDefense = new System.Windows.Forms.Label();
            this.enemyHeight = new System.Windows.Forms.Label();
            this.enemyPosition = new System.Windows.Forms.Label();
            this.enemyHealth = new System.Windows.Forms.Label();
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
            this.playerPossibleWeaponAttacks = new System.Windows.Forms.Label();
            this.showHeightDifference = new System.Windows.Forms.CheckBox();
            this.dumpWeapon = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.fieldHeight = new System.Windows.Forms.Label();
            this.fieldPosition = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.levelUpButton = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.treeInformation = new System.Windows.Forms.Label();
            this.treeName = new System.Windows.Forms.Label();
            this.treeList = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.gameBoard)).BeginInit();
            this.enemyInfo.SuspendLayout();
            this.playerInfo.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
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
            this.enemyInfo.Controls.Add(this.playerXP);
            this.enemyInfo.Controls.Add(this.playerLevel);
            this.enemyInfo.Controls.Add(this.playerVitatlity);
            this.enemyInfo.Controls.Add(this.playerEndurance);
            this.enemyInfo.Controls.Add(this.playerAgility);
            this.enemyInfo.Controls.Add(this.playerStrength);
            this.enemyInfo.Controls.Add(this.playerDefense);
            this.enemyInfo.Controls.Add(this.playerHeight);
            this.enemyInfo.Controls.Add(this.playerWeaponAttacks);
            this.enemyInfo.Controls.Add(this.playerHealth);
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
            // playerXP
            // 
            this.playerXP.AutoSize = true;
            this.playerXP.Location = new System.Drawing.Point(7, 188);
            this.playerXP.Name = "playerXP";
            this.playerXP.Size = new System.Drawing.Size(49, 13);
            this.playerXP.TabIndex = 21;
            this.playerXP.Text = "playerXP";
            // 
            // playerLevel
            // 
            this.playerLevel.AutoSize = true;
            this.playerLevel.Location = new System.Drawing.Point(7, 171);
            this.playerLevel.Name = "playerLevel";
            this.playerLevel.Size = new System.Drawing.Size(61, 13);
            this.playerLevel.TabIndex = 17;
            this.playerLevel.Text = "playerLevel";
            // 
            // playerVitatlity
            // 
            this.playerVitatlity.AutoSize = true;
            this.playerVitatlity.Location = new System.Drawing.Point(115, 43);
            this.playerVitatlity.Name = "playerVitatlity";
            this.playerVitatlity.Size = new System.Drawing.Size(65, 13);
            this.playerVitatlity.TabIndex = 15;
            this.playerVitatlity.Text = "playerVitality";
            // 
            // playerEndurance
            // 
            this.playerEndurance.AutoSize = true;
            this.playerEndurance.Location = new System.Drawing.Point(115, 30);
            this.playerEndurance.Name = "playerEndurance";
            this.playerEndurance.Size = new System.Drawing.Size(87, 13);
            this.playerEndurance.TabIndex = 14;
            this.playerEndurance.Text = "playerEndurance";
            // 
            // playerAgility
            // 
            this.playerAgility.AutoSize = true;
            this.playerAgility.Location = new System.Drawing.Point(115, 17);
            this.playerAgility.Name = "playerAgility";
            this.playerAgility.Size = new System.Drawing.Size(62, 13);
            this.playerAgility.TabIndex = 13;
            this.playerAgility.Text = "playerAgility";
            // 
            // playerStrength
            // 
            this.playerStrength.AutoSize = true;
            this.playerStrength.Location = new System.Drawing.Point(115, 4);
            this.playerStrength.Name = "playerStrength";
            this.playerStrength.Size = new System.Drawing.Size(75, 13);
            this.playerStrength.TabIndex = 12;
            this.playerStrength.Text = "playerStrength";
            // 
            // playerDefense
            // 
            this.playerDefense.AutoSize = true;
            this.playerDefense.Location = new System.Drawing.Point(4, 108);
            this.playerDefense.Name = "playerDefense";
            this.playerDefense.Size = new System.Drawing.Size(75, 13);
            this.playerDefense.TabIndex = 11;
            this.playerDefense.Text = "playerDefense";
            // 
            // playerHeight
            // 
            this.playerHeight.AutoSize = true;
            this.playerHeight.Location = new System.Drawing.Point(3, 95);
            this.playerHeight.Name = "playerHeight";
            this.playerHeight.Size = new System.Drawing.Size(66, 13);
            this.playerHeight.TabIndex = 10;
            this.playerHeight.Text = "playerHeight";
            // 
            // playerWeaponAttacks
            // 
            this.playerWeaponAttacks.AutoSize = true;
            this.playerWeaponAttacks.Location = new System.Drawing.Point(4, 82);
            this.playerWeaponAttacks.Name = "playerWeaponAttacks";
            this.playerWeaponAttacks.Size = new System.Drawing.Size(112, 13);
            this.playerWeaponAttacks.TabIndex = 9;
            this.playerWeaponAttacks.Text = "playerWeaponAttacks";
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
            // playerPossibleWeaponAttacks
            // 
            this.playerPossibleWeaponAttacks.AutoSize = true;
            this.playerPossibleWeaponAttacks.Location = new System.Drawing.Point(825, 389);
            this.playerPossibleWeaponAttacks.Name = "playerPossibleWeaponAttacks";
            this.playerPossibleWeaponAttacks.Size = new System.Drawing.Size(69, 13);
            this.playerPossibleWeaponAttacks.TabIndex = 16;
            this.playerPossibleWeaponAttacks.Text = "weaponType";
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
            // dumpWeapon
            // 
            this.dumpWeapon.Location = new System.Drawing.Point(951, 383);
            this.dumpWeapon.Name = "dumpWeapon";
            this.dumpWeapon.Size = new System.Drawing.Size(75, 31);
            this.dumpWeapon.TabIndex = 18;
            this.dumpWeapon.Text = "Dump";
            this.dumpWeapon.UseVisualStyleBackColor = true;
            this.dumpWeapon.Click += new System.EventHandler(this.DumpWeapon_Click);
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
            this.levelUpButton.Location = new System.Drawing.Point(1059, 599);
            this.levelUpButton.Name = "levelUpButton";
            this.levelUpButton.Size = new System.Drawing.Size(77, 28);
            this.levelUpButton.TabIndex = 20;
            this.levelUpButton.Text = "Level Up";
            this.levelUpButton.UseVisualStyleBackColor = true;
            this.levelUpButton.Click += new System.EventHandler(this.LevelUpButton_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.treeInformation);
            this.panel2.Controls.Add(this.treeName);
            this.panel2.Controls.Add(this.treeList);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Location = new System.Drawing.Point(824, 418);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(201, 209);
            this.panel2.TabIndex = 21;
            // 
            // treeInformation
            // 
            this.treeInformation.AutoSize = true;
            this.treeInformation.Location = new System.Drawing.Point(7, 114);
            this.treeInformation.MaximumSize = new System.Drawing.Size(180, 100);
            this.treeInformation.Name = "treeInformation";
            this.treeInformation.Size = new System.Drawing.Size(77, 13);
            this.treeInformation.TabIndex = 3;
            this.treeInformation.Text = "treeInformation";
            // 
            // treeName
            // 
            this.treeName.AutoSize = true;
            this.treeName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.treeName.Location = new System.Drawing.Point(10, 97);
            this.treeName.Name = "treeName";
            this.treeName.Size = new System.Drawing.Size(61, 13);
            this.treeName.TabIndex = 2;
            this.treeName.Text = "treeName";
            // 
            // treeList
            // 
            this.treeList.FormattingEnabled = true;
            this.treeList.Location = new System.Drawing.Point(7, 34);
            this.treeList.Name = "treeList";
            this.treeList.Size = new System.Drawing.Size(183, 56);
            this.treeList.TabIndex = 1;
            this.treeList.SelectedIndexChanged += new System.EventHandler(this.TreeList_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(10, 10);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(117, 20);
            this.label2.TabIndex = 0;
            this.label2.Text = "Skills and Titles";
            // 
            // MainGameWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1241, 639);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.levelUpButton);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.dumpWeapon);
            this.Controls.Add(this.showHeightDifference);
            this.Controls.Add(this.playerPossibleWeaponAttacks);
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
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainGameWindow_FormClosing);
            this.Load += new System.EventHandler(this.MainGameWindow_Load);
            this.Shown += new System.EventHandler(this.MainGameWindow_Shown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MainGameWindow_MouseMove);
            ((System.ComponentModel.ISupportInitialize)(this.gameBoard)).EndInit();
            this.enemyInfo.ResumeLayout(false);
            this.enemyInfo.PerformLayout();
            this.playerInfo.ResumeLayout(false);
            this.playerInfo.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
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
        private System.Windows.Forms.Label playerHealth;
        private System.Windows.Forms.Label enemyHealth;
        private System.Windows.Forms.Label playerWeaponAttacks;
        private System.Windows.Forms.Label playerPossibleWeaponAttacks;
        private System.Windows.Forms.Label enemyPosition;
        private System.Windows.Forms.Label playerHeight;
        private System.Windows.Forms.Label enemyHeight;
        private System.Windows.Forms.CheckBox showHeightDifference;
        private System.Windows.Forms.Button dumpWeapon;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label fieldHeight;
        private System.Windows.Forms.Label fieldPosition;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label playerDefense;
        private System.Windows.Forms.Label enemyDefense;
        private System.Windows.Forms.Label playerVitatlity;
        private System.Windows.Forms.Label playerEndurance;
        private System.Windows.Forms.Label playerAgility;
        private System.Windows.Forms.Label playerStrength;
        private System.Windows.Forms.Button levelUpButton;
        private System.Windows.Forms.Label playerLevel;
        private System.Windows.Forms.Label playerXP;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label treeInformation;
        private System.Windows.Forms.Label treeName;
        private System.Windows.Forms.ListBox treeList;
        private System.Windows.Forms.Label label2;
    }
}

