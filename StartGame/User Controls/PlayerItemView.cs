using StartGame.Forms;
using StartGame.Items;
using StartGame.PlayerData;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace StartGame.User_Controls
{
    partial class PlayerItemView : UserControl
    {
        private Player player;
        private readonly List<Item> items = new List<Item>();
        MainGameWindow mainGame;
        private Item active;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="player"></param>
        /// <param name="mainGame">If set it is taken that the player is in battle</param>
        public void Activate(Player player, MainGameWindow mainGame)
        {
            InitializeComponent();
            this.player = player;
            items.AddRange(player.troop.Items);
            this.mainGame = mainGame;
        }

        private void PlayerItemView_Load(object sender, EventArgs e)
        {
        }

        public void Render()
        {
            items.Clear();
            items.AddRange(player.troop.Items);

            if (itemList.Items.Count != items.Count)
            {
                itemList.Items.Clear();
                itemList.Items.AddRange(items.ToArray());
            }

            if (itemList.SelectedIndex != -1)
            {
                active = items[itemList.SelectedIndex];
                switch (active)
                {
                    //TODO: Compare with active items
                    case Armour a:
                        itemName.Text = a.name;
                        itemDescription.Text = a.Description;

                        Body dummy = new Body();
                        itemImage.Image = dummy.Render(false, new List<Armour> { a }, 8);

                        itemButton1.Enabled = a.active || !player.troop.armours.Exists(b => b.active && a.layer == b.layer && b.affected.Intersect(a.affected).Count() != 0);
                        itemButton1.Visible = true;
                        itemButton1.Text = a.active ? "Unequip" : "Equip";
                        itemButton2.Visible = true;
                        itemButton2.Text = "Drop";
                        break;

                    case Jewelry j:
                        itemName.Text = j.name;
                        itemDescription.Text = j.Description;
                        itemImage.Image = new Bitmap(1, 1);
                        itemButton1.Enabled = j.Active || player.GetJewelryType(j.type).Space;
                        itemButton1.Visible = true;
                        itemButton1.Text = !j.Active ? "Equip" : "Unequip";
                        itemButton2.Visible = true;
                        itemButton2.Text = "Drop";
                        break;

                    case Ammo a:
                        itemName.Text = a.name;
                        itemDescription.Text = a.description;
                        itemImage.Image = new Bitmap(1, 1);
                        itemButton1.Enabled = player.troop.weapons.Exists(w => w is RangedWeapon);
                        itemButton1.Text = "Load Weapon";
                        itemButton1.Visible = true;
                        itemButton2.Visible = true;
                        itemButton2.Text = "Drop";
                        break;

                    case Food f:
                        itemName.Text = f.name;
                        itemDescription.Text = f.Description;
                        itemImage.Image = new Bitmap(1, 1);
                        itemButton1.Enabled = true;
                        itemButton1.Text = "Eat Food";
                        itemButton1.Visible = true;
                        itemButton2.Visible = true;
                        itemButton2.Text = "Drop";
                        break;

                    default:
                        break;
                }
            }
            else
            {
                itemName.Text = "";
                itemDescription.Text = "";
                itemImage.Image = new Bitmap(1, 1);
                itemButton1.Visible = false;
                itemButton2.Visible = false;
            }
        }

        private void ItemList_SelectedIndexChange(object sender, EventArgs e)
        {
            Render();
        }

        private void ItemButton1_Click(object sender, EventArgs e)
        {
            switch (active)
            {
                case Armour a:
                    a.active = !a.active;
                    Render();
                    break;

                case Jewelry j:
                    j.Player = player;
                    j.Active = !j.Active;
                    Render();
                    break;

                case Ammo a:
                    SelectItem selectItem = new SelectItem(player.troop.weapons.Where(w => w is RangedWeapon rw && rw.AmmoType == a.ammoType).Select(w => w as Item).ToList(), true);
                    selectItem.ShowDialog();
                    RangedWeapon r = selectItem.Selected as RangedWeapon;
                    if (r is null)
                        break;
                    r.AddAmo(a);
                    Render();
                    break;

                case Food f:
                    f.UseFood(player);
                    Render();
                    if(mainGame != null)
                    {
                        player.actionPoints.RawValue -= 0.5;
                    }
                    break;

                default:
                    throw new NotImplementedException("This type of item does not support button 1");
            }
        }

        private void ItemButton2_Click(object sender, EventArgs e)
        {
            //The button simply means drop the item
            switch (active)
            {
                case Armour a:
                    a.active = false;
                    player.troop.armours.Remove(a);
                    Render();
                    break;

                case Jewelry j:
                    j.Active = false;
                    player.troop.jewelries.Remove(j);
                    Render();
                    break;

                case Ammo am:
                    player.troop.items.Remove(am);
                    Render();
                    break;
                case Food f:
                    player.troop.items.Remove(f);
                    Render();
                    break;
                default:
                    throw new NotImplementedException("This type of item does not support button 2");
            }
        }
    }
}