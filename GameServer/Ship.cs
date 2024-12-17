using System.Drawing;
using System.Reflection;

namespace GameServer
{
    public class Ship
    {
        public int Id { get; private set; }
        public PointF Position { get; private set; }
        public float Health { get; private set; } = 100;

        public Ship(int id, PointF startPosition)
        {
            Id = id;
            Position = startPosition;
        }

        public void MoveForward()
        {
            Position = new PointF(Position.X, Position.Y - 10);
        }

        public void MoveBack()
        {
            Position = new PointF(Position.X, Position.Y + 10);
        }

        public void RotateLeft() { /* Логика вращения влево */ }
        public void RotateRight() { /* Логика вращения вправо */ }

        public Torpedo Fire()
        {
            return new Torpedo(Position);
        }
    }
}
