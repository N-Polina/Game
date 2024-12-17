using System;
using System.Drawing;
using SharpDX.Direct2D1;

namespace Game.Classes
{
    public class Torpedo : GameObjectMove
    {
        private IShip iship = null;

        public Torpedo(SharpDX.Direct2D1.Factory factory, float x, float y, float alfa) : base(factory, x, y)
        {
            this.alfa = alfa;
            this.stop = false;
        }

        public IShip GetShip() => iship;
        public void SetShip(IShip iship) => this.iship = iship;

        public override void Draw(WindowRenderTarget windowRenderTarget)
        {
            var param = GetPolygon();
            GeometrySink sink1;
            var geo1 = new PathGeometry(factory);

            sink1 = geo1.Open();
            sink1.BeginFigure(new SharpDX.Vector2(param[0].X, param[0].Y), new FigureBegin());
            sink1.AddLines(new SharpDX.Mathematics.Interop.RawVector2[] {
            new SharpDX.Vector2(param[1].X, param[1].Y),
            new SharpDX.Vector2(param[2].X, param[2].Y),
            new SharpDX.Vector2(param[3].X, param[3].Y),
            new SharpDX.Vector2(param[4].X, param[4].Y)
            });
            sink1.EndFigure(new FigureEnd());
            sink1.Close();

            var solidColorBrush = new SolidColorBrush(windowRenderTarget, SharpDX.Color.Red);
            windowRenderTarget.FillGeometry(geo1, solidColorBrush, null);
            //graphics.FillPolygon(GameConsts.TORPEDO_BRUSH, GetPolygon());
        }

        public override PointF GetNos()
        {
            var h = 0.5 * GameConsts.ROCK_LENGTH;
            return new PointF()
            {
                X = x + (float)(h * Math.Cos(alfa)),
                Y = y + (float)(h * Math.Sin(alfa))
            };
        }
        public override PointF[] GetPolygon()
        {
            var w = 0.5 * GameConsts.ROCK_SIZE;
            var h = 0.5 * GameConsts.ROCK_LENGTH;

            var x1 = x + (float)(h * Math.Cos(alfa));
            var y1 = y + (float)(h * Math.Sin(alfa));
            var x2 = x - (float)(h * Math.Cos(alfa));
            var y2 = y - (float)(h * Math.Sin(alfa));

            var x5 = x2 + (float)(w * Math.Cos(alfa + 0.5 * Math.PI));
            var y5 = y2 + (float)(w * Math.Sin(alfa + 0.5 * Math.PI));
            var x6 = x2 - (float)(w * Math.Cos(alfa + 0.5 * Math.PI));
            var y6 = y2 - (float)(w * Math.Sin(alfa + 0.5 * Math.PI));

            var x9 = x + (float)(0.25 * h * Math.Cos(alfa));
            var y9 = y + (float)(0.25 * h * Math.Sin(alfa));

            var x7 = x9 + (float)(w * Math.Cos(alfa + 0.5 * Math.PI));
            var y7 = y9 + (float)(w * Math.Sin(alfa + 0.5 * Math.PI));
            var x8 = x9 - (float)(w * Math.Cos(alfa + 0.5 * Math.PI));
            var y8 = y9 - (float)(w * Math.Sin(alfa + 0.5 * Math.PI));

            return new PointF[] {
                new PointF(x5, y5),
                new PointF(x7, y7),
                new PointF(x1, y1),
                new PointF(x8, y8),
                new PointF(x6, y6)
            };
        }

        public override void Rotate() { }
    }
}
