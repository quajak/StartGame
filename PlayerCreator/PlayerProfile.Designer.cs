namespace PlayerCreator
{
    partial class PlayerProfile
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
            this.label1 = new System.Windows.Forms.Label();
            this.name = new System.Windows.Forms.TextBox();
            this.ok = new System.Windows.Forms.Button();
            this.playerStatsPanel = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.activeWeaponName = new System.Windows.Forms.Label();
            this.nextActiveWeapon = new System.Windows.Forms.Button();
            this.lastActiveWeapon = new System.Windows.Forms.Button();
            this.playerActiveWeaponType = new System.Windows.Forms.Label();
            this.playerActiveWeaponRange = new System.Windows.Forms.Label();
            this.playerActiveWeaponDamage = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.playerName = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.weaponCreatorPanel = new System.Windows.Forms.Panel();
            this.addNewWeapon = new System.Windows.Forms.Button();
            this.panel5 = new System.Windows.Forms.Panel();
            this.weaponcreatornamebox = new System.Windows.Forms.Label();
            this.createWeaponName = new System.Windows.Forms.TextBox();
            this.weaponCreatorType = new System.Windows.Forms.ListBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.weaponCreatorRange = new System.Windows.Forms.NumericUpDown();
            this.weaponCreatorDamage = new System.Windows.Forms.NumericUpDown();
            this.panel1 = new System.Windows.Forms.Panel();
            this.Finish = new System.Windows.Forms.Button();
            this.playerStatsPanel.SuspendLayout();
            this.panel2.SuspendLayout();
            this.weaponCreatorPanel.SuspendLayout();
            this.panel5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.weaponCreatorRange)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.weaponCreatorDamage)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Name";
            // 
            // name
            // 
            this.name.Location = new System.Drawing.Point(44, 6);
            this.name.Name = "name";
            this.name.Size = new System.Drawing.Size(202, 20);
            this.name.TabIndex = 1;
            // 
            // ok
            // 
            this.ok.Location = new System.Drawing.Point(174, 276);
            this.ok.Name = "ok";
            this.ok.Size = new System.Drawing.Size(75, 28);
            this.ok.TabIndex = 2;
            this.ok.Text = "OK";
            this.ok.UseVisualStyleBackColor = true;
            this.ok.Click += new System.EventHandler(this.Ok_Click);
            // 
            // playerStatsPanel
            // 
            this.playerStatsPanel.Controls.Add(this.panel2);
            this.playerStatsPanel.Controls.Add(this.label3);
            this.playerStatsPanel.Controls.Add(this.playerName);
            this.playerStatsPanel.Location = new System.Drawing.Point(267, 13);
            this.playerStatsPanel.Name = "playerStatsPanel";
            this.playerStatsPanel.Size = new System.Drawing.Size(305, 307);
            this.playerStatsPanel.TabIndex = 5;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.activeWeaponName);
            this.panel2.Controls.Add(this.nextActiveWeapon);
            this.panel2.Controls.Add(this.lastActiveWeapon);
            this.panel2.Controls.Add(this.playerActiveWeaponType);
            this.panel2.Controls.Add(this.playerActiveWeaponRange);
            this.panel2.Controls.Add(this.playerActiveWeaponDamage);
            this.panel2.Controls.Add(this.label4);
            this.panel2.Location = new System.Drawing.Point(3, 42);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(299, 218);
            this.panel2.TabIndex = 2;
            // 
            // activeWeaponName
            // 
            this.activeWeaponName.AutoSize = true;
            this.activeWeaponName.Location = new System.Drawing.Point(6, 36);
            this.activeWeaponName.Name = "activeWeaponName";
            this.activeWeaponName.Size = new System.Drawing.Size(35, 13);
            this.activeWeaponName.TabIndex = 6;
            this.activeWeaponName.Text = "label5";
            // 
            // nextActiveWeapon
            // 
            this.nextActiveWeapon.Location = new System.Drawing.Point(221, 192);
            this.nextActiveWeapon.Name = "nextActiveWeapon";
            this.nextActiveWeapon.Size = new System.Drawing.Size(75, 23);
            this.nextActiveWeapon.TabIndex = 5;
            this.nextActiveWeapon.Text = "Next";
            this.nextActiveWeapon.UseVisualStyleBackColor = true;
            this.nextActiveWeapon.Click += new System.EventHandler(this.NextActiveWeapon_Click);
            // 
            // lastActiveWeapon
            // 
            this.lastActiveWeapon.Location = new System.Drawing.Point(7, 192);
            this.lastActiveWeapon.Name = "lastActiveWeapon";
            this.lastActiveWeapon.Size = new System.Drawing.Size(75, 23);
            this.lastActiveWeapon.TabIndex = 4;
            this.lastActiveWeapon.Text = "Last";
            this.lastActiveWeapon.UseVisualStyleBackColor = true;
            this.lastActiveWeapon.Click += new System.EventHandler(this.LastActiveWeapon_Click);
            // 
            // playerActiveWeaponType
            // 
            this.playerActiveWeaponType.AutoSize = true;
            this.playerActiveWeaponType.Location = new System.Drawing.Point(5, 118);
            this.playerActiveWeaponType.Name = "playerActiveWeaponType";
            this.playerActiveWeaponType.Size = new System.Drawing.Size(35, 13);
            this.playerActiveWeaponType.TabIndex = 3;
            this.playerActiveWeaponType.Text = "label9";
            // 
            // playerActiveWeaponRange
            // 
            this.playerActiveWeaponRange.AutoSize = true;
            this.playerActiveWeaponRange.Location = new System.Drawing.Point(6, 87);
            this.playerActiveWeaponRange.Name = "playerActiveWeaponRange";
            this.playerActiveWeaponRange.Size = new System.Drawing.Size(35, 13);
            this.playerActiveWeaponRange.TabIndex = 2;
            this.playerActiveWeaponRange.Text = "label5";
            // 
            // playerActiveWeaponDamage
            // 
            this.playerActiveWeaponDamage.AutoSize = true;
            this.playerActiveWeaponDamage.Location = new System.Drawing.Point(6, 57);
            this.playerActiveWeaponDamage.Name = "playerActiveWeaponDamage";
            this.playerActiveWeaponDamage.Size = new System.Drawing.Size(35, 13);
            this.playerActiveWeaponDamage.TabIndex = 1;
            this.playerActiveWeaponDamage.Text = "label5";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(100, 12);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(85, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Player Weapons";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(125, 6);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(63, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Player Stats";
            // 
            // playerName
            // 
            this.playerName.AutoSize = true;
            this.playerName.Location = new System.Drawing.Point(3, 26);
            this.playerName.Name = "playerName";
            this.playerName.Size = new System.Drawing.Size(35, 13);
            this.playerName.TabIndex = 0;
            this.playerName.Text = "label2";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(102, 6);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(85, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Weapon Creator";
            // 
            // weaponCreatorPanel
            // 
            this.weaponCreatorPanel.Controls.Add(this.addNewWeapon);
            this.weaponCreatorPanel.Controls.Add(this.label2);
            this.weaponCreatorPanel.Controls.Add(this.panel5);
            this.weaponCreatorPanel.Location = new System.Drawing.Point(578, 13);
            this.weaponCreatorPanel.Name = "weaponCreatorPanel";
            this.weaponCreatorPanel.Size = new System.Drawing.Size(321, 260);
            this.weaponCreatorPanel.TabIndex = 5;
            // 
            // addNewWeapon
            // 
            this.addNewWeapon.Location = new System.Drawing.Point(225, 210);
            this.addNewWeapon.Name = "addNewWeapon";
            this.addNewWeapon.Size = new System.Drawing.Size(75, 23);
            this.addNewWeapon.TabIndex = 5;
            this.addNewWeapon.Text = "Add";
            this.addNewWeapon.UseVisualStyleBackColor = true;
            this.addNewWeapon.Click += new System.EventHandler(this.AddNewWeaponClick);
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.weaponcreatornamebox);
            this.panel5.Controls.Add(this.createWeaponName);
            this.panel5.Controls.Add(this.weaponCreatorType);
            this.panel5.Controls.Add(this.label6);
            this.panel5.Controls.Add(this.label7);
            this.panel5.Controls.Add(this.label8);
            this.panel5.Controls.Add(this.weaponCreatorRange);
            this.panel5.Controls.Add(this.weaponCreatorDamage);
            this.panel5.Location = new System.Drawing.Point(6, 22);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(315, 182);
            this.panel5.TabIndex = 4;
            // 
            // weaponcreatornamebox
            // 
            this.weaponcreatornamebox.AutoSize = true;
            this.weaponcreatornamebox.Location = new System.Drawing.Point(6, 8);
            this.weaponcreatornamebox.Name = "weaponcreatornamebox";
            this.weaponcreatornamebox.Size = new System.Drawing.Size(35, 13);
            this.weaponcreatornamebox.TabIndex = 10;
            this.weaponcreatornamebox.Text = "Name";
            // 
            // createWeaponName
            // 
            this.createWeaponName.Location = new System.Drawing.Point(194, 5);
            this.createWeaponName.Name = "createWeaponName";
            this.createWeaponName.Size = new System.Drawing.Size(118, 20);
            this.createWeaponName.TabIndex = 9;
            // 
            // weaponCreatorType
            // 
            this.weaponCreatorType.FormattingEnabled = true;
            this.weaponCreatorType.Location = new System.Drawing.Point(192, 88);
            this.weaponCreatorType.Name = "weaponCreatorType";
            this.weaponCreatorType.Size = new System.Drawing.Size(120, 43);
            this.weaponCreatorType.TabIndex = 8;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 102);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(31, 13);
            this.label6.TabIndex = 7;
            this.label6.Text = "Type";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 59);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(39, 13);
            this.label7.TabIndex = 6;
            this.label7.Text = "Range";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 36);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(47, 13);
            this.label8.TabIndex = 5;
            this.label8.Text = "Damage";
            // 
            // weaponCreatorRange
            // 
            this.weaponCreatorRange.Location = new System.Drawing.Point(192, 59);
            this.weaponCreatorRange.Name = "weaponCreatorRange";
            this.weaponCreatorRange.Size = new System.Drawing.Size(120, 20);
            this.weaponCreatorRange.TabIndex = 1;
            this.weaponCreatorRange.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // weaponCreatorDamage
            // 
            this.weaponCreatorDamage.Location = new System.Drawing.Point(192, 31);
            this.weaponCreatorDamage.Name = "weaponCreatorDamage";
            this.weaponCreatorDamage.Size = new System.Drawing.Size(120, 20);
            this.weaponCreatorDamage.TabIndex = 0;
            this.weaponCreatorDamage.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.ok);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.name);
            this.panel1.Location = new System.Drawing.Point(12, 13);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(249, 307);
            this.panel1.TabIndex = 1;
            // 
            // Finish
            // 
            this.Finish.Location = new System.Drawing.Point(579, 280);
            this.Finish.Name = "Finish";
            this.Finish.Size = new System.Drawing.Size(123, 37);
            this.Finish.TabIndex = 6;
            this.Finish.Text = "Finish";
            this.Finish.UseVisualStyleBackColor = true;
            this.Finish.Click += new System.EventHandler(this.Finish_Click);
            // 
            // PlayerProfile
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(911, 332);
            this.Controls.Add(this.Finish);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.weaponCreatorPanel);
            this.Controls.Add(this.playerStatsPanel);
            this.Name = "PlayerProfile";
            this.Text = "PlayerProfile";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PlayerProfile_FormClosing);
            this.Load += new System.EventHandler(this.PlayerProfile_Load);
            this.playerStatsPanel.ResumeLayout(false);
            this.playerStatsPanel.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.weaponCreatorPanel.ResumeLayout(false);
            this.weaponCreatorPanel.PerformLayout();
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.weaponCreatorRange)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.weaponCreatorDamage)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox name;
        private System.Windows.Forms.Button ok;
        private System.Windows.Forms.Panel playerStatsPanel;
        private System.Windows.Forms.Label playerName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel weaponCreatorPanel;
        private System.Windows.Forms.Button addNewWeapon;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.ListBox weaponCreatorType;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.NumericUpDown weaponCreatorRange;
        private System.Windows.Forms.NumericUpDown weaponCreatorDamage;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button nextActiveWeapon;
        private System.Windows.Forms.Button lastActiveWeapon;
        private System.Windows.Forms.Label playerActiveWeaponType;
        private System.Windows.Forms.Label playerActiveWeaponRange;
        private System.Windows.Forms.Label playerActiveWeaponDamage;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label activeWeaponName;
        private System.Windows.Forms.Label weaponcreatornamebox;
        private System.Windows.Forms.TextBox createWeaponName;
        private System.Windows.Forms.Button Finish;
    }
}