using StartGame.World;
using StartGame.World.Cities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;

namespace StartGame.Rendering
{
    public class WorldRenderer
    {
        public List<OverlayObject> overlayObjects = new List<OverlayObject>();
        private readonly World.World world;
        private readonly PlayerData.HumanPlayer player;
        private Bitmap currentMap;
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
            {
                currentMap?.Dispose();
                currentMap = DrawMap();
            }
            Redraw = false;
            Bitmap toReturn = new Bitmap(renderWidth, renderHeight);
            Bitmap toDraw = currentMap.ResizeImage((int)(currentMap.Width * (double)size / 20d), (int)(currentMap.Height * size / 20d));
            using (Graphics g = Graphics.FromImage(toReturn))
            {
                g.DrawImage(toDraw, Position.Mult(-1));
            }
            toDraw.Dispose();
            return toReturn;
        }

        private Bitmap DrawMap(int size = 20)
        {
            Trace.TraceInformation("WorldRenderer::DrawMap");
            Bitmap worldImage = DrawBackground(size);
            using (Graphics g = Graphics.FromImage(worldImage))
            {
                //Draw features
                Trace.TraceInformation($"Rendering {world.features.Count} features!");
                foreach (var feature in world.features.OrderBy(f => f.level))
                {
                    if(feature.alignment == RenderingAlignment.Left)
                        g.DrawImage(feature.image, feature.position.X * size, feature.position.Y * size, size, size);
                    else if(feature.alignment == RenderingAlignment.Center)
                    {
                        int x = (feature.position.X - feature.size.X / 2) * size;
                        int y = (feature.position.Y - feature.size.Y / 2) * size;
                        g.DrawImage(feature.image, x, y, size * feature.size.X, size * feature.size.Y);
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
                    else if (obj is OverlayLine line)
                    {
                        g.DrawLine(new Pen(line.color), line.start, line.end);
                    }
                }

                //Update Overlay
                overlayObjects = overlayObjects.Where(o => !o.once).ToList();
            }
            return worldImage;
        }

        public Bitmap DrawHeightMap(int size = 20)
        {
            Bitmap worldImage = new Bitmap(World.World.WORLD_SIZE * size, World.World.WORLD_SIZE * size);
            using (Graphics g = Graphics.FromImage(worldImage))
            {
                //Draw background
                for (int x = 0; x <= world.worldMap.GetUpperBound(0); x++)
                {
                    for (int y = 0; y <= world.worldMap.GetUpperBound(1); y++)
                    {
                        g.FillRectangle(new SolidBrush(Color.FromArgb((int)(255 * world.heightMap[x, y]), 0, 0, 0)), x * size, y * size, size, size);
                    }
                }
            }
            return worldImage;
        }

        public Bitmap DrawRawHeightMap(int size = 20)
        {
            Bitmap worldImage = new Bitmap(World.World.WORLD_SIZE * size, World.World.WORLD_SIZE * size);
            using (Graphics g = Graphics.FromImage(worldImage))
            {
                //Draw background
                for (int x = 0; x <= world.worldMap.GetUpperBound(0); x++)
                {
                    for (int y = 0; y <= world.worldMap.GetUpperBound(1); y++)
                    {
                        try
                        {
                            g.FillRectangle(new SolidBrush(Color.FromArgb((int)(255 * world.rawheightMap[x, y]), 0, 0, 0)), x * size, y * size, size, size);

                        }
                        catch (Exception e)
                        {
                            goto End;
                        }
                    }
                }
                End:
                Trace.TraceInformation("Finished rendering raw height!");
            }
            return worldImage;
        }

        public Bitmap DrawTemperatureMap(int size = 20)
        {
            Bitmap worldImage = new Bitmap(World.World.WORLD_SIZE * size, World.World.WORLD_SIZE * size);
            using (Graphics g = Graphics.FromImage(worldImage))
            {
                //Draw background
                for (int x = 0; x <= world.worldMap.GetUpperBound(0); x++)
                {
                    for (int y = 0; y <= world.worldMap.GetUpperBound(1); y++)
                    {
                        g.FillRectangle(new SolidBrush(Color.FromArgb(255, 0, (int)(255 * (1 + world.temperatureMap[x, y]) / 2), 255)), x * size, y * size, size, size);
                    }
                }
            }
            return worldImage;
        }

        public Bitmap DrawRainfallMap(int size = 20)
        {
            Bitmap worldImage = new Bitmap(World.World.WORLD_SIZE * size, World.World.WORLD_SIZE * size);
            using (Graphics g = Graphics.FromImage(worldImage))
            {
                //Draw background
                for (int x = 0; x <= world.worldMap.GetUpperBound(0); x++)
                {
                    for (int y = 0; y <= world.worldMap.GetUpperBound(1); y++)
                    {
                        g.FillRectangle(new SolidBrush(Color.FromArgb(255, 255, 255, (int)(255 * world.rainfallMap[x, y]))), x * size, y * size, size, size);
                    }
                }
            }
            return worldImage;
        }

        public Bitmap DrawMineralMap(int size = 20)
        {
            Bitmap worldImage = new Bitmap(World.World.WORLD_SIZE * size, World.World.WORLD_SIZE * size);
            using (Graphics g = Graphics.FromImage(worldImage))
            {
                //Draw background
                for (int x = 0; x <= world.worldMap.GetUpperBound(0); x++)
                {
                    for (int y = 0; y <= world.worldMap.GetUpperBound(1); y++)
                    {
                        g.FillRectangle(new SolidBrush(Color.FromArgb(255, 255, 255 - (int)(255 * world.mineralMap[x, y]), 255)), x * size, y * size, size, size);
                    }
                }
            }
            return worldImage;
        }

        public Bitmap DrawAgriculturalMap(int size = 20)
        {
            Bitmap worldImage = new Bitmap(World.World.WORLD_SIZE * size, World.World.WORLD_SIZE * size);
            using (Graphics g = Graphics.FromImage(worldImage))
            {
                //Draw background
                for (int x = 0; x <= world.worldMap.GetUpperBound(0); x++)
                {
                    for (int y = 0; y <= world.worldMap.GetUpperBound(1); y++)
                    {
                        g.FillRectangle(new SolidBrush(Color.FromArgb((int)(255 * world.agriculturalMap[x, y]), 255, 160, 0)), x * size, y * size, size, size);
                    }
                }
            }
            return worldImage;
        }

        public Bitmap DrawValueMap(int size = 20)
        {
            Bitmap worldImage = new Bitmap(World.World.WORLD_SIZE * size, World.World.WORLD_SIZE * size);
            using (Graphics g = Graphics.FromImage(worldImage))
            {
                //Draw background
                for (int x = 0; x <= world.worldMap.GetUpperBound(0); x++)
                {
                    for (int y = 0; y <= world.worldMap.GetUpperBound(1); y++)
                    {
                        g.FillRectangle(new SolidBrush(Color.FromArgb(255, 255, 255 - (int)(255 * world.valueMap[x, y]), 0)), x * size, y * size, size, size);
                    }
                }
            }
            return worldImage;
        }

        public Bitmap DrawIslands(int size = 20)
        {
            Random random = new Random();
            Brush[] brushes = new Brush[Island.ID + 1];
            for (int i = 0; i < brushes.Length; i++)
            {
                brushes[i] = new SolidBrush(Color.FromArgb(random.Next(255), random.Next(255), random.Next(255)));
            }
            Bitmap worldImage = new Bitmap(World.World.WORLD_SIZE * size, World.World.WORLD_SIZE * size);
            using (Graphics g = Graphics.FromImage(worldImage))
            {
                //Draw background
                for (int x = 0; x <= world.worldMap.GetUpperBound(0); x++)
                {
                    for (int y = 0; y <= world.worldMap.GetUpperBound(1); y++)
                    {
                        Island island = world.worldMap[x, y].island;
                        if (island != null)
                        {
                            g.FillRectangle(brushes[island.id], x * size, y * size, size, size);
                        }
                    }
                }
            }
            return worldImage;
        }

        public Bitmap DrawBackground(int size = 20)
        {
            Bitmap worldImage = new Bitmap(World.World.WORLD_SIZE * size, World.World.WORLD_SIZE * size);
            using (Graphics g = Graphics.FromImage(worldImage))
            {
                for (int x = 0; x <= world.worldMap.GetUpperBound(0); x++)
                {
                    for (int y = 0; y <= world.worldMap.GetUpperBound(1); y++)
                    {
                        switch (world.worldMap[x, y].type)
                        {
                            case World.WorldTileType.Ocean:
                                g.FillRectangle(new SolidBrush(Color.Blue), x * size, y * size, size, size);
                                break;

                            case World.WorldTileType.River:
                                throw new NotImplementedException();
                            case World.WorldTileType.TemperateGrassland:
                                g.FillRectangle(new SolidBrush(Color.Green), x * size, y * size, size, size);
                                break;

                            case World.WorldTileType.Rainforest:
                                g.FillRectangle(new SolidBrush(Color.FromArgb(255, 3, 72, 38)), x * size, y * size, size, size);
                                break;

                            case World.WorldTileType.Desert:
                                g.FillRectangle(new SolidBrush(Color.FromArgb(255, 204, 225, 64)), x * size, y * size, size, size);
                                break;

                            case World.WorldTileType.Tundra:
                                g.FillRectangle(new SolidBrush(Color.FromArgb(255, 9, 225, 223)), x * size, y * size, size, size);
                                break;

                            case World.WorldTileType.TemperateForest:
                                g.FillRectangle(new SolidBrush(Color.FromArgb(255, 94, 183, 80)), x * size, y * size, size, size);
                                break;

                            case World.WorldTileType.Savanna:
                                g.FillRectangle(new SolidBrush(Color.FromArgb(255, 223, 230, 175)), x * size, y * size, size, size);
                                break;

                            case World.WorldTileType.Alpine:
                                g.FillRectangle(new SolidBrush(Color.FromArgb(255, 73, 73, 73)), x * size, y * size, size, size);
                                break;

                            default:
                                throw new NotImplementedException();
                        }

                        //apply shader
                        try
                        {
                            g.FillRectangle(new SolidBrush(Color.FromArgb((int)(255 * world.rawheightMap[x, y] / 2), 0, 0, 0)), x * size, y * size, size, size);
                        }
                        catch (Exception e)
                        {
                            Trace.TraceError($"Error occured when rendering shader {e.Message} {e.StackTrace}");
                            goto End;
                        }
                    }
                }
                End:
                return worldImage;
            }
        }

        public Bitmap DrawNationMap(int size = 20)
        {
            Bitmap worldImage = new Bitmap(World.World.WORLD_SIZE * size, World.World.WORLD_SIZE * size);
            using (Graphics g = Graphics.FromImage(worldImage))
            {
                //Draw background
                IEnumerable<Island> settledIslands = world.nation.islands.Where(n => n.Settled).Select(n => n.island);
                for (int x = 0; x <= world.worldMap.GetUpperBound(0); x++)
                {
                    for (int y = 0; y <= world.worldMap.GetUpperBound(1); y++)
                    {
                        if (settledIslands.Contains(world.worldMap[x, y].island))
                            g.FillRectangle(new SolidBrush(Color.Gray), x * size, y * size, size, size);
                        else if(world.worldMap[x,y].type == WorldTileType.Ocean)
                            g.FillRectangle(new SolidBrush(Color.LightBlue), x * size, y * size, size, size);
                    }
                }
                //Draw cities
                foreach (var city in world.nation.cities)
                {
                    Color c = Color.Green;
                    if (city == world.nation.Captial)
                        c = Color.Red;
                    else if (city is LargeCity)
                        c = Color.Yellow;
                    else if (city is MediumCity)
                        c = Color.Beige;
                    else if (world.worldMap.Get(city.position).sorroundingTiles.rawMaptiles.Any(m => !World.World.IsLand(m.type)))
                        c = Color.Blue;
                    g.FillRectangle(new SolidBrush(c), city.position.X - size * 2, city.position.Y - size * 2, size * 4, size * 4);
                }
            }
            return worldImage;
        }
    }
}