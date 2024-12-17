using System.Drawing;

namespace Game.Classes
{
    public class GameConsts
    {


        public const float SHIP_LENGTH = 50f;
        public const float SHIP_SIZE = 20f;
        public const float TORPEDO_LENGTH = 15f;
        public const float TORPEDO_SIZE = 5f;
        public const float ROCK_SIZE = 15;
        public const float ROCK_LENGTH = 30;

        public static readonly Font LIFES_FONT = new Font("Arial", 14);
        public static readonly Brush LIFES_BRUSH = new SolidBrush(Color.Gray);
        public static readonly Brush SHIP_BRUSH = new SolidBrush(Color.Gray);
        public static readonly Pen SHIP_PEN = new Pen(Color.Gray);
        public static readonly Pen TORPEDO_PEN = new Pen(Color.Gray);
        public static readonly Brush TORPEDO_BRUSH = new SolidBrush(Color.Gray);
        public static readonly Brush ROCK_BRUSH = new SolidBrush(Color.Gray);

        public const float SHIP_ANGLE_ROTATE = 0.1f;

        public const float AREA_BORDER = 20f;
    }
}
