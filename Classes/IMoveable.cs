using System.Drawing;

namespace Game.Classes
{
    // Интерфейс перемещения.
    public interface IMoveable
    {
        void Move(); // Метод перемещения на каждом шаге.
        void Rotate(); // Метод вращения на каждом шаге.
        void Stop(); // Остановка действий игрового объекта.
        PointF GetNos(); // Передняя точка движенния.
    }
}