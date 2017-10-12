namespace StartGame
{
    partial class MapCreator
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
            this.gameBoard = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.seedInput = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.heightDifference = new System.Windows.Forms.TrackBar();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.Finished = new System.Windows.Forms.Button();
            this.cancel = new System.Windows.Forms.Button();
            this.mapType = new System.Windows.Forms.Label();
            this.getData = new System.Windows.Forms.Button();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.goalChooser = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.debug = new System.Windows.Forms.CheckBox();
            this.randomise = new System.Windows.Forms.Button();
            this.recalculate = new System.Windows.Forms.Button();
            this.pos = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.gameBoard)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.heightDifference)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.goalChooser)).BeginInit();
            this.SuspendLayout();
            // 
            // gameBoard
            // 
            this.gameBoard.Location = new System.Drawing.Point(12, 96);
            this.gameBoard.Name = "gameBoard";
            this.gameBoard.Size = new System.Drawing.Size(310, 310);
            this.gameBoard.TabIndex = 2;
            this.gameBoard.TabStop = false;
            this.gameBoard.MouseClick += new System.Windows.Forms.MouseEventHandler(this.GameBoard_MouseClick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 40F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(204, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(326, 63);
            this.label1.TabIndex = 3;
            this.label1.Text = "Map Creator";
            // 
            // seedInput
            // 
            this.seedInput.Location = new System.Drawing.Point(370, 93);
            this.seedInput.Name = "seedInput";
            this.seedInput.Size = new System.Drawing.Size(209, 20);
            this.seedInput.TabIndex = 4;
            this.seedInput.TextChanged += new System.EventHandler(this.SeedInput_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(329, 96);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Seed:";
            // 
            // heightDifference
            // 
            this.heightDifference.Location = new System.Drawing.Point(332, 151);
            this.heightDifference.Maximum = 9;
            this.heightDifference.Minimum = 1;
            this.heightDifference.Name = "heightDifference";
            this.heightDifference.Size = new System.Drawing.Size(333, 45);
            this.heightDifference.TabIndex = 6;
            this.heightDifference.Value = 5;
            this.heightDifference.Scroll += new System.EventHandler(this.HeightDifference_Scroll);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(639, 135);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(26, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Sea";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(328, 135);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(56, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Mountains";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(478, 135);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(40, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "Normal";
            // 
            // Finished
            // 
            this.Finished.Location = new System.Drawing.Point(548, 346);
            this.Finished.Name = "Finished";
            this.Finished.Size = new System.Drawing.Size(116, 59);
            this.Finished.TabIndex = 10;
            this.Finished.Text = "Finished";
            this.Finished.UseVisualStyleBackColor = true;
            this.Finished.Click += new System.EventHandler(this.Finished_Click);
            // 
            // cancel
            // 
            this.cancel.Location = new System.Drawing.Point(426, 347);
            this.cancel.Name = "cancel";
            this.cancel.Size = new System.Drawing.Size(116, 59);
            this.cancel.TabIndex = 11;
            this.cancel.Text = "Cancel";
            this.cancel.UseVisualStyleBackColor = true;
            this.cancel.Click += new System.EventHandler(this.Cancel_Click);
            // 
            // mapType
            // 
            this.mapType.AutoSize = true;
            this.mapType.Location = new System.Drawing.Point(332, 190);
            this.mapType.Name = "mapType";
            this.mapType.Size = new System.Drawing.Size(58, 13);
            this.mapType.TabIndex = 12;
            this.mapType.Text = "Map Type:";
            // 
            // getData
            // 
            this.getData.Location = new System.Drawing.Point(329, 381);
            this.getData.Name = "getData";
            this.getData.Size = new System.Drawing.Size(75, 23);
            this.getData.TabIndex = 13;
            this.getData.Text = "Calculate";
            this.getData.UseVisualStyleBackColor = true;
            this.getData.Click += new System.EventHandler(this.getData_Click);
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Location = new System.Drawing.Point(433, 309);
            this.numericUpDown1.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(120, 20);
            this.numericUpDown1.TabIndex = 15;
            this.numericUpDown1.ValueChanged += new System.EventHandler(this.NumericUpDown1_ValueChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(334, 311);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(93, 13);
            this.label6.TabIndex = 16;
            this.label6.Text = "Continent strength";
            // 
            // goalChooser
            // 
            this.goalChooser.Location = new System.Drawing.Point(433, 283);
            this.goalChooser.Maximum = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.goalChooser.Name = "goalChooser";
            this.goalChooser.Size = new System.Drawing.Size(120, 20);
            this.goalChooser.TabIndex = 17;
            this.goalChooser.ValueChanged += new System.EventHandler(this.GoalChooser_ValueChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(334, 290);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(88, 13);
            this.label7.TabIndex = 18;
            this.label7.Text = "Show dis for goal";
            // 
            // debug
            // 
            this.debug.AutoSize = true;
            this.debug.Location = new System.Drawing.Point(433, 260);
            this.debug.Name = "debug";
            this.debug.Size = new System.Drawing.Size(58, 17);
            this.debug.TabIndex = 19;
            this.debug.Text = "Debug";
            this.debug.UseVisualStyleBackColor = true;
            // 
            // randomise
            // 
            this.randomise.Location = new System.Drawing.Point(585, 91);
            this.randomise.Name = "randomise";
            this.randomise.Size = new System.Drawing.Size(79, 23);
            this.randomise.TabIndex = 20;
            this.randomise.Text = "Randomise";
            this.randomise.UseVisualStyleBackColor = true;
            this.randomise.Click += new System.EventHandler(this.Randomise_Click);
            // 
            // recalculate
            // 
            this.recalculate.Location = new System.Drawing.Point(332, 352);
            this.recalculate.Name = "recalculate";
            this.recalculate.Size = new System.Drawing.Size(75, 23);
            this.recalculate.TabIndex = 21;
            this.recalculate.Text = "Recalculate";
            this.recalculate.UseVisualStyleBackColor = true;
            this.recalculate.Click += new System.EventHandler(this.Recalculate_Click);
            // 
            // pos
            // 
            this.pos.AutoSize = true;
            this.pos.Location = new System.Drawing.Point(12, 77);
            this.pos.Name = "pos";
            this.pos.Size = new System.Drawing.Size(44, 13);
            this.pos.TabIndex = 22;
            this.pos.Text = "Position";
            // 
            // MapCreator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(683, 430);
            this.Controls.Add(this.pos);
            this.Controls.Add(this.recalculate);
            this.Controls.Add(this.randomise);
            this.Controls.Add(this.debug);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.goalChooser);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.numericUpDown1);
            this.Controls.Add(this.getData);
            this.Controls.Add(this.mapType);
            this.Controls.Add(this.cancel);
            this.Controls.Add(this.Finished);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.heightDifference);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.seedInput);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.gameBoard);
            this.Name = "MapCreator";
            this.Text = "MapCreator";
            this.Load += new System.EventHandler(this.MapCreator_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gameBoard)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.heightDifference)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.goalChooser)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox gameBoard;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox seedInput;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TrackBar heightDifference;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button Finished;
        private System.Windows.Forms.Button cancel;
        private System.Windows.Forms.Label mapType;
        private System.Windows.Forms.Button getData;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown goalChooser;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.CheckBox debug;
        private System.Windows.Forms.Button randomise;
        private System.Windows.Forms.Button recalculate;
        private System.Windows.Forms.Label pos;
    }
}