using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using StartGame.User_Controls;
using StartGame.PlayerData;

namespace StartGame
{
    internal enum PlayerTab
    { stats, items, statuses, weapons, trees, spells };

    partial class PlayerView : UserControl
    {
        private PlayerBaseStats playerProfile;
        private PlayerItemView itemView;
        private PlayerStatusView playerStatus;
        private PlayerSpellView playerSpell;
        private PlayerWeaponView playerWeapon;
        private PlayerTreeView playerTree;

        private UserControl active;
        private PlayerTab tab;
        private Player player;

        private bool allowAction;

        public PlayerView()
        {
            InitializeComponent();
        }

        public void Activate(Player player, MainGameWindow main, bool AllowAction)
        {
            allowAction = AllowAction;
            this.player = player;
            Point spawnPoint = new Point(0, 20);
            playerProfile = new PlayerBaseStats
            {
                Location = spawnPoint
            };
            playerProfile.Activate(player);
            Controls.Add(playerProfile);

            itemView = new PlayerItemView()
            {
                Location = spawnPoint
            };
            itemView.Activate(player);
            itemView.Visible = false;
            Controls.Add(itemView);

            playerStatus = new PlayerStatusView
            {
                Location = spawnPoint
            };
            playerStatus.Activate(player);
            playerStatus.Visible = false;
            Controls.Add(playerStatus);

            playerSpell = new PlayerSpellView
            {
                Location = spawnPoint
            };
            playerSpell.Activate(player, main, allowAction);
            playerSpell.Visible = false;
            Controls.Add(playerSpell);

            playerWeapon = new PlayerWeaponView
            {
                Location = spawnPoint
            };
            playerWeapon.Visible = false;
            playerWeapon.Activate(player, main, allowAction);
            Controls.Add(playerWeapon);

            playerTree = new PlayerTreeView
            {
                Location = spawnPoint
            };
            playerTree.Visible = false;
            playerTree.Activate(player);
            Controls.Add(playerTree);

            active = playerProfile;
            tab = PlayerTab.stats;
            Render();
        }

        public void Render()
        {
            switch (tab)
            {
                case PlayerTab.stats:
                    if (!(active is PlayerBaseStats))
                    {
                        active.Visible = false;
                    }
                    active = playerProfile;
                    playerProfile.Render();
                    break;

                case PlayerTab.items:
                    if (!(active is PlayerItemView))
                    {
                        active.Visible = false;
                    }
                    active = itemView;
                    itemView.Render();
                    break;

                case PlayerTab.statuses:
                    if (!(active is PlayerStatusView))
                    {
                        active.Visible = false;
                    }
                    active = playerStatus;
                    playerStatus.Render();
                    break;

                case PlayerTab.spells:
                    if (!(active is PlayerSpellView))
                    {
                        active.Visible = false;
                    }
                    active = playerSpell;
                    playerSpell.Render();
                    break;

                case PlayerTab.trees:
                    if (!(active is PlayerTreeView))
                    {
                        active.Visible = false;
                    }
                    active = playerTree;
                    playerTree.Render();
                    break;

                case PlayerTab.weapons:
                    if (!(active is PlayerWeaponView))
                    {
                        active.Visible = false;
                    }
                    active = playerWeapon;
                    playerWeapon.Render();
                    break;

                default:
                    break;
            }
            active.Visible = true;
        }

        private void MainPlayerStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
        }

        private void ItemsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tab = PlayerTab.items;
            Render();
        }

        private void BaseStatsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tab = PlayerTab.stats;
            Render();
        }

        private void StatusesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tab = PlayerTab.statuses;
            Render();
        }

        private void SpellsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tab = PlayerTab.spells;
            Render();
        }

        private void WeaponsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tab = PlayerTab.weapons;
            Render();
        }

        private void SkillsAndTitlesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tab = PlayerTab.trees;
            Render();
        }
    }
}