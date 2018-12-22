using StartGame.Items;
using StartGame.PlayerData;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using StartGame.Mission;
using StartGame.Rendering;

namespace StartGame.World
{
    partial class MissionResult : Form
    {
        WorldRenderer worldRenderer;
        private readonly HumanPlayer player;
        private readonly CampaignController controller;
        private readonly Campaign campaign;
        private readonly Mission.Mission lastMission;
        private Random random = new Random();

        private List<Item> reward = new List<Item>();

        //TODO: Add discounts

        public MissionResult(HumanPlayer player, CampaignController controller, Campaign campaign, Mission.Mission lastMission, List<Armour> loot)
        {
            worldRenderer = new WorldRenderer(World.Instance, player);
            InitializeComponent();
            this.player = player;
            this.controller = controller;
            this.campaign = campaign;
            this.lastMission = lastMission;

            //Enable level up
            levelUpButton.Enabled = player.storedLevelUps != 0;
            levelUpButton.Text = $"Level up x{player.storedLevelUps}";

            //Generate reward
            Reward _reward = lastMission.Reward();
            if (_reward.jewelryReward != null)
            {
                if (_reward.jewelryReward.jewelries != null)
                {
                    reward.AddRange(_reward.jewelryReward.jewelries);
                }

                while (_reward.jewelryReward.number != 0)
                {
                    //Determine the quality
                    MathNet.Numerics.Distributions.Normal normal = new MathNet.Numerics.Distributions.Normal();
                    int offset = (int)normal.InverseCumulativeDistribution(random.NextDouble());
                    int num = E.GetQualityPos(_reward.jewelryReward.quality) + offset;
                    num = Math.Min(Math.Max(num, 0), Enum.GetNames(typeof(Quality)).Length); //Bound the value
                    Quality quality = E.GetQuality(num);
                    reward.Add(Jewelry.GenerateJewelry(quality));
                    _reward.jewelryReward.number--;
                }
            }
            if (!(_reward.weaponReward is null))
            {
                reward.Add(campaign.CalculateWeaponReward(_reward.weaponReward));
            }
            if (!(_reward.spellReward is null))
            {
                Spell spell = _reward.spellReward.spell;
                if (spell != null && !player.spells.Exists(s => spell.name == s.name))
                {
                    reward.Add(spell);
                }
            }
            if (_reward.Money != 0)
            {
                reward.Add(new Coin(_reward.Money));
            }

            reward.AddRange(loot);

            //Populate reward list
            foreach (var item in reward)
            {
                lootList.Items.Add(item);
            }

            playerView.Activate(player, null, false);

            Render();
        }

        private void Render()
        {
            gainLoot.Enabled = lootList.SelectedIndex != -1;
            playerView.Render();
        }

        private void WorldView_Load(object sender, EventArgs e)
        {
        }

        private void NextMission_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void LevelUp_Click(object sender, EventArgs e)
        {
            LevelUp levelUp = new LevelUp(player, player.storedLevelUps);
            levelUp.ShowDialog();

            player.level += player.storedLevelUps;
            player.storedLevelUps = 0;

            levelUpButton.Enabled = false;
            levelUpButton.Text = "Already leveled up";
        }

        private void GainLoot_Click(object sender, EventArgs e)
        {
            if (lootList.SelectedIndex != -1)
            {
                Item item = reward[lootList.SelectedIndex];
                switch (item)
                {
                    case Spell s:
                        player.spells.Add(s);
                        lootList.Items.Remove(s);
                        reward.Remove(s);
                        break;

                    case Weapon w:
                        WeaponView weaponView = new WeaponView(w, true);
                        weaponView.ShowDialog();
                        if (weaponView.decision)
                        {
                            player.troop.weapons.Add(w);
                        }
                        lootList.Items.Remove(w);
                        reward.Remove(w);
                        break;

                    case Coin c:
                        player.money.rawValue += c.amount;
                        lootList.Items.Remove(c);
                        reward.Remove(c);
                        Render();
                        break;

                    case Armour a:
                        player.troop.armours.Add(a);
                        lootList.Items.Remove(a);
                        reward.Remove(a);
                        break;

                    case Jewelry j:
                        player.troop.jewelries.Add(j);
                        lootList.Items.Remove(j);
                        reward.Remove(j);
                        break;

                    default:
                        throw new NotImplementedException($"Reward must be spell, coin or weapon not: {item.GetType()}");
                }
            }
        }

        private void GainAllLoot_Click(object sender, EventArgs e)
        {
            foreach (var item in reward)
            {
                switch (item)
                {
                    case Spell s:
                        player.spells.Add(s);
                        break;

                    case Weapon w:
                        player.troop.weapons.Add(w);
                        break;

                    case Coin c:
                        player.money.rawValue += c.amount;
                        break;

                    case Armour a:
                        player.troop.armours.Add(a);
                        break;

                    case Jewelry j:
                        player.troop.jewelries.Add(j);
                        break;

                    default:
                        throw new NotImplementedException($"Reward must be spell, armour, coin, jewelry or weapon not: {item.GetType()}");
                }
            }
            reward.Clear();
            lootList.Items.Clear();
            Render();
        }

        private void LootList_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }
    }
}