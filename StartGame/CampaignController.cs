using PlayerCreator;
using StartGame.Items;
using StartGame.PlayerData;
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
                Troop playerTroop = new Troop("Player", new Weapon(5, BaseAttackType.melee, BaseDamageType.blunt, 1, "Punch", 2, false), Resources.playerTroop, 0
                    , null, player)
                {
                    armours = new List<Armour>
                    {
                        new Armour("Woolen Tunic", 50, new List<BodyParts>{BodyParts.LeftUpperArm,BodyParts.RightUpperArm,BodyParts.Torso}, Material.Materials.First(m => m.name == "Wool"),Quality.Common, ArmourLayer.clothing),
                        new Armour("Old Pants", 40, new List<BodyParts> { BodyParts.UpperLegs, BodyParts.LeftLowerLeg, BodyParts.RightLowerLeg, BodyParts.LeftShin, BodyParts.RightShin }, Material.Materials.First(m => m.name == "Cloth"), Quality.Poor, ArmourLayer.clothing),
                        new Armour("Wooden Shoes", 32, new List<BodyParts> { BodyParts.LeftFoot, BodyParts.RightFoot }, Material.Materials.First(m => m.name == "Wood"), Quality.Poor, ArmourLayer.light)
                    }
                };
                playerTroop.armours.ForEach(a => a.active = true);
                playerTroop.weapons.Add(new Weapon(50, BaseAttackType.magic, BaseDamageType.magic, 40, "GOD", 10, true));
                player.troop = playerTroop;
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

                player.mana.rawValue = player.mana.MaxValue().Value;
                player.troop.health.rawValue += campaign.healthRegen;
                player.troop.health.rawValue = player.troop.health.Value > player.troop.health.MaxValue().Value ? player.troop.health.MaxValue().Value : player.troop.health.Value;

                List<Armour> lootableArmour = new List<Armour>();
                foreach (var deadPlayer in campaign.activeGame.killedPlayers)
                {
                    lootableArmour.AddRange(deadPlayer.troop.armours);
                }

                Random random = new Random();
                lootableArmour = lootableArmour.OrderBy(a => random.Next()).ToList(); // I know it is not the most effiecent but that does not matter here

                int chosen = 0;
                List<Armour> loot = new List<Armour>();
                foreach (var lootpiece in lootableArmour)
                {
                    if (random.NextDouble() < 1d / (chosen + 2d))
                    {
                        lootpiece.active = false;
                        loot.Add(lootpiece);
                        chosen++;
                    }
                }
                //Show world map
                WorldView worldView = new WorldView(player, this, campaign, campaign.mission, loot);
                worldView.ShowDialog();
            } while (campaign.Next());

            MessageBox.Show("You have won!");
        }
    }
}