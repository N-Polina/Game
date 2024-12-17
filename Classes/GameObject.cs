using System.Drawing;
using SharpDX.Direct2D1;

namespace Game.Classes
{
    // Базовый класс игрового объекта.
    public abstract class GameObject : IDrawable
    {
        // координаты центра прямоугольной области игрового объекта
        // на игровом поле
        protected float x, y;
        protected SharpDX.Direct2D1.Factory factory;

        public GameObject (SharpDX.Direct2D1.Factory factory, float x, float y)
        {
            this.factory = factory;
            this.x = x;
            this.y = y;
        }

        public float GetX() => x;
        public float GetY() => y;

        public void SetX(float x) => this.x = x;
        public void SetY(float y) => this.y = y;

        // метод перерисовки
        public abstract void Draw(WindowRenderTarget windowRenderTarget);
        
        // область объекта.
        public abstract PointF[] GetPolygon();
    }
}