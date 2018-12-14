using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using StartGame.Items;
using StartGame.PlayerData;

namespace StartGame.User_Controls
{
    partial class PlayerItemView : UserControl
    {
        private Player player;
        private List<Item> items = new List<Item>();

        private Item active;

        public void Activate(Player player)
        {
            InitializeComponent();
            this.player = player;
            items.AddRange(player.troop.Items);
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
                        itemButton2.Text = "Sell";
                        break;

                    case Jewelry j:
                        itemName.Text = j.name;
                        itemDescription.Text = j.Description;
                        itemImage.Image = new Bitmap(1, 1);
                        itemButton1.Enabled = j.Active || player.GetJewelryType(j.type).Space;
                        itemButton1.Visible = true;
                        itemButton1.Text = !j.Active ? "Equip" : "Unequip";
                        itemButton2.Visible = true;
                        itemButton2.Text = "Sell";
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

                default:
                    throw new NotImplementedException("This type of item does not support button 1");
            }
        }

        private void ItemButton2_Click(object sender, EventArgs e)
        {
            switch (active)
            {
                case Armour a:
                    //Sell the armour
                    a.active = false;
                    player.troop.armours.Remove(a);
                    player.money.rawValue += a.Value;
                    Render();
                    break;

                case Jewelry j:
                    j.Active = false;
                    player.troop.jewelries.Remove(j);
                    player.money.rawValue += j.Value;
                    Render();
                    break;

                default:
                    throw new NotImplementedException("This type of item does nt support button 2");
            }
        }
    }
}