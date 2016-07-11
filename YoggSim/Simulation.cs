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
        public static double WindfuryManaModifier { get; private set; } = 2;
    }
}
