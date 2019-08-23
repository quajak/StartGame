using StartGame.Entities;
using StartGame.Rendering;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace StartGame.Rendering
{
    public abstract class RenderObject
    {
        internal Point position;
        internal Point toRender;
        public Bitmap image;
        public readonly bool isSolid;
        public bool visible = true;
        public bool rendered = false;
        private Animation animation;
        public int time;
        public IEnumerator<Point> movementAnimationPoints;
        public string Name = "None";

        public Animation Animation
        {
            get => animation; set
            {
                rendered = false;
                animation = value;
                switch (animation)
                {
                    case LinearPointAnimation a:
                        movementAnimationPoints = a.Animate().GetEnumerator();
                        time = a.time;
                        break;

                    case ListPointAnimation b:
                        movementAnimationPoints = b.Animate().GetEnumerator();
                        time = b.time;
                        break;

                    case TeleportPointAnimation c:
                        movementAnimationPoints = c.Animate().GetEnumerator();
                        time = c.time;
                        break;

                    default:
                        throw new NotImplementedException("This type of animation is not supported!");
                }
            }
        }

        public RenderObject(Point ToRender, Bitmap Image, bool IsSolid = true)
        {
            toRender = ToRender;
            image = Image;
            isSolid = IsSolid;
        }

        /// <summary>
        /// Create new calculation of the same animation type, from position to new position. Will return the toRender position if the same;
        /// </summary>
        public void CalculateAnimation()
        {
            Trace.TraceInformation($"RenderObject::CalculateAnimation Name: {Name} Position: {position} toRender {toRender}");
            if (position == toRender)
            {
                switch (animation)
                {
                    case LinearPointAnimation a:
                        Animation = new LinearPointAnimation(position, toRender, a.step);
                        rendered = true;
                        break;

                    case ListPointAnimation _:
                        rendered = true;
                        Animation = new ListPointAnimation(new List<Point>() { new Point(0, 0) }, toRender);
                        break;

                    case TeleportPointAnimation _:
                        rendered = true;
                        Animation = new TeleportPointAnimation(position, toRender);
                        break;

                    default:
                        throw new NotImplementedException("This type of animation is not supported!");
                }
            }
            else
            {
                switch (animation)
                {
                    case LinearPointAnimation a:
                        animation = new LinearPointAnimation(position, toRender, a.step);
                        movementAnimationPoints = (animation as LinearPointAnimation).Animate().GetEnumerator();
                        rendered = false; //TODO: Actually be able to create animations with length 0
                        break;

                    case ListPointAnimation _:
                        animation = new LinearPointAnimation(position, toRender); //List points animations are turned into linaer point animations
                        movementAnimationPoints = (animation as LinearPointAnimation).Animate().GetEnumerator();
                        rendered = false; //TODO: Actually be able to create animations with length 0
                        break;

                    case TeleportPointAnimation _:
                        rendered = true;
                        Animation = new TeleportPointAnimation(position, toRender);
                        break;

                    default:
                        throw new NotImplementedException("This type of animation is not supported!");
                }
            }
            time = animation.time;
        }

        /// <summary>
        /// Set Position of Render Object.
        /// </summary>
        /// <param name="position">Position</param>
        /// <param name="setType">Determines what position should be set </param>
        public void SetPosition(Point position, PositionSetType setType)
        {
            StackTrace stackTrace = new StackTrace();
            //Trace.TraceInformation($"RenderObject::SetPosition {Name} From: {this.position} To: {position} Type: {setType} Stack: {stackTrace.GetFrame(3).GetMethod().Name}");
            switch (setType)
            {
                case PositionSetType.absolute:
                    this.position = position;
                    break;

                case PositionSetType.goal:
                    rendered = false;
                    toRender = position;
                    break;

                default:
                    break;
            }
        }
    }

    public enum PositionSetType
    { absolute, goal };

    [DebuggerDisplay("Entity Renderer: {Name} : {position} => {toRender} : {time}")]
    public class EntityRenderObject : RenderObject
    {
        public Point Position => position.Mult(MapCreator.fieldSize);

        public EntityRenderObject(Point position, Bitmap image, string name, Animation animation = null) : base(position, image)
        {
            Name = name;
            if (animation is null)
            {
                Animation = new LinearPointAnimation(position, toRender);
            }
            else
            {
                Animation = animation;
            }
        }

        public EntityRenderObject(Entity entity, Animation animation = null) : base(entity.Position, entity.Image, entity.blocking)
        {
            Trace.TraceInformation($"Created new render object for entity name {entity.Name} - {entity}");
            Name = entity.Name;
            if (animation is null)
            {
                Animation = new LinearPointAnimation(position, toRender);
            }
            else
            {
                Animation = animation;
            }
        }
    }
}

