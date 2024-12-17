using SharpDX;

namespace Game.Classes
{
    class ShipFactory
    {
        public static ShipDecorator GetShipType1(Ship ship)
        {
            ship.ShipColor = Color.Red; // Используем SharpDX.Color
            return new Decorator1(new Decorator3(ship));
        }

        public static ShipDecorator GetShipType2(Ship ship)
        {
            ship.ShipColor = Color.Green;
            return new Decorator2(new Decorator3(ship));
        }

        public static ShipDecorator GetShipType3(Ship ship)
        {
            ship.ShipColor = Color.Blue;
            return new Decorator1(new Decorator4(ship));
        }

        public static ShipDecorator GetShipType4(Ship ship)
        {
            ship.ShipColor = Color.Yellow;
            return new Decorator4(new Decorator0(ship));
        }

        public static ShipDecorator GetShipType5(Ship ship)
        {
            ship.ShipColor = Color.Purple;
            return new Decorator1(new Decorator2(new Decorator4(ship)));
        }
    }
}
