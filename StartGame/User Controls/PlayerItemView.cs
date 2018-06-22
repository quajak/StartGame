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

            List<string> itemNames = items.ConvertAll(t => t.name);
            List<string> diff = itemList.Items.Cast<string>().Except(itemNames).ToList();
            foreach (string dif in diff)
            {
                itemList.Items.Remove(dif);
            }

            diff = itemNames.Except(itemList.Items.Cast<string>()).ToList();
            foreach (string dif in diff)
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

                        itemButton1.Enabled = true;
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
    }
}