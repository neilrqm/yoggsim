using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoggSim
{
    public class Player : Character
    {
        private int numSecrets = 0;
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

        public new void ModifyHealth(int value)
        {
            if (value < 0) TotalFaceDamage += value;
            else TotalFaceHeal += value;
            base.ModifyHealth(value);
        }

        public void AddSecretValue(double manaValue)
        {
            numSecrets++;
            if (numSecrets <= 5)
            {
                // shortcut: not taking into consideration the situation where a Yogg plays one secret twice)
                SecretValue += manaValue;
            }
        }

        public int Armour { get; set; } = 0;
        public int HandSize { get; set; }
        public int TotalDraws { get; set; } = 0;
        public int TotalMills { get; set; } = 0;
        public int CardsAddedToDeck { get; set; } = 0;
        public int TotalFaceDamage { get; set; } = 0;
        public int TotalFaceHeal { get; set; } = 0;
        public double SecretValue { get; private set; } = 0;
        public double ActivatedDeathrattleValue { get; set; } = 0;
        public int HeroAttackBuff { get; set; } = 0;
        public double OtherEffectManaValue { get; set; } = 0;
    }
}
