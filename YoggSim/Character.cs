using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoggSim
{
    abstract public class Character
    {
        public Character()
        {

        }

        public void ModifyHealth(int value)
        {
            Damage -= value;
            if (Damage < 0) Damage = 0;
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
        public bool Frozen { get; set; } = false;   // could do some fancy stuff with the Minion.EffectList class if this status stuff gets out of hand.
    }
}
