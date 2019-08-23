namespace StartGame.World.Cities
{
    partial class CityView
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
            this.cityName = new System.Windows.Forms.Label();
            this.cityDescription = new System.Windows.Forms.Label();
            this.buildingList = new System.Windows.Forms.ListBox();
            this.buildingInfo = new System.Windows.Forms.Label();
            this.buildingOptionList = new System.Windows.Forms.ListBox();
            this.actionOptionList = new System.Windows.Forms.ListBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.actionOptionLabel = new System.Windows.Forms.Label();
            this.playerView = new StartGame.PlayerView();
            this.SuspendLayout();
            // 
            // cityName
            // 
            this.cityName.AutoSize = true;
            this.cityName.Font = new System.Drawing.Font("Pristina", 18F);
            this.cityName.Location = new System.Drawing.Point(12, 9);
            this.cityName.Name = "cityName";
            this.cityName.Size = new System.Drawing.Size(92, 32);
            this.cityName.TabIndex = 0;
            this.cityName.Text = "cityName";
            // 
            // cityDescription
            // 
            this.cityDescription.AutoSize = true;
            this.cityDescription.Location = new System.Drawing.Point(13, 45);
            this.cityDescription.MaximumSize = new System.Drawing.Size(400, 0);
            this.cityDescription.Name = "cityDescription";
            this.cityDescription.Size = new System.Drawing.Size(76, 13);
            this.cityDescription.TabIndex = 1;
            this.cityDescription.Text = "cityDescription";
            this.cityDescription.Click += new System.EventHandler(this.CityDescription_Click);
            // 
            // buildingList
            // 
            this.buildingList.FormattingEnabled = true;
            this.buildingList.Location = new System.Drawing.Point(12, 74);
            this.buildingList.Name = "buildingList";
            this.buildingList.Size = new System.Drawing.Size(120, 264);
            this.buildingList.TabIndex = 2;
            this.buildingList.SelectedIndexChanged += new System.EventHandler(this.BuildingList_SelectedIndexChanged);
            // 
            // buildingInfo
            // 
            this.buildingInfo.AutoSize = true;
            this.buildingInfo.Location = new System.Drawing.Point(139, 74);
            this.buildingInfo.MaximumSize = new System.Drawing.Size(120, 200);
            this.buildingInfo.Name = "buildingInfo";
            this.buildingInfo.Size = new System.Drawing.Size(61, 13);
            this.buildingInfo.TabIndex = 3;
            this.buildingInfo.Text = "buildingInfo";
            this.buildingInfo.Visible = false;
            // 
            // buildingOptionList
            // 
            this.buildingOptionList.FormattingEnabled = true;
            this.buildingOptionList.Location = new System.Drawing.Point(138, 165);
            this.buildingOptionList.Name = "buildingOptionList";
            this.buildingOptionList.Size = new System.Drawing.Size(120, 173);
            this.buildingOptionList.TabIndex = 4;
            this.buildingOptionList.Visible = false;
            this.buildingOptionList.SelectedIndexChanged += new System.EventHandler(this.BuildingOptionList_SelectedIndexChanged);
            // 
            // actionOptionList
            // 
            this.actionOptionList.FormattingEnabled = true;
            this.actionOptionList.Location = new System.Drawing.Point(267, 35);
            this.actionOptionList.Name = "actionOptionList";
            this.actionOptionList.Size = new System.Drawing.Size(186, 121);
            this.actionOptionList.TabIndex = 5;
            this.actionOptionList.Visible = false;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(265, 306);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(56, 32);
            this.button1.TabIndex = 6;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = false;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(329, 306);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(56, 32);
            this.button2.TabIndex = 7;
            this.button2.Text = "button2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Visible = false;
            // 
            // actionOptionLabel
            // 
            this.actionOptionLabel.AutoSize = true;
            this.actionOptionLabel.Location = new System.Drawing.Point(264, 165);
            this.actionOptionLabel.MaximumSize = new System.Drawing.Size(186, 200);
            this.actionOptionLabel.Name = "actionOptionLabel";
            this.actionOptionLabel.Size = new System.Drawing.Size(93, 13);
            this.actionOptionLabel.TabIndex = 8;
            this.actionOptionLabel.Text = "actionOptionLabel";
            this.actionOptionLabel.Visible = false;
            this.actionOptionLabel.Click += new System.EventHandler(this.ActionOptionLabel_Click);
            // 
            // playerView
            // 
            this.playerView.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.playerView.Location = new System.Drawing.Point(459, 9);
            this.playerView.Name = "playerView";
            this.playerView.Size = new System.Drawing.Size(408, 350);
            this.playerView.TabIndex = 9;
            // 
            // CityView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(864, 367);
            this.Controls.Add(this.playerView);
            this.Controls.Add(this.actionOptionLabel);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.actionOptionList);
            this.Controls.Add(this.buildingOptionList);
            this.Controls.Add(this.buildingInfo);
            this.Controls.Add(this.buildingList);
            this.Controls.Add(this.cityDescription);
            this.Controls.Add(this.cityName);
            this.Name = "CityView";
            this.Text = "CityView";
            this.Load += new System.EventHandler(this.CityView_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label cityName;
        private System.Windows.Forms.Label cityDescription;
        private System.Windows.Forms.ListBox buildingList;
        private System.Windows.Forms.Label buildingInfo;
        private System.Windows.Forms.ListBox buildingOptionList;
        public System.Windows.Forms.ListBox actionOptionList;
        public System.Windows.Forms.Button button1;
        public System.Windows.Forms.Button button2;
        public System.Windows.Forms.Label actionOptionLabel;
        public PlayerView playerView;
    }
}