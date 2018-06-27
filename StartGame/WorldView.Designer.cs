namespace StartGame
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
            this.nextMission = new System.Windows.Forms.Button();
            this.levelUpButton = new System.Windows.Forms.Button();
            this.lootList = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.gainLoot = new System.Windows.Forms.Button();
            this.spellShopList = new System.Windows.Forms.ListBox();
            this.Label2 = new System.Windows.Forms.Label();
            this.spellInfo = new System.Windows.Forms.Panel();
            this.spellCost = new System.Windows.Forms.Label();
            this.spellShopBuy = new System.Windows.Forms.Button();
            this.spellDescription = new System.Windows.Forms.Label();
            this.spellName = new System.Windows.Forms.Label();
            this.playerView = new StartGame.PlayerView();
            this.gainAllLoot = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.itemShopList = new System.Windows.Forms.ListBox();
            this.shopItemName = new System.Windows.Forms.Label();
            this.shopItemDescription = new System.Windows.Forms.Label();
            this.shopItemPicture = new System.Windows.Forms.PictureBox();
            this.itemShopBuy = new System.Windows.Forms.Button();
            this.spellInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.shopItemPicture)).BeginInit();
            this.SuspendLayout();
            // 
            // nextMission
            // 
            this.nextMission.Location = new System.Drawing.Point(740, 383);
            this.nextMission.Name = "nextMission";
            this.nextMission.Size = new System.Drawing.Size(81, 38);
            this.nextMission.TabIndex = 0;
            this.nextMission.Text = "Next Mission";
            this.nextMission.UseVisualStyleBackColor = true;
            this.nextMission.Click += new System.EventHandler(this.NextMission_Click);
            // 
            // levelUpButton
            // 
            this.levelUpButton.Location = new System.Drawing.Point(12, 12);
            this.levelUpButton.Name = "levelUpButton";
            this.levelUpButton.Size = new System.Drawing.Size(75, 23);
            this.levelUpButton.TabIndex = 1;
            this.levelUpButton.Text = "Level Up";
            this.levelUpButton.UseVisualStyleBackColor = true;
            this.levelUpButton.Click += new System.EventHandler(this.LevelUp_Click);
            // 
            // lootList
            // 
            this.lootList.FormattingEnabled = true;
            this.lootList.Location = new System.Drawing.Point(278, 27);
            this.lootList.Name = "lootList";
            this.lootList.Size = new System.Drawing.Size(120, 95);
            this.lootList.TabIndex = 2;
            this.lootList.SelectedIndexChanged += new System.EventHandler(this.LootList_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(331, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(28, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Loot";
            // 
            // gainLoot
            // 
            this.gainLoot.Location = new System.Drawing.Point(278, 129);
            this.gainLoot.Name = "gainLoot";
            this.gainLoot.Size = new System.Drawing.Size(120, 23);
            this.gainLoot.TabIndex = 4;
            this.gainLoot.Text = "Gain Loot";
            this.gainLoot.UseVisualStyleBackColor = true;
            this.gainLoot.Click += new System.EventHandler(this.GainLoot_Click);
            // 
            // spellShopList
            // 
            this.spellShopList.FormattingEnabled = true;
            this.spellShopList.Location = new System.Drawing.Point(114, 27);
            this.spellShopList.Name = "spellShopList";
            this.spellShopList.Size = new System.Drawing.Size(157, 95);
            this.spellShopList.TabIndex = 5;
            this.spellShopList.SelectedIndexChanged += new System.EventHandler(this.SpellShopList_SelectedIndexChanged);
            // 
            // Label2
            // 
            this.Label2.AutoSize = true;
            this.Label2.Location = new System.Drawing.Point(168, 8);
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(58, 13);
            this.Label2.TabIndex = 6;
            this.Label2.Text = "Spell Shop";
            // 
            // spellInfo
            // 
            this.spellInfo.Controls.Add(this.spellCost);
            this.spellInfo.Controls.Add(this.spellShopBuy);
            this.spellInfo.Controls.Add(this.spellDescription);
            this.spellInfo.Controls.Add(this.spellName);
            this.spellInfo.Location = new System.Drawing.Point(114, 129);
            this.spellInfo.Name = "spellInfo";
            this.spellInfo.Size = new System.Drawing.Size(157, 116);
            this.spellInfo.TabIndex = 8;
            // 
            // spellCost
            // 
            this.spellCost.AutoSize = true;
            this.spellCost.Location = new System.Drawing.Point(11, 74);
            this.spellCost.Name = "spellCost";
            this.spellCost.Size = new System.Drawing.Size(49, 13);
            this.spellCost.TabIndex = 3;
            this.spellCost.Text = "spellCost";
            // 
            // spellShopBuy
            // 
            this.spellShopBuy.Location = new System.Drawing.Point(7, 90);
            this.spellShopBuy.Name = "spellShopBuy";
            this.spellShopBuy.Size = new System.Drawing.Size(75, 23);
            this.spellShopBuy.TabIndex = 2;
            this.spellShopBuy.Text = "Buy Spell";
            this.spellShopBuy.UseVisualStyleBackColor = true;
            this.spellShopBuy.Click += new System.EventHandler(this.SpellShopBuy_Click);
            // 
            // spellDescription
            // 
            this.spellDescription.AutoSize = true;
            this.spellDescription.Location = new System.Drawing.Point(4, 21);
            this.spellDescription.Name = "spellDescription";
            this.spellDescription.Size = new System.Drawing.Size(81, 13);
            this.spellDescription.TabIndex = 1;
            this.spellDescription.Text = "spellDescription";
            // 
            // spellName
            // 
            this.spellName.AutoSize = true;
            this.spellName.Location = new System.Drawing.Point(4, 4);
            this.spellName.Name = "spellName";
            this.spellName.Size = new System.Drawing.Size(56, 13);
            this.spellName.TabIndex = 0;
            this.spellName.Text = "spellName";
            // 
            // playerView
            // 
            this.playerView.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.playerView.Location = new System.Drawing.Point(420, 27);
            this.playerView.Name = "playerView";
            this.playerView.Size = new System.Drawing.Size(408, 350);
            this.playerView.TabIndex = 10;
            // 
            // gainAllLoot
            // 
            this.gainAllLoot.Location = new System.Drawing.Point(278, 159);
            this.gainAllLoot.Name = "gainAllLoot";
            this.gainAllLoot.Size = new System.Drawing.Size(120, 23);
            this.gainAllLoot.TabIndex = 11;
            this.gainAllLoot.Text = "Gain All Loot";
            this.gainAllLoot.UseVisualStyleBackColor = true;
            this.gainAllLoot.Click += new System.EventHandler(this.GainAllLoot_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(84, 256);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(58, 13);
            this.label3.TabIndex = 12;
            this.label3.Text = "Main Shop";
            // 
            // itemShopList
            // 
            this.itemShopList.FormattingEnabled = true;
            this.itemShopList.Location = new System.Drawing.Point(12, 272);
            this.itemShopList.Name = "itemShopList";
            this.itemShopList.Size = new System.Drawing.Size(90, 147);
            this.itemShopList.TabIndex = 13;
            this.itemShopList.SelectedIndexChanged += new System.EventHandler(this.ItemShopList_SelectedIndexChanged);
            // 
            // shopItemName
            // 
            this.shopItemName.AutoSize = true;
            this.shopItemName.Location = new System.Drawing.Point(114, 272);
            this.shopItemName.Name = "shopItemName";
            this.shopItemName.Size = new System.Drawing.Size(78, 13);
            this.shopItemName.TabIndex = 14;
            this.shopItemName.Text = "shopItemName";
            // 
            // shopItemDescription
            // 
            this.shopItemDescription.Location = new System.Drawing.Point(117, 289);
            this.shopItemDescription.Name = "shopItemDescription";
            this.shopItemDescription.Size = new System.Drawing.Size(175, 130);
            this.shopItemDescription.TabIndex = 15;
            this.shopItemDescription.Text = "shopItemDescription";
            // 
            // shopItemPicture
            // 
            this.shopItemPicture.Location = new System.Drawing.Point(298, 272);
            this.shopItemPicture.Name = "shopItemPicture";
            this.shopItemPicture.Size = new System.Drawing.Size(52, 120);
            this.shopItemPicture.TabIndex = 16;
            this.shopItemPicture.TabStop = false;
            // 
            // itemShopBuy
            // 
            this.itemShopBuy.Location = new System.Drawing.Point(298, 398);
            this.itemShopBuy.Name = "itemShopBuy";
            this.itemShopBuy.Size = new System.Drawing.Size(55, 23);
            this.itemShopBuy.TabIndex = 17;
            this.itemShopBuy.Text = "Buy";
            this.itemShopBuy.UseVisualStyleBackColor = true;
            this.itemShopBuy.Click += new System.EventHandler(this.ItemShopBuy_Click);
            // 
            // WorldView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(833, 433);
            this.Controls.Add(this.itemShopBuy);
            this.Controls.Add(this.shopItemPicture);
            this.Controls.Add(this.shopItemDescription);
            this.Controls.Add(this.shopItemName);
            this.Controls.Add(this.itemShopList);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.gainAllLoot);
            this.Controls.Add(this.playerView);
            this.Controls.Add(this.spellInfo);
            this.Controls.Add(this.Label2);
            this.Controls.Add(this.spellShopList);
            this.Controls.Add(this.gainLoot);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lootList);
            this.Controls.Add(this.levelUpButton);
            this.Controls.Add(this.nextMission);
            this.Name = "WorldView";
            this.Text = "WorldView";
            this.Load += new System.EventHandler(this.WorldView_Load);
            this.spellInfo.ResumeLayout(false);
            this.spellInfo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.shopItemPicture)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button nextMission;
        private System.Windows.Forms.Button levelUpButton;
        private System.Windows.Forms.ListBox lootList;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button gainLoot;
        private System.Windows.Forms.ListBox spellShopList;
        private System.Windows.Forms.Label Label2;
        private System.Windows.Forms.Panel spellInfo;
        private System.Windows.Forms.Button spellShopBuy;
        private System.Windows.Forms.Label spellDescription;
        private System.Windows.Forms.Label spellName;
        private System.Windows.Forms.Label spellCost;
        private PlayerView playerView;
        private System.Windows.Forms.Button gainAllLoot;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ListBox itemShopList;
        private System.Windows.Forms.Label shopItemName;
        private System.Windows.Forms.Label shopItemDescription;
        private System.Windows.Forms.PictureBox shopItemPicture;
        private System.Windows.Forms.Button itemShopBuy;
    }
}