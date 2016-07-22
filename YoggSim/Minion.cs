using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoggSim
{
    /// <summary>
    /// Represents a minion card
    /// </summary>
    public class Minion : Character
    {
        public class EffectList
        {
            public int AttackModifier { get; set; } = 0;
            public int HealthModifier { get; set; } = 0;
            public double DeathrattleValue { get; set; } = 0.0;
            public double OtherEffectValue { get; set; } = 0.0;
            public bool DestroyAtEndOfTurn { get; set; } = false;
            public bool DestroyAtStartOfNextTurn { get; set; } = false;
            public bool Stealth { get; set; } = false;
            public bool CommandingShout { get; set; } = false;
        }
        public Minion(Minion source)
        {
            Name = source.Name;
            Mana = source.Mana;
            Description = source.Description;
            Effects.AttackModifier = source.Effects.AttackModifier;
            Effects.HealthModifier = source.Effects.HealthModifier;
            Effects.DeathrattleValue = source.Effects.DeathrattleValue;
            Effects.OtherEffectValue = source.Effects.OtherEffectValue;
            Effects.DestroyAtEndOfTurn = source.Effects.DestroyAtEndOfTurn;
            Effects.DestroyAtStartOfNextTurn = source.Effects.DestroyAtStartOfNextTurn;
            Effects.Stealth = source.Effects.Stealth;
            Effects.CommandingShout = source.Effects.CommandingShout;
            Attack = source.Attack;
            TotalHealth = source.TotalHealth;
            Damage = source.Damage;
            Race = source.Race;
            CreatedByYogg = source.CreatedByYogg;
            ReturnToOpponent = source.ReturnToOpponent;
            Frozen = source.Frozen;
        }
        public Minion(string name, int attack, int health, int mana, double otherEffectMana)
        {
            Name = name;
            Attack = attack;
            TotalHealth = health;
            Mana = mana;
            Effects.OtherEffectValue = otherEffectMana;
        }
        public Minion(JsonCard card)
        {
            Name = card.name ?? "";
            Mana = card.mana ?? default(int);
            Attack = card.attack ?? default(int);
            TotalHealth = card.health ?? default(int);
            Race = card.race ?? "";
            Damage = 0;
            Description = card.description ?? "";
            if (Description.Contains("Deathrattle"))
            {
                Effects.DeathrattleValue = GetDeathrattleValue(card);
            }
            Effects.OtherEffectValue = GetOtherEffectValue(card);
        }

        public new void ModifyHealth(int value)
        {
            base.ModifyHealth(value);
            if (Effects.CommandingShout && ActualHealth <= 0) Damage = TotalHealth + Effects.HealthModifier - 1;
        }

        public void Silence()
        {
            Effects.AttackModifier = 0;
            Effects.DeathrattleValue = 0;
            Effects.HealthModifier = 0;
            Effects.OtherEffectValue = 0;
            Effects.DestroyAtEndOfTurn = false;
            Effects.DestroyAtStartOfNextTurn = false;
            Effects.Stealth = false;
            Effects.CommandingShout = false;
            Frozen = false;
        }

        public string Name { get; private set; } = "";
        public int Mana { get; private set; } = 0;
        public string Description { get; private set; } = "Empty card";
        public string Race { get; private set; } = "";
        public bool CreatedByYogg { get; set; } = false;
        public EffectList Effects { get; private set; } = new EffectList();
        public bool ReturnToOpponent { get; set; } = false; // flags that the minion will be returned to opponent at the end of the turn.

        private static double GetOtherEffectValue(JsonCard card)
        {
            double totalValue = 0.0;
            if (card.description != null && card.description.Contains("Windfury") && !card.description.Contains("Give a friendly minion Windfury.")) totalValue += 2.0;
            return totalValue;
        }

        private static double GetDeathrattleValue(JsonCard card)
        {
            string drText = card.description;
            if (card.description.Contains("Draw a card")) return 1.0;
            if (card.description.Contains("Summon a 4/5 Baine Bloodhoof")) return 2.0;
            if (card.description.Contains("Deal 2 damage to ALL characters")) return 0.5;
            if (card.description.Contains("Deal 2 damage to the enemy hero")) return 0.1;
            if (card.description.Contains("Summon a 2/1 Damaged Golem")) return 0.8;
            if (card.description.Contains("Equip a 5/3 Ashbringer")) return 2.5;
            if (card.description.Contains("Summon a 3/3 Finkle Einhorn for your opponent")) return -1.5;
            if (card.description.Contains("Summon two 2/2 Hyenas")) return 1.0;
            if (card.description.Contains("Take control of a random enemy minion")) return 1.5;
            if (card.description.Contains("Replace your hero with Ragnaros, the Firelord")) return 1.5;
            if (card.description.Contains("Add 3 copies of Arcane Missiles to your hand.")) return 1.5;
            if (card.description.Contains("Summon a Dreadsteed.")) return 3.0;
            if (card.description.Contains("Return this to your hand and summon a 4/4 Nerubian.")) return 3.5;
            if (card.description.Contains("Deathrattle: Lose a Mana Crystal.")) return -1.5;
            if (card.description.Contains("If you're holding a Dragon, deal 3 damage to all minions.")) return 1.5;
            if (card.description.Contains("Reveal a minion in each deck. If yours costs more, return this to your hand.")) return 0.5;
            if (card.description.Contains("Give a random friendly minion +3/+3.")) return 1.5;
            if (card.description.Contains("Deal 1 damage to a random enemy.")) return 0.2;
            if (card.description.Contains("Summon three 2/2 Runts.")) return 2.5;
            if (card.description.Contains("Summon a random 1-Cost minion.")) return 1.0;
            if (card.description.Contains("Add a Coin to your hand.")) return 1.0;
            if (card.description.Contains("Add a random Toxin card to your hand.")) return 1.0;
            if (card.description.Contains("Deal 8 damage to all minions.")) return 1.5;
            if (card.description.Contains("Restore 8 Health to the enemy hero.")) return -.5;
            if (card.description.Contains("Deal 1 damage to all minions.")) return 0.5;
            if (card.description.Contains("Give a random friendly minion +1/+1.")) return 0.5;
            if (card.description.Contains("Summon two 1/1 Spiders.")) return 1.5;
            if (card.description.Contains("Give a random friendly minion Divine Shield.")) return 0.2;
            if (card.description.Contains("Summon a 1/1 Shadowbeast.")) return 0.5;
            if (card.description.Contains("Summon a 2/2 Slime.")) return 1.0;
            if (card.description.Contains("Give your minions +1/+1.")) return 1.5;
            if (card.description.Contains("Give your weapon +2 Attack.")) return 0.5;
            if (card.description.Contains("Summon a 5/5 Faceless Destroyer.")) return 3.5;
            if (card.description.Contains("Put all Dragons from your hand into the battlefield.")) return 2.0;
            if (card.description.Contains("Add a random class card to your hand (from your opponent's class).")) return 0.5;
            if (card.description.Contains("Copy a card from your opponent's deck and add it to your hand.")) return 1.0;
            // A few more cards that match "Deathrattle" in their card text, but don't actually have deathrattles.
            if (card.description.Contains("Discover a Deathrattle card.")) return 0.0;
            if (card.description.Contains("Choose a friendly minion. Gain a copy of its Deathrattle effect.")) return 0.0;
            if (card.description.Contains("Summon your Deathrattle minions that died this game.")) return 0.0;
            if (card.description.Contains("Give all Deathrattle minions in your hand +1/+1.")) return 0.0;
            if (card.description.Contains("Trigger a friendly minion's Deathrattle effect.")) return 0.0;
            throw new ImportException(string.Format("A minion ({0}) unexpectedly matched 'Deathrattle' in its card text.", card.name));
        }
    }
}
