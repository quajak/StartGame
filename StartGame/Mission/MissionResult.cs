using StartGame.Items;
using StartGame.Mission;
using StartGame.PlayerData;
using StartGame.Rendering;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace StartGame.World
{
    partial class MissionResult : Form
    {
        private readonly HumanPlayer player;
        private readonly List<Item> reward = new List<Item>();

        //TODO: Add discounts

        public MissionResult(HumanPlayer player, double progression, Mission.Mission lastMission, List<Armour> loot, string closeButtonText)
        {
            InitializeComponent();
            this.player = player;
            nextMission.Text = closeButtonText;

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
                    int offset = (int)normal.InverseCumulativeDistribution(World.random.NextDouble());
                    int num = E.GetQualityPos(_reward.jewelryReward.quality) + offset;
                    num = Math.Min(Math.Max(num, 0), Enum.GetNames(typeof(Quality)).Length); //Bound the value
                    Quality quality = E.GetQuality(num);
                    reward.Add(Jewelry.GenerateJewelry(quality));
                    _reward.jewelryReward.number--;
                }
            }
            if (!(_reward.weaponReward is null))
            {
                reward.Add(Campaign.CalculateWeaponReward(_reward.weaponReward, (int)(progression * 10), 10));
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
            if (_reward.itemReward != null)
            {
                reward.Add(_reward.itemReward.reward);
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
                            lootList.Items.Remove(w);
                            reward.Remove(w);
                        }
                        break;

                    case Coin c:
                        player.Money.RawValue += c.amount;
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

                    case SellableItem s:
                        player.troop.items.Add(s);
                        reward.Remove(s);
                        lootList.Items.Remove(s);
                        break;

                    default:
                        throw new NotImplementedException($"Reward must be spell, coin or weapon not: {item.GetType()}");
                }
                if (lootList.Items.Count != 0)
                    lootList.SelectedIndex = 0;
                else
                    gainAllLoot.Enabled = false;
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
                        player.Money.RawValue += c.amount;
                        break;

                    case Armour a:
                        player.troop.armours.Add(a);
                        break;

                    case Jewelry j:
                        player.troop.jewelries.Add(j);
                        break;
                    case SellableItem s:
                        player.troop.items.Add(s);
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
            gainLoot.Enabled = lootList.SelectedItem != null;
        }
    }
}