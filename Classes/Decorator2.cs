namespace Game.Classes
{
    public class Decorator2 : ShipDecorator
    {
        public Decorator2(IShip iship) : base(iship)
        {

        }
        public override float Weapon()
        {
            return 6f * base.Weapon(); //  Множитель шагов выпущенной торпеды.
        }
        public override float Bron()
        {
            return 2f * base.Bron(); // Количество вычитаемых жизней при попадании.
        }
    }
}
