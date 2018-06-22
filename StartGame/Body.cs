using StartGame.Items;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartGame
{
    internal class Body
    {
        public List<BodyPart> bodyParts;

        public BodyPart[,] body;

        public Body()
        {
            bodyParts = BodyPart.GenerateHuman();
            body = new BodyPart[7, 15];
            foreach (var part in bodyParts)
            {
                part.Position(ref body);
            }
        }

        public Bitmap Render(bool useIndividualColors = false, List<Armour> armours = null, int size = 12)
        {
            Bitmap image = new Bitmap((body.GetUpperBound(0) + 1) * size, (body.GetUpperBound(1) + 1) * size);
            using (Graphics g = Graphics.FromImage(image))
            {
                g.Clear(Color.Blue);

                //Reset all body parts to black color
                Color col = Color.FromArgb(255, 209, 207, 142);
                bodyParts.ForEach(b => b.color = col);
                if (armours != null)
                {
                    //Sort armour by type so clothing is drawn first and heavy last
                    armours = armours.OrderByDescending(a => (int)a.layer).ToList();

                    Dictionary<ArmourLayer, Color> colors = new Dictionary<ArmourLayer, Color>
                    {
                        {ArmourLayer.clothing, Color.SandyBrown },
                        {ArmourLayer.light, Color.LightGray },
                        {ArmourLayer.heavy, Color.DarkGray }
                    };

                    //Add armour
                    foreach (var a in armours)
                    {
                        a.affected.ForEach(bp => bodyParts.Find(b => b.part == bp).color = colors[a.layer]);
                    }
                }

                //Draw body
                for (int x = 0; x <= body.GetUpperBound(0); x++)
                {
                    for (int y = 0; y <= body.GetUpperBound(1); y++)
                    {
                        if (body[x, y] is null)
                        {
                        }
                        else
                        {
                            g.FillRectangle(new SolidBrush(body[x, y].color), x * size, y * size, size, size);
                        }
                    }
                }
            }
            return image;
        }
    }

    internal enum BodyParts
    { Head, Neck, LeftUpperArm, RightUpperArm, LeftLowerArm, RightLowerArm, LeftHand, RightHand, Torso, UpperLegs, LeftLowerLeg, RightLowerLeg, LeftShin, RightShin, LeftFoot, RightFoot }

    internal abstract class BodyPart
    {
        public readonly string name;
        public readonly int size;
        public Color color;
        public readonly BodyParts part;

        public BodyPart(string name, int size, BodyParts part, Color color = new Color())
        {
            if (color == new Color()) color = Color.Black;
            this.name = name;
            this.size = size;
            this.color = color;
            this.part = part;
        }

        public static List<BodyPart> GenerateHuman()
        {
            return new List<BodyPart>
            {
                new Head(),
                new Neck(),
                new LeftUpperArm(),
                new RightUpperArm(),
                new LeftLowerArm(),
                new RightLowerArm(),
                new LeftHand(),
                new RightHand(),
                new Torso(),
                new UpperLegs(),
                new LeftLowerLeg(),
                new RightLowerLeg(),
                new LeftShin(),
                new RightShin(),
                new LeftFoot(),
                new RightFoot()
            };
        }

        public abstract void Position(ref BodyPart[,] body);
    }

    #region Individual Body Parts

    internal class Head : BodyPart
    {
        public Head() : base("Head", 10, BodyParts.Head)
        {
        }

        public override void Position(ref BodyPart[,] body)
        {
            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 2; y++)
                {
                    body[x + 2, y] = this;
                }
            }
        }
    }

    internal class Neck : BodyPart
    {
        public Neck() : base("Neck", 3, BodyParts.Neck)
        {
        }

        public override void Position(ref BodyPart[,] body)
        {
            body[3, 2] = this;
        }
    }

    internal class Torso : BodyPart
    {
        public Torso() : base("Torso", 35, BodyParts.Torso)
        {
        }

        public override void Position(ref BodyPart[,] body)
        {
            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 5; y++)
                {
                    body[x + 2, 3 + y] = this;
                }
            }
        }
    }

    internal class LeftUpperArm : BodyPart
    {
        public LeftUpperArm() : base("Left Upper Arm", 5, BodyParts.LeftUpperArm)
        {
        }

        public override void Position(ref BodyPart[,] body)
        {
            body[0, 3] = this;
            body[1, 3] = this;
            body[0, 4] = this;
        }
    }

    internal class RightUpperArm : BodyPart
    {
        public RightUpperArm() : base("Right Upper Arm", 5, BodyParts.RightUpperArm)
        {
        }

        public override void Position(ref BodyPart[,] body)
        {
            body[5, 3] = this;
            body[6, 3] = this;
            body[6, 4] = this;
        }
    }

    internal class LeftLowerArm : BodyPart
    {
        public LeftLowerArm() : base("Left Lower Arm", 3, BodyParts.LeftLowerArm)
        {
        }

        public override void Position(ref BodyPart[,] body)
        {
            body[0, 5] = this;
            body[0, 6] = this;
        }
    }

    internal class RightLowerArm : BodyPart
    {
        public RightLowerArm() : base("Right Lower Arm", 3, BodyParts.RightLowerArm)
        {
        }

        public override void Position(ref BodyPart[,] body)
        {
            body[6, 5] = this;
            body[6, 6] = this;
        }
    }

    internal class LeftHand : BodyPart
    {
        public LeftHand() : base("Left Hand", 2, BodyParts.LeftHand)
        {
        }

        public override void Position(ref BodyPart[,] body)
        {
            body[0, 7] = this;
        }
    }

    internal class RightHand : BodyPart
    {
        public RightHand() : base("Right Hand", 2, BodyParts.RightHand)
        {
        }

        public override void Position(ref BodyPart[,] body)
        {
            body[6, 7] = this;
        }
    }

    internal class UpperLegs : BodyPart
    {
        public UpperLegs() : base("Upper Legs", 11, BodyParts.UpperLegs)
        {
        }

        public override void Position(ref BodyPart[,] body)
        {
            body[3, 8] = this;
            body[4, 8] = this;
            body[2, 9] = this;
            body[2, 10] = this;
            body[4, 10] = this;
            body[4, 9] = this;
            body[2, 8] = this;
        }
    }

    internal class LeftLowerLeg : BodyPart
    {
        public LeftLowerLeg() : base("Left Lower Leg", 5, BodyParts.LeftLowerLeg)
        {
        }

        public override void Position(ref BodyPart[,] body)
        {
            body[2, 11] = this;
            body[2, 12] = this;
        }
    }

    internal class RightLowerLeg : BodyPart
    {
        public RightLowerLeg() : base("Right Lower Leg", 5, BodyParts.RightLowerLeg)
        {
        }

        public override void Position(ref BodyPart[,] body)
        {
            body[4, 11] = this;
            body[4, 12] = this;
        }
    }

    internal class LeftShin : BodyPart
    {
        public LeftShin() : base("Left Shin", 3, BodyParts.LeftShin)
        {
        }

        public override void Position(ref BodyPart[,] body)
        {
            body[2, 13] = this;
        }
    }

    internal class RightShin : BodyPart
    {
        public RightShin() : base("Right Shin", 3, BodyParts.RightShin)
        {
        }

        public override void Position(ref BodyPart[,] body)
        {
            body[4, 13] = this;
        }
    }

    internal class LeftFoot : BodyPart
    {
        public LeftFoot() : base("Left Foot", 4, BodyParts.LeftFoot)
        {
        }

        public override void Position(ref BodyPart[,] body)
        {
            body[2, 14] = this;
        }
    }

    internal class RightFoot : BodyPart
    {
        public RightFoot() : base("Right Foot", 4, BodyParts.RightFoot)
        {
        }

        public override void Position(ref BodyPart[,] body)
        {
            body[4, 14] = this;
        }
    }

    #endregion Individual Body Parts
}