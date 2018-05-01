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
            this.gainXP.Size = new System.Drawing.Size(95, 21);
            this.gainXP.TabIndex = 3;
            this.gainXP.Text = "Gain 10 XP";
            this.gainXP.UseVisualStyleBackColor = true;
            this.gainXP.Click += new System.EventHandler(this.gainXP_Click);
            // 
            // DebugEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(211, 294);
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
    }
}