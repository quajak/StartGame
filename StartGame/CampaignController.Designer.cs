namespace StartGame
{
    partial class CampaignController
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
            this.missionNumber = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.startCampaign = new System.Windows.Forms.Button();
            this.difficultyBar = new System.Windows.Forms.TrackBar();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.missionNumber)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.difficultyBar)).BeginInit();
            this.SuspendLayout();
            // 
            // missionNumber
            // 
            this.missionNumber.Location = new System.Drawing.Point(152, 50);
            this.missionNumber.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.missionNumber.Minimum = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.missionNumber.Name = "missionNumber";
            this.missionNumber.Size = new System.Drawing.Size(120, 20);
            this.missionNumber.TabIndex = 0;
            this.missionNumber.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.label1.Location = new System.Drawing.Point(13, 52);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Missions";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F);
            this.label2.Location = new System.Drawing.Point(52, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(172, 25);
            this.label2.TabIndex = 2;
            this.label2.Text = "Campaign Creator";
            // 
            // startCampaign
            // 
            this.startCampaign.Location = new System.Drawing.Point(94, 226);
            this.startCampaign.Name = "startCampaign";
            this.startCampaign.Size = new System.Drawing.Size(75, 23);
            this.startCampaign.TabIndex = 3;
            this.startCampaign.Text = "Start";
            this.startCampaign.UseVisualStyleBackColor = true;
            this.startCampaign.Click += new System.EventHandler(this.StartCampaign_Click);
            // 
            // difficultyBar
            // 
            this.difficultyBar.Location = new System.Drawing.Point(16, 81);
            this.difficultyBar.Minimum = 1;
            this.difficultyBar.Name = "difficultyBar";
            this.difficultyBar.Size = new System.Drawing.Size(256, 45);
            this.difficultyBar.TabIndex = 4;
            this.difficultyBar.Value = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.label3.Location = new System.Drawing.Point(13, 113);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(30, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Hard";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F);
            this.label4.Location = new System.Drawing.Point(102, 141);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(57, 16);
            this.label4.TabIndex = 6;
            this.label4.Text = "Difficulty";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.label5.Location = new System.Drawing.Point(242, 113);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(30, 13);
            this.label5.TabIndex = 7;
            this.label5.Text = "Easy";
            // 
            // CampaignCreator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.difficultyBar);
            this.Controls.Add(this.startCampaign);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.missionNumber);
            this.Name = "CampaignCreator";
            this.Text = "CampaignCreator";
            this.Load += new System.EventHandler(this.CampaignCreator_Load);
            ((System.ComponentModel.ISupportInitialize)(this.missionNumber)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.difficultyBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown missionNumber;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button startCampaign;
        private System.Windows.Forms.TrackBar difficultyBar;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
    }
}