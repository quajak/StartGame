using StartGame.Items;
using System;
using System.Windows.Forms;

namespace StartGame
{
    partial class WeaponView : Form
    {
        public bool decision = false;

        public WeaponView(Weapon weapon, bool isQuestion = false)
        {
            InitializeComponent();
            attacks.Text = $"Attacks: {weapon.maxAttacks}";
            damage.Text = $"Damage: {weapon.attackDamage}";
            name.Text = $"{weapon.name}";
            range.Text = $"Range: {weapon.range}";
            type.Text = $"Type: {weapon.type}";
            discardeable.Text = $"{(weapon.discardeable ? "Can" : "Can not")} be discarded!";
            attackCost.Text = $"{weapon.attackCost} Action {(weapon.attackCost > 1 ? "Points" : "Point")}";
            if (!isQuestion)
            {
                cancel.Visible = false;
                ok.Visible = false;
            }
        }

        private void WeaponView_Load(object sender, EventArgs e)
        {
        }

        private void Ok_Click(object sender, EventArgs e)
        {
            decision = true;
            Close();
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}