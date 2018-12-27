namespace StartGame.World
{
    partial class WorldView
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
            this.playerView = new StartGame.PlayerView();
            this.worldMapView = new System.Windows.Forms.PictureBox();
            this.focusOnPlayer = new System.Windows.Forms.Button();
            this.gameRunningControl = new System.Windows.Forms.Button();
            this.worldTimeLabel = new System.Windows.Forms.Label();
            this.startMission = new System.Windows.Forms.Button();
            this.generateNewWorld = new System.Windows.Forms.Button();
            this.coords = new System.Windows.Forms.Label();
            this.enterCity = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.worldMapView)).BeginInit();
            this.SuspendLayout();
            // 
            // playerView
            // 
            this.playerView.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.playerView.Location = new System.Drawing.Point(380, 12);
            this.playerView.Name = "playerView";
            this.playerView.Size = new System.Drawing.Size(408, 350);
            this.playerView.TabIndex = 0;
            // 
            // worldMapView
            // 
            this.worldMapView.Location = new System.Drawing.Point(12, 12);
            this.worldMapView.Name = "worldMapView";
            this.worldMapView.Size = new System.Drawing.Size(347, 350);
            this.worldMapView.TabIndex = 1;
            this.worldMapView.TabStop = false;
            this.worldMapView.Click += new System.EventHandler(this.WorldMapView_Click);
            this.worldMapView.MouseClick += new System.Windows.Forms.MouseEventHandler(this.WorldMapView_MouseClick);
            this.worldMapView.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.WorldMapView_MouseDoubleClick);
            this.worldMapView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.WorldMapView_MouseDown);
            this.worldMapView.MouseMove += new System.Windows.Forms.MouseEventHandler(this.WorldMapView_MouseMove);
            this.worldMapView.MouseUp += new System.Windows.Forms.MouseEventHandler(this.WorldMapView_MouseUp);
            // 
            // focusOnPlayer
            // 
            this.focusOnPlayer.Location = new System.Drawing.Point(12, 369);
            this.focusOnPlayer.Name = "focusOnPlayer";
            this.focusOnPlayer.Size = new System.Drawing.Size(75, 23);
            this.focusOnPlayer.TabIndex = 2;
            this.focusOnPlayer.Text = "Show Player";
            this.focusOnPlayer.UseVisualStyleBackColor = true;
            this.focusOnPlayer.Click += new System.EventHandler(this.FocusOnPlayer_Click);
            // 
            // gameRunningControl
            // 
            this.gameRunningControl.Location = new System.Drawing.Point(721, 368);
            this.gameRunningControl.Name = "gameRunningControl";
            this.gameRunningControl.Size = new System.Drawing.Size(66, 51);
            this.gameRunningControl.TabIndex = 3;
            this.gameRunningControl.Text = "Run";
            this.gameRunningControl.UseVisualStyleBackColor = true;
            this.gameRunningControl.Click += new System.EventHandler(this.GameRunningControl_Click);
            // 
            // worldTimeLabel
            // 
            this.worldTimeLabel.AutoSize = true;
            this.worldTimeLabel.Location = new System.Drawing.Point(12, 405);
            this.worldTimeLabel.Name = "worldTimeLabel";
            this.worldTimeLabel.Size = new System.Drawing.Size(26, 13);
            this.worldTimeLabel.TabIndex = 4;
            this.worldTimeLabel.Text = "time";
            // 
            // startMission
            // 
            this.startMission.Location = new System.Drawing.Point(640, 369);
            this.startMission.Name = "startMission";
            this.startMission.Size = new System.Drawing.Size(75, 49);
            this.startMission.TabIndex = 5;
            this.startMission.Text = "Start Mission";
            this.startMission.UseVisualStyleBackColor = true;
            this.startMission.Visible = false;
            this.startMission.Click += new System.EventHandler(this.StartMission_Click);
            // 
            // generateNewWorld
            // 
            this.generateNewWorld.Location = new System.Drawing.Point(94, 368);
            this.generateNewWorld.Name = "generateNewWorld";
            this.generateNewWorld.Size = new System.Drawing.Size(75, 23);
            this.generateNewWorld.TabIndex = 6;
            this.generateNewWorld.Text = "Generate";
            this.generateNewWorld.UseVisualStyleBackColor = true;
            this.generateNewWorld.Click += new System.EventHandler(this.GenerateNewWorld_Click);
            // 
            // coords
            // 
            this.coords.AutoSize = true;
            this.coords.Location = new System.Drawing.Point(94, 405);
            this.coords.Name = "coords";
            this.coords.Size = new System.Drawing.Size(35, 13);
            this.coords.TabIndex = 7;
            this.coords.Text = "label1";
            // 
            // enterCity
            // 
            this.enterCity.Location = new System.Drawing.Point(559, 368);
            this.enterCity.Name = "enterCity";
            this.enterCity.Size = new System.Drawing.Size(75, 50);
            this.enterCity.TabIndex = 8;
            this.enterCity.Text = "Enter City";
            this.enterCity.UseVisualStyleBackColor = true;
            this.enterCity.Visible = false;
            this.enterCity.Click += new System.EventHandler(this.Button1_Click);
            // 
            // WorldView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.enterCity);
            this.Controls.Add(this.coords);
            this.Controls.Add(this.generateNewWorld);
            this.Controls.Add(this.startMission);
            this.Controls.Add(this.worldTimeLabel);
            this.Controls.Add(this.gameRunningControl);
            this.Controls.Add(this.focusOnPlayer);
            this.Controls.Add(this.worldMapView);
            this.Controls.Add(this.playerView);
            this.Name = "WorldView";
            this.Text = "WorldView";
            this.Load += new System.EventHandler(this.WorldView_Load);
            ((System.ComponentModel.ISupportInitialize)(this.worldMapView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private PlayerView playerView;
        private System.Windows.Forms.PictureBox worldMapView;
        private System.Windows.Forms.Button focusOnPlayer;
        private System.Windows.Forms.Button gameRunningControl;
        private System.Windows.Forms.Label worldTimeLabel;
        private System.Windows.Forms.Button startMission;
        private System.Windows.Forms.Button generateNewWorld;
        private System.Windows.Forms.Label coords;
        private System.Windows.Forms.Button enterCity;
    }
}