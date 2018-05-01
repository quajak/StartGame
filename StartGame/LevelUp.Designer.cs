namespace PlayerCreator
{
    partial class LevelUp
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
            this.playerName = new System.Windows.Forms.Label();
            this.playerStrength = new System.Windows.Forms.Label();
            this.playerAgiltiy = new System.Windows.Forms.Label();
            this.playerEndurance = new System.Windows.Forms.Label();
            this.playerVitality = new System.Windows.Forms.Label();
            this.strengthUp = new System.Windows.Forms.Button();
            this.agilityUp = new System.Windows.Forms.Button();
            this.enduranceUp = new System.Windows.Forms.Button();
            this.vitalityUp = new System.Windows.Forms.Button();
            this.strengthDown = new System.Windows.Forms.Button();
            this.agilityDown = new System.Windows.Forms.Button();
            this.enduranceDown = new System.Windows.Forms.Button();
            this.vitalityDown = new System.Windows.Forms.Button();
            this.playerMaxHealth = new System.Windows.Forms.Label();
            this.playerDefense = new System.Windows.Forms.Label();
            this.playerActionPoints = new System.Windows.Forms.Label();
            this.ok = new System.Windows.Forms.Button();
            this.playerDodge = new System.Windows.Forms.Label();
            this.wisdomDown = new System.Windows.Forms.Button();
            this.wisdomUp = new System.Windows.Forms.Button();
            this.playerWisdom = new System.Windows.Forms.Label();
            this.intelligenceDown = new System.Windows.Forms.Button();
            this.intelligenceUp = new System.Windows.Forms.Button();
            this.playerIntelligence = new System.Windows.Forms.Label();
            this.playerMana = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // playerName
            // 
            this.playerName.AutoSize = true;
            this.playerName.Location = new System.Drawing.Point(87, 13);
            this.playerName.Name = "playerName";
            this.playerName.Size = new System.Drawing.Size(63, 13);
            this.playerName.TabIndex = 0;
            this.playerName.Text = "playerName";
            // 
            // playerStrength
            // 
            this.playerStrength.AutoSize = true;
            this.playerStrength.Location = new System.Drawing.Point(12, 50);
            this.playerStrength.Name = "playerStrength";
            this.playerStrength.Size = new System.Drawing.Size(75, 13);
            this.playerStrength.TabIndex = 3;
            this.playerStrength.Text = "playerStrength";
            // 
            // playerAgiltiy
            // 
            this.playerAgiltiy.AutoSize = true;
            this.playerAgiltiy.Location = new System.Drawing.Point(12, 71);
            this.playerAgiltiy.Name = "playerAgiltiy";
            this.playerAgiltiy.Size = new System.Drawing.Size(62, 13);
            this.playerAgiltiy.TabIndex = 4;
            this.playerAgiltiy.Text = "playerAgiltiy";
            // 
            // playerEndurance
            // 
            this.playerEndurance.AutoSize = true;
            this.playerEndurance.Location = new System.Drawing.Point(12, 93);
            this.playerEndurance.Name = "playerEndurance";
            this.playerEndurance.Size = new System.Drawing.Size(87, 13);
            this.playerEndurance.TabIndex = 5;
            this.playerEndurance.Text = "playerEndurance";
            // 
            // playerVitality
            // 
            this.playerVitality.AutoSize = true;
            this.playerVitality.Location = new System.Drawing.Point(12, 116);
            this.playerVitality.Name = "playerVitality";
            this.playerVitality.Size = new System.Drawing.Size(65, 13);
            this.playerVitality.TabIndex = 6;
            this.playerVitality.Text = "playerVitality";
            // 
            // strengthUp
            // 
            this.strengthUp.Location = new System.Drawing.Point(118, 49);
            this.strengthUp.Name = "strengthUp";
            this.strengthUp.Size = new System.Drawing.Size(26, 23);
            this.strengthUp.TabIndex = 7;
            this.strengthUp.Text = "+";
            this.strengthUp.UseVisualStyleBackColor = true;
            this.strengthUp.Click += new System.EventHandler(this.StrengthUp_Click);
            // 
            // agilityUp
            // 
            this.agilityUp.Location = new System.Drawing.Point(118, 70);
            this.agilityUp.Name = "agilityUp";
            this.agilityUp.Size = new System.Drawing.Size(26, 23);
            this.agilityUp.TabIndex = 8;
            this.agilityUp.Text = "+";
            this.agilityUp.UseVisualStyleBackColor = true;
            this.agilityUp.Click += new System.EventHandler(this.AgilityUp_Click);
            // 
            // enduranceUp
            // 
            this.enduranceUp.Location = new System.Drawing.Point(118, 92);
            this.enduranceUp.Name = "enduranceUp";
            this.enduranceUp.Size = new System.Drawing.Size(26, 23);
            this.enduranceUp.TabIndex = 9;
            this.enduranceUp.Text = "+";
            this.enduranceUp.UseVisualStyleBackColor = true;
            this.enduranceUp.Click += new System.EventHandler(this.EnduranceUp_Click);
            // 
            // vitalityUp
            // 
            this.vitalityUp.Location = new System.Drawing.Point(118, 115);
            this.vitalityUp.Name = "vitalityUp";
            this.vitalityUp.Size = new System.Drawing.Size(26, 23);
            this.vitalityUp.TabIndex = 10;
            this.vitalityUp.Text = "+";
            this.vitalityUp.UseVisualStyleBackColor = true;
            this.vitalityUp.Click += new System.EventHandler(this.VitalityUp_Click);
            // 
            // strengthDown
            // 
            this.strengthDown.Location = new System.Drawing.Point(144, 49);
            this.strengthDown.Name = "strengthDown";
            this.strengthDown.Size = new System.Drawing.Size(26, 23);
            this.strengthDown.TabIndex = 11;
            this.strengthDown.Text = "-";
            this.strengthDown.UseVisualStyleBackColor = true;
            this.strengthDown.Click += new System.EventHandler(this.StrengthDown_Click);
            // 
            // agilityDown
            // 
            this.agilityDown.Location = new System.Drawing.Point(144, 70);
            this.agilityDown.Name = "agilityDown";
            this.agilityDown.Size = new System.Drawing.Size(26, 23);
            this.agilityDown.TabIndex = 12;
            this.agilityDown.Text = "-";
            this.agilityDown.UseVisualStyleBackColor = true;
            this.agilityDown.Click += new System.EventHandler(this.AgilityDown_Click);
            // 
            // enduranceDown
            // 
            this.enduranceDown.Location = new System.Drawing.Point(144, 92);
            this.enduranceDown.Name = "enduranceDown";
            this.enduranceDown.Size = new System.Drawing.Size(26, 23);
            this.enduranceDown.TabIndex = 13;
            this.enduranceDown.Text = "-";
            this.enduranceDown.UseVisualStyleBackColor = true;
            this.enduranceDown.Click += new System.EventHandler(this.EnduranceDown_Click);
            // 
            // vitalityDown
            // 
            this.vitalityDown.Location = new System.Drawing.Point(144, 115);
            this.vitalityDown.Name = "vitalityDown";
            this.vitalityDown.Size = new System.Drawing.Size(26, 23);
            this.vitalityDown.TabIndex = 14;
            this.vitalityDown.Text = "-";
            this.vitalityDown.UseVisualStyleBackColor = true;
            this.vitalityDown.Click += new System.EventHandler(this.VitalityDown_Click);
            // 
            // playerMaxHealth
            // 
            this.playerMaxHealth.AutoSize = true;
            this.playerMaxHealth.Location = new System.Drawing.Point(186, 49);
            this.playerMaxHealth.Name = "playerMaxHealth";
            this.playerMaxHealth.Size = new System.Drawing.Size(86, 13);
            this.playerMaxHealth.TabIndex = 15;
            this.playerMaxHealth.Text = "playerMaxHealth";
            // 
            // playerDefense
            // 
            this.playerDefense.AutoSize = true;
            this.playerDefense.Location = new System.Drawing.Point(186, 71);
            this.playerDefense.Name = "playerDefense";
            this.playerDefense.Size = new System.Drawing.Size(75, 13);
            this.playerDefense.TabIndex = 16;
            this.playerDefense.Text = "playerDefense";
            // 
            // playerActionPoints
            // 
            this.playerActionPoints.AutoSize = true;
            this.playerActionPoints.Location = new System.Drawing.Point(186, 92);
            this.playerActionPoints.Name = "playerActionPoints";
            this.playerActionPoints.Size = new System.Drawing.Size(94, 13);
            this.playerActionPoints.TabIndex = 17;
            this.playerActionPoints.Text = "playerActionPoints";
            // 
            // ok
            // 
            this.ok.Location = new System.Drawing.Point(197, 226);
            this.ok.Name = "ok";
            this.ok.Size = new System.Drawing.Size(75, 23);
            this.ok.TabIndex = 18;
            this.ok.Text = "OK";
            this.ok.UseVisualStyleBackColor = true;
            this.ok.Click += new System.EventHandler(this.Ok_Click);
            // 
            // playerDodge
            // 
            this.playerDodge.AutoSize = true;
            this.playerDodge.Location = new System.Drawing.Point(186, 115);
            this.playerDodge.Name = "playerDodge";
            this.playerDodge.Size = new System.Drawing.Size(67, 13);
            this.playerDodge.TabIndex = 19;
            this.playerDodge.Text = "playerDodge";
            // 
            // wisdomDown
            // 
            this.wisdomDown.Location = new System.Drawing.Point(144, 138);
            this.wisdomDown.Name = "wisdomDown";
            this.wisdomDown.Size = new System.Drawing.Size(26, 23);
            this.wisdomDown.TabIndex = 22;
            this.wisdomDown.Text = "-";
            this.wisdomDown.UseVisualStyleBackColor = true;
            this.wisdomDown.Click += new System.EventHandler(this.WisdomDown_Click);
            // 
            // wisdomUp
            // 
            this.wisdomUp.Location = new System.Drawing.Point(118, 138);
            this.wisdomUp.Name = "wisdomUp";
            this.wisdomUp.Size = new System.Drawing.Size(26, 23);
            this.wisdomUp.TabIndex = 21;
            this.wisdomUp.Text = "+";
            this.wisdomUp.UseVisualStyleBackColor = true;
            this.wisdomUp.Click += new System.EventHandler(this.WisdomUp_Click);
            // 
            // playerWisdom
            // 
            this.playerWisdom.AutoSize = true;
            this.playerWisdom.Location = new System.Drawing.Point(12, 139);
            this.playerWisdom.Name = "playerWisdom";
            this.playerWisdom.Size = new System.Drawing.Size(73, 13);
            this.playerWisdom.TabIndex = 20;
            this.playerWisdom.Text = "playerWisdom";
            // 
            // intelligenceDown
            // 
            this.intelligenceDown.Location = new System.Drawing.Point(144, 163);
            this.intelligenceDown.Name = "intelligenceDown";
            this.intelligenceDown.Size = new System.Drawing.Size(26, 23);
            this.intelligenceDown.TabIndex = 25;
            this.intelligenceDown.Text = "-";
            this.intelligenceDown.UseVisualStyleBackColor = true;
            this.intelligenceDown.Click += new System.EventHandler(this.IntelligenceDown_Click);
            // 
            // intelligenceUp
            // 
            this.intelligenceUp.Location = new System.Drawing.Point(118, 163);
            this.intelligenceUp.Name = "intelligenceUp";
            this.intelligenceUp.Size = new System.Drawing.Size(26, 23);
            this.intelligenceUp.TabIndex = 24;
            this.intelligenceUp.Text = "+";
            this.intelligenceUp.UseVisualStyleBackColor = true;
            this.intelligenceUp.Click += new System.EventHandler(this.IntelligenceUp_Click);
            // 
            // playerIntelligence
            // 
            this.playerIntelligence.AutoSize = true;
            this.playerIntelligence.Location = new System.Drawing.Point(12, 164);
            this.playerIntelligence.Name = "playerIntelligence";
            this.playerIntelligence.Size = new System.Drawing.Size(89, 13);
            this.playerIntelligence.TabIndex = 23;
            this.playerIntelligence.Text = "playerIntelligence";
            // 
            // playerMana
            // 
            this.playerMana.AutoSize = true;
            this.playerMana.Location = new System.Drawing.Point(186, 138);
            this.playerMana.Name = "playerMana";
            this.playerMana.Size = new System.Drawing.Size(62, 13);
            this.playerMana.TabIndex = 26;
            this.playerMana.Text = "playerMana";
            // 
            // LevelUp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.playerMana);
            this.Controls.Add(this.intelligenceDown);
            this.Controls.Add(this.intelligenceUp);
            this.Controls.Add(this.playerIntelligence);
            this.Controls.Add(this.wisdomDown);
            this.Controls.Add(this.wisdomUp);
            this.Controls.Add(this.playerWisdom);
            this.Controls.Add(this.playerDodge);
            this.Controls.Add(this.ok);
            this.Controls.Add(this.playerActionPoints);
            this.Controls.Add(this.playerDefense);
            this.Controls.Add(this.playerMaxHealth);
            this.Controls.Add(this.vitalityDown);
            this.Controls.Add(this.enduranceDown);
            this.Controls.Add(this.agilityDown);
            this.Controls.Add(this.strengthDown);
            this.Controls.Add(this.vitalityUp);
            this.Controls.Add(this.enduranceUp);
            this.Controls.Add(this.agilityUp);
            this.Controls.Add(this.strengthUp);
            this.Controls.Add(this.playerVitality);
            this.Controls.Add(this.playerEndurance);
            this.Controls.Add(this.playerAgiltiy);
            this.Controls.Add(this.playerStrength);
            this.Controls.Add(this.playerName);
            this.Name = "LevelUp";
            this.Text = "LevelUp";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label playerName;
        private System.Windows.Forms.Label playerStrength;
        private System.Windows.Forms.Label playerAgiltiy;
        private System.Windows.Forms.Label playerEndurance;
        private System.Windows.Forms.Label playerVitality;
        private System.Windows.Forms.Button strengthUp;
        private System.Windows.Forms.Button agilityUp;
        private System.Windows.Forms.Button enduranceUp;
        private System.Windows.Forms.Button vitalityUp;
        private System.Windows.Forms.Button strengthDown;
        private System.Windows.Forms.Button agilityDown;
        private System.Windows.Forms.Button enduranceDown;
        private System.Windows.Forms.Button vitalityDown;
        private System.Windows.Forms.Label playerMaxHealth;
        private System.Windows.Forms.Label playerDefense;
        private System.Windows.Forms.Label playerActionPoints;
        private System.Windows.Forms.Button ok;
        private System.Windows.Forms.Label playerDodge;
        private System.Windows.Forms.Button wisdomDown;
        private System.Windows.Forms.Button wisdomUp;
        private System.Windows.Forms.Label playerWisdom;
        private System.Windows.Forms.Button intelligenceDown;
        private System.Windows.Forms.Button intelligenceUp;
        private System.Windows.Forms.Label playerIntelligence;
        private System.Windows.Forms.Label playerMana;
    }
}