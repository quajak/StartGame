using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartGame
{
    internal class World
    {
        private static World world;

        public static World Instance
        {
            get
            {
                if (world is null) world = new World();
                return world;
            }
            set => world = value;
        }

        public List<Spell> Spells { get => spells; }

        private List<Spell> spells;

        private World()
        {
            spells = new List<Spell>()
            {
                new HealingSpell(5, 15),
                new EarthQuakeSpell(5, 5, 45),
                new LightningBoltSpell(15, 40),
                new DebuffSpell(2, 1, 5, 0, 8),
                new TeleportSpell(2, 0, 96),
                new FireBall(8, 3, 4, 0, 23)
            };
        }

        public Spell GainSpell<T>() where T : Spell
        {
            Spell spell = spells.Where(s => s is T).FirstOrDefault();
            spells.Remove(spell);
            return spell;
        }

        public Spell GainSpell(string name)
        {
            Spell spell = spells.Where(s => s.name == name).FirstOrDefault();
            spells.Remove(spell);
            return spell;
        }
    }
}