namespace StartGame
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
            this.cancel = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.nextTroop = new System.Windows.Forms.Button();
            this.lastTroop = new System.Windows.Forms.Button();
            this.weaponAttack = new System.Windows.Forms.NumericUpDown();
            this.weaponRange = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.weaponType = new System.Windows.Forms.ListBox();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.weaponAttack)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.weaponRange)).BeginInit();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Name";
            // 
            // name
            // 
            this.name.Location = new System.Drawing.Point(54, 12);
            this.name.Name = "name";
            this.name.Size = new System.Drawing.Size(212, 20);
            this.name.TabIndex = 1;
            // 
            // ok
            // 
            this.ok.Location = new System.Drawing.Point(197, 193);
            this.ok.Name = "ok";
            this.ok.Size = new System.Drawing.Size(75, 56);
            this.ok.TabIndex = 2;
            this.ok.Text = "OK";
            this.ok.UseVisualStyleBackColor = true;
            this.ok.Click += new System.EventHandler(this.Ok_Click);
            // 
            // cancel
            // 
            this.cancel.Location = new System.Drawing.Point(116, 193);
            this.cancel.Name = "cancel";
            this.cancel.Size = new System.Drawing.Size(75, 56);
            this.cancel.TabIndex = 3;
            this.cancel.Text = "Cancel";
            this.cancel.UseVisualStyleBackColor = true;
            this.cancel.Click += new System.EventHandler(this.Cancel_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Controls.Add(this.lastTroop);
            this.panel1.Controls.Add(this.nextTroop);
            this.panel1.Location = new System.Drawing.Point(16, 44);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(256, 143);
            this.panel1.TabIndex = 4;
            // 
            // nextTroop
            // 
            this.nextTroop.Enabled = false;
            this.nextTroop.Location = new System.Drawing.Point(178, 117);
            this.nextTroop.Name = "nextTroop";
            this.nextTroop.Size = new System.Drawing.Size(75, 23);
            this.nextTroop.TabIndex = 0;
            this.nextTroop.Text = "Next";
            this.nextTroop.UseVisualStyleBackColor = true;
            // 
            // lastTroop
            // 
            this.lastTroop.Enabled = false;
            this.lastTroop.Location = new System.Drawing.Point(3, 117);
            this.lastTroop.Name = "lastTroop";
            this.lastTroop.Size = new System.Drawing.Size(75, 23);
            this.lastTroop.TabIndex = 1;
            this.lastTroop.Text = "Last";
            this.lastTroop.UseVisualStyleBackColor = true;
            // 
            // weaponAttack
            // 
            this.weaponAttack.Location = new System.Drawing.Point(122, 0);
            this.weaponAttack.Name = "weaponAttack";
            this.weaponAttack.Size = new System.Drawing.Size(120, 20);
            this.weaponAttack.TabIndex = 0;
            this.weaponAttack.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // weaponRange
            // 
            this.weaponRange.Location = new System.Drawing.Point(122, 28);
            this.weaponRange.Name = "weaponRange";
            this.weaponRange.Size = new System.Drawing.Size(120, 20);
            this.weaponRange.TabIndex = 1;
            this.weaponRange.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 7);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(47, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Damage";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.weaponType);
            this.panel2.Controls.Add(this.label5);
            this.panel2.Controls.Add(this.label4);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.weaponRange);
            this.panel2.Controls.Add(this.weaponAttack);
            this.panel2.Location = new System.Drawing.Point(8, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(245, 108);
            this.panel2.TabIndex = 4;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 30);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(39, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Range";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 73);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(31, 13);
            this.label5.TabIndex = 7;
            this.label5.Text = "Type";
            // 
            // weaponType
            // 
            this.weaponType.FormattingEnabled = true;
            this.weaponType.Location = new System.Drawing.Point(122, 57);
            this.weaponType.Name = "weaponType";
            this.weaponType.Size = new System.Drawing.Size(120, 43);
            this.weaponType.TabIndex = 8;
            // 
            // PlayerProfile
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.cancel);
            this.Controls.Add(this.ok);
            this.Controls.Add(this.name);
            this.Controls.Add(this.label1);
            this.Name = "PlayerProfile";
            this.Text = "PlayerProfile";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PlayerProfile_FormClosing);
            this.Load += new System.EventHandler(this.PlayerProfile_Load);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.weaponAttack)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.weaponRange)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox name;
        private System.Windows.Forms.Button ok;
        private System.Windows.Forms.Button cancel;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button lastTroop;
        private System.Windows.Forms.Button nextTroop;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown weaponRange;
        private System.Windows.Forms.NumericUpDown weaponAttack;
        private System.Windows.Forms.ListBox weaponType;
    }
}