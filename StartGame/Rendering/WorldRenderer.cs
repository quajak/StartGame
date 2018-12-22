using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartGame.Rendering
{
    public class WorldRenderer
    {
        public List<OverlayObject> overlayObjects = new List<OverlayObject>();
        private readonly World.World world;
        private readonly PlayerData.HumanPlayer player;
        Bitmap currentMap;
        public bool Redraw = false;
        public Point Position;
        public WorldRenderer(World.World world, PlayerData.HumanPlayer player)
        {
            this.world = world ?? throw new ArgumentNullException(nameof(world));
            this.player = player;
        }

        public Bitmap Render(int renderWidth, int renderHeight, int size = 20)
        {
            if (Redraw || currentMap == null)
                currentMap = DrawMap();
            Redraw = false;
            Bitmap toReturn = new Bitmap(renderWidth, renderHeight);
            Bitmap toDraw = currentMap.ResizeImage((int)(currentMap.Width * (double)size / 20d), (int)(currentMap.Height * size / 20d));
            using (Graphics g = Graphics.FromImage(toReturn))
            {
                g.DrawImage(toDraw, Position.Mult(-1 * size / 20d));
            }
            return toReturn;
            
        }

        Bitmap DrawMap(int size = 20)
        {
            Trace.TraceInformation("Rendering the world!");
            Bitmap worldImage = new Bitmap(World.World.WORLD_SIZE * size, World.World.WORLD_SIZE * size);
            using (Graphics g = Graphics.FromImage(worldImage))
            {
                //Draw background
                for (int x = 0; x <= world.worldMap.GetUpperBound(0); x++)
                {
                    for (int y = 0; y <= world.worldMap.GetUpperBound(1); y++)
                    {
                        switch (world.worldMap[x,y].type)
                        {
                            case World.WorldTileType.Ocean:
                                g.FillRectangle(new SolidBrush(Color.Blue), x * size, y * size, size, size);
                                break;
                            case World.WorldTileType.River:
                                throw new NotImplementedException();
                            case World.WorldTileType.Grassland:
                                g.FillRectangle(new SolidBrush(Color.Green), x * size, y * size, size, size);
                                break;
                            case World.WorldTileType.Mountain:
                                g.FillRectangle(new SolidBrush(Color.Gray), x * size, y * size, size, size);
                                break;
                            case World.WorldTileType.Hill:
                                throw new NotImplementedException();
                            case World.WorldTileType.Forest:
                                throw new NotImplementedException();
                            default:
                                throw new NotImplementedException();
                        }
                    }
                }

                //Draw actors
                Trace.TraceInformation($"Rendering {world.actors.Count} actors!");
                foreach (var actor in world.actors)
                {
                    g.DrawImage(actor.troop.image, actor.WorldPosition.X * size, actor.WorldPosition.Y * size, size, size);

                }

                //Draw overlay objects
                foreach (var obj in overlayObjects)
                {
                    if (obj is OverlayRectangle)
                    {
                        OverlayRectangle rect = obj as OverlayRectangle;
                        if (rect.filled)
                            g.FillRectangle(new SolidBrush(rect.fillColor), rect.x, rect.y, rect.width, rect.height);
                        g.DrawRectangle(new Pen(rect.borderColor), rect.x, rect.y, rect.width, rect.height);
                    }
                    else if (obj is OverlayText)
                    {
                        OverlayText txt = obj as OverlayText;
                        g.DrawString(txt.text, SystemFonts.DefaultFont, new SolidBrush(txt.color),
                            new PointF(txt.x, txt.y));
                    }
                    else if(obj is OverlayLine line)
                    {
                        g.DrawLine(new Pen(line.color), line.start, line.end);
                    }
                }

                //Update Overlay
                overlayObjects = overlayObjects.Where(o => !o.once).ToList();

            }
            return worldImage;
        }
    }
}
