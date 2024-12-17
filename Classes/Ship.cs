using System;
using System.Drawing;
using SharpDX.Direct2D1;

namespace Game.Classes
{
    public enum DirectionMove
    {
        Forward, Back
    }
    public enum DirectionRotate
    {
        Left, Right
    }
    public enum Doing
    {
        Move, Rotate, Stop
    }
    public struct ShipArea
    {
        public float x1, x2, x3, x4, x5, x6, x7, x8, x9;
        public float y1, y2, y3, y4, y5, y6, y7, y8, y9;
    }

    public class Ship : GameObjectMove, IShip
    {
        private DirectionMove directionMove; // Направление перемещения (вперёд, назад).
        private DirectionRotate directionRotate; // Направление вращения (влево, вправо).

        private Doing doing; // Что делает кораблю в данный момент.
        private float life; // Количество жизней.

        // Номер выбранной пушки.
        private int selectWeapon = 0;
        private Color blue;

        // Цвет корабля
        public SharpDX.Color ShipColor { get; set; }


        public Ship(SharpDX.Direct2D1.Factory factory, float x, float y, SharpDX.Color color): base(factory, x, y)
        {
            directionMove = DirectionMove.Forward;
            doing = Doing.Move;
            speed = 2;
            life = 10f;
            ShipColor = color; // Устанавливаем цвет корабля
        }

        public Ship(Factory factory, float x, float y, Color blue) : base(factory, x, y)
        {
            this.blue = blue;
        }

        public float GetLife() => life;

        // Двигаться вперёд.
        public void MoveForward()
        {
            stop = false;
            directionMove = DirectionMove.Forward;
            doing = Doing.Move;
        }

        // Двигаться назад.
        public void MoveBack()
        {
            stop = false;
            directionMove = DirectionMove.Back;
            doing = Doing.Move;
        }

        // Поворот влево.
        public void RotateLeft()
        {
            stop = false;
            directionRotate = DirectionRotate.Left;
            doing = Doing.Rotate;
        }

        // Поворот вправо.
        public void RotateRight()
        {
            stop = false;
            directionRotate = DirectionRotate.Right;
            doing = Doing.Rotate;
        }

        // Выбор пушки.
        public void ChangeWeapon()
        {
            selectWeapon = selectWeapon + 1 > 3 ? 0 : selectWeapon + 1;
        }

        public Torpedo Fire()
        {
            var param = GetShipParam();
            switch (selectWeapon)
            {
                case 0:
                    return new Torpedo(factory, param.x2, param.y2, alfa + (float)Math.PI);
                case 1:
                    return new Torpedo(factory, param.x3, param.y3, alfa + 0.5f * (float)Math.PI);
                case 2:
                    return new Torpedo(factory, param.x1, param.y1, alfa);
                default:
                    return new Torpedo(factory, param.x4, param.y4, alfa - 0.5f * (float)Math.PI);
            }
        }

        public override PointF[] GetPolygon()
        {
            var param = GetShipParam();

            return new PointF[] {
                new PointF(param.x5, param.y5),
                new PointF(param.x6, param.y6),
                new PointF(param.x8, param.y8),
                new PointF(param.x1, param.y1),
                new PointF(param.x7, param.y7)
            };
        }
        public override PointF GetNos()
        {
            return directionMove == DirectionMove.Forward ? 
                new PointF()
                {
                    X = x + (float)(GameConsts.SHIP_LENGTH * Math.Cos(alfa)),
                    Y = y + (float)(GameConsts.SHIP_LENGTH * Math.Sin(alfa))
                } : 
                new PointF()
                {
                    X = x - (float)(GameConsts.SHIP_LENGTH * Math.Cos(alfa)),
                    Y = y - (float)(GameConsts.SHIP_LENGTH * Math.Sin(alfa))
                };
        }

