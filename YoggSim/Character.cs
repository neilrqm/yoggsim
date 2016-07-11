using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoggSim
{
    public class Character
    {
        public Character()
        {

        }

        public int TotalHealth { get; protected set; }
        public int ActualHealth
        {
            get
            {
                if (this is Minion)
                {
                    return (TotalHealth + ((Minion)this).Effects.HealthModifier) - Damage;
                }
                return TotalHealth - Damage;
            }
        }
        public int Attack { get; protected set; }
        public int Damage { get; set; }
    }
}
