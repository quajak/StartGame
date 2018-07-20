namespace StartGame
{
    partial class MainGameMenu
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
            this.startGame = new System.Windows.Forms.Button();
            this.setMap = new System.Windows.Forms.Button();
            this.quit = new System.Windows.Forms.Button();
            this.startCampaign = new System.Windows.Forms.Button();
            this.armourCreator = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 32F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(20, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(241, 51);
            this.label1.TabIndex = 6;
            this.label1.Text = "Start Game";
            // 
            // startGame
            // 
            this.startGame.Location = new System.Drawing.Point(83, 92);
            this.startGame.Name = "startGame";
            this.startGame.Size = new System.Drawing.Size(100, 23);
            this.startGame.TabIndex = 1;
            this.startGame.Text = "Play Single Match";
            this.startGame.UseVisualStyleBackColor = true;
            this.startGame.Click += new System.EventHandler(this.StartGame_Click);
            // 
            // setMap
            // 
            this.setMap.Location = new System.Drawing.Point(83, 121);
            this.setMap.Name = "setMap";
            this.setMap.Size = new System.Drawing.Size(100, 23);
            this.setMap.TabIndex = 2;
            this.setMap.Text = "Set Map";
            this.setMap.UseVisualStyleBackColor = true;
            this.setMap.Click += new System.EventHandler(this.SetMap_Click);
            // 
            // quit
            // 
            this.quit.Location = new System.Drawing.Point(83, 184);
            this.quit.Name = "quit";
            this.quit.Size = new System.Drawing.Size(100, 23);
            this.quit.TabIndex = 4;
            this.quit.Text = "Quit";
            this.quit.UseVisualStyleBackColor = true;
            this.quit.Click += new System.EventHandler(this.Quit_Click);
            // 
            // startCampaign
            // 
            this.startCampaign.Location = new System.Drawing.Point(83, 63);
            this.startCampaign.Name = "startCampaign";
            this.startCampaign.Size = new System.Drawing.Size(100, 23);
            this.startCampaign.TabIndex = 0;
            this.startCampaign.Text = "Start Campaign";
            this.startCampaign.UseVisualStyleBackColor = true;
            this.startCampaign.Click += new System.EventHandler(this.Button1_Click);
            // 
            // armourCreator
            // 
            this.armourCreator.Location = new System.Drawing.Point(83, 231);
            this.armourCreator.Name = "armourCreator";
            this.armourCreator.Size = new System.Drawing.Size(100, 23);
            this.armourCreator.TabIndex = 5;
            this.armourCreator.Text = "Armour Creator";
            this.armourCreator.UseVisualStyleBackColor = true;
            this.armourCreator.Click += new System.EventHandler(this.ArmourCreator_Click);
            // 
            // MainGameMenu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 268);
            this.Controls.Add(this.armourCreator);
            this.Controls.Add(this.startCampaign);
            this.Controls.Add(this.quit);
            this.Controls.Add(this.setMap);
            this.Controls.Add(this.startGame);
            this.Controls.Add(this.label1);
            this.Name = "MainGameMenu";
            this.Text = "MainGameMenu";
            this.Load += new System.EventHandler(this.MainGameMenu_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button startGame;
        private System.Windows.Forms.Button setMap;
        private System.Windows.Forms.Button quit;
        private System.Windows.Forms.Button startCampaign;
        private System.Windows.Forms.Button armourCreator;
    }
}