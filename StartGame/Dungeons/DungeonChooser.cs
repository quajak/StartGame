using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StartGame.Dungeons
{
    public partial class DungeonChooser : Form
    {
        public Dungeon selected = null;

        public DungeonChooser()
        {
            InitializeComponent();
            List<string> dungeons = Dungeon.GetDungeons();
            dungeons = dungeons.Where(d => Dungeon.Load(d).IsValid().Item1).ToList(); //TODO: Run all async
            dungeonList.Items.AddRange(dungeons.ToArray());
        }

        private void LoadExternalDungeon_Click(object sender, EventArgs e)
        {
            externalDungeonFolderBrowser.SelectedPath = Directory.GetCurrentDirectory();
            externalDungeonFolderBrowser.Description = "Directory of external dungeon";
            DialogResult dialog = externalDungeonFolderBrowser.ShowDialog();
            if (dialog == DialogResult.OK && externalDungeonFolderBrowser.SelectedPath != Directory.GetCurrentDirectory())
            {
                selected = Dungeon.LoadPath(externalDungeonFolderBrowser.SelectedPath);
                Close();
            }
        }

        private void LoadDungeon_Click(object sender, EventArgs e)
        {
            if (dungeonList.SelectedItem is null) return;
            selected = Dungeon.Load(dungeonList.SelectedItem as string);
            Close();
        }

        private void DungeonChooser_Load(object sender, EventArgs e)
        {
        }
    }
}