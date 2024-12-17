using System;
using System.Drawing;

namespace Game.Classes
{
    public class Geometry
    {

        // Проверка попадания точки в область.
        // Полигон должен быть задан по часовой стрелке.
        public static bool Inside(PointF[] points, PointF point)
        {
            int sign = 1;
            for (int i = 0; i < points.Length && sign >= 0; i++)
            {
                var nextPointF = i < points.Length - 1 ? points[i + 1] : points[0];
                Line line = new Line(points[i], nextPointF);
                sign = Math.Sign(line.Y(point));
            }
            return sign > 0;
        }
        public static bool AllInside(PointF[] points, PointF[] inPoints)
        {
            bool inSide = true;
            foreach (var point in points)
                inSide &= Inside(inPoints, point);
            return inSide;
        }
        public static bool AnyInside(PointF[] points, PointF[] inPoints)
        {
            bool inSide = false;
            foreach (var point in points)
                inSide |= Inside(inPoints, point);
            return inSide;
        }
    }
}
