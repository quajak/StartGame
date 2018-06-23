using PlayerCreator;
using StartGame.Items;
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
    partial class WorldView : Form
    {
        private readonly HumanPlayer player;
        private readonly CampaignController controller;
        private readonly Campaign campaign;
        private readonly Mission lastMission;

        private List<Item> reward = new List<Item>();

        //TODO: Add discounts

        public WorldView(HumanPlayer player, CampaignController controller, Campaign campaign, Mission lastMission, List<Armour> loot)
        {
            InitializeComponent();
            this.player = player;
            this.controller = controller;
            this.campaign = campaign;
            this.lastMission = lastMission;

            //Enable level up
            levelUp.Enabled = player.storedLevelUps != 0;
            levelUp.Text = $"Level up x{player.storedLevelUps}";

            //Generate reward
            Reward _reward = lastMission.Reward();
            if (!(_reward.weaponReward is null))
            {
                reward.Add(campaign.CalculateWeaponReward((WeaponReward)_reward.weaponReward));
            }
            if (!(_reward.spellReward is null))
            {
                Spell spell = ((SpellReward)_reward.spellReward).spell;
                if (!player.spells.Exists(s => spell.name == s.name))
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

            //Populate shops
            PopulateSpellShop();
            PopulateItemShop();

            playerView.Activate(player, null, false);

            Render();
        }

        private List<Item> itemShopItems;

        private void PopulateItemShop()
        {
            if (itemShopItems is null)
            {
                //Generate items
                //TODO: Take level of player into consideration
                itemShopItems = new List<Item>();

                int itemNumber = player.level + 2;
                for (int i = 0; i < itemNumber; i++)
                {
                    //generate a piece of armour
                    itemShopItems.Add(ArmorPrefabs.CreateArmour(true));
                }
            }

            itemShopList.Items.Clear();
            itemShopList.Items.AddRange(itemShopItems.ToArray());
        }

        private void PopulateSpellShop()
        {
            spellShopList.Items.Clear();
            // Populate spell shop
            List<Spell> spells = World.Instance.Spells;
            spells.RemoveAll(s => player.spells.Exists(p => p == s));
            foreach (var spell in spells)
            {
                spellShopList.Items.Add(spell.name);
            }
        }

        private void Render()
        {
            gainLoot.Enabled = lootList.SelectedIndex != -1;
            if (spellShopList.SelectedIndex != -1)
            {
                Spell selectedSpell = World.Instance.Spells.First(s => s.name == (string)spellShopList.SelectedItem);
                spellShopBuy.Enabled = selectedSpell.buyCost <= player.money;
                spellCost.Text = $"Cost: {selectedSpell.buyCost}";
                spellDescription.Text = selectedSpell.Description(true);
                spellName.Text = selectedSpell.name;
            }
            else
            {
                spellShopBuy.Enabled = false;
                spellCost.Text = "";
                spellDescription.Text = "";
                spellName.Text = "";
            }
            if (itemShopList.SelectedIndex != -1)
            {
                Item item = itemShopItems[itemShopList.SelectedIndex];
                switch (item)
                {
                    case Armour a:
                        shopItemName.Visible = true;
                        shopItemDescription.Visible = true;
                        shopItemPicture.Visible = true;
                        shopItemName.Text = a.name;
                        shopItemDescription.Text = a.Description;
                        shopItemPicture.Image = new Body().Render(false, new List<Armour> { a }, 8);
                        itemShopBuy.Enabled = player.money >= a.Value;
                        break;

                    default:
                        throw new NotImplementedException();
                }
            }
            else
            {
                itemShopBuy.Enabled = false;
                shopItemName.Visible = false;
                shopItemDescription.Visible = false;
                shopItemPicture.Visible = false;
            }

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

            levelUp.Enabled = false;
            levelUp.Text = "Already leveled up";
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
                        lootList.Items.Remove(s.name);
                        reward.Remove(s);
                        break;

                    case Weapon w:
                        WeaponView weaponView = new WeaponView(w, true);
                        weaponView.ShowDialog();
                        if (weaponView.decision)
                        {
                            player.troop.weapons.Add(w);
                        }
                        lootList.Items.Remove(w.name);
                        reward.Remove(w);
                        break;

                    case Coin c:
                        player.money += c.amount;
                        lootList.Items.Remove(c.name);
                        reward.Remove(c);
                        Render();
                        break;

                    default:
                        throw new NotImplementedException($"Reward must be spell, coin or weapon not: {item.GetType()}");
                }
            }
        }

        private void LootList_SelectedIndexChanged(object sender, EventArgs e)
        {
            Render();
        }

        private void SpellShopList_SelectedIndexChanged(object sender, EventArgs e)
        {
            Render();
        }

        private void SpellShopBuy_Click(object sender, EventArgs e)
        {
            if (spellShopList.SelectedIndex != -1 && World.Instance.Spells.First(s => s.name == (string)spellShopList.SelectedItem).buyCost <= player.money)
            {
                Spell spell = World.Instance.GainSpell((string)spellShopList.SelectedItem ?? throw new Exception("Spell bought can't be null"));
                player.money -= spell.buyCost;
                player.spells.Add(spell);

                PopulateSpellShop();
                Render();
            }
            else
            {
                throw new Exception("Should not be able to buy spell when nothing is selected or player has too little money!");
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
                        player.money += c.amount;
                        break;

                    case Armour a:
                        player.troop.armours.Add(a);
                        break;

                    default:
                        throw new NotImplementedException($"Reward must be spell, armour, coin or weapon not: {item.GetType()}");
                }
            }
            reward.Clear();
            lootList.Items.Clear();
            Render();
        }

        private void ItemShopList_SelectedIndexChanged(object sender, EventArgs e)
        {
            Render();
        }

        private void ItemShopBuy_Click(object sender, EventArgs e)
        {
            if (itemShopList.SelectedIndex != -1)
            {
                Item item = itemShopItems[itemShopList.SelectedIndex];
                switch (item)
                {
                    case Armour a:
                        if (a.Value > player.money) throw new Exception();
                        player.money -= a.Value;
                        player.troop.armours.Add(a);
                        itemShopItems.Remove(item);
                        Render();
                        break;

                    default:
                        throw new NotImplementedException();
                }
            }
            else
            {
                throw new Exception("How can this be? Should not be able to buy when nothing is selected");
            }
        }
    }
}