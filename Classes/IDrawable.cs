using System.Drawing;
using SharpDX.Direct2D1;

namespace Game.Classes
{
    // Интерфейс перерисовки.
    public interface IDrawable
    {
        void Draw(WindowRenderTarget windowRenderTarget);
        float GetX(); 
        float GetY();
        void SetX(float x); 
        void SetY(float y);
        PointF[] GetPolygon(); // Возвращает область (полигон) объекта.
    }
}
