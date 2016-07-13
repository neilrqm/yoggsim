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

        public Minion GetRandomFriendlyMinion()
        {
            int minionNumber = Simulation.Rng.Next(PlayerMinions.Count);
            return PlayerMinions[minionNumber];
        }

        public Minion GetRandomDemon()
        {
            List<Minion> demonsOnBoard = new List<Minion>();
            foreach (Minion m in PlayerMinions)
            {
                if (m.Race == "demon" && m.CreatedByYogg)   // shortcut (or refinement???): assuming Yogg isn't run in a deck that naturally runs demons
                {
                    demonsOnBoard.Add(m);
                }
            }
            foreach (Minion m in OpponentMinions)
            {
                // shortcut: assume an 8/9 chance that a randomly selected minion is incorrectly marked as a demon
                if (m.Race == "demon" && (Simulation.Rng.Next(9) == 0 || m.CreatedByYogg))
                {
                    demonsOnBoard.Add(m);
                }
            }
            if (demonsOnBoard.Count == 0) return null;
            return demonsOnBoard[Simulation.Rng.Next(demonsOnBoard.Count)];
        }

        public Character GetRandomUndamagedCharacter(bool minionsOnly)
        {
            List<Character> validCharacters = new List<Character>();
            if (!minionsOnly)
            {
                if (Player.Damage == 0) validCharacters.Add(Player);
                if (Opponent.Damage == 0) validCharacters.Add(Opponent);
            }
            foreach (Minion m in PlayerMinions)
            {
                if (m.Damage == 0) validCharacters.Add(m);
            }
            foreach (Minion m in OpponentMinions)
            {
                if (m.Damage == 0) validCharacters.Add(m);
            }
            if (validCharacters.Count == 0) return null;
            return validCharacters[Simulation.Rng.Next(validCharacters.Count)];
        }

        public Player Player { get; set; }
        public Player Opponent { get; set; }
        public List<Minion> PlayerMinions { get; set; }
        public List<Minion> OpponentMinions { get; set; }
        public int PlayerHandSize { get; set; }
        public int OpponentHandSize { get; set; }
    }
}
