namespace GameAI.Pathfinding.Core
{
    using UnityEngine;

    public struct IntRect
    {
        public int xmin, ymin, xmax, ymax;

        public IntRect(int xmin, int ymin, int xmax, int ymax)
        {
            this.xmin = xmin;
            this.xmax = xmax;
            this.ymin = ymin;
            this.ymax = ymax;
        }

        public bool Contains(int x, int y)
        {
            return !(x < xmin || y < ymin || x > xmax || y > ymax);
        }

        public int Width
        {
            get
            {
                return xmax - xmin + 1;
            }
        }

        public int Height
        {
            get
            {
                return ymax - ymin + 1;
            }
        }

        /// <summary>
        /// Returns if this rectangle is valid.
        /// An invalid rect could have e.g xmin > xmax.
        /// Rectamgles with a zero area area invalid.
        /// </summary>
        public bool IsValid()
        {
            return xmin <= xmax && ymin <= ymax;
        }

        public static bool operator ==(IntRect a, IntRect b)
        {
            return a.xmin == b.xmin && a.xmax == b.xmax && a.ymin == b.ymin && a.ymax == b.ymax;
        }

        public static bool operator !=(IntRect a, IntRect b)
        {
            return a.xmin != b.xmin || a.xmax != b.xmax || a.ymin != b.ymin || a.ymax != b.ymax;
        }

        public override bool Equals(System.Object obj)
        {
            var rect = (IntRect)obj;

            return xmin == rect.xmin && xmax == rect.xmax && ymin == rect.ymin && ymax == rect.ymax;
        }

        public override int GetHashCode()
        {
            return xmin * 131071 ^ xmax * 3571 ^ ymin * 3109 ^ ymax * 7;
        }

        /// <summary>
        /// Returns the intersection rect between the two rects.
        /// The intersection rect is the area which is inside both rects.
        /// If the rects do not have an intersection, an invalid rect is returned.
        /// See: IsValid
        /// </summary>
        public static IntRect Intersection(IntRect a, IntRect b)
        {
            return new IntRect(
                System.Math.Max(a.xmin, b.xmin),
                System.Math.Max(a.ymin, b.ymin),
                System.Math.Min(a.xmax, b.xmax),
                System.Math.Min(a.ymax, b.ymax)
                );
        }

        /// <summary>Returns if the two rectangles intersect each other</summary>
        public static bool Intersects(IntRect a, IntRect b)
        {
            return !(a.xmin > b.xmax || a.ymin > b.ymax || a.xmax < b.xmin || a.ymax < b.ymin);
        }

        /// <summary>
        /// Returns a new rect which contains both input rects.
        /// This rectangle may contain areas outside both input rects as well in some cases.
        /// </summary>
        public static IntRect Union(IntRect a, IntRect b)
        {
            return new IntRect(
                System.Math.Min(a.xmin, b.xmin),
                System.Math.Min(a.ymin, b.ymin),
                System.Math.Max(a.xmax, b.xmax),
                System.Math.Max(a.ymax, b.ymax)
                );
        }

        /// <summary>Returns a new IntRect which is expanded to contain the point</summary>
        public IntRect ExpandToContain(int x, int y)
        {
            return new IntRect(
                System.Math.Min(xmin, x),
                System.Math.Min(ymin, y),
                System.Math.Max(xmax, x),
                System.Math.Max(ymax, y)
                );
        }

        /// <summary>Returns a new rect which is expanded by range in all directions.</summary>
        /// <param name="range">How far to expand. Negative values are permitted.</param>
        public IntRect Expand(int range)
        {
            return new IntRect(xmin - range,
                ymin - range,
                xmax + range,
                ymax + range
                );
        }

        public override string ToString()
        {
            return "[x: " + xmin + "..." + xmax + ", y: " + ymin + "..." + ymax + "]";
        }

        /// <summary>Draws some debug lines representing the rect</summary>
        public void DebugDraw(GraphTransform transform, Color color)
        {
            Vector3 p1 = transform.Transform(new Vector3(xmin, 0, ymin));
            Vector3 p2 = transform.Transform(new Vector3(xmin, 0, ymax));
            Vector3 p3 = transform.Transform(new Vector3(xmax, 0, ymax));
            Vector3 p4 = transform.Transform(new Vector3(xmax, 0, ymin));

            Debug.DrawLine(p1, p2, color);
            Debug.DrawLine(p2, p3, color);
            Debug.DrawLine(p3, p4, color);
            Debug.DrawLine(p4, p1, color);
        }
    }
}