namespace StartGame.GameMap
{
    partial class Map
    {
        public object RenderController = new object(); //TODO: Whenever an render object is manipulated it needs to lock this controller
        public List<RenderObject> renderObjects = new List<RenderObject>();

        public List<EntityRenderObject> EntityRenderObjects =>
            renderObjects.Where(o => o is EntityRenderObject).Cast<EntityRenderObject>().ToList();

        public EntityRenderObject GetEntityRenderObject(string Name)
        {
            return EntityRenderObjects.FirstOrDefault(e => e.Name == Name);
        }

        #region Render Objects

        public void RemoveEntityRenderObject(string name)
        {
            renderObjects.RemoveAll(o => {
                if (o is EntityRenderObject entity)
                {
                    return entity.Name == name;
                }
                return false;
            });
        }

        #endregion Render Objects

        #region Render

        private Bitmap renderedEntityMap;

        //Past background creation flags
        private bool usedDebug = false;

        private int usedSize = 20;
        private int usedContinentAlpha = 0;
        private int usedShowGoal = 1;
        private int usedColorAlpha = 255;
        private bool usedInverseHeights = false;

        /// <summary>
        /// Function used to render all objects using their animations.
        /// </summary>
        /// <param name="gameBoard"> PictureBox used to set the image in </param>
        /// <param name="time"> Milliseconds between frames </param>
        /// <param name="debug"></param>
        /// <param name="size"></param>
        /// <param name="continentAlpha"></param>
        /// <param name="showGoal"></param>
        /// <param name="colorAlpha"></param>
        /// <param name="showInverseHeight"></param>
        public void Render(PictureBox gameBoard, List<Bitmap> frames, bool forceEntityRendering = false,
            bool debug = false, int size = MapCreator.fieldSize, int continentAlpha = 0, int showGoal = 1, int colorAlpha = 255, bool showInverseHeight = false,
            bool forceDrawBackground = false)
        {
            TimeSpan _time = DateTime.Now.TimeOfDay;
            lock (RenderController)
            {
                renderObjects.ForEach(o => { if (o.time <= 0) o.CalculateAnimation(); });

                if (!forceDrawBackground && debug == usedDebug && size == usedSize && continentAlpha == usedContinentAlpha && usedShowGoal == showGoal
                    && colorAlpha == usedColorAlpha && usedInverseHeights == showInverseHeight)
                {
                    //All parameters are the same
                    rawBackground = rawBackground ?? DrawMapBackground(gameBoard.Width, gameBoard.Height, debug, size, continentAlpha, showGoal, colorAlpha, showInverseHeight);
                }
                else
                {
                    rawBackground = DrawMapBackground(gameBoard.Width, gameBoard.Height, debug, size, continentAlpha, showGoal, colorAlpha, showInverseHeight);
                    usedDebug = debug;
                    usedSize = size;
                    usedContinentAlpha = continentAlpha;
                    usedShowGoal = showGoal;
                    usedColorAlpha = colorAlpha;
                    usedInverseHeights = showInverseHeight;
                }

                if ((renderObjects.Count != 0 && renderObjects.Exists(o => !o.rendered)) || forceEntityRendering)
                {
                    int time = renderObjects.Max(o => o.time);
                    int[] times = new int[renderObjects.Count]; //DEBUG
                    int[] aTimes = new int[renderObjects.Count]; //DEBUG
                    Point[] points = new Point[renderObjects.Count]; //DEBUG
                    for (int i = 0; i < renderObjects.Count; i++)
                    {
                        //Trace.TraceInformation($"Rendering {renderObjects[i].Name} in {renderObjects[i].time} from {points[i] = renderObjects[i].position}");
                        times[i] = renderObjects[i].time;
                        points[i] = renderObjects[i].position;
                    }
                    //Trace.TraceInformation($"Rendering {renderObjects.Count} objects at {_time} in {time} steps.");
                    for (int i = 0; i < time + 1; i++)
                    {
                        //Move all entites
                        int c = 0; //DEBUG
                        renderObjects.ForEach(o => {
                            if (o.movementAnimationPoints.MoveNext())
                            {
                                o.SetPosition(o.movementAnimationPoints.Current, PositionSetType.absolute);
                                o.time--;
                                aTimes[c]++; //DEBUG
                                if (o.time < 0) throw new Exception("The rest time of an animation can not be less than 0");
                            }
                            c++;
                        });

                        //Render the new screen
                        renderedEntityMap = new Bitmap(rawBackground);

                        using (Graphics g = Graphics.FromImage(renderedEntityMap))
                        {
                            foreach (RenderObject obj in renderObjects)
                            {
                                if (obj is EntityRenderObject entity)
                                {
                                    g.DrawImage(obj.image, entity.Position.X, entity.Position.Y, 20, 20);
                                }
                            }
                        }
                        //TODO: Allow overlay in background
                        //Add overlay
                        Bitmap imageOverlay = new Bitmap(gameBoard.Width, gameBoard.Height);
                        using (Graphics g = Graphics.FromImage(imageOverlay))
                        {
                            g.Clear(Color.Transparent);
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
                            }
                        }

                        //Combine
                        using (Graphics g = Graphics.FromImage(renderedEntityMap))
                        {
                            g.DrawImage(imageOverlay, new Point(0, 0));
                        }

                        lock (frames)
                        {
                            frames.Add(new Bitmap(renderedEntityMap));
                        }

                        //int sleepTime = frameTime - (int)(DateTime.Now.TimeOfDay - _time).TotalMilliseconds;
                        //_time = DateTime.Now.TimeOfDay;
                        //Thread.Sleep(sleepTime > 10 ? sleepTime : 0);
                    }
                    //Set all render objects as renderd
                    renderObjects.ForEach(o => o.rendered = true);

                    //Check that all animations have completed -- DEBUG Test
                    if (renderObjects.Exists(o => o.time != 0)) throw new Exception("All animations should have finished at this point!");
                }
                else
                {
                    Trace.TraceInformation($"Skipped rendering {renderObjects.Count} objects at {_time}.");
                    //Use the last generated image
                    Bitmap rendered = new Bitmap(renderedEntityMap ?? rawBackground);

                    //Add overlay
                    Bitmap imageOverlay = new Bitmap(gameBoard.Width, gameBoard.Height);
                    using (Graphics g = Graphics.FromImage(imageOverlay))
                    {
                        g.Clear(Color.Transparent);
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
                        }
                    }

                    //Combine
                    using (Graphics g = Graphics.FromImage(rendered))
                    {
                        g.DrawImage(imageOverlay, new Point(0, 0));
                    }

                    lock (frames)
                    {
                        frames.Add(new Bitmap(rendered));
                    }
                }
                //Update Overlay
                overlayObjects = overlayObjects.Where(o => !o.once).ToList();
            }
        }

        #endregion Render

        #region Drawing

        private Image rawBackground; //No troops

        /// <summary>
        /// Function which generates a Bitmap from the map without any troops or overlay
        /// </summary>
        /// <param name="Width">Width of the bitmap</param>
        /// <param name="Height">Height of the bitmap</param>
        /// <param name="Debug">Will show cost of field from a specific goal and count the number of tiles</param>
        /// <param name="size">Size of field in pixel</param>
        /// <param name="continentAlpha">Sets alpha of continent alpha, if 0 they are not shown</param>
        /// <param name="showGoal">Which goal will be selected for debug</param>
        /// <param name="colorAlpha">Sets alpha of field color</param>
        /// <param name="showInverseHeight">Determine if height should be inversed for grey overlay color</param>
        /// <returns></returns>
        public Bitmap DrawMapBackground(int Width, int Height, bool Debug = false, int size = 20, int continentAlpha = 100, int showGoal = 1, int colorAlpha = 255, bool showInverseHeight = false)
        {
            averageTile = 0.5;
            hillTile = 0;
            flatTile = 0;
            waterTile = 0;
            Font font = new Font(FontFamily.GenericSansSerif, 8);
            Bitmap mapBackground = new Bitmap(Width, Height);
            using (Graphics g = Graphics.FromImage(mapBackground))
            {
                g.Clear(Color.Transparent);
                for (int x = 0; x <= map.GetUpperBound(0); x++)
                {
                    for (int y = 0; y <= map.GetUpperBound(1); y++)
                    {
                        //Average tile
                        averageTile = (averageTile + map[x, y].Height) / 2;

                        Color b = map[x, y].shader;
                        Color c = map[x, y].color;
                        Color d = Color.Black;
                        if (Debug)
                        {
                            c = Color.FromArgb((int)(map[x, y].Height * 255), 0, 0, 0);
                            switch (map[x, y].type.type)
                            {
                                case MapTileTypeEnum.land:
                                    flatTile++;
                                    break;

                                case MapTileTypeEnum.mountain:
                                    hillTile += 2;
                                    break;

                                case MapTileTypeEnum.hill:
                                    hillTile++;
                                    break;

                                case MapTileTypeEnum.shallowWater:
                                    waterTile++;
                                    break;

                                case MapTileTypeEnum.deepWater:
                                    waterTile += 2;
                                    break;

                                case MapTileTypeEnum.road:
                                    break;

                                case MapTileTypeEnum.wall:
                                    break;

                                default:
                                    throw new NotImplementedException();
                            }
                        }
                        else
                        {
                            //Adjust alpha of color
                            if (colorAlpha != 255)
                                c = Color.FromArgb(colorAlpha, c.R, c.G, c.B);

                            if (showInverseHeight)
                                b = Color.FromArgb(255 - b.A, 0, 0, 0);

                            d = map[x, y].continent.color;
                            d = Color.FromArgb(continentAlpha, d.R, d.G, d.B);
                        }

                        if (Debug)
                        {
                            if (!(map[x, y].Costs is null) && map[x, y].Costs.Length > showGoal)
                            {
                                string s = map[x, y].Costs[showGoal].ToString();
                                g.FillRectangle(Brushes.Black, x * size, y * size, size, size);
                                g.DrawString(s, font, Brushes.White, new Point(x * size, y * size));
                            }
                        }
                        else
                        {
                            g.FillRectangle(new SolidBrush(c), x * size, y * size, size, size);
                            g.FillRectangle(new SolidBrush(d), x * size, y * size, size, size);
                            g.FillRectangle(new SolidBrush(b), x * size, y * size, size, size);
                        }
                    }
                }
                rawBackground = mapBackground;
            }
            return mapBackground;
        }

        #endregion Drawing

        #region Overlay

        private enum OverlayType
        { wholeMission, render, once };

        public List<OverlayObject> overlayObjects = new List<OverlayObject>();

        #endregion Overlay
    }
}