namespace StartGame.User_Controls
{
    partial class PlayerSpellView
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
            this.label4 = new System.Windows.Forms.Label();
            this.spellInfo = new System.Windows.Forms.Panel();
            this.castSpell = new System.Windows.Forms.Button();
            this.spellDescription = new System.Windows.Forms.Label();
            this.spellName = new System.Windows.Forms.Label();
            this.spellList = new System.Windows.Forms.ListBox();
            this.spellInfo.SuspendLayout();
            this.SuspendLayout();
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.label4.Location = new System.Drawing.Point(120, 15);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(52, 20);
            this.label4.TabIndex = 8;
            this.label4.Text = "Spells";
            // 
            // spellInfo
            // 
            this.spellInfo.Controls.Add(this.castSpell);
            this.spellInfo.Controls.Add(this.spellDescription);
            this.spellInfo.Controls.Add(this.spellName);
            this.spellInfo.Location = new System.Drawing.Point(3, 126);
            this.spellInfo.Name = "spellInfo";
            this.spellInfo.Size = new System.Drawing.Size(294, 110);
            this.spellInfo.TabIndex = 9;
            // 
            // castSpell
            // 
            this.castSpell.Location = new System.Drawing.Point(7, 84);
            this.castSpell.Name = "castSpell";
            this.castSpell.Size = new System.Drawing.Size(119, 23);
            this.castSpell.TabIndex = 2;
            this.castSpell.Text = "Cast Spell";
            this.castSpell.UseVisualStyleBackColor = true;
            this.castSpell.Click += new System.EventHandler(this.CastSpell_Click);
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
            this.spellName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.spellName.Location = new System.Drawing.Point(4, 4);
            this.spellName.Name = "spellName";
            this.spellName.Size = new System.Drawing.Size(65, 13);
            this.spellName.TabIndex = 0;
            this.spellName.Text = "spellName";
            // 
            // spellList
            // 
            this.spellList.FormattingEnabled = true;
            this.spellList.Location = new System.Drawing.Point(3, 38);
            this.spellList.Name = "spellList";
            this.spellList.Size = new System.Drawing.Size(294, 82);
            this.spellList.TabIndex = 10;
            this.spellList.SelectedIndexChanged += new System.EventHandler(this.SpellList_SelectedIndexChanged);
            // 
            // PlayerSpellView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label4);
            this.Controls.Add(this.spellInfo);
            this.Controls.Add(this.spellList);
            this.Name = "PlayerSpellView";
            this.Size = new System.Drawing.Size(300, 250);
            this.spellInfo.ResumeLayout(false);
            this.spellInfo.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel spellInfo;
        private System.Windows.Forms.Button castSpell;
        private System.Windows.Forms.Label spellDescription;
        private System.Windows.Forms.Label spellName;
        private System.Windows.Forms.ListBox spellList;
    }
}
