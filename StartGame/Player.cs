using System.Windows.Forms;
using PlayerCreator;

namespace StartGame
{
    internal class Player
    {
        private PlayerType type;
        public string Name;
        public Troop troop;
        public double actionPoints = 0;
        public int maxActionPoints = 4;
        public bool active = false;

        public Player(PlayerType Type, string Name)
        {
            type = Type;
            this.Name = Name;
        }

        public void PlayTurn(Button actionDescriber)
        {
            if (type == PlayerType.localHuman)
            {
                actionPoints = maxActionPoints;
                active = true;
                MessageBox.Show("It is your turn!");
                actionDescriber.Text = "End Turn";
            }
            else
            {
                active = false;
                MessageBox.Show($"It is {Name}'s turn!");
                actionDescriber.Text = "Next Turn";
            }
        }

        public void ActionButtonPressed(MainGameWindow mainGameWindow)
        {
            mainGameWindow.NextTurn();
        }
    }

    internal enum PlayerType
    { localHuman, computer };
}