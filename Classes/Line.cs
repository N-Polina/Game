using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;

namespace Game.Classes
{
	public class Line
	{
		private readonly float A, B, C, sin, cos;
		public Line(PointF point1, PointF point2) : this(
			point1.Y - point2.Y,
			point2.X - point1.X,
			point1.X * point2.Y - point2.X * point1.Y)
		{
		}
		public Line(float A, float B, float C)
		{
			this.A = A;
			this.B = B;
			this.C = C;
			float length = (float)Math.Sqrt(A * A + B * B);
			sin = length != 0 ? -A / length : float.NaN;
			cos = length != 0 ? B / length : float.NaN;
		}
		public float Y(PointF point)
		{
			return A * point.X + B * point.Y + C;
		}
		public float Sin()
		{
			return sin;
		}
		public float Cos()
		{
			return cos;
		}
		public float GetA()
		{
			return A;
		}
		public float GetB()
		{
			return B;
		}
		public float GetC()
		{
			return C;
		}
	}
}
