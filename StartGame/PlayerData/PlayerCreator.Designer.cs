namespace StartGame.PlayerData
{
    partial class PlayerCreator
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
            this.playerView1 = new StartGame.PlayerView();
            this.label1 = new System.Windows.Forms.Label();
            this.strengthValue = new System.Windows.Forms.NumericUpDown();
            this.agilityValue = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.enduranceValue = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.vitalityValue = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.intelligenceValue = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.wisdomValue = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.playerName = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.playerXP = new System.Windows.Forms.NumericUpDown();
            this.label9 = new System.Windows.Forms.Label();
            this.imageSelected = new System.Windows.Forms.PictureBox();
            this.imageNext = new System.Windows.Forms.Button();
            this.imageLast = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.strengthValue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.agilityValue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.enduranceValue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.vitalityValue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.intelligenceValue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.wisdomValue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.playerXP)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageSelected)).BeginInit();
            this.SuspendLayout();
            // 
            // playerView1
            // 
            this.playerView1.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.playerView1.Location = new System.Drawing.Point(13, 13);
            this.playerView1.Name = "playerView1";
            this.playerView1.Size = new System.Drawing.Size(408, 350);
            this.playerView1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(429, 211);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Strength";
            // 
            // strengthValue
            // 
            this.strengthValue.Location = new System.Drawing.Point(495, 209);
            this.strengthValue.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.strengthValue.Name = "strengthValue";
            this.strengthValue.Size = new System.Drawing.Size(120, 20);
            this.strengthValue.TabIndex = 2;
            this.strengthValue.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.strengthValue.ValueChanged += new System.EventHandler(this.StrengthValue_ValueChanged);
            // 
            // agilityValue
            // 
            this.agilityValue.Location = new System.Drawing.Point(495, 235);
            this.agilityValue.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.agilityValue.Name = "agilityValue";
            this.agilityValue.Size = new System.Drawing.Size(120, 20);
            this.agilityValue.TabIndex = 4;
            this.agilityValue.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.agilityValue.ValueChanged += new System.EventHandler(this.AgilityValue_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(429, 237);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(34, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Agility";
            // 
            // enduranceValue
            // 
            this.enduranceValue.Location = new System.Drawing.Point(495, 261);
            this.enduranceValue.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.enduranceValue.Name = "enduranceValue";
            this.enduranceValue.Size = new System.Drawing.Size(120, 20);
            this.enduranceValue.TabIndex = 6;
            this.enduranceValue.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.enduranceValue.ValueChanged += new System.EventHandler(this.EnduranceValue_ValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(429, 263);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Endurance";
            // 
            // vitalityValue
            // 
            this.vitalityValue.Location = new System.Drawing.Point(495, 287);
            this.vitalityValue.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.vitalityValue.Name = "vitalityValue";
            this.vitalityValue.Size = new System.Drawing.Size(120, 20);
            this.vitalityValue.TabIndex = 8;
            this.vitalityValue.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.vitalityValue.ValueChanged += new System.EventHandler(this.VitalityValue_ValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(429, 289);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(37, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Vitality";
            // 
            // intelligenceValue
            // 
            this.intelligenceValue.Location = new System.Drawing.Point(495, 313);
            this.intelligenceValue.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.intelligenceValue.Name = "intelligenceValue";
            this.intelligenceValue.Size = new System.Drawing.Size(120, 20);
            this.intelligenceValue.TabIndex = 10;
            this.intelligenceValue.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.intelligenceValue.ValueChanged += new System.EventHandler(this.IntelligenceValue_ValueChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(429, 315);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(61, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "Intelligence";
            // 
            // wisdomValue
            // 
            this.wisdomValue.Location = new System.Drawing.Point(495, 339);
            this.wisdomValue.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.wisdomValue.Name = "wisdomValue";
            this.wisdomValue.Size = new System.Drawing.Size(120, 20);
            this.wisdomValue.TabIndex = 12;
            this.wisdomValue.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.wisdomValue.ValueChanged += new System.EventHandler(this.WisdomValue_ValueChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(429, 341);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(45, 13);
            this.label6.TabIndex = 11;
            this.label6.Text = "Wisdom";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(428, 13);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(35, 13);
            this.label7.TabIndex = 13;
            this.label7.Text = "Name";
            // 
            // playerName
            // 
            this.playerName.Location = new System.Drawing.Point(469, 13);
            this.playerName.Name = "playerName";
            this.playerName.Size = new System.Drawing.Size(100, 20);
            this.playerName.TabIndex = 14;
            this.playerName.TextChanged += new System.EventHandler(this.PlayerName_TextChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(432, 42);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(21, 13);
            this.label8.TabIndex = 15;
            this.label8.Text = "XP";
            // 
            // playerXP
            // 
            this.playerXP.Location = new System.Drawing.Point(469, 36);
            this.playerXP.Name = "playerXP";
            this.playerXP.Size = new System.Drawing.Size(120, 20);
            this.playerXP.TabIndex = 16;
            this.playerXP.ValueChanged += new System.EventHandler(this.PlayerXP_ValueChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(435, 59);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(36, 13);
            this.label9.TabIndex = 17;
            this.label9.Text = "Image";
            // 
            // imageSelected
            // 
            this.imageSelected.Location = new System.Drawing.Point(477, 59);
            this.imageSelected.Name = "imageSelected";
            this.imageSelected.Size = new System.Drawing.Size(80, 50);
            this.imageSelected.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.imageSelected.TabIndex = 18;
            this.imageSelected.TabStop = false;
            // 
            // imageNext
            // 
            this.imageNext.Location = new System.Drawing.Point(519, 115);
            this.imageNext.Name = "imageNext";
            this.imageNext.Size = new System.Drawing.Size(38, 23);
            this.imageNext.TabIndex = 19;
            this.imageNext.Text = ">";
            this.imageNext.UseVisualStyleBackColor = true;
            this.imageNext.Click += new System.EventHandler(this.ImageNext_Click);
            // 
            // imageLast
            // 
            this.imageLast.Location = new System.Drawing.Point(477, 115);
            this.imageLast.Name = "imageLast";
            this.imageLast.Size = new System.Drawing.Size(38, 23);
            this.imageLast.TabIndex = 20;
            this.imageLast.Text = "<";
            this.imageLast.UseVisualStyleBackColor = true;
            this.imageLast.Click += new System.EventHandler(this.ImageLast_Click);
            // 
            // PlayerCreator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 375);
            this.Controls.Add(this.imageLast);
            this.Controls.Add(this.imageNext);
            this.Controls.Add(this.imageSelected);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.playerXP);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.playerName);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.wisdomValue);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.intelligenceValue);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.vitalityValue);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.enduranceValue);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.agilityValue);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.strengthValue);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.playerView1);
            this.Name = "PlayerCreator";
            this.Text = "PlayerCreator";
            ((System.ComponentModel.ISupportInitialize)(this.strengthValue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.agilityValue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.enduranceValue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.vitalityValue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.intelligenceValue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.wisdomValue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.playerXP)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageSelected)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private PlayerView playerView1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown strengthValue;
        private System.Windows.Forms.NumericUpDown agilityValue;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown enduranceValue;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown vitalityValue;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown intelligenceValue;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown wisdomValue;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox playerName;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.NumericUpDown playerXP;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.PictureBox imageSelected;
        private System.Windows.Forms.Button imageNext;
        private System.Windows.Forms.Button imageLast;
    }
}