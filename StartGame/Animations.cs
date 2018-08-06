using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Linq;

namespace StartGame
{
    public abstract class Animation
    {
        public int time;
    }

    public class LinearAnimation : Animation
    {
        private readonly int start;
        private readonly int end;
        private readonly int step;
        private int current;

        public LinearAnimation(int start, int end, int step = 1) : base()
        {
            Contract.Requires(Math.Abs(start - end) / step > 0);
            Contract.EndContractBlock();

            time = Math.Abs(start - end) / step + 1;
            this.start = start;
            this.end = end;
            this.step = step;
        }

        /// <summary>
        ///  Function which returns every value between the start and end.
        /// </summary>
        /// <param name="MaxSteps">Max number of steps rendered. If reached returns end value instantly</param>
        /// <returns></returns>
        public IEnumerable<int> Animate(int MaxSteps = 100)
        {
            Contract.Requires(MaxSteps > 0);
            Contract.EndContractBlock();

            current = start;
            int counter = 0;
            do
            {
                yield return current;
                counter++;
                if (step == MaxSteps)
                {
                    yield return end;
                    yield break;
                }
                if (current > end)
                    current -= step;
                else if (current < end)
                    current += step;
            } while (current != end);
            yield return end;
        }
    }

    public class LinearPointAnimation : Animation
    {
        private Point start;
        private Point end;
        private Point current;
        public readonly int step;
        private LinearAnimation xAnimation;
        private LinearAnimation yAnimation;

        private IEnumerator<int> xChange;
        private IEnumerator<int> yChange;

        public LinearPointAnimation(Point start, Point end, int step = 1) : base()
        {
            this.start = start;
            this.end = end;
            this.step = step;
            current = start;
            xAnimation = new LinearAnimation(start.X, end.X, step);
            yAnimation = new LinearAnimation(start.Y, end.Y, step);
            xChange = xAnimation.Animate().GetEnumerator();
            yChange = yAnimation.Animate().GetEnumerator();
            time = Math.Max(xAnimation.time, yAnimation.time) + 1;
        }

        public IEnumerable<Point> Animate(int MaxSteps = 100)
        {
            Contract.Requires(MaxSteps > 0);
            Contract.EndContractBlock();
            int step = 0;
            current = start;

            do
            {
                yield return current;
                step++;
                if (step == MaxSteps)
                {
                    yield return end;
                    yield break;
                }

                xChange.MoveNext();
                yChange.MoveNext();
                current.X = xChange.Current;
                current.Y = yChange.Current;
            } while (current != end);
            yield return end;
        }
    }

    public class TeleportPointAnimation : Animation
    {
        private readonly Point start;
        private readonly Point end;

        public TeleportPointAnimation(Point Start, Point End)
        {
            start = Start;
            end = End;
            time = 2;
        }

        public IEnumerable<Point> Animate()
        {
            yield return start;
            yield return end;
        }
    }

    internal class ListPointAnimation : Animation
    {
        private readonly List<Point> movements;
        private readonly Point start;
        private Point current;
        public bool finished = false;

        /// <summary>
        /// Animation where the position changes by the values of a movement every frame
        /// </summary>
        /// <param name="movements">List of points which describe change of start point </param>
        /// <param name="start"> Point to start movements at </param>
        public ListPointAnimation(List<Point> movements, Point start)
        {
            this.movements = movements;
            this.start = start;
            time = movements.Count();
        }

        public IEnumerable<Point> Animate(int MaxSteps = 100)
        {
            Contract.Requires(MaxSteps > 0);
            Contract.EndContractBlock();
            current = start;

            while (movements.Count != 0)
            {
                Point change = movements[0];
                movements.Remove(change);
                current.X += change.X;
                current.Y += change.Y;
                yield return current;
            }
            finished = true;
        }
    }
}