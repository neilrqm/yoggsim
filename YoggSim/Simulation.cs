using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoggSim
{
    class Simulation
    {
        Board board;

        void RunSimulation()
        {
            board = new Board(GetInitialDamage(), GetInitialDamage(), GetHandSize(), GetHandSize());
            // nrqm - Minion yogg = new Minion();
        }

        int GetInitialDamage()
        {
            return 0;
        }
        int GetHandSize()
        {
            return 0;
        }

        public static Random Rng { get; private set; } = new Random();

        public static double AttackManaModifier { get; private set; } = 0.5;
        public static double HealthManaModifier { get; private set; } = 0.5;
        public static double WindfuryManaValue { get; private set; } = 2.0;
        public static double TauntManaValue { get; private set; } = 1.0;
        public static double MageSecretManaValue { get; private set; } = 3.0;
        public static double HunterSecretManaValue { get; private set; } = 2.0;
        public static double PaladinSecretManaValue { get; private set; } = 1.0;
        public static double ChargeManaModifier { get; private set; } = 0.4;
        public static double CardDiscountManaModifier { get; private set; } = 0.5;
        public static double ResummonMinionDeathrattleModifier { get; private set; } = 0.5;
        public static double AttackBuffOtherMinionsModifier { get; private set; } = 1.0;
        public static double DivineShieldManaValue { get; private set; } = 1.0;
    }
}
