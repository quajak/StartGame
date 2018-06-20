namespace StartGame
{
    partial class DebugEditor
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
            this.incActionPoints = new System.Windows.Forms.Button();
            this.killAllEnemies = new System.Windows.Forms.Button();
            this.winMission = new System.Windows.Forms.Button();
            this.gainXP = new System.Windows.Forms.Button();
            this.GainHealth = new System.Windows.Forms.Button();
            this.CooldownDec = new System.Windows.Forms.Button();
            this.GainMana = new System.Windows.Forms.Button();
            this.gainMoney = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // incActionPoints
            // 
            this.incActionPoints.Location = new System.Drawing.Point(11, 10);
            this.incActionPoints.Name = "incActionPoints";
            this.incActionPoints.Size = new System.Drawing.Size(95, 19);
            this.incActionPoints.TabIndex = 0;
            this.incActionPoints.Text = "Give 10 Action Points";
            this.incActionPoints.UseVisualStyleBackColor = true;
            this.incActionPoints.Click += new System.EventHandler(this.IncActionPoints_Click);
            // 
            // killAllEnemies
            // 
            this.killAllEnemies.Location = new System.Drawing.Point(10, 35);
            this.killAllEnemies.Name = "killAllEnemies";
            this.killAllEnemies.Size = new System.Drawing.Size(96, 19);
            this.killAllEnemies.TabIndex = 1;
            this.killAllEnemies.Text = "Kill all enemies";
            this.killAllEnemies.UseVisualStyleBackColor = true;
            this.killAllEnemies.Click += new System.EventHandler(this.KillAllEnemies_Click);
            // 
            // winMission
            // 
            this.winMission.Location = new System.Drawing.Point(13, 61);
            this.winMission.Name = "winMission";
            this.winMission.Size = new System.Drawing.Size(92, 19);
            this.winMission.TabIndex = 2;
            this.winMission.Text = "Win Mission";
            this.winMission.UseVisualStyleBackColor = true;
            this.winMission.Click += new System.EventHandler(this.WinMission_Click);
            // 
            // gainXP
            // 
            this.gainXP.Location = new System.Drawing.Point(10, 86);
            this.gainXP.Name = "gainXP";
            this.gainXP.Size = new System.Drawing.Size(98, 21);
            this.gainXP.TabIndex = 3;
            this.gainXP.Text = "Gain 10 XP";
            this.gainXP.UseVisualStyleBackColor = true;
            this.gainXP.Click += new System.EventHandler(this.GainXP_Click);
            // 
            // GainHealth
            // 
            this.GainHealth.Location = new System.Drawing.Point(12, 113);
            this.GainHealth.Name = "GainHealth";
            this.GainHealth.Size = new System.Drawing.Size(95, 21);
            this.GainHealth.TabIndex = 4;
            this.GainHealth.Text = "Gain 10 Health";
            this.GainHealth.UseVisualStyleBackColor = true;
            this.GainHealth.Click += new System.EventHandler(this.GainHealth_Click);
            // 
            // CooldownDec
            // 
            this.CooldownDec.Location = new System.Drawing.Point(13, 140);
            this.CooldownDec.Name = "CooldownDec";
            this.CooldownDec.Size = new System.Drawing.Size(95, 21);
            this.CooldownDec.TabIndex = 5;
            this.CooldownDec.Text = "Cooldown dec";
            this.CooldownDec.UseVisualStyleBackColor = true;
            this.CooldownDec.Click += new System.EventHandler(this.CooldownDec_Click);
            // 
            // GainMana
            // 
            this.GainMana.Location = new System.Drawing.Point(10, 167);
            this.GainMana.Name = "GainMana";
            this.GainMana.Size = new System.Drawing.Size(95, 21);
            this.GainMana.TabIndex = 6;
            this.GainMana.Text = "Gain 10 Mana";
            this.GainMana.UseVisualStyleBackColor = true;
            this.GainMana.Click += new System.EventHandler(this.GainMana_Click);
            // 
            // gainMoney
            // 
            this.gainMoney.Location = new System.Drawing.Point(13, 195);
            this.gainMoney.Name = "gainMoney";
            this.gainMoney.Size = new System.Drawing.Size(92, 23);
            this.gainMoney.TabIndex = 7;
            this.gainMoney.Text = "Gain 10 Coins";
            this.gainMoney.UseVisualStyleBackColor = true;
            this.gainMoney.Click += new System.EventHandler(this.GainMoney_Click);
            // 
            // DebugEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(211, 294);
            this.Controls.Add(this.gainMoney);
            this.Controls.Add(this.GainMana);
            this.Controls.Add(this.CooldownDec);
            this.Controls.Add(this.GainHealth);
            this.Controls.Add(this.gainXP);
            this.Controls.Add(this.winMission);
            this.Controls.Add(this.killAllEnemies);
            this.Controls.Add(this.incActionPoints);
            this.Name = "DebugEditor";
            this.Text = "DebugEditor";
            this.Load += new System.EventHandler(this.DebugEditor_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button incActionPoints;
        private System.Windows.Forms.Button killAllEnemies;
        private System.Windows.Forms.Button winMission;
        private System.Windows.Forms.Button gainXP;
        private System.Windows.Forms.Button GainHealth;
        private System.Windows.Forms.Button CooldownDec;
        private System.Windows.Forms.Button GainMana;
        private System.Windows.Forms.Button gainMoney;
    }
}