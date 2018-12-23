using System.Diagnostics;
using System.Drawing;

namespace StartGame.Rendering
{
    public class OverlayObject
    {
        public int x;
        public int y;
        public bool once;

        public OverlayObject(int X, int Y, bool Once = true)
        {
            x = X;
            y = Y;
            once = Once;
        }
    }

    internal class OverlayRectangle : OverlayObject
    {
        public int width;
        public int height;
        public readonly Color borderColor;
        public bool filled;
        public readonly Color fillColor;

        public OverlayRectangle(int X, int Y, int Width, int Height, Color BorderColor,
            bool Filled = false, Color FillColor = new Color(), bool Once = true) : base(X, Y, Once)
        {
            width = Width;
            height = Height;
            borderColor = BorderColor;
            filled = Filled;
            fillColor = FillColor;
        }
    }

    internal class OverlayText : OverlayObject
    {
        public Color color;
        public string text;

        public OverlayText(int X, int Y, Color Color, string Text, bool Once = true) : base(X, Y, Once)
        {
            color = Color;
            text = Text;
        }
    }

    [DebuggerDisplay("OverlayLine - {start} => {end}")]
    internal class OverlayLine : OverlayObject
    {
        public Point start;
        public Point end;
        public Color color;

        public OverlayLine(Point start, Point end, Color color, bool Once = true) : base(start.X, start.Y, Once)
        {
            this.start = start;
            this.color = color;
            this.end = end;
        }
    }
}