using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoggSim
{
    public class Player : Character
    {

        public Player(int initialDamage, int initialHandSize)
        {
            Attack = 0;
            TotalHealth = 30 - initialDamage;
            Damage = initialDamage;
            HandSize = initialHandSize;
        }

        /// <summary>
        /// Draw a number of cards from the player's deck.
        /// </summary>
        public void Draw(int numCards)
        {
            HandSize += numCards;
            TotalDraws += numCards;
            if (HandSize > 10)
            {
                TotalMills += HandSize - 10;
                HandSize = 10;
            }
            if (HandSize < 0) HandSize = 0;
        }

        /// <summary>
        /// Add a number of cards to the player's hand (without drawing them from the deck)
        /// </summary>
        public void AddCards(int numCards)
        {
            HandSize += numCards;
            if (HandSize > 10) HandSize = 10;
            if (HandSize < 0) HandSize = 0;
        }

        public int HandSize { get; set; }
        public int TotalDraws { get; set; }
        public int TotalMills { get; set; }
    }
}
