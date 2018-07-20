using StartGame.PlayerData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartGame.Items
{
    internal static class ArmorPrefabs
    {
        private static Random random = new Random();

        public static Armour CreateArmour(bool allowUsed)
        {
            Armour toReturn;
            int chosen = random.Next(6);
            Quality quality = Extensions.GetQuality(random.Next(8));
            switch (chosen)
            {
                case 0:
                    //Helmet
                    Material material = Material.Random(MaterialTypes.General);
                    toReturn = new Armour($"{Enum.GetName(typeof(Quality), quality)} Helmet", random.Next(100), new List<BodyParts> { BodyParts.Head }, material, quality, material.GetArmourLayer());
                    break;

                case 1:
                    //Chestplate
                    material = Material.Random(MaterialTypes.Metal);
                    toReturn = new Armour($"{Enum.GetName(typeof(Quality), quality)} Chestplate", random.Next(100), new List<BodyParts> { BodyParts.Torso }, material, quality, material.GetArmourLayer());
                    break;

                case 2:
                    //Shirt
                    material = Material.Random(MaterialTypes.Clothing);
                    toReturn = new Armour($"{Enum.GetName(typeof(Quality), quality)} Shirt", random.Next(100), new List<BodyParts> { BodyParts.Torso }, material, quality, material.GetArmourLayer());
                    break;

                case 3:
                    //Hose
                    material = Material.Random(MaterialTypes.Clothing);
                    toReturn = new Armour($"{Enum.GetName(typeof(Quality), quality)} Hose", random.Next(100), new List<BodyParts> { BodyParts.UpperLegs, BodyParts.LeftLowerLeg, BodyParts.RightLowerLeg, BodyParts.LeftShin, BodyParts.RightShin }
                    , material, quality, material.GetArmourLayer());
                    break;

                case 4:
                    //Shoes
                    material = Material.Random(MaterialTypes.Armour);
                    toReturn = new Armour($"{Enum.GetName(typeof(Quality), quality)} Shoes", random.Next(100), new List<BodyParts> { BodyParts.LeftFoot, BodyParts.RightFoot }, material, quality, material.GetArmourLayer());
                    break;

                case 5:
                    //Gauntlet
                    material = Material.Random(MaterialTypes.Metal);
                    toReturn = new Armour($"{Enum.GetName(typeof(Quality), quality)} Gauntlet", random.Next(100), new List<BodyParts> { BodyParts.LeftHand, BodyParts.RightHand }, material, quality, material.GetArmourLayer());
                    break;

                default:
                    throw new NotImplementedException();
            }

            if (toReturn is null) throw new Exception();
            if (allowUsed)
            {
                toReturn.durability = (int)(toReturn.durability * (random.Next(60, 100) / 100d));
            }
            return toReturn;
        }
    }
}