        public ShipArea GetShipParam()
        {
            var x1 = x + (float)(GameConsts.SHIP_LENGTH * Math.Cos(alfa));
            var y1 = y + (float)(GameConsts.SHIP_LENGTH * Math.Sin(alfa));
            var x2 = x - (float)(GameConsts.SHIP_LENGTH * Math.Cos(alfa));
            var y2 = y - (float)(GameConsts.SHIP_LENGTH * Math.Sin(alfa));

            var x3 = x + (float)(GameConsts.SHIP_SIZE * Math.Cos(alfa + 0.5 * Math.PI));
            var y3 = y + (float)(GameConsts.SHIP_SIZE * Math.Sin(alfa + 0.5 * Math.PI));
            var x4 = x - (float)(GameConsts.SHIP_SIZE * Math.Cos(alfa + 0.5 * Math.PI));
            var y4 = y - (float)(GameConsts.SHIP_SIZE * Math.Sin(alfa + 0.5 * Math.PI));

            var x5 = x2 + (float)(GameConsts.SHIP_SIZE * Math.Cos(alfa + 0.5 * Math.PI));
            var y5 = y2 + (float)(GameConsts.SHIP_SIZE * Math.Sin(alfa + 0.5 * Math.PI));
            var x6 = x2 - (float)(GameConsts.SHIP_SIZE * Math.Cos(alfa + 0.5 * Math.PI));
            var y6 = y2 - (float)(GameConsts.SHIP_SIZE * Math.Sin(alfa + 0.5 * Math.PI));

            var x9 = x + (float)(0.25 * GameConsts.SHIP_LENGTH * Math.Cos(alfa));
            var y9 = y + (float)(0.25 * GameConsts.SHIP_LENGTH * Math.Sin(alfa));

            var x7 = x9 + (float)(GameConsts.SHIP_SIZE * Math.Cos(alfa + 0.5 * Math.PI));
            var y7 = y9 + (float)(GameConsts.SHIP_SIZE * Math.Sin(alfa + 0.5 * Math.PI));
            var x8 = x9 - (float)(GameConsts.SHIP_SIZE * Math.Cos(alfa + 0.5 * Math.PI));
            var y8 = y9 - (float)(GameConsts.SHIP_SIZE * Math.Sin(alfa + 0.5 * Math.PI));

            return new ShipArea
            {
                x1 = x1,
                x2 = x2,
                x3 = x3,
                x4 = x4,
                x5 = x5,
                x6 = x6,
                x7 = x7,
                x8 = x8,
                x9 = x9,
                y1 = y1,
                y2 = y2,
                y3 = y3,
                y4 = y4,
                y5 = y5,
                y6 = y6,
                y7 = y7,
                y8 = y8,
                y9 = y9
            };
        }
        private System.Drawing.Color ConvertToDrawingColor(SharpDX.Color color)
        {
            return System.Drawing.Color.FromArgb(
                (int)(color.A * 255),
                (int)(color.R * 255),
                (int)(color.G * 255),
                (int)(color.B * 255)
            );
        }


        public override void Draw(WindowRenderTarget windowRenderTarget)
        {
            var param = GetShipParam();
            // Рисование оружия.
            DrawWeapon(param.x2, param.y2, windowRenderTarget, selectWeapon == 0);
            DrawWeapon(param.x3, param.y3, windowRenderTarget, selectWeapon == 1);
            DrawWeapon(param.x1, param.y1, windowRenderTarget, selectWeapon == 2);
            DrawWeapon(param.x4, param.y4, windowRenderTarget, selectWeapon == 3);



            // Рисуем корабль с заданным цветом
            using (var geo1 = new PathGeometry(factory))
            {
                using (var sink = geo1.Open())
                {
                    sink.BeginFigure(new SharpDX.Vector2(param.x5, param.y5), FigureBegin.Filled);
                    sink.AddLines(new SharpDX.Mathematics.Interop.RawVector2[]
                    {
                        new SharpDX.Vector2(param.x7, param.y7),
                        new SharpDX.Vector2(param.x1, param.y1),
                        new SharpDX.Vector2(param.x4, param.y4),
                        new SharpDX.Vector2(param.x6, param.y6)
                    });
                    sink.EndFigure(FigureEnd.Closed);
                    sink.Close();
                }

                using (var solidColorBrush = new SolidColorBrush(windowRenderTarget, ShipColor))
                {
                    windowRenderTarget.FillGeometry(geo1, solidColorBrush);
                }
            }
        }

        private void DrawWeapon(float x, float y, WindowRenderTarget windowRenderTarget, bool selected)
        {
            int r = 3;
            using (var solidColorBrush = new SolidColorBrush(windowRenderTarget, selected ? SharpDX.Color.GreenYellow : SharpDX.Color.DarkBlue))
            {
                windowRenderTarget.DrawEllipse(new Ellipse(new SharpDX.Mathematics.Interop.RawVector2(x, y), r, r), solidColorBrush);
            }
        }

        public override void Move()
        {
            if (doing == Doing.Move)
            {
                speed = directionMove == DirectionMove.Forward ? Math.Abs(speed) : -Math.Abs(speed);
                base.Move();
            }
        }

        public override void Rotate()
        {
            if (!stop && doing == Doing.Rotate)
            {
                double directionSign = directionRotate == DirectionRotate.Left ? -1 : 1;
                alfa += (float)(directionSign * GameConsts.SHIP_ANGLE_ROTATE);
            }
        }

        public void MinusLife() => life = life > 0 ? life - 1 : 0;
    }
}
