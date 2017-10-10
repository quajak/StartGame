using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace StartGame
{
    partial class TroopViewer : Form
    {
        private Troop[] troops;
        private MainGameWindow mainGameWindow;

        public TroopViewer(MainGameWindow MainGameWindow)
        {
            mainGameWindow = MainGameWindow;
            troops = mainGameWindow.troops;
            InitializeComponent();
        }

        private void TroopViewer_Load(object sender, EventArgs e)
        {
            for (int i = 0; i < troops.Length; i++)
            {
                troopList.Items.Add(troops[i].name);
            }
        }

        private void TroopList_SelectedIndexChanged(object sender, EventArgs e)
        {
            formViewer.Image = new Bitmap(formViewer.Width, formViewer.Height);
            if (troopList.SelectedIndex == -1) return;
            //Update info
            Troop troop = troops[troopList.SelectedIndex];
            name.Text = troop.name;
            troopType.Text = troop.type.ToString();
            //Draw shape
            using (Graphics g = Graphics.FromImage(formViewer.Image))
            {
                g.Clear(Color.White);
                Color b = mainGameWindow.troopTypes.Where(t => t.troopType == troop.type).First().borderColor;
                Color f = mainGameWindow.troopTypes.Where(t => t.troopType == troop.type).First().fillColor;
                for (int x = 0; x < 3; x++)
                {
                    for (int y = 0; y < 3; y++)
                    {
                        if (troop.form[x, y])
                        {
                            g.FillRectangle(new SolidBrush(f), x * 30, y * 30, 30, 30);
                            g.DrawRectangle(new Pen(b), x * 30, y * 30, 30, 30);
                        }
                    }
                }
            }
        }
    }
}