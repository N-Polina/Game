namespace Game.Classes
{
    public class Decorator1 : ShipDecorator
    {
        public Decorator1(IShip iship) : base(iship)
        {

        }
        // Виртуальные методы декоратора.
        public override float Speed()
        {
            return 6f * base.Speed(); // Множитель шагов.
        }
    }
}
