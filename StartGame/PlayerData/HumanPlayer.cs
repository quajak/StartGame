using System;

namespace StartGame.PlayerData
{
    public class HumanPlayer : Player
    {
        public MainGameWindow main;

        public int level = 1;
        public int xp = 0;
        public int levelXP = 5;
        public int storedLevelUps = 0;

        public HumanPlayer(PlayerType Type, string Name, Map Map, Player[] Enemies, MainGameWindow window, int Money) : base(Type, Name, Map, Enemies, 0, 1, 1, 1, 1, 1, 10)
        {
            main = window;
            money = new Attribute(Money, "Money", "Coins");
        }

        public void GainXP(int XP)
        {
            xp += XP;
            if (xp >= levelXP)
            {
                xp -= levelXP;
                storedLevelUps += 1;
                levelXP = Math.Max((int)(1.1 * levelXP), levelXP + 1);

                //Set Level up
                main.SetUpdateState(storedLevelUps > 0);

                //Immediate effect
                troop.health.rawValue = troop.health.MaxValue().Value;
                main.WriteConsole("Level Up! Healing to max hp!");
            }
        }

        public override void PlayTurn(MainGameWindow main, bool SingleTurn)
        {
        }
    }
}