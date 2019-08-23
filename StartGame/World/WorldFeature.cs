using StartGame.Properties;
using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace StartGame.World
{
    public enum RenderingAlignment {Left, Center}

    public class WorldMapText : WorldFeature
    {

        static Bitmap GenerateText(string text)
        {
            Bitmap bmp = new Bitmap(text.Length * 40, 40);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.Transparent);
                g.DrawString(text, new Font(FontFamily.GenericMonospace, 20), Brushes.Black, new Point(0, 0));
            }
            return bmp;
        }
        public WorldMapText(string text, Point position) : base(++ID, position, GenerateText(text), false, 2)
        {
            size = new Point(text.Length * 40 / 20, 2);
            alignment = RenderingAlignment.Center;
            MinimumSize = 8;
        }
    }

    public class WorldFeature
    {
        public static int ID = 0;
        public int id;
        /// <summary>
        /// Is the co-ordinate of the world feature in the worldMap, not on screen
        /// </summary>
        public Point position;
        public Bitmap image;
        private readonly bool blocking;
        public readonly int level;
        public Point size = new Point(1,1);
        public RenderingAlignment alignment = RenderingAlignment.Left;
        /// <summary>
        /// Will not be rendered if zoom level is below this value
        /// </summary>
        public int MinimumSize = 0;

        public WorldFeature(int id, Point position, Bitmap image, bool blocking = true, int level = 0)
        {
            this.id = id;
            this.position = position;
            this.image = image;
            this.blocking = blocking;
            this.level = level;
        }

        /// <summary>
        /// Determins if a certain co-ordinate is within this feature
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool Blocks(int x, int y)
        {
            return blocking && x.Between(position.X, position.X + size.X - 1) && y.Between(position.Y, position.Y + size.Y - 1);
        }

        internal bool Blocks(Point p)
        {
            return Blocks(p.X, p.Y);
        }
    }

    public class Road : WorldFeature
    {
        public Road(Point position) : base(++ID, position, Resources.Road)
        {
            World.Instance.costMap[position.X, position.Y] = 0.5;
            World.Instance.worldMap[position.X, position.Y].movementCost = 0.5;
            //World.Instance.costMap[position.X, position.Y] = World.Instance.costMap[position.X, position.Y].Cut(0.5, 1000);
            Bitmap aImage = new Bitmap(image.Width, image.Height);
            using (Graphics gfx = Graphics.FromImage(aImage))
            {

                //create a color matrix object  
                ColorMatrix matrix = new ColorMatrix {

                    //set the opacity  
                    Matrix33 = 0.5f
                };

                //create image attributes  
                ImageAttributes attributes = new ImageAttributes();

                //set the color(opacity) of the image  
                attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                gfx.DrawImage(image, new Rectangle(0, 0, aImage.Width, aImage.Height), 0, 0, aImage.Width, aImage.Width, GraphicsUnit.Pixel, attributes);
            }
            image = aImage;
        }
    }
}