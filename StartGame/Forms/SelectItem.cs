using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using StartGame.Items;
namespace StartGame.Forms
{
    public partial class SelectItem : Form
    {
        public Item Selected;
        private readonly List<Item> items;

        public SelectItem(List<Item> items, bool canCancel)
        {
            InitializeComponent();

            if (items.Count == 0)
                throw new ArgumentException();
            itemList.Items.AddRange(items.Select(s => s.name).ToArray());
            cancelButton.Visible = canCancel;
            itemList.SelectedItem = 0;
            this.items = items;
        }

        private void SelectItem_Load(object sender, EventArgs e)
        {

        }

        private void ItemList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(itemList.SelectedItem != null)
            {
                selectButton.Enabled = true;
            }
        }

        private void SelectButton_Click(object sender, EventArgs e)
        {
            if(itemList.SelectedItem != null)
            {
                Selected = items.Find(i => i.name == itemList.SelectedItem as string);
                Close();
            }
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
