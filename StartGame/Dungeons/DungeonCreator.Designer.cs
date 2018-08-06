namespace StartGame.Dungeons
{
    partial class DungeonCreator
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
            this.dungeonPicture = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.mapWidth = new System.Windows.Forms.NumericUpDown();
            this.mapHeight = new System.Windows.Forms.NumericUpDown();
            this.tileChooser = new System.Windows.Forms.ListBox();
            this.tileSetHeight = new System.Windows.Forms.TrackBar();
            this.tileHeight = new System.Windows.Forms.Label();
            this.isDungeonRoomValid = new System.Windows.Forms.CheckBox();
            this.isDungeonValid = new System.Windows.Forms.CheckBox();
            this.entityChooser = new System.Windows.Forms.ListBox();
            this.addEntity = new System.Windows.Forms.Button();
            this.roomName = new System.Windows.Forms.TextBox();
            this.finishEntityCreation = new System.Windows.Forms.Button();
            this.roomList = new System.Windows.Forms.ListBox();
            this.newRoomName = new System.Windows.Forms.TextBox();
            this.addRoom = new System.Windows.Forms.Button();
            this.commandBar = new System.Windows.Forms.PictureBox();
            this.selectedData = new System.Windows.Forms.Label();
            this.selectedEntities = new System.Windows.Forms.ListBox();
            this.selectedEntityData = new System.Windows.Forms.Label();
            this.showRoomInvalidityMessage = new System.Windows.Forms.Button();
            this.showDungeonInvalidityMessage = new System.Windows.Forms.Button();
            this.setPlayerEntryPoint = new System.Windows.Forms.Button();
            this.setMapTileFree = new System.Windows.Forms.CheckBox();
            this.tilePaintPartSelector = new System.Windows.Forms.CheckedListBox();
            this.loadDungeon = new System.Windows.Forms.Button();
            this.dungeonList = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.dungeonName = new System.Windows.Forms.TextBox();
            this.saveDungeon = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dungeonPicture)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mapWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mapHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tileSetHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.commandBar)).BeginInit();
            this.SuspendLayout();
            // 
            // dungeonPicture
            // 
            this.dungeonPicture.Location = new System.Drawing.Point(12, 12);
            this.dungeonPicture.Name = "dungeonPicture";
            this.dungeonPicture.Size = new System.Drawing.Size(400, 400);
            this.dungeonPicture.TabIndex = 0;
            this.dungeonPicture.TabStop = false;
            this.dungeonPicture.Click += new System.EventHandler(this.DungeonPicture_Click);
            this.dungeonPicture.MouseClick += new System.Windows.Forms.MouseEventHandler(this.DungeonPicture_MouseClick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(419, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(109, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Size (Width x Height):";
            // 
            // mapWidth
            // 
            this.mapWidth.Location = new System.Drawing.Point(535, 12);
            this.mapWidth.Maximum = new decimal(new int[] {
            21,
            0,
            0,
            0});
            this.mapWidth.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.mapWidth.Name = "mapWidth";
            this.mapWidth.Size = new System.Drawing.Size(57, 20);
            this.mapWidth.TabIndex = 2;
            this.mapWidth.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.mapWidth.ValueChanged += new System.EventHandler(this.MapWidth_ValueChanged);
            // 
            // mapHeight
            // 
            this.mapHeight.Location = new System.Drawing.Point(598, 13);
            this.mapHeight.Maximum = new decimal(new int[] {
            21,
            0,
            0,
            0});
            this.mapHeight.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.mapHeight.Name = "mapHeight";
            this.mapHeight.Size = new System.Drawing.Size(57, 20);
            this.mapHeight.TabIndex = 3;
            this.mapHeight.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.mapHeight.ValueChanged += new System.EventHandler(this.MapHeight_ValueChanged);
            // 
            // tileChooser
            // 
            this.tileChooser.FormattingEnabled = true;
            this.tileChooser.Location = new System.Drawing.Point(422, 52);
            this.tileChooser.Name = "tileChooser";
            this.tileChooser.Size = new System.Drawing.Size(120, 95);
            this.tileChooser.TabIndex = 4;
            this.tileChooser.SelectedIndexChanged += new System.EventHandler(this.TileChooser_SelectedIndexChanged);
            // 
            // tileSetHeight
            // 
            this.tileSetHeight.Location = new System.Drawing.Point(424, 153);
            this.tileSetHeight.Name = "tileSetHeight";
            this.tileSetHeight.Size = new System.Drawing.Size(118, 45);
            this.tileSetHeight.TabIndex = 5;
            this.tileSetHeight.Scroll += new System.EventHandler(this.TileSetHeight_Scroll);
            // 
            // tileHeight
            // 
            this.tileHeight.AutoSize = true;
            this.tileHeight.Location = new System.Drawing.Point(424, 183);
            this.tileHeight.Name = "tileHeight";
            this.tileHeight.Size = new System.Drawing.Size(51, 13);
            this.tileHeight.TabIndex = 6;
            this.tileHeight.Text = "tileHeight";
            // 
            // isDungeonRoomValid
            // 
            this.isDungeonRoomValid.AutoCheck = false;
            this.isDungeonRoomValid.AutoSize = true;
            this.isDungeonRoomValid.Location = new System.Drawing.Point(661, 9);
            this.isDungeonRoomValid.Name = "isDungeonRoomValid";
            this.isDungeonRoomValid.Size = new System.Drawing.Size(127, 17);
            this.isDungeonRoomValid.TabIndex = 7;
            this.isDungeonRoomValid.Text = "Dungeon Room Valid";
            this.isDungeonRoomValid.UseVisualStyleBackColor = true;
            // 
            // isDungeonValid
            // 
            this.isDungeonValid.AutoCheck = false;
            this.isDungeonValid.AutoSize = true;
            this.isDungeonValid.Location = new System.Drawing.Point(661, 32);
            this.isDungeonValid.Name = "isDungeonValid";
            this.isDungeonValid.Size = new System.Drawing.Size(96, 17);
            this.isDungeonValid.TabIndex = 8;
            this.isDungeonValid.Text = "Dungeon Valid";
            this.isDungeonValid.UseVisualStyleBackColor = true;
            // 
            // entityChooser
            // 
            this.entityChooser.FormattingEnabled = true;
            this.entityChooser.Location = new System.Drawing.Point(549, 52);
            this.entityChooser.Name = "entityChooser";
            this.entityChooser.Size = new System.Drawing.Size(120, 95);
            this.entityChooser.TabIndex = 9;
            this.entityChooser.SelectedIndexChanged += new System.EventHandler(this.EntityChooser_SelectedIndexChanged);
            // 
            // addEntity
            // 
            this.addEntity.Location = new System.Drawing.Point(549, 153);
            this.addEntity.Name = "addEntity";
            this.addEntity.Size = new System.Drawing.Size(55, 23);
            this.addEntity.TabIndex = 10;
            this.addEntity.Text = "Add";
            this.addEntity.UseVisualStyleBackColor = true;
            this.addEntity.Click += new System.EventHandler(this.AddEntity_Click);
            // 
            // roomName
            // 
            this.roomName.Location = new System.Drawing.Point(424, 28);
            this.roomName.Name = "roomName";
            this.roomName.Size = new System.Drawing.Size(100, 20);
            this.roomName.TabIndex = 11;
            this.roomName.TextChanged += new System.EventHandler(this.RoomName_TextChanged);
            // 
            // finishEntityCreation
            // 
            this.finishEntityCreation.Location = new System.Drawing.Point(611, 153);
            this.finishEntityCreation.Name = "finishEntityCreation";
            this.finishEntityCreation.Size = new System.Drawing.Size(58, 23);
            this.finishEntityCreation.TabIndex = 12;
            this.finishEntityCreation.Text = "Finish";
            this.finishEntityCreation.UseVisualStyleBackColor = true;
            this.finishEntityCreation.Click += new System.EventHandler(this.FinishEntityCreation_Click);
            // 
            // roomList
            // 
            this.roomList.FormattingEnabled = true;
            this.roomList.Location = new System.Drawing.Point(12, 471);
            this.roomList.Name = "roomList";
            this.roomList.Size = new System.Drawing.Size(120, 82);
            this.roomList.TabIndex = 13;
            this.roomList.SelectedIndexChanged += new System.EventHandler(this.RoomList_SelectedIndexChanged);
            // 
            // newRoomName
            // 
            this.newRoomName.Location = new System.Drawing.Point(138, 533);
            this.newRoomName.Name = "newRoomName";
            this.newRoomName.Size = new System.Drawing.Size(100, 20);
            this.newRoomName.TabIndex = 14;
            // 
            // addRoom
            // 
            this.addRoom.Location = new System.Drawing.Point(244, 530);
            this.addRoom.Name = "addRoom";
            this.addRoom.Size = new System.Drawing.Size(75, 23);
            this.addRoom.TabIndex = 15;
            this.addRoom.Text = "New Room";
            this.addRoom.UseVisualStyleBackColor = true;
            this.addRoom.Click += new System.EventHandler(this.AddRoom_Click);
            // 
            // commandBar
            // 
            this.commandBar.Location = new System.Drawing.Point(12, 416);
            this.commandBar.Name = "commandBar";
            this.commandBar.Size = new System.Drawing.Size(400, 20);
            this.commandBar.TabIndex = 16;
            this.commandBar.TabStop = false;
            this.commandBar.Click += new System.EventHandler(this.CommandBar_Click);
            this.commandBar.MouseClick += new System.Windows.Forms.MouseEventHandler(this.CommandBar_MouseClick);
            // 
            // selectedData
            // 
            this.selectedData.AutoSize = true;
            this.selectedData.Location = new System.Drawing.Point(424, 303);
            this.selectedData.Name = "selectedData";
            this.selectedData.Size = new System.Drawing.Size(70, 13);
            this.selectedData.TabIndex = 17;
            this.selectedData.Text = "selectedData";
            this.selectedData.Visible = false;
            // 
            // selectedEntities
            // 
            this.selectedEntities.FormattingEnabled = true;
            this.selectedEntities.Location = new System.Drawing.Point(424, 320);
            this.selectedEntities.Name = "selectedEntities";
            this.selectedEntities.Size = new System.Drawing.Size(120, 95);
            this.selectedEntities.TabIndex = 18;
            this.selectedEntities.Visible = false;
            this.selectedEntities.SelectedIndexChanged += new System.EventHandler(this.SelectedEntities_SelectedIndexChanged);
            // 
            // selectedEntityData
            // 
            this.selectedEntityData.AutoSize = true;
            this.selectedEntityData.Location = new System.Drawing.Point(424, 422);
            this.selectedEntityData.Name = "selectedEntityData";
            this.selectedEntityData.Size = new System.Drawing.Size(96, 13);
            this.selectedEntityData.TabIndex = 19;
            this.selectedEntityData.Text = "selectedEntityData";
            this.selectedEntityData.Visible = false;
            // 
            // showRoomInvalidityMessage
            // 
            this.showRoomInvalidityMessage.Location = new System.Drawing.Point(794, 5);
            this.showRoomInvalidityMessage.Name = "showRoomInvalidityMessage";
            this.showRoomInvalidityMessage.Size = new System.Drawing.Size(20, 23);
            this.showRoomInvalidityMessage.TabIndex = 20;
            this.showRoomInvalidityMessage.Text = "?";
            this.showRoomInvalidityMessage.UseVisualStyleBackColor = true;
            this.showRoomInvalidityMessage.Click += new System.EventHandler(this.ShowRoomInvalidityMessage_Click);
            // 
            // showDungeonInvalidityMessage
            // 
            this.showDungeonInvalidityMessage.Location = new System.Drawing.Point(794, 28);
            this.showDungeonInvalidityMessage.Name = "showDungeonInvalidityMessage";
            this.showDungeonInvalidityMessage.Size = new System.Drawing.Size(20, 23);
            this.showDungeonInvalidityMessage.TabIndex = 22;
            this.showDungeonInvalidityMessage.Text = "?";
            this.showDungeonInvalidityMessage.UseVisualStyleBackColor = true;
            this.showDungeonInvalidityMessage.Click += new System.EventHandler(this.ShowDungeonInvalidityMessage_Click);
            // 
            // setPlayerEntryPoint
            // 
            this.setPlayerEntryPoint.Location = new System.Drawing.Point(139, 471);
            this.setPlayerEntryPoint.Name = "setPlayerEntryPoint";
            this.setPlayerEntryPoint.Size = new System.Drawing.Size(180, 34);
            this.setPlayerEntryPoint.TabIndex = 23;
            this.setPlayerEntryPoint.Text = "Set Player Entry Point";
            this.setPlayerEntryPoint.UseVisualStyleBackColor = true;
            this.setPlayerEntryPoint.Click += new System.EventHandler(this.SetPlayerEntryPoint_Click);
            // 
            // setMapTileFree
            // 
            this.setMapTileFree.AutoSize = true;
            this.setMapTileFree.Location = new System.Drawing.Point(424, 205);
            this.setMapTileFree.Name = "setMapTileFree";
            this.setMapTileFree.Size = new System.Drawing.Size(65, 17);
            this.setMapTileFree.TabIndex = 24;
            this.setMapTileFree.Text = "Blocked";
            this.setMapTileFree.UseVisualStyleBackColor = true;
            // 
            // tilePaintPartSelector
            // 
            this.tilePaintPartSelector.FormattingEnabled = true;
            this.tilePaintPartSelector.Location = new System.Drawing.Point(421, 228);
            this.tilePaintPartSelector.Name = "tilePaintPartSelector";
            this.tilePaintPartSelector.Size = new System.Drawing.Size(120, 64);
            this.tilePaintPartSelector.TabIndex = 25;
            // 
            // loadDungeon
            // 
            this.loadDungeon.Location = new System.Drawing.Point(809, 530);
            this.loadDungeon.Name = "loadDungeon";
            this.loadDungeon.Size = new System.Drawing.Size(52, 23);
            this.loadDungeon.TabIndex = 26;
            this.loadDungeon.Text = "Load";
            this.loadDungeon.UseVisualStyleBackColor = true;
            this.loadDungeon.Click += new System.EventHandler(this.LoadDungeon_Click);
            // 
            // dungeonList
            // 
            this.dungeonList.FormattingEnabled = true;
            this.dungeonList.Location = new System.Drawing.Point(740, 468);
            this.dungeonList.Name = "dungeonList";
            this.dungeonList.Size = new System.Drawing.Size(120, 56);
            this.dungeonList.TabIndex = 27;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 445);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(82, 13);
            this.label2.TabIndex = 28;
            this.label2.Text = "Dungeon Name";
            // 
            // dungeonName
            // 
            this.dungeonName.Location = new System.Drawing.Point(100, 445);
            this.dungeonName.Name = "dungeonName";
            this.dungeonName.Size = new System.Drawing.Size(100, 20);
            this.dungeonName.TabIndex = 29;
            this.dungeonName.Text = "Dungeon";
            this.dungeonName.TextChanged += new System.EventHandler(this.DungeonName_TextChanged);
            // 
            // saveDungeon
            // 
            this.saveDungeon.Location = new System.Drawing.Point(740, 530);
            this.saveDungeon.Name = "saveDungeon";
            this.saveDungeon.Size = new System.Drawing.Size(48, 23);
            this.saveDungeon.TabIndex = 30;
            this.saveDungeon.Text = "Save";
            this.saveDungeon.UseVisualStyleBackColor = true;
            this.saveDungeon.Click += new System.EventHandler(this.SaveDungeon_Click);
            // 
            // DungeonCreator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(873, 566);
            this.Controls.Add(this.saveDungeon);
            this.Controls.Add(this.dungeonName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.dungeonList);
            this.Controls.Add(this.loadDungeon);
            this.Controls.Add(this.tilePaintPartSelector);
            this.Controls.Add(this.setMapTileFree);
            this.Controls.Add(this.setPlayerEntryPoint);
            this.Controls.Add(this.showDungeonInvalidityMessage);
            this.Controls.Add(this.showRoomInvalidityMessage);
            this.Controls.Add(this.selectedEntityData);
            this.Controls.Add(this.selectedEntities);
            this.Controls.Add(this.selectedData);
            this.Controls.Add(this.commandBar);
            this.Controls.Add(this.addRoom);
            this.Controls.Add(this.newRoomName);
            this.Controls.Add(this.roomList);
            this.Controls.Add(this.finishEntityCreation);
            this.Controls.Add(this.roomName);
            this.Controls.Add(this.addEntity);
            this.Controls.Add(this.entityChooser);
            this.Controls.Add(this.isDungeonValid);
            this.Controls.Add(this.isDungeonRoomValid);
            this.Controls.Add(this.tileHeight);
            this.Controls.Add(this.tileSetHeight);
            this.Controls.Add(this.tileChooser);
            this.Controls.Add(this.mapHeight);
            this.Controls.Add(this.mapWidth);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dungeonPicture);
            this.Name = "DungeonCreator";
            this.Text = "DungeonCreator";
            ((System.ComponentModel.ISupportInitialize)(this.dungeonPicture)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mapWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mapHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tileSetHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.commandBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox dungeonPicture;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown mapWidth;
        private System.Windows.Forms.NumericUpDown mapHeight;
        private System.Windows.Forms.ListBox tileChooser;
        private System.Windows.Forms.TrackBar tileSetHeight;
        private System.Windows.Forms.Label tileHeight;
        private System.Windows.Forms.CheckBox isDungeonRoomValid;
        private System.Windows.Forms.CheckBox isDungeonValid;
        private System.Windows.Forms.ListBox entityChooser;
        private System.Windows.Forms.Button addEntity;
        private System.Windows.Forms.TextBox roomName;
        private System.Windows.Forms.Button finishEntityCreation;
        private System.Windows.Forms.ListBox roomList;
        private System.Windows.Forms.TextBox newRoomName;
        private System.Windows.Forms.Button addRoom;
        private System.Windows.Forms.PictureBox commandBar;
        private System.Windows.Forms.Label selectedData;
        private System.Windows.Forms.ListBox selectedEntities;
        private System.Windows.Forms.Label selectedEntityData;
        private System.Windows.Forms.Button showRoomInvalidityMessage;
        private System.Windows.Forms.Button showDungeonInvalidityMessage;
        private System.Windows.Forms.Button setPlayerEntryPoint;
        private System.Windows.Forms.CheckBox setMapTileFree;
        private System.Windows.Forms.CheckedListBox tilePaintPartSelector;
        private System.Windows.Forms.Button loadDungeon;
        private System.Windows.Forms.ListBox dungeonList;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox dungeonName;
        private System.Windows.Forms.Button saveDungeon;
    }
}