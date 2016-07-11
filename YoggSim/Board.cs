using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoggSim
{
    public class Board
    {
        public Board(int initialPlayerDamage, int initialOpponentDamage, int playerHandSize, int opponentHandSize)
        {
            Player = new Player(initialPlayerDamage, playerHandSize);
            Opponent = new Player(initialOpponentDamage, opponentHandSize);
            PlayerMinions = new List<Minion>();
            OpponentMinions = new List<Minion>();
            PlayerHandSize = playerHandSize;
            OpponentHandSize = opponentHandSize;
        }

        public int TotalMinions
        {
            get
            {
                return PlayerMinions.Count + OpponentMinions.Count;
            }
        }

        public Minion GetRandomMinion()
        {
            int minionNumber = Simulation.Rng.Next(TotalMinions);
            if (TotalMinions == 0) return null;
            if (minionNumber < PlayerMinions.Count)
            {
                return PlayerMinions[minionNumber];
            }
            else
            {
                return OpponentMinions[minionNumber - PlayerMinions.Count];
            }
        }

        public Character GetRandomCharacter()
        {
            int characterNumber = Simulation.Rng.Next(TotalMinions + 2);
            if (characterNumber == 0) return Player;
            if (characterNumber == 1) return Opponent;
            else return GetRandomMinion();
        }

        public Character GetRandomOpponent()
        {
            int characterNumber = Simulation.Rng.Next(OpponentMinions.Count + 1);
            if (characterNumber < OpponentMinions.Count)
            {
                return OpponentMinions[characterNumber];
            }
            return Opponent;
        }

        public Player Player { get; set; }
        public Player Opponent { get; set; }
        public List<Minion> PlayerMinions { get; set; }
        public List<Minion> OpponentMinions { get; set; }
        public int PlayerHandSize { get; set; }
        public int OpponentHandSize { get; set; }
    }
}
