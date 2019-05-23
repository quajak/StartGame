using StartGame.GameMap;

namespace StartGame.PlayerData
{
    internal class BanditAI : Player
    {
        public BanditAI(PlayerType Type, string Name, Map Map, Player[] Enemies) : base(Type, Name, Map, Enemies, 3, 0, 2, 0, 1, 3, 5)
        {
        }

        public override void PlayTurn(MainGameWindow main, bool SingleTurn)
        {
            AI.GeneralFighterAI(main, SingleTurn, this, map);
        }
    }

    internal class BearAI : Player
    {
        public BearAI(PlayerType Type, Map Map, Player[] Enemies) : base(Type, "Bear", Map, Enemies, 8, 2, 5, 3, 1, 2, 8)
        {
        }

        public override void PlayTurn(MainGameWindow main, bool SingleTurn)
        {
            AI.GeneralFighterAI(main, SingleTurn, this, map);
        }
    }
    internal class IceElementalAI : Player
    {
        public IceElementalAI(PlayerType Type, Map Map, Player[] Enemies, int damage) : base(Type, "Ice Elemental", Map, Enemies, 0, 6, 4, 4, 2, 0, 5,
            new System.Collections.Generic.List<Spell> { new IceSpikeSpell(damage, 2) })
        {
        }

        public override void PlayTurn(MainGameWindow main, bool SingleTurn)
        {
            AI.GeneralMageAI(main, SingleTurn, this, map, spells);
        }

        public override void Initialise(MainGameWindow main)
        {
            spells.ForEach(s => s.Initialise(main, map));
        }
    }
}