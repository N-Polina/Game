using System;
using System.Drawing;
using SharpDX.Direct2D1;


namespace Game.Classes
{
    public abstract class ShipDecorator : IShip
    {
        protected readonly IShip iship;
        private float kol;

        public ShipDecorator(IShip iship)
        {
            this.iship = iship;
            kol = ZapasHoda();
        }

        public void ChangeWeapon() => iship.ChangeWeapon();
        public virtual float GetLife() => iship.GetLife();
        public virtual void RotateLeft()
        {
            kol = ZapasHoda();
            iship.RotateLeft();
        }
        public void RotateRight()
        {
            kol = ZapasHoda();
            iship.RotateRight();
        }

        public Torpedo Fire()
        {
            var torpedo = iship.Fire();
            torpedo.SetSpeed(Weapon()); // Устанавливаем скорость торпеды.
            torpedo.SetShip(iship); // Устаналиваем обновлённый корабль, с которого выпущена торпеда.
            return torpedo;
        }

        public void MinusLife()
        {
            for (int i = 0; i < Bron(); i++)
                iship.MinusLife();
        }

        public void Move()
        {
            if (kol > 0)
            {
                for (int i = 0; i < Speed(); i++) iship.Move();
                kol -= 1f;
            }
        }

        public void MoveBack()
        {
            kol = ZapasHoda();
            iship.MoveBack();
        }

        public void MoveForward()
        {
            kol = ZapasHoda();
            iship.MoveForward();
        }
  
        // Виртуальные методы декоратора.
        public virtual float Speed()
        {
            return 1f; // Множитель шагов.
        }
        public virtual float Weapon()
        {
            return 1f; //  Множитель шагов выпущенной торпеды.
        }
        public virtual float Bron()
        {
            return 1f; // Количество вычитаемых жизней при попадании.
        }
        public virtual float ZapasHoda()
        {
            return 20f; // Количество шагов движения корабля.
        }

        public void Draw(WindowRenderTarget windowRenderTarget)
        {
            iship.Draw(windowRenderTarget);
        }

        public float GetX()
        {
            return iship.GetX();
        }

        public float GetY()
        {
            return iship.GetY();
        }

        public void SetX(float x)
        {
            iship.SetX(x);
        }

        public void SetY(float y)
        {
            iship.SetY(y);
        }

        public PointF[] GetPolygon()
        {
            return iship.GetPolygon();
        }

        public void Rotate()
        {
            if (kol > 0)
            {
                iship.Rotate();
                kol--;
            }
        }

        public void Stop()
        {
            iship.Stop();
        }

        public PointF GetNos()
        {
            return iship.GetNos();
        }

        public Ship GetBaseShip()
        {
            if (iship is Ship ship)
            {
                return ship;
            }

            if (iship is ShipDecorator decorator)
            {
                return decorator.GetBaseShip();
            }

            throw new InvalidOperationException("Базовый корабль не найден");
        }

    }
}