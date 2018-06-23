using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartGame.Items
{
    internal enum Quality
    { Broken = -15, Poor = -10, Simple = -5, Common = 0, Good = 5, Superior = 10, Exceptional = 15, Legendary = 25 }

    //To convert from 0-> 6 to this use method in Extension methods

    internal enum Materials
    { Leather, Iron, Steel, Chainmail, Wool, Pelt, Cloth, Wood }

    internal enum MaterialTypes
    {
        All,
        General, //Exludes wood
        Metal,
        Clothing
    }

    internal abstract class Material
    {
        public readonly string name;
        public readonly Materials material;
        public readonly int hardness;
        public readonly int density;
        public readonly int durability;
        public readonly int mass;
        public readonly int value;
        public readonly List<ArmourLayer> armourLayers;
        public readonly int magicResistance;

        /// <summary>
        /// Material describes the qualities of an item
        /// </summary>
        /// <param name="name">Name of material</param>
        /// <param name="materials">Enum version of material</param>
        /// <param name="hardness">How well material protects again sharp attacks 0 - 100</param>
        /// <param name="density">How well it protects again blunt attacks 0 - 100</param>
        /// <param name="durability">Decides durability of object 0 - 100</param>
        /// <param name="mass">Determines how much an object ways in gram</param>
        /// <param name="magicResistance">How good the material repells magic attacks 0 - 100</param>
        /// <param name="armourLayers">Used to randomly create armour specify in which layer it can occur</param>
        public Material(string name, Materials materials, int hardness, int density, int magicResistance, int durability, int mass, int value, List<ArmourLayer> armourLayers)
        {
            this.name = name;
            this.material = materials;
            this.hardness = hardness;
            this.density = density;
            this.magicResistance = magicResistance;
            this.durability = durability;
            this.mass = mass;
            this.value = value;
            this.armourLayers = armourLayers;
        }

        public static List<Material> Materials = new List<Material>()
        {
            new Leather(),
            new Iron(),
            new Steel(),
            new Chainmail(),
            new Pelt(),
            new Wool(),
            new Cloth(),
            new Wood()
        };

        private static Random random = new Random();

        public static Material Random(MaterialTypes types)
        {
            List<Material> allowed = new List<Material>(Materials);
            switch (types)
            {
                case MaterialTypes.All:
                    break;

                case MaterialTypes.General:
                    allowed.RemoveAll(m => m.material == Items.Materials.Wood);
                    break;

                case MaterialTypes.Metal:
                    List<Materials> filter = new List<Materials> { Items.Materials.Iron, Items.Materials.Steel, Items.Materials.Chainmail };
                    allowed.RemoveAll(m => !filter.Exists(f => f == m.material));
                    break;

                case MaterialTypes.Clothing:
                    filter = new List<Materials> { Items.Materials.Cloth, Items.Materials.Pelt, Items.Materials.Wool, Items.Materials.Leather };
                    allowed.RemoveAll(m => !filter.Exists(f => f == m.material));
                    break;

                default:
                    throw new NotImplementedException();
            }
            return allowed[random.Next(allowed.Count)];
        }
    }

    internal class Wood : Material
    {
        public Wood() : base("Wood", Items.Materials.Wood, 60, 8, 0, 20, 340, 4, new List<ArmourLayer> { ArmourLayer.light })
        {
        }
    }

    internal class Leather : Material
    {
        public Leather() : base("Leather", Items.Materials.Leather, 45, 80, 0, 60, 400, 10, new List<ArmourLayer> { ArmourLayer.light, ArmourLayer.clothing })
        {
        }
    }

    internal class Iron : Material
    {
        public Iron() : base("Iron", Items.Materials.Iron, 65, 40, 0, 70, 1000, 20, new List<ArmourLayer> { ArmourLayer.light, ArmourLayer.heavy })
        {
        }
    }

    internal class Steel : Material
    {
        public Steel() : base("Steel", Items.Materials.Steel, 90, 30, 0, 80, 1050, 25, new List<ArmourLayer> { ArmourLayer.heavy })
        {
        }
    }

    internal class Chainmail : Material
    {
        public Chainmail() : base("Chainmail", Items.Materials.Chainmail, 80, 60, 0, 65, 800, 30, new List<ArmourLayer> { ArmourLayer.light })
        {
        }
    }

    internal class Wool : Material
    {
        public Wool() : base("Wool", Items.Materials.Wool, 8, 39, 0, 27, 100, 7, new List<ArmourLayer> { ArmourLayer.clothing })
        {
        }
    }

    internal class Cloth : Material
    {
        public Cloth() : base("Cloth", Items.Materials.Cloth, 15, 4, 0, 35, 50, 5, new List<ArmourLayer> { ArmourLayer.clothing })
        {
        }
    }

    internal class Pelt : Material
    {
        public Pelt() : base("Pelt", Items.Materials.Pelt, 24, 60, 0, 73, 200, 11, new List<ArmourLayer> { ArmourLayer.clothing, ArmourLayer.light })
        {
        }
    }
}