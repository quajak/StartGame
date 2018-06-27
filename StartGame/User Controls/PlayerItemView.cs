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
        private HumanPlayer player;
        private List<Item> items = new List<Item>();

        private Item active;

        public void Activate(HumanPlayer player)
        {
            InitializeComponent();
            this.player = player;
            items.AddRange(player.troop.armours);
        }

        private void PlayerItemView_Load(object sender, EventArgs e)
        {
        }

        public void Render()
        {
            items.Clear();
            items.AddRange(player.troop.armours);

            List<Item> itemNames = player.troop.armours.Cast<Item>().ToList();
            List<Item> diff = itemList.Items.Cast<Item>().Except(itemNames).ToList();
            foreach (Item dif in diff)
            {
                itemList.Items.Remove(dif);
            }

            diff = itemNames.Except(itemList.Items.Cast<Item>()).ToList();
            foreach (Item dif in diff)
            {
                itemList.Items.Add(dif);
            }

            if (itemList.SelectedIndex != -1)
            {
                active = items[itemList.SelectedIndex];
                switch (active)
                {
                    case Armour a:
                        itemName.Text = a.name;
                        itemDescription.Text = a.Description;

                        Body dummy = new Body();
                        itemImage.Image = dummy.Render(false, new List<Armour> { a }, 8);

                        itemButton1.Enabled = a.active || !player.troop.armours.Exists(b => b.active && b.affected.Intersect(a.affected).Count() != 0);
                        itemButton1.Visible = true;
                        itemButton1.Text = a.active ? "Take off" : "Wear";
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
            }

            itemButton2.Visible = true;
            itemButton2.Text = "Sell";
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
                    player.money += a.Value;
                    Render();
                    break;

                default:
                    throw new NotImplementedException("This type of item does nt support button 2");
            }
        }
    }
}