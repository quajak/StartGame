using PlayerCreator;
using StartGame.Items;
using StartGame.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StartGame
{
    partial class CampaignController : Form
    {
        private HumanPlayer player;
        private Campaign campaign;

        public CampaignController(HumanPlayer _player)
        {
            player = _player;
            if (player.troop is null)
            {
                MessageBox.Show("Please design your player, before starting the campaign.");
                Troop playerTroop = new Troop("Player", 10, new Weapon(5, BaseAttackType.melee, BaseDamageType.blunt, 1, "Punch", 2, false), Resources.playerTroop, 0, null);
                playerTroop.weapons.Add(new Weapon(50, BaseAttackType.magic, BaseDamageType.magic, 40, "GOD", 10, true));
                player = new HumanPlayer(PlayerType.localHuman, Resources.BasePlayerName, null, null, null, 0)
                {
                    troop = playerTroop
                };
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
                try
                {
                    campaign.activeGame.ShowDialog();
                }
                catch (Exception f)
                {
                    Trace.TraceError(f.ToString());
                    MessageBox.Show(f.ToString());
                }

                if (campaign.activeGame.dead)
                {
                    MessageBox.Show("You have lost the campaign as you have died! Good luck next time!");
                    return;
                }

                //Reset troop stats
                foreach (Weapon weapon in player.troop.weapons)
                {
                    if (weapon.type != BaseAttackType.range)
                        weapon.attacks = weapon.maxAttacks;
                }

                player.troop.health += campaign.healthRegen;
                player.troop.health = player.troop.health > player.troop.maxHealth ? player.troop.maxHealth : player.troop.health;

                //Show world map
                WorldView worldView = new WorldView(player, this, campaign, campaign.mission);
                worldView.ShowDialog();
            } while (campaign.Next());

            MessageBox.Show("You have won!");
        }
    }
}