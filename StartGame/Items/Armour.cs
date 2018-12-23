using StartGame.PlayerData;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace StartGame.Items
{
    public enum ArmourLayer
    {
        [Description("Heavy armour")]
        heavy,

        [Description("Light armour")]
        light,

        [Description("Clothing")]
        clothing,

        [Description("Jewelry")]
        jewelry
    }

    //TODO: Make armour only fit one body type
    public class Armour : Item
    {
        public readonly List<BodyParts> affected;
        private List<BodyPart> parts;
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

        public int Value => (int)(material.value * parts.Sum(b => b.size) * (1 + ((int)quality + 15d) / 100) * Math.Max(Math.Pow(Math.Max(durability + (int)quality, 10) / (double)maxDurability, 2d), 0.3) + 1);

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
            affected = Affected;
            parts = new Body().bodyParts.Where(p => affected.Exists(a => a == p.part)).ToList();
            weight = parts.Sum(p => p.size) * material.mass / 10;
            this.material = material;
        }

        public string Description => $"This a piece of {layer.Description()} made out of {material.name}. It is of {Enum.GetName(typeof(Quality), quality)} quality and has a value of {Value} coins. It weighs {weight} grams.\n" +
            $"Blunt defense: {bluntDefense}\nSharp defense: {sharpDefense}\nMagic defense: {magicDefense}\nDurability: {durability}/{maxDurability}";
    }
}