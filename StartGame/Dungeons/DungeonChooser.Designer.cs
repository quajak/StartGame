namespace StartGame.Dungeons
{
    partial class DungeonChooser
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
            this.loadExternalDungeon = new System.Windows.Forms.Button();
            this.dungeonList = new System.Windows.Forms.ListBox();
            this.loadDungeon = new System.Windows.Forms.Button();
            this.externalDungeonFolderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            this.SuspendLayout();
            // 
            // loadExternalDungeon
            // 
            this.loadExternalDungeon.Location = new System.Drawing.Point(13, 129);
            this.loadExternalDungeon.Name = "loadExternalDungeon";
            this.loadExternalDungeon.Size = new System.Drawing.Size(120, 23);
            this.loadExternalDungeon.TabIndex = 34;
            this.loadExternalDungeon.Text = "Load External";
            this.loadExternalDungeon.UseVisualStyleBackColor = true;
            this.loadExternalDungeon.Click += new System.EventHandler(this.LoadExternalDungeon_Click);
            // 
            // dungeonList
            // 
            this.dungeonList.FormattingEnabled = true;
            this.dungeonList.Location = new System.Drawing.Point(12, 12);
            this.dungeonList.Name = "dungeonList";
            this.dungeonList.Size = new System.Drawing.Size(120, 82);
            this.dungeonList.TabIndex = 33;
            // 
            // loadDungeon
            // 
            this.loadDungeon.Location = new System.Drawing.Point(12, 100);
            this.loadDungeon.Name = "loadDungeon";
            this.loadDungeon.Size = new System.Drawing.Size(121, 23);
            this.loadDungeon.TabIndex = 32;
            this.loadDungeon.Text = "Load";
            this.loadDungeon.UseVisualStyleBackColor = true;
            this.loadDungeon.Click += new System.EventHandler(this.LoadDungeon_Click);
            // 
            // DungeonChooser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(155, 171);
            this.Controls.Add(this.loadExternalDungeon);
            this.Controls.Add(this.dungeonList);
            this.Controls.Add(this.loadDungeon);
            this.Name = "DungeonChooser";
            this.Text = "DungeonChooser";
            this.Load += new System.EventHandler(this.DungeonChooser_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button loadExternalDungeon;
        private System.Windows.Forms.ListBox dungeonList;
        private System.Windows.Forms.Button loadDungeon;
        private System.Windows.Forms.FolderBrowserDialog externalDungeonFolderBrowser;
    }
}