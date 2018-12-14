using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using StartGame.PlayerData;

namespace StartGame.User_Controls
{
    partial class PlayerStatusView : UserControl
    {
        private Player player;

        public PlayerStatusView()
        {
            InitializeComponent();
        }

        public void Activate(Player player)
        {
            this.player = player;
        }

        public void Render()
        {
            if (player is null) return;
            List<string> playerStatusNames = player.troop.statuses.ConvertAll(t => t.name);
            List<string> diff = statusList.Items.Cast<string>().Except(playerStatusNames).ToList();
            foreach (string dif in diff)
            {
                statusList.Items.Remove(dif);
            }

            if (statusList.SelectedIndex != -1)
            {
                statusTitle.Text = player.troop.statuses[statusList.SelectedIndex].name;
                statusDescription.Text = player.troop.statuses[statusList.SelectedIndex].Description();
            }
            else
            {
                statusTitle.Text = "";
                statusDescription.Text = "";
            }

            diff = playerStatusNames.Except(statusList.Items.Cast<string>()).ToList();
            foreach (string dif in diff)
            {
                statusList.Items.Add(dif);
            }
        }

        private void StatusTitle_Click(object sender, EventArgs e)
        {
        }

        private void PlayerStatusView_Load(object sender, EventArgs e)
        {
        }

        private void StatusList_SelectedIndexChanged(object sender, EventArgs e)
        {
            Render();
        }
    }
}