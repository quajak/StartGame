namespace StartGame.DebugViews
{
    partial class ItemCreator
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
            this.createArmour = new System.Windows.Forms.Button();
            this.armourName = new System.Windows.Forms.TextBox();
            this.playerView = new StartGame.PlayerView();
            this.armourMaterialList = new System.Windows.Forms.ListBox();
            this.baseArmourDurability = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.armourQualityList = new System.Windows.Forms.ListBox();
            this.armourLayerList = new System.Windows.Forms.ListBox();
            this.output = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.jewelryName = new System.Windows.Forms.TextBox();
            this.jewelryQuality = new System.Windows.Forms.ListBox();
            this.jewelryMaterial = new System.Windows.Forms.ListBox();
            this.jewelryCreator = new System.Windows.Forms.Button();
            this.jewelryType = new System.Windows.Forms.ListBox();
            ((System.ComponentModel.ISupportInitialize)(this.baseArmourDurability)).BeginInit();
            this.SuspendLayout();
            // 
            // createArmour
            // 
            this.createArmour.Location = new System.Drawing.Point(1009, 569);
            this.createArmour.Name = "createArmour";
            this.createArmour.Size = new System.Drawing.Size(106, 23);
            this.createArmour.TabIndex = 2;
            this.createArmour.Text = "Create Armour";
            this.createArmour.UseVisualStyleBackColor = true;
            this.createArmour.Click += new System.EventHandler(this.CreateArmour_Click);
            // 
            // armourName
            // 
            this.armourName.Location = new System.Drawing.Point(879, 38);
            this.armourName.Name = "armourName";
            this.armourName.Size = new System.Drawing.Size(224, 20);
            this.armourName.TabIndex = 3;
            // 
            // playerView
            // 
            this.playerView.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.playerView.Location = new System.Drawing.Point(12, 12);
            this.playerView.Name = "playerView";
            this.playerView.Size = new System.Drawing.Size(408, 350);
            this.playerView.TabIndex = 0;
            // 
            // armourMaterialList
            // 
            this.armourMaterialList.FormattingEnabled = true;
            this.armourMaterialList.Location = new System.Drawing.Point(883, 497);
            this.armourMaterialList.Name = "armourMaterialList";
            this.armourMaterialList.Size = new System.Drawing.Size(120, 95);
            this.armourMaterialList.TabIndex = 5;
            // 
            // baseArmourDurability
            // 
            this.baseArmourDurability.Location = new System.Drawing.Point(1009, 497);
            this.baseArmourDurability.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.baseArmourDurability.Name = "baseArmourDurability";
            this.baseArmourDurability.Size = new System.Drawing.Size(94, 20);
            this.baseArmourDurability.TabIndex = 6;
            this.baseArmourDurability.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(1009, 478);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Base Durability";
            // 
            // armourQualityList
            // 
            this.armourQualityList.FormattingEnabled = true;
            this.armourQualityList.Location = new System.Drawing.Point(1009, 380);
            this.armourQualityList.Name = "armourQualityList";
            this.armourQualityList.Size = new System.Drawing.Size(94, 95);
            this.armourQualityList.TabIndex = 8;
            // 
            // armourLayerList
            // 
            this.armourLayerList.FormattingEnabled = true;
            this.armourLayerList.Location = new System.Drawing.Point(1009, 267);
            this.armourLayerList.Name = "armourLayerList";
            this.armourLayerList.Size = new System.Drawing.Size(94, 95);
            this.armourLayerList.TabIndex = 9;
            this.armourLayerList.SelectedIndexChanged += new System.EventHandler(this.ArmourLayerList_SelectedIndexChanged);
            // 
            // output
            // 
            this.output.Location = new System.Drawing.Point(13, 380);
            this.output.Name = "output";
            this.output.Size = new System.Drawing.Size(407, 20);
            this.output.TabIndex = 10;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(963, 12);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(40, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "Armour";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(680, 12);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(42, 13);
            this.label3.TabIndex = 12;
            this.label3.Text = "Jewelry";
            // 
            // jewelryName
            // 
            this.jewelryName.Location = new System.Drawing.Point(669, 38);
            this.jewelryName.Name = "jewelryName";
            this.jewelryName.Size = new System.Drawing.Size(182, 20);
            this.jewelryName.TabIndex = 13;
            // 
            // jewelryQuality
            // 
            this.jewelryQuality.FormattingEnabled = true;
            this.jewelryQuality.Location = new System.Drawing.Point(775, 471);
            this.jewelryQuality.Name = "jewelryQuality";
            this.jewelryQuality.Size = new System.Drawing.Size(94, 95);
            this.jewelryQuality.TabIndex = 14;
            // 
            // jewelryMaterial
            // 
            this.jewelryMaterial.FormattingEnabled = true;
            this.jewelryMaterial.Location = new System.Drawing.Point(672, 471);
            this.jewelryMaterial.Name = "jewelryMaterial";
            this.jewelryMaterial.Size = new System.Drawing.Size(94, 95);
            this.jewelryMaterial.TabIndex = 15;
            // 
            // jewelryCreator
            // 
            this.jewelryCreator.Location = new System.Drawing.Point(669, 572);
            this.jewelryCreator.Name = "jewelryCreator";
            this.jewelryCreator.Size = new System.Drawing.Size(106, 23);
            this.jewelryCreator.TabIndex = 16;
            this.jewelryCreator.Text = "Create Jewelry";
            this.jewelryCreator.UseVisualStyleBackColor = true;
            this.jewelryCreator.Click += new System.EventHandler(this.JewelryCreator_Click);
            // 
            // jewelryType
            // 
            this.jewelryType.FormattingEnabled = true;
            this.jewelryType.Location = new System.Drawing.Point(669, 369);
            this.jewelryType.Name = "jewelryType";
            this.jewelryType.Size = new System.Drawing.Size(97, 95);
            this.jewelryType.TabIndex = 17;
            // 
            // ItemCreator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1115, 618);
            this.Controls.Add(this.jewelryType);
            this.Controls.Add(this.jewelryCreator);
            this.Controls.Add(this.jewelryMaterial);
            this.Controls.Add(this.jewelryQuality);
            this.Controls.Add(this.jewelryName);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.output);
            this.Controls.Add(this.armourLayerList);
            this.Controls.Add(this.armourQualityList);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.baseArmourDurability);
            this.Controls.Add(this.armourMaterialList);
            this.Controls.Add(this.armourName);
            this.Controls.Add(this.createArmour);
            this.Controls.Add(this.playerView);
            this.Name = "ItemCreator";
            this.Text = "ArmourCreator";
            this.Load += new System.EventHandler(this.ArmourCreator_Load);
            ((System.ComponentModel.ISupportInitialize)(this.baseArmourDurability)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private PlayerView playerView;
        private System.Windows.Forms.Button createArmour;
        private System.Windows.Forms.TextBox armourName;
        private System.Windows.Forms.ListBox armourMaterialList;
        private System.Windows.Forms.NumericUpDown baseArmourDurability;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox armourQualityList;
        private System.Windows.Forms.ListBox armourLayerList;
        private System.Windows.Forms.TextBox output;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox jewelryName;
        private System.Windows.Forms.ListBox jewelryQuality;
        private System.Windows.Forms.ListBox jewelryMaterial;
        private System.Windows.Forms.Button jewelryCreator;
        private System.Windows.Forms.ListBox jewelryType;
    }
}