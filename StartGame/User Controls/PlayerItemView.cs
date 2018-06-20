using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StartGame.User_Controls
{
    partial class PlayerItemView : UserControl
    {
        private HumanPlayer player;

        public void Activate(HumanPlayer player)
        {
            InitializeComponent();
            this.player = player;
        }

        private void PlayerItemView_Load(object sender, EventArgs e)
        {
        }

        public void Render()
        {
        }
    }
}