using System.Drawing;
using System.Reflection;

namespace GameServer
{
    public class Rock
    {
        public PointF Position { get; private set; }

        public Rock(PointF position)
        {
            Position = position;
        }
    }
}
