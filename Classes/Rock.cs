using SharpDX.Direct2D1;
using System.Drawing;

namespace Game.Classes
{
    public class Rock : GameObject
    {
        readonly RoundedRectangleGeometry rectangleGeometry;

        public Rock(SharpDX.Direct2D1.Factory factory, float x, float y) : base(factory, x, y) {
            rectangleGeometry = new RoundedRectangleGeometry(factory, new RoundedRectangle()
            {
                RadiusX = 5,
                RadiusY = 5,
                Rect = new SharpDX.RectangleF(x, y, GameConsts.ROCK_SIZE, GameConsts.ROCK_LENGTH)
            });
        }

        public override void Draw(WindowRenderTarget windowRenderTarget)
        {
            var solidColorBrush = new SolidColorBrush(windowRenderTarget, SharpDX.Color.DarkBlue);
            windowRenderTarget.FillGeometry(rectangleGeometry, solidColorBrush, null);
        }

        public override PointF[] GetPolygon()
        {
            var w = GameConsts.ROCK_SIZE;
            var h = GameConsts.ROCK_LENGTH;

            return new PointF[] {
                new PointF(x - w, y - h),
                new PointF(x + w, y - h),
                new PointF(x + w, y + h),
                new PointF(x - w, y + h)
            };
        }
    }
}
