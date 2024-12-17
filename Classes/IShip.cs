namespace Game.Classes
{
    public interface IShip : IDrawable, IMoveable
    {
        void MoveForward();
        void MoveBack();
        void RotateLeft();
        void RotateRight();
        void ChangeWeapon();
        Torpedo Fire();
        float GetLife();
        void MinusLife();
    }
}
