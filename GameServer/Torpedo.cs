using System.Drawing;
using System.Reflection;

namespace GameServer
{
    public class Torpedo
    {
        public PointF Position { get; private set; }

        public Torpedo(PointF startPosition)
        {
            Position = startPosition;
        }

        public void Move()
        {
            Position = new PointF(Position.X, Position.Y - 5);
        }
    }
}
