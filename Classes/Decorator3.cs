namespace Game.Classes
{
    public class Decorator3 : ShipDecorator
    {
        public Decorator3(IShip iship) : base(iship)
        {

        }
        public override float Bron()
        {
            return 2f * base.Bron(); // Количество вычитаемых жизней при попадании.
        }
    }
}
