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
    partial class PlayerSpellView : UserControl
    {
        private HumanPlayer player;
        private MainGameWindow main;
        private bool allowAction;

        public PlayerSpellView()
        {
            InitializeComponent();
        }

        public void Activate(HumanPlayer Player, MainGameWindow Main, bool AllowAction)
        {
            allowAction = AllowAction;
            main = Main;
            player = Player;
            Render();
        }

        public void Render()
        {
            List<string> spellNames = player.spells.ConvertAll(t => t.name);
            List<string> diff = spellList.Items.Cast<string>().Except(spellNames).ToList();
            foreach (string dif in diff)
            {
                spellList.Items.Remove(dif);
            }

            diff = spellNames.Except(spellList.Items.Cast<string>()).ToList();
            foreach (string dif in diff)
            {
                spellList.Items.Add(dif);
            }

            int index = spellList.SelectedIndex;
            if (index != -1)
            {
                Spell spell = player.spells[index];
                spellName.Text = spell.name;
                spellDescription.Text = spell.Description(false);
            }
            else
            {
                spellName.Text = "";
                spellDescription.Text = "";
            }

            if (index != -1 && allowAction)
            {
                Spell spell = player.spells[index];
                castSpell.Enabled = main.activePlayer.Name == player.Name && main.gameStarted && spell.manaCost <= player.mana && spell.Ready;
            }
            else if (allowAction)
            {
                castSpell.Enabled = false;
            }
            else
            {
                castSpell.Visible = false;
            }

            castSpell.Text = "Cast spell"; //When casting spell in MainGame code, the text can be changed!
        }

        private void CastSpell_Click(object sender, EventArgs e)
        {
        }

        private void SpellList_SelectedIndexChanged(object sender, EventArgs e)
        {
            Render();
        }
    }
}