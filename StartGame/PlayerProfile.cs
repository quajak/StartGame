﻿using StartGame.Properties;
using System;
using System.Windows.Forms;

namespace StartGame
{
    public partial class PlayerProfile : Form
    {
        public PlayerProfile()
        {
            InitializeComponent();
            name.Text = Settings.Default.Name;
        }

        private void Ok_Click(object sender, EventArgs e)
        {
            if (name.Text.Length == 0)
            {
                MessageBox.Show("You're name can not be empty!");
                return;
            }
            Settings.Default.Name = name.Text;
            Settings.Default.Save();
            Close();
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void PlayerProfile_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (name.Text.Length == 0)
            {
                Settings.Default.Name = Resources.BasePlayerName;
                Settings.Default.Save();
            }
        }
    }
}