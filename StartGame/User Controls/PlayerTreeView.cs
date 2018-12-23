using StartGame.PlayerData;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace StartGame.User_Controls
{
    partial class PlayerTreeView : UserControl
    {
        private Player player;

        public PlayerTreeView()
        {
            InitializeComponent();
        }

        public void Activate(Player Player)
        {
            player = Player;
            Render();
        }

        public void Render()
        {
            List<string> playerTreeNames = player.trees.ConvertAll(t => t.name);
            List<string> diff = treeList.Items.Cast<string>().Except(playerTreeNames).ToList();
            foreach (string dif in diff)
            {
                treeList.Items.Remove(dif);
            }

            diff = playerTreeNames.Except(treeList.Items.Cast<string>()).ToList();
            foreach (string dif in diff)
            {
                treeList.Items.Add(dif);
            }

            int selected = treeList.SelectedIndex;
            if (selected == -1)
            {
                treeName.Text = "";
                treeInformation.Text = "";
                skillLevel.Text = "";
            }
            else
            {
                Tree tree = player.trees[selected];
                treeName.Text = tree.name;
                treeInformation.Text = tree.description + " \n " + "Found by " + tree.reason;
                if (tree is Skill skill)
                {
                    skillLevel.Text = $"Level {skill.level}: {skill.Xp}/{skill.maxXP}";
                }
                else
                {
                    skillLevel.Text = "";
                }
            }
        }

        private void PlayerTreeView_Load(object sender, EventArgs e)
        {
        }

        private void TreeList_SelectedIndexChanged(object sender, EventArgs e)
        {
            Render();
        }
    }
}