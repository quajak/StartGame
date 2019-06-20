﻿using StartGame.World;
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
        private Bitmap currentMap;
        public bool Redraw = false;
        /// <summary>
        /// Top left corner of rendered view
        /// </summary>
        public Point Position;

        public WorldRenderer()
        {
        }

        public Bitmap Render(int renderWidth, int renderHeight, int size = 20)
        {
            E.TraceFunction($"{renderWidth}, {renderHeight}, {size}");
            currentMap?.Dispose();
            //Only draw the needed area
            currentMap = DrawMap((Position.X / size).Min(0), (Position.Y / size).Min(0), renderWidth / size + 1, renderHeight / size + 1, size);
            Bitmap toReturn = new Bitmap(renderWidth, renderHeight);
            using (Graphics g = Graphics.FromImage(toReturn))
            {
                g.DrawImage(currentMap, Position.Remainder(20).Mult(-1));
            }
            return toReturn;
        }
        private Bitmap DrawMap(int X, int Y, int Width, int Height, int size = 20)
        {
            Trace.TraceInformation($"WorldRenderer::DrawMap({X}, {Y}, {Width}, {Height}, {size})");
            DateTime time = DateTime.Now;
            Bitmap worldImage = DrawBackground(X, Y, Math.Min(World.World.Instance.costMap.GetUpperBound(0), X + Width), Math.Min(World.World.Instance.costMap.GetUpperBound(0), Y + Height), size);
            Trace.TraceInformation($"Rendering background in {(DateTime.Now - time).TotalMilliseconds}!");
            using (Graphics g = Graphics.FromImage(worldImage))
            {
                DateTime point = DateTime.Now;
                //Draw features
                int features = 0;
                foreach (var feature in World.World.Instance.features.OrderBy(f => f.level))
                {
                    if (feature.MinimumSize <= size &&  feature.position.X.Between(X, X + Width) && feature.position.Y.Between(Y, Y + Height))
                    {
                        features++;
                        if (feature.alignment == RenderingAlignment.Left)
                            g.DrawImage(feature.image, (feature.position.X- X) * size, (feature.position.Y- Y) * size, size, size);
                        else if (feature.alignment == RenderingAlignment.Center)
                        {
                            int x = ((feature.position.X-X) - feature.size.X / 2) * size;
                            int y = ((feature.position.Y-Y) - feature.size.Y / 2) * size;
                            g.DrawImage(feature.image, x, y, size * feature.size.X, size * feature.size.Y);
                        }
                    }
                }
                Trace.TraceInformation($"Rendering {features} features in {(DateTime.Now - point).TotalMilliseconds}!");
                point = DateTime.Now;
                //Draw actors
                foreach (var actor in World.World.Instance.actors)
                {
                    g.DrawImage(actor.troop.Image, (actor.WorldPosition.X-X) * size, (actor.WorldPosition.Y-Y) * size, size, size);
                }
                Trace.TraceInformation($"Rendering {World.World.Instance.actors.Count} actors in {(DateTime.Now - point).TotalMilliseconds}!");

                //Draw overlay objects
                foreach (var obj in overlayObjects)
                {
                    if (obj is OverlayRectangle)
                    {
                        OverlayRectangle rect = obj as OverlayRectangle;
                        if (rect.filled)
                            g.FillRectangle(new SolidBrush(rect.fillColor), rect.x - X * size, rect.y- Y * size, rect.width, rect.height);
                        g.DrawRectangle(new Pen(rect.borderColor), rect.x- X * size, rect.y- Y * size, rect.width, rect.height);
                    }
                    else if (obj is OverlayText)
                    {
                        OverlayText txt = obj as OverlayText;
                        g.DrawString(txt.text, SystemFonts.DefaultFont, new SolidBrush(txt.color),
                            new PointF(txt.x / 20 * size - X * size, txt.y/ 20 * size- Y * size));
                    }
                    else if (obj is OverlayLine line)
                    {
                        g.DrawLine(new Pen(line.color), line.start.Div(20).Mult(size).Sub(X * size, Y * size).Add(10,10), line.end.Div(20).Mult(size).Sub(X * size, Y * size).Add(10,10));
                    }
                }

                //Update Overlay
                overlayObjects = overlayObjects.Where(o => !o.once).ToList();
            }
            Trace.TraceInformation($"Finished in {(DateTime.Now - time).TotalMilliseconds}");
            return worldImage;
        }

        internal Image DrawLatWindMap(int size)
        {
            Bitmap worldImage = new Bitmap(World.World.WORLD_SIZE * size, World.World.WORLD_SIZE * size);
            using (Graphics g = Graphics.FromImage(worldImage))
            {
                //Draw background
                for (int x = 0; x <= World.World.Instance.worldMap.GetUpperBound(0); x++)
                {
                    for (int y = 0; y <= World.World.Instance.worldMap.GetUpperBound(1); y++)
                    {

                        WeatherPoint point = World.World.Instance.atmosphere[x * World.World.MaxZ + y * World.World.WORLD_SIZE * World.World.MaxZ];
                        double v = point.u;
                        if (v > 0)
                        {
                            g.FillRectangle(new SolidBrush(Color.FromArgb((int)v.Cut(0, 255), 0, 0, 255)), x * size, y * size, size, size);
                        }
                        else if (v == 0)
                        {

                        }
                        else
                        {
                            g.FillRectangle(new SolidBrush(Color.FromArgb((int)v.Cut(0, 255),0, 255, 0)), x * size, y * size, size, size);
                        }
                    }
                }
            }
            return worldImage;
        }

        internal Image DrawLonWindMap(int size)
        {
            Bitmap worldImage = new Bitmap(World.World.WORLD_SIZE * size, World.World.WORLD_SIZE * size);
            using (Graphics g = Graphics.FromImage(worldImage))
            {
                //Draw background
                for (int x = 0; x <= World.World.Instance.worldMap.GetUpperBound(0); x++)
                {
                    for (int y = 0; y <= World.World.Instance.worldMap.GetUpperBound(1); y++)
                    {

                        WeatherPoint point = World.World.Instance.atmosphere[x * World.World.MaxZ + y * World.World.WORLD_SIZE * World.World.MaxZ];
                        double v = point.v;
                        if(v > 0)
                        {
                            g.FillRectangle(new SolidBrush(Color.FromArgb((int)v.Cut(0,255),0, 0, 255)), x * size, y * size, size, size);
                        }
                        else if(v == 0)
                        {

                        }
                        else
                        {
                            v = Math.Abs(v);
                            g.FillRectangle(new SolidBrush(Color.FromArgb((int)v.Cut(0,255),0,255, 0)), x * size, y * size, size, size);
                        }
                    }
                }
            }
            return worldImage;
        }

        internal Image DrawDTMap(int size)
        {
            Bitmap worldImage = new Bitmap(World.World.WORLD_SIZE * size, World.World.WORLD_SIZE * size);
            using (Graphics g = Graphics.FromImage(worldImage))
            {
                //Draw background
                for (int x = 0; x <= World.World.Instance.worldMap.GetUpperBound(0); x++)
                {
                    for (int y = 0; y <= World.World.Instance.worldMap.GetUpperBound(1); y++)
                    {

                        WeatherPoint point = World.World.Instance.atmosphere[x * World.World.MaxZ + y * World.World.WORLD_SIZE * World.World.MaxZ];
                        double v = 10 * point.dT;
                        if (v > 0)
                        {
                            g.FillRectangle(new SolidBrush(Color.FromArgb((int)v.Cut(0, 255), 255, 0, 0)), x * size, y * size, size, size);
                        }
                        else if (v == 0)
                        {

                        }
                        else
                        {
                            v = Math.Abs(v);
                            g.FillRectangle(new SolidBrush(Color.FromArgb(255, 255 - (int)v.Cut(0, 255), 255 - (int)v.Cut(0, 255), 255)), x * size, y * size, size, size);
                        }
                    }
                }
            }
            return worldImage;
        }

        internal Image DrawDPMap(int size)
        {
            Bitmap worldImage = new Bitmap(World.World.WORLD_SIZE * size, World.World.WORLD_SIZE * size);
            using (Graphics g = Graphics.FromImage(worldImage))
            {
                //Draw background
                for (int x = 0; x <= World.World.Instance.worldMap.GetUpperBound(0); x++)
                {
                    for (int y = 0; y <= World.World.Instance.worldMap.GetUpperBound(1); y++)
                    {

                        WeatherPoint point = World.World.Instance.atmosphere[x * World.World.MaxZ + y * World.World.WORLD_SIZE * World.World.MaxZ];
                        double v = 10 * point.dP;
                        if (v > 0)
                        {
                            g.FillRectangle(new SolidBrush(Color.FromArgb((int)v.Cut(0, 255), 0, 0, 255)), x * size, y * size, size, size);
                        }
                        else if (v == 0)
                        {

                        }
                        else
                        {
                            v = Math.Abs(v);
                            g.FillRectangle(new SolidBrush(Color.FromArgb(0,(int)v.Cut(0, 255), 255, 255)), x * size, y * size, size, size);
                        }
                    }
                }
            }
            return worldImage;
        }

        internal Image DrawPressureMap(int size)
        {
            Bitmap worldImage = new Bitmap(World.World.WORLD_SIZE * size, World.World.WORLD_SIZE * size);
            using (Graphics g = Graphics.FromImage(worldImage))
            {
                //Draw background
                for (int x = 0; x <= World.World.Instance.worldMap.GetUpperBound(0); x++)
                {
                    for (int y = 0; y <= World.World.Instance.worldMap.GetUpperBound(1); y++)
                    {
                        g.FillRectangle(new SolidBrush(Color.FromArgb((int)(Math.Min(2 * (World.World.Instance.atmosphere[x * World.World.MaxZ + y * World.World.WORLD_SIZE * World.World.MaxZ].pressure - 960).Cut(0,255), 255)), 255, 0, 255)), x * size, y * size, size, size);
                    }
                }
            }
            return worldImage;
        }

        public Bitmap DrawHeightMap(int size = 20)
        {
            Bitmap worldImage = new Bitmap(World.World.WORLD_SIZE * size, World.World.WORLD_SIZE * size);
            using (Graphics g = Graphics.FromImage(worldImage))
            {
                //Draw background
                for (int x = 0; x <= World.World.Instance.worldMap.GetUpperBound(0); x++)
                {
                    for (int y = 0; y <= World.World.Instance.worldMap.GetUpperBound(1); y++)
                    {
                        g.FillRectangle(new SolidBrush(Color.FromArgb((int)(255 * World.World.Instance.heightMap[x, y]), 0, 0, 0)), x * size, y * size, size, size);
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
                for (int x = 0; x <= World.World.Instance.worldMap.GetUpperBound(0); x++)
                {
                    for (int y = 0; y <= World.World.Instance.worldMap.GetUpperBound(1); y++)
                    {
                        try
                        {
                            g.FillRectangle(new SolidBrush(Color.FromArgb((int)(255 * World.World.Instance.rawheightMap[x, y]), 0, 0, 0)), x * size, y * size, size, size);

                        }
                        catch (Exception)
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
                for (int x = 0; x <= World.World.Instance.worldMap.GetUpperBound(0); x++)
                {
                    for (int y = 0; y <= World.World.Instance.worldMap.GetUpperBound(1); y++)
                    {
                        g.FillRectangle(new SolidBrush(Color.FromArgb(255, 0, (int)(255 * (1 + World.World.Instance.temperatureMap[x, y]) / 2), 255)), x * size, y * size, size, size);
                    }
                }
            }
            return worldImage;
        }

        public Bitmap DrawAtmosphereTemperatureMap(int size = 20)
        {
            Bitmap worldImage = new Bitmap(World.World.WORLD_SIZE * size, World.World.WORLD_SIZE * size);
            using (Graphics g = Graphics.FromImage(worldImage))
            {
                //Draw background
                for (int x = 0; x <= World.World.Instance.worldMap.GetUpperBound(0); x++)
                {
                    for (int y = 0; y <= World.World.Instance.worldMap.GetUpperBound(1); y++)
                    {
                        g.FillRectangle(new SolidBrush(
                            Color.FromArgb((int)((World.World.Instance.atmosphere[x * World.World.MaxZ + y * World.World.WORLD_SIZE * World.World.MaxZ].temperature + 40)/ 100d * 255).Cut(0, 255), 0, 0, 255)),
                            x * size, y * size, size, size);
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
                for (int x = 0; x <= World.World.Instance.worldMap.GetUpperBound(0); x++)
                {
                    for (int y = 0; y <= World.World.Instance.worldMap.GetUpperBound(1); y++)
                    {
                        g.FillRectangle(new SolidBrush(Color.FromArgb(255, 255, 255, (int)(255 * World.World.Instance.rainfallMap[x, y]))), x * size, y * size, size, size);
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
                for (int x = 0; x <= World.World.Instance.worldMap.GetUpperBound(0); x++)
                {
                    for (int y = 0; y <= World.World.Instance.worldMap.GetUpperBound(1); y++)
                    {
                        g.FillRectangle(new SolidBrush(Color.FromArgb(255, 255, 255 - (int)(255 * World.World.Instance.mineralMap[x, y]), 255)), x * size, y * size, size, size);
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
                for (int x = 0; x <= World.World.Instance.worldMap.GetUpperBound(0); x++)
                {
                    for (int y = 0; y <= World.World.Instance.worldMap.GetUpperBound(1); y++)
                    {
                        g.FillRectangle(new SolidBrush(Color.FromArgb((int)(255 * World.World.Instance.agriculturalMap[x, y]), 255, 160, 0)), x * size, y * size, size, size);
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
                for (int x = 0; x <= World.World.Instance.worldMap.GetUpperBound(0); x++)
                {
                    for (int y = 0; y <= World.World.Instance.worldMap.GetUpperBound(1); y++)
                    {
                        g.FillRectangle(new SolidBrush(Color.FromArgb(255, 255, 255 - (int)(255 * World.World.Instance.valueMap[x, y]), 0)), x * size, y * size, size, size);
                    }
                }
            }
            return worldImage;
        }

        public Bitmap DrawIslands(int size = 20)
        {
            Brush[] brushes = new Brush[Island.ID + 1];
            for (int i = 0; i < brushes.Length; i++)
            {
                brushes[i] = new SolidBrush(Color.FromArgb(World.World.random.Next(255), World.World.random.Next(255), World.World.random.Next(255)));
            }
            Bitmap worldImage = new Bitmap(World.World.WORLD_SIZE * size, World.World.WORLD_SIZE * size);
            using (Graphics g = Graphics.FromImage(worldImage))
            {
                //Draw background
                for (int x = 0; x <= World.World.Instance.worldMap.GetUpperBound(0); x++)
                {
                    for (int y = 0; y <= World.World.Instance.worldMap.GetUpperBound(1); y++)
                    {
                        Island island = World.World.Instance.worldMap[x, y].island;
                        if (island != null)
                        {
                            g.FillRectangle(brushes[island.id], x * size, y * size, size, size);
                        }
                    }
                }
            }
            return worldImage;
        }

        public Bitmap DrawBackground()
        {
            return DrawBackground(0, 0, World.World.Instance.worldMap.GetUpperBound(0), World.World.Instance.worldMap.GetUpperBound(1));
        }

        public Bitmap DrawBackground(int X, int Y, int EX, int EY, int size = 20)
        {
            E.TraceFunction($"{X}, {Y}, {EX}, {EY}, {size}");
            SolidBrush blue = new SolidBrush(Color.Blue);
            SolidBrush green = new SolidBrush(Color.Green);
            SolidBrush rainforest = new SolidBrush(Color.FromArgb(255, 3, 72, 38));
            SolidBrush desert = new SolidBrush(Color.FromArgb(255, 204, 225, 64));
            SolidBrush tundra = new SolidBrush(Color.FromArgb(255, 9, 225, 223));
            SolidBrush temperateForest = new SolidBrush(Color.FromArgb(255, 94, 183, 80));
            SolidBrush savanna = new SolidBrush(Color.FromArgb(255, 237, 189, 78));
            SolidBrush alpine = new SolidBrush(Color.FromArgb(255, 73, 73, 73));
            SolidBrush seaIce = new SolidBrush(Color.FromArgb(255, 0, 145, 193));
            Bitmap worldImage = new Bitmap((EX-X)*size, (EY-Y)*size);
            using (Graphics g = Graphics.FromImage(worldImage))
            {
                for (int x = X; x <= EX; x++)
                {
                    for (int y = Y; y <= EY; y++)
                    {
                        switch (World.World.Instance.worldMap[x, y].type)
                        {
                            case WorldTileType.Ocean:
                                g.FillRectangle(blue, (x - X) * size, (y - Y) * size, size, size);
                                break;

                            case WorldTileType.River:
                                throw new NotImplementedException();
                            case WorldTileType.TemperateGrassland:
                                g.FillRectangle(green, (x - X) * size, (y - Y) * size, size, size);
                                break;

                            case WorldTileType.Rainforest:
                                g.FillRectangle(rainforest, (x - X) * size, (y - Y) * size, size, size);
                                break;

                            case WorldTileType.Desert:
                                g.FillRectangle(desert, (x - X) * size, (y - Y) * size, size, size);
                                break;

                            case WorldTileType.Tundra:
                                g.FillRectangle(tundra, (x - X) * size, (y - Y) * size, size, size);
                                break;

                            case WorldTileType.TemperateForest:
                                g.FillRectangle(temperateForest, (x - X) * size, (y - Y) * size, size, size);
                                break;

                            case WorldTileType.Savanna:
                                g.FillRectangle(savanna, (x - X) * size, (y - Y) * size, size, size);
                                break;

                            case WorldTileType.Alpine:
                                g.FillRectangle(alpine, (x - X) * size, (y - Y) * size, size, size);
                                break;
                            case WorldTileType.SeaIce:
                                g.FillRectangle(seaIce, (x - X) * size, (y - Y) * size, size, size);
                                break;
                            default:
                                throw new NotImplementedException();
                        }

                        //apply shader
                        try
                        {
                            g.FillRectangle(new SolidBrush(Color.FromArgb((int)(255 * World.World.Instance.rawheightMap[x, y] / 2), 0, 0, 0)), (x-X) * size, (y-Y) * size, size, size);
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
                IEnumerable<Island> settledIslands = World.World.Instance.nation.islands.Where(n => n.Settled).Select(n => n.island);
                for (int x = 0; x <= World.World.Instance.worldMap.GetUpperBound(0); x++)
                {
                    for (int y = 0; y <= World.World.Instance.worldMap.GetUpperBound(1); y++)
                    {
                        if (settledIslands.Contains(World.World.Instance.worldMap[x, y].island))
                            g.FillRectangle(new SolidBrush(Color.Gray), x * size, y * size, size, size);
                        else if (World.World.Instance.worldMap[x, y].type == WorldTileType.Ocean)
                            g.FillRectangle(new SolidBrush(Color.LightBlue), x * size, y * size, size, size);
                    }
                }
                //Draw cities
                foreach (var city in World.World.Instance.nation.cities)
                {
                    Color c = Color.Green;
                    if (city == World.World.Instance.nation.Captial)
                        c = Color.Red;
                    else if (city is LargeCity)
                        c = Color.Yellow;
                    else if (city is MediumCity)
                        c = Color.Beige;
                    else if (World.World.Instance.worldMap.Get(city.position).sorroundingTiles.rawMaptiles.Any(m => !World.World.IsLand(m.type)))
                        c = Color.Blue;
                    g.FillRectangle(new SolidBrush(c), city.position.X - size * 2, city.position.Y - size * 2, size * 4, size * 4);
                }

                //Draw farms
                foreach (var farm in World.World.Instance.nation.farms)
                {
                    Color c = Color.LightGreen;
                    g.FillRectangle(new SolidBrush(c), farm.position.X - size, farm.position.Y - size, size * 2, size * 2);
                }

                //Draw mines
                foreach (var mine in World.World.Instance.nation.mines)
                {
                    Color c = Color.LightSalmon;
                    g.FillRectangle(new SolidBrush(c), mine.position.X - size, mine.position.Y - size, size * 2, size * 2);
                }
            }
            return worldImage;
        }
    }
}