namespace StartGame.User_Controls
{
    partial class PlayerWeaponView
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
            this.dumpWeapon = new System.Windows.Forms.Button();
            this.playerPossibleWeaponAttacks = new System.Windows.Forms.Label();
            this.changeWeapon = new System.Windows.Forms.Button();
            this.playerPossibleWeaponType = new System.Windows.Forms.Label();
            this.playerPossibleWeaponDamage = new System.Windows.Forms.Label();
            this.playerPossibleAttackRange = new System.Windows.Forms.Label();
            this.playerPossibleWeaponName = new System.Windows.Forms.Label();
            this.playerWeaponList = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.ammoList = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // dumpWeapon
            // 
            this.dumpWeapon.Location = new System.Drawing.Point(251, 216);
            this.dumpWeapon.Name = "dumpWeapon";
            this.dumpWeapon.Size = new System.Drawing.Size(75, 31);
            this.dumpWeapon.TabIndex = 26;
            this.dumpWeapon.Text = "Dump";
            this.dumpWeapon.UseVisualStyleBackColor = true;
            this.dumpWeapon.Click += new System.EventHandler(this.DumpWeapon_Click);
            // 
            // playerPossibleWeaponAttacks
            // 
            this.playerPossibleWeaponAttacks.AutoSize = true;
            this.playerPossibleWeaponAttacks.Location = new System.Drawing.Point(13, 222);
            this.playerPossibleWeaponAttacks.Name = "playerPossibleWeaponAttacks";
            this.playerPossibleWeaponAttacks.Size = new System.Drawing.Size(69, 13);
            this.playerPossibleWeaponAttacks.TabIndex = 25;
            this.playerPossibleWeaponAttacks.Text = "weaponType";
            // 
            // changeWeapon
            // 
            this.changeWeapon.Location = new System.Drawing.Point(251, 166);
            this.changeWeapon.Name = "changeWeapon";
            this.changeWeapon.Size = new System.Drawing.Size(75, 43);
            this.changeWeapon.TabIndex = 24;
            this.changeWeapon.Text = "Use";
            this.changeWeapon.UseVisualStyleBackColor = true;
            this.changeWeapon.Click += new System.EventHandler(this.ChangeWeapon_Click);
            // 
            // playerPossibleWeaponType
            // 
            this.playerPossibleWeaponType.AutoSize = true;
            this.playerPossibleWeaponType.Location = new System.Drawing.Point(12, 209);
            this.playerPossibleWeaponType.Name = "playerPossibleWeaponType";
            this.playerPossibleWeaponType.Size = new System.Drawing.Size(69, 13);
            this.playerPossibleWeaponType.TabIndex = 23;
            this.playerPossibleWeaponType.Text = "weaponType";
            // 
            // playerPossibleWeaponDamage
            // 
            this.playerPossibleWeaponDamage.AutoSize = true;
            this.playerPossibleWeaponDamage.Location = new System.Drawing.Point(12, 183);
            this.playerPossibleWeaponDamage.Name = "playerPossibleWeaponDamage";
            this.playerPossibleWeaponDamage.Size = new System.Drawing.Size(85, 13);
            this.playerPossibleWeaponDamage.TabIndex = 22;
            this.playerPossibleWeaponDamage.Text = "weaponDamage";
            // 
            // playerPossibleAttackRange
            // 
            this.playerPossibleAttackRange.AutoSize = true;
            this.playerPossibleAttackRange.Location = new System.Drawing.Point(13, 196);
            this.playerPossibleAttackRange.Name = "playerPossibleAttackRange";
            this.playerPossibleAttackRange.Size = new System.Drawing.Size(77, 13);
            this.playerPossibleAttackRange.TabIndex = 21;
            this.playerPossibleAttackRange.Text = "weaponRange";
            // 
            // playerPossibleWeaponName
            // 
            this.playerPossibleWeaponName.AutoSize = true;
            this.playerPossibleWeaponName.Location = new System.Drawing.Point(12, 166);
            this.playerPossibleWeaponName.Name = "playerPossibleWeaponName";
            this.playerPossibleWeaponName.Size = new System.Drawing.Size(73, 13);
            this.playerPossibleWeaponName.TabIndex = 20;
            this.playerPossibleWeaponName.Text = "weaponName";
            // 
            // playerWeaponList
            // 
            this.playerWeaponList.FormattingEnabled = true;
            this.playerWeaponList.Location = new System.Drawing.Point(14, 65);
            this.playerWeaponList.Name = "playerWeaponList";
            this.playerWeaponList.Size = new System.Drawing.Size(203, 95);
            this.playerWeaponList.TabIndex = 19;
            this.playerWeaponList.SelectedIndexChanged += new System.EventHandler(this.PlayerWeaponList_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Modern No. 20", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(93, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 25);
            this.label1.TabIndex = 27;
            this.label1.Text = "Weapons";
            // 
            // ammoList
            // 
            this.ammoList.FormattingEnabled = true;
            this.ammoList.Location = new System.Drawing.Point(224, 65);
            this.ammoList.Name = "ammoList";
            this.ammoList.Size = new System.Drawing.Size(102, 95);
            this.ammoList.TabIndex = 28;
            this.ammoList.SelectedIndexChanged += new System.EventHandler(this.AmmoList_SelectedIndexChanged);
            // 
            // PlayerWeaponView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.Controls.Add(this.ammoList);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dumpWeapon);
            this.Controls.Add(this.playerPossibleWeaponAttacks);
            this.Controls.Add(this.changeWeapon);
            this.Controls.Add(this.playerPossibleWeaponType);
            this.Controls.Add(this.playerPossibleWeaponDamage);
            this.Controls.Add(this.playerPossibleAttackRange);
            this.Controls.Add(this.playerPossibleWeaponName);
            this.Controls.Add(this.playerWeaponList);
            this.Name = "PlayerWeaponView";
            this.Size = new System.Drawing.Size(338, 250);
            this.Load += new System.EventHandler(this.PlayerWeaponView_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button dumpWeapon;
        private System.Windows.Forms.Label playerPossibleWeaponAttacks;
        private System.Windows.Forms.Button changeWeapon;
        private System.Windows.Forms.Label playerPossibleWeaponType;
        private System.Windows.Forms.Label playerPossibleWeaponDamage;
        private System.Windows.Forms.Label playerPossibleAttackRange;
        private System.Windows.Forms.Label playerPossibleWeaponName;
        private System.Windows.Forms.ListBox playerWeaponList;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox ammoList;
    }
}
