using StartGame.PlayerData;
using System;
using System.Collections.Generic;

namespace StartGame.Items
{
    public static class ArmourPrefabs
    {
        public static Armour CreateArmour(bool allowUsed)
        {
            Armour toReturn;
            Quality quality = E.GetQuality(World.World.random.Next(8));
            int baseDurability = World.World.random.Next(20);
            toReturn = CreateArmour(allowUsed, quality, baseDurability);
            return toReturn;
        }

        public static Armour CreateArmour(bool allowUsed, Quality quality, int baseDurability)
        {
            Armour toReturn;
            int chosen = World.World.random.Next(6);
            switch (chosen)
            {
                case 0:
                    //Helmet
                    Material material = Material.Random(MaterialTypes.General);
                    toReturn = new Armour($"{Enum.GetName(typeof(Quality), quality)} Helmet", baseDurability, new List<BodyParts> { BodyParts.Head }, material, quality, material.GetArmourLayer());
                    break;

                case 1:
                    //Chestplate
                    material = Material.Random(MaterialTypes.Metal);
                    toReturn = new Armour($"{Enum.GetName(typeof(Quality), quality)} Chestplate", baseDurability, new List<BodyParts> { BodyParts.Torso }, material, quality, material.GetArmourLayer());
                    break;

                case 2:
                    //Shirt
                    material = Material.Random(MaterialTypes.Clothing);
                    toReturn = new Armour($"{Enum.GetName(typeof(Quality), quality)} Shirt", baseDurability, new List<BodyParts> { BodyParts.Torso }, material, quality, material.GetArmourLayer());
                    break;

                case 3:
                    //Hose
                    material = Material.Random(MaterialTypes.Clothing);
                    toReturn = new Armour($"{Enum.GetName(typeof(Quality), quality)} Hose", baseDurability, new List<BodyParts> { BodyParts.UpperLegs, BodyParts.LeftLowerLeg, BodyParts.RightLowerLeg, BodyParts.LeftShin, BodyParts.RightShin }
                    , material, quality, material.GetArmourLayer());
                    break;

                case 4:
                    //Shoes
                    material = Material.Random(MaterialTypes.Armour);
                    toReturn = new Armour($"{Enum.GetName(typeof(Quality), quality)} Shoes", baseDurability, new List<BodyParts> { BodyParts.LeftFoot, BodyParts.RightFoot }, material, quality, material.GetArmourLayer());
                    break;

                case 5:
                    //Gauntlet
                    material = Material.Random(MaterialTypes.Metal);
                    toReturn = new Armour($"{Enum.GetName(typeof(Quality), quality)} Gauntlet", baseDurability, new List<BodyParts> { BodyParts.LeftHand, BodyParts.RightHand }, material, quality, material.GetArmourLayer());
                    break;

                default:
                    throw new NotImplementedException();
            }

            if (toReturn is null) throw new Exception();
            if (allowUsed)
            {
                toReturn.durability = (int)(toReturn.durability * (World.World.random.Next(60, 100) / 100d));
            }

            return toReturn;
        }
    }
}