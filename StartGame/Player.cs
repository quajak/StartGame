using System.Windows.Forms;

namespace StartGame
{
    internal class Player
    {
        private PlayerType type;
        public string Name;

        public Player(PlayerType Type, string Name)
        {
            type = Type;
            this.Name = Name;
        }

        public void PlayTurn(Button actionDescriber)
        {
            if (type == PlayerType.localHuman)
            {
                MessageBox.Show("It is your turn!");
                actionDescriber.Text = "End Turn";
            }
            else
            {
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