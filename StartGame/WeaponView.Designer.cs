namespace StartGame
{
    partial class WeaponView
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
            this.name = new System.Windows.Forms.Label();
            this.damage = new System.Windows.Forms.Label();
            this.attacks = new System.Windows.Forms.Label();
            this.range = new System.Windows.Forms.Label();
            this.type = new System.Windows.Forms.Label();
            this.cancel = new System.Windows.Forms.Button();
            this.ok = new System.Windows.Forms.Button();
            this.discardeable = new System.Windows.Forms.Label();
            this.attackCost = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // name
            // 
            this.name.AutoSize = true;
            this.name.Location = new System.Drawing.Point(121, 9);
            this.name.Name = "name";
            this.name.Size = new System.Drawing.Size(35, 13);
            this.name.TabIndex = 0;
            this.name.Text = "Name";
            // 
            // damage
            // 
            this.damage.AutoSize = true;
            this.damage.Location = new System.Drawing.Point(12, 58);
            this.damage.Name = "damage";
            this.damage.Size = new System.Drawing.Size(45, 13);
            this.damage.TabIndex = 1;
            this.damage.Text = "damage";
            // 
            // attacks
            // 
            this.attacks.AutoSize = true;
            this.attacks.Location = new System.Drawing.Point(14, 103);
            this.attacks.Name = "attacks";
            this.attacks.Size = new System.Drawing.Size(43, 13);
            this.attacks.TabIndex = 2;
            this.attacks.Text = "Attacks";
            // 
            // range
            // 
            this.range.AutoSize = true;
            this.range.Location = new System.Drawing.Point(14, 141);
            this.range.Name = "range";
            this.range.Size = new System.Drawing.Size(34, 13);
            this.range.TabIndex = 3;
            this.range.Text = "range";
            // 
            // type
            // 
            this.type.AutoSize = true;
            this.type.Location = new System.Drawing.Point(14, 184);
            this.type.Name = "type";
            this.type.Size = new System.Drawing.Size(27, 13);
            this.type.TabIndex = 4;
            this.type.Text = "type";
            // 
            // cancel
            // 
            this.cancel.Location = new System.Drawing.Point(205, 277);
            this.cancel.Name = "cancel";
            this.cancel.Size = new System.Drawing.Size(75, 23);
            this.cancel.TabIndex = 5;
            this.cancel.Text = "Cancel";
            this.cancel.UseVisualStyleBackColor = true;
            this.cancel.Click += new System.EventHandler(this.Cancel_Click);
            // 
            // ok
            // 
            this.ok.Location = new System.Drawing.Point(124, 277);
            this.ok.Name = "ok";
            this.ok.Size = new System.Drawing.Size(75, 23);
            this.ok.TabIndex = 6;
            this.ok.Text = "OK";
            this.ok.UseVisualStyleBackColor = true;
            this.ok.Click += new System.EventHandler(this.Ok_Click);
            // 
            // discardeable
            // 
            this.discardeable.AutoSize = true;
            this.discardeable.Location = new System.Drawing.Point(13, 277);
            this.discardeable.Name = "discardeable";
            this.discardeable.Size = new System.Drawing.Size(67, 13);
            this.discardeable.TabIndex = 7;
            this.discardeable.Text = "discardeable";
            // 
            // attackCost
            // 
            this.attackCost.AutoSize = true;
            this.attackCost.Location = new System.Drawing.Point(12, 222);
            this.attackCost.Name = "attackCost";
            this.attackCost.Size = new System.Drawing.Size(58, 13);
            this.attackCost.TabIndex = 8;
            this.attackCost.Text = "attackCost";
            // 
            // WeaponView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 312);
            this.Controls.Add(this.attackCost);
            this.Controls.Add(this.discardeable);
            this.Controls.Add(this.ok);
            this.Controls.Add(this.cancel);
            this.Controls.Add(this.type);
            this.Controls.Add(this.range);
            this.Controls.Add(this.attacks);
            this.Controls.Add(this.damage);
            this.Controls.Add(this.name);
            this.Name = "WeaponView";
            this.Text = "WeaponView";
            this.Load += new System.EventHandler(this.WeaponView_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label name;
        private System.Windows.Forms.Label damage;
        private System.Windows.Forms.Label attacks;
        private System.Windows.Forms.Label range;
        private System.Windows.Forms.Label type;
        private System.Windows.Forms.Button cancel;
        private System.Windows.Forms.Button ok;
        private System.Windows.Forms.Label discardeable;
        private System.Windows.Forms.Label attackCost;
    }
}