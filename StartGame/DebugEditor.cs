using StartGame.PlayerData;
using System;
using System.Collections.Generic;
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
                main.humanPlayer.actionPoints.rawValue += 10;
            main.ShowPlayerStats();
        }

        private void KillAllEnemies_Click(object sender, EventArgs e)
        {
            main.actionOccuring = true;
            List<Player> players = new List<Player>(main.players);
            players.ForEach(p => {
                if (p.Name != main.humanPlayer.Name)
                {
                    main.DamagePlayer(100, DamageType.unblockable, p);
                }
            });
            main.actionOccuring = false;
            main.RenderMap();
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
            main.humanPlayer.troop.health.rawValue += 10;
            main.ShowPlayerStats();
        }

        private void CooldownDec_Click(object sender, EventArgs e)
        {
            main.humanPlayer.spells.ForEach(s => s.coolDown = 0);
            main.UpdateSpellInfo();
        }

        private void GainMana_Click(object sender, EventArgs e)
        {
            main.humanPlayer.mana.rawValue += 10;
            main.ShowPlayerStats();
        }

        private void GainMoney_Click(object sender, EventArgs e)
        {
            main.humanPlayer.money.rawValue += 10;
        }
    }
}