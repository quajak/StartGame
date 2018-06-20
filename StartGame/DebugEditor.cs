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
    internal partial class DebugEditor : Form
    {
        private readonly MainGameWindow main;

        public DebugEditor(MainGameWindow main)
        {
            InitializeComponent();
            this.main = main;
        }

        private void DebugEditor_Load(object sender, EventArgs e)
        {
        }

        private void IncActionPoints_Click(object sender, EventArgs e)
        {
            if (main.humanPlayer != null)
                main.humanPlayer.actionPoints += 10;
            main.ShowPlayerStats();
        }

        private void KillAllEnemies_Click(object sender, EventArgs e)
        {
            List<Player> players = new List<Player>(main.players);
            players.ForEach(p =>
            {
                if (p.Name != main.humanPlayer.Name)
                {
                    main.DamagePlayer(100, DamageType.unblockable, p);
                }
            });
        }

        private void WinMission_Click(object sender, EventArgs e)
        {
            main.forceWin = true;
            main.PlayerWins();
        }

        private void GainXP_Click(object sender, EventArgs e)
        {
            main.humanPlayer.GainXP(10);
            main.ShowPlayerStats();
        }

        private void GainHealth_Click(object sender, EventArgs e)
        {
            main.humanPlayer.troop.health += 10;
            main.ShowPlayerStats();
        }

        private void CooldownDec_Click(object sender, EventArgs e)
        {
            main.humanPlayer.spells.ForEach(s => s.coolDown = 0);
            main.UpdateSpellInfo();
        }

        private void GainMana_Click(object sender, EventArgs e)
        {
            main.humanPlayer.mana += 10;
            main.ShowPlayerStats();
        }

        private void GainMoney_Click(object sender, EventArgs e)
        {
            main.humanPlayer.money += 10;
        }
    }
}