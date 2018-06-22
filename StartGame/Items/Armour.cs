using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartGame.Items
{
    internal enum ArmourLayer
    {
        [Description("Heavy armour")]
        heavy,

        [Description("Light armour")]
        light,

        [Description("Clothing")]
        clothing
    }

    //TODO: Make armour only fit one body type
    internal class Armour : Item
    {
        public readonly List<BodyParts> affected;
        private readonly Material material;
        public int sharpDefense; //Blocks a flat number of damage
        public int bluntDefense; //Decreases by a percentage
        public int magicDefense; //Decreases by a percentage
        public int durability;
        public int maxDurability;
        public Quality quality;
        public readonly ArmourLayer layer;
        public int weight;
        public bool active = false;

        public Armour(string name, int baseDurability, List<BodyParts> Affected, Material material, Quality Quality, ArmourLayer layer) : base(name)
        {
            if (Affected.Count == 0) throw new ArgumentException("Must affect at least on body part");
            quality = Quality;
            this.layer = layer;
            magicDefense = material.magicResistance;
            magicDefense += (int)(magicDefense * ((double)(int)magicDefense) / 100d);
            sharpDefense = material.hardness;
            sharpDefense += (int)(sharpDefense * ((double)(int)quality) / 100d) / 10;
            bluntDefense = material.density;
            bluntDefense += (int)(bluntDefense * ((double)(int)quality) / 100d);
            durability = material.durability * Affected.Count + baseDurability;
            durability += (int)(durability * ((double)(int)quality * 2) / 100d);
            maxDurability = durability;
            weight = Affected.Count * material.mass;
            affected = Affected;
            this.material = material;
        }

        public string Description => $"This a piece of {layer.Description()} made out of {material.name}. It is of {quality} quality. It weighs {weight} grams.\n" +
            $"Blunt defense: {bluntDefense}\nSharp defense: {sharpDefense}\nMagic defense: {magicDefense}\nDurability: {durability}/{maxDurability}";
    }
}