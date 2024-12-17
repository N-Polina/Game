namespace Game.Classes
{
    public class Decorator4 : ShipDecorator
    {
        public Decorator4(IShip iship) : base(iship)
        {

        }
        public override float ZapasHoda()
        {
            return 15f * base.ZapasHoda(); // Количество шагов движения корабля.
        }
    }
}
