namespace StartGame.User_Controls
{
    partial class PlayerItemView
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.itemList = new System.Windows.Forms.ListBox();
            this.itemName = new System.Windows.Forms.Label();
            this.itemImage = new System.Windows.Forms.PictureBox();
            this.itemDescription = new System.Windows.Forms.Label();
            this.itemButton1 = new System.Windows.Forms.Button();
            this.itemButton2 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.itemImage)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Modern No. 20", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(162, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 25);
            this.label1.TabIndex = 0;
            this.label1.Text = "Items";
            // 
            // itemList
            // 
            this.itemList.FormattingEnabled = true;
            this.itemList.Location = new System.Drawing.Point(4, 64);
            this.itemList.Name = "itemList";
            this.itemList.Size = new System.Drawing.Size(120, 264);
            this.itemList.TabIndex = 1;
            this.itemList.SelectedIndexChanged += new System.EventHandler(this.ItemList_SelectedIndexChange);
            // 
            // itemName
            // 
            this.itemName.AutoSize = true;
            this.itemName.Font = new System.Drawing.Font("Modern No. 20", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.itemName.Location = new System.Drawing.Point(130, 64);
            this.itemName.Name = "itemName";
            this.itemName.Size = new System.Drawing.Size(77, 17);
            this.itemName.TabIndex = 2;
            this.itemName.Text = "itemName";
            // 
            // itemImage
            // 
            this.itemImage.Location = new System.Drawing.Point(293, 85);
            this.itemImage.Name = "itemImage";
            this.itemImage.Size = new System.Drawing.Size(112, 240);
            this.itemImage.TabIndex = 3;
            this.itemImage.TabStop = false;
            // 
            // itemDescription
            // 
            this.itemDescription.AutoSize = true;
            this.itemDescription.Location = new System.Drawing.Point(136, 81);
            this.itemDescription.MaximumSize = new System.Drawing.Size(150, 200);
            this.itemDescription.Name = "itemDescription";
            this.itemDescription.Size = new System.Drawing.Size(79, 13);
            this.itemDescription.TabIndex = 4;
            this.itemDescription.Text = "itemDescription";
            // 
            // itemButton1
            // 
            this.itemButton1.Location = new System.Drawing.Point(131, 301);
            this.itemButton1.Name = "itemButton1";
            this.itemButton1.Size = new System.Drawing.Size(75, 23);
            this.itemButton1.TabIndex = 5;
            this.itemButton1.Text = "itemButton1";
            this.itemButton1.UseVisualStyleBackColor = true;
            this.itemButton1.Click += new System.EventHandler(this.ItemButton1_Click);
            // 
            // itemButton2
            // 
            this.itemButton2.Location = new System.Drawing.Point(133, 272);
            this.itemButton2.Name = "itemButton2";
            this.itemButton2.Size = new System.Drawing.Size(75, 23);
            this.itemButton2.TabIndex = 6;
            this.itemButton2.Text = "itemButton2";
            this.itemButton2.UseVisualStyleBackColor = true;
            this.itemButton2.Click += new System.EventHandler(this.ItemButton2_Click);
            // 
            // PlayerItemView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.Controls.Add(this.itemButton2);
            this.Controls.Add(this.itemButton1);
            this.Controls.Add(this.itemDescription);
            this.Controls.Add(this.itemImage);
            this.Controls.Add(this.itemName);
            this.Controls.Add(this.itemList);
            this.Controls.Add(this.label1);
            this.Name = "PlayerItemView";
            this.Size = new System.Drawing.Size(408, 330);
            this.Load += new System.EventHandler(this.PlayerItemView_Load);
            ((System.ComponentModel.ISupportInitialize)(this.itemImage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox itemList;
        private System.Windows.Forms.Label itemName;
        private System.Windows.Forms.PictureBox itemImage;
        private System.Windows.Forms.Label itemDescription;
        private System.Windows.Forms.Button itemButton1;
        private System.Windows.Forms.Button itemButton2;
    }
}
