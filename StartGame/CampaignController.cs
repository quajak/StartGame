﻿using PlayerCreator;
using StartGame.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StartGame
{
    partial class CampaignController : Form
    {
        private Player player;
        private Campaign campaign;

        public CampaignController(Player _player)
        {
            player = _player;
            if (player.troop is null)
            {
                MessageBox.Show("Please design your player, before starting the campaign.");
                Troop playerTroop = new Troop("Player", 10, new Weapon(5, AttackType.melee, 1, "Punch", 2, false), Resources.playerTroop, 0);
                playerTroop.weapons.Add(new Weapon(50, AttackType.magic, 40, "GOD", 10, true));
                player = new Player(PlayerType.localHuman, Resources.BasePlayerName, null, null)
                {
                    troop = playerTroop
                };
                //return;
            }
            InitializeComponent();
        }

        private void CampaignCreator_Load(object sender, EventArgs e)
        {
        }

        private void StartCampaign_Click(object sender, EventArgs e)
        {
            campaign = new Campaign(player, (int)missionNumber.Value, difficultyBar.Value);
            campaign.Start();
            Hide();

            do
            {
                campaign.activeGame.ShowDialog();

                if (campaign.activeGame.dead)
                {
                    MessageBox.Show("You have lost the campaign as you have died! Good luck next time!");
                    return;
                }
                //Level up
                LevelUp levelUp = new LevelUp(player, 3 + campaign.difficulty / 5);
                levelUp.ShowDialog();

                //Reset troop stats
                foreach (Weapon weapon in player.troop.weapons)
                {
                    if (weapon.type != AttackType.range)
                        weapon.attacks = weapon.maxAttacks;
                }

                player.troop.health += campaign.healthRegen;
                player.troop.health = player.troop.health > player.troop.maxHealth ? player.troop.maxHealth : player.troop.health;
            } while (campaign.Next());

            MessageBox.Show("You have won!");
        }
    }
}