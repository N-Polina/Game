using System;
using System.Drawing;

namespace Game.Classes
{
    public abstract class GameObjectMove : GameObject, IMoveable
    {
        // Двигается или стоит (true - стоит на месте).
        protected bool stop = true;

        // Угол поворота относительно центра прямоугольной области, рад.
        protected float alfa = 0f;

        // Скорость перемещения.
        protected float speed = 1f;

        public GameObjectMove(SharpDX.Direct2D1.Factory factory, float x, float y) : base(factory, x, y) { }

        public void Stop() => stop = true;
        public abstract void Rotate();
        public void SetSpeed(float speed) => this.speed = speed;

        public abstract PointF GetNos();
        public virtual void Move()
        {
            if (!stop)
            {
                x += (float)(speed * Math.Cos(alfa));
                y += (float)(speed * Math.Sin(alfa));
            }
        }
    }
}
