using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using StartGame.GameMap;


namespace StartGame.PlayerData
{
    internal class ElementalWizard : Player
    {
        //TODO: Make the AI more difficult by adding more spells, or making him move
        //private readonly int difficulty;

        private int lastHealth;

        public ElementalWizard(PlayerType Type, string Name, Map Map, Player[] Enemies, int Difficulty, int Round) : base(Type, Name, Map, Enemies, 10,
            3 + Round + Difficulty / 2, 1, 1, 5, 1, 10,
                new List<Spell>() { new FireBall(Difficulty / 2 + Round + 1, Difficulty / 3 + 1 ,6 - Difficulty / 2, 0, 0),
                new TeleportSpell(8- Difficulty/2, 0, 0)
            })
        {
            map = Map;
        }

        public override void Initialise(MainGameWindow main)
        {
            spells.ForEach(s => s.Initialise(main, map));
            lastHealth = troop.health.Value;
        }

        public override void PlayTurn(MainGameWindow main, bool singleTurn)
        {
            //Decrease spell cooldown
            foreach (Spell spell in spells)
            {
                spell.coolDown = spell.coolDown == 0 ? 0 : spell.coolDown - 1;
            }

            //Panic if he has been hit or player is close and he is on low health
            if (troop.health.Value != lastHealth || (AIUtility.Distance(troop.Position, enemies[0].troop.Position) < 4 && troop.health.Value != troop.health.MaxValue().Value))
            {
                lastHealth = troop.health.Value;
                //If teleport spell is ready
                if (spells[1].Ready)
                {
                    //Find heighest free space
                    var HeightSorted = from field in map.map.Cast<MapTile>()
                                       where field.free
                                       where AIUtility.Distance(field.position, enemies[0].troop.Position) > 10
                                       orderby field.Height descending
                                       select field;
                    //Teleport
                    spells[1].Activate(new SpellInformation() { positions = new List<Point>() { troop.Position, HeightSorted.Take(1).ToList()[0].position }, mage = this });
                    return; //Finish turn
                }
                else
                {
                    main.WriteConsole("The wizard wimpers");
                    return;
                }
            }

            //Attack
            if (spells[0].Ready)
            {
                spells[0].Activate(new SpellInformation() { positions = new List<Point> { enemies[0].troop.Position }, mage = this });
            }
        }
    }
}