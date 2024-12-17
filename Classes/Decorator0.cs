using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Classes
{
    public class Decorator0 : ShipDecorator
    {
        public Decorator0(IShip iship) : base(iship)
        {

        }
        public override float ZapasHoda()
        {
            return 1f * base.ZapasHoda(); // Количество шагов движения корабля.
        }
    }
}
