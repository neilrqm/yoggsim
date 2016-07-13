using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;
using System.IO;
using System.Text.RegularExpressions;
using System.Reflection;

namespace YoggSim
{
    // a few classes defining the structure of the JSON file we're importing card data from
    [DataContract]
    public class JsonCardEffect
    {
        [DataMember]
        public string effect;
        [DataMember]
        public string extra;
    }
    [DataContract]
    public class JsonCard
    {
        [DataMember]
        public int? id;
        [DataMember]
        public string name;
        [DataMember]
        public string description;
        [DataMember]
        public string image_url;
        [DataMember]
        public string hero;
        [DataMember]
        public string category;
        [DataMember]
        public string type;
        [DataMember]
        public string quality;
        [DataMember]
        public string race;
        [DataMember]
        public string set;
        [DataMember]
        public int? mana;
        [DataMember]
        public int? attack;
        [DataMember]
        public int? health;
        [DataMember]
        public bool collectible;
        [DataMember]
        public List<JsonCardEffect> effect_list;
    }
    [DataContract]
    public class JsonCardList
    {
        [DataMember]
        public List<JsonCard> cards;
    }

    // raised when there's a problem with importing from the list of cards
    public class ImportException : Exception { public ImportException(string msg) : base(msg) { } }

    // Delegate for spell function stored in the Spells class.
    public delegate void SpellFunction(Board b);

    /// <summary>
    /// Represents a spell card.
    /// </summary>
    public class Spell
    {
        public Spell(JsonCard card)
        {
            Mana = card.mana ?? default(int);
            Name = card.name ?? "";
            Description = card.description ?? "";

            // find the function corresponding to this spell.
            string functionName = Regex.Replace(Name, @"\W", "");
            MethodInfo method = typeof(Spells).GetMethod(functionName);
            SpellFunction = (SpellFunction)Delegate.CreateDelegate(typeof(SpellFunction), method);
        }

        public int Mana { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public SpellFunction SpellFunction { get; private set; }
    }

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
            Attack = source.Attack;
            TotalHealth = source.TotalHealth;
            Damage = source.Damage;
            Race = source.Race;
        }
        public Minion(string name, int attack, int health, int mana)
        {
            Name = name;
            Attack = attack;
            TotalHealth = health;
            Mana = mana;
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
        }

        public string Name { get; private set; } = "";
        public int Mana { get; private set; } = 0;
        public string Description { get; private set; } = "Empty card";
        public string Race { get; private set; } = "";
        public bool CreatedByYogg { get; set; } = false;
        public EffectList Effects { get; private set; } = new EffectList();

        private static double GetOtherEffectValue(JsonCard card)
        {
            double totalValue = 0.0;
            if (card.description.Contains("Windfury") && !card.description.Contains("Give a friendly minion Windfury.")) totalValue += 2.0;
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
    public abstract class CardFactory
    {
        private static List<Minion> minions = new List<Minion>();
        private static List<Spell> spells = new List<Spell>();
        private static List<Minion> demons = new List<Minion>();
        static CardFactory()
        {
            JsonCardList cards;
            string cardDataFilePath = "../../../../hearthstone-db/cards/all-collectibles.json";
            string[] standardSets = new string[] { "basic", "expert", "brm", "loe", "wtog" };
            // using card info JSON file from https://github.com/pdyck/hearthstone-db/
            if (!File.Exists(cardDataFilePath))
            {
                throw new ImportException("Could not find card data file.");
            }
            using (Stream stream = File.OpenRead(cardDataFilePath))
            {
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(JsonCardList));
                cards = (JsonCardList)ser.ReadObject(stream);
            }
            // load spells and minions that are collectible and in the standard format.
            foreach (JsonCard card in cards.cards)
            {
                if (card.collectible && standardSets.Contains(card.set))
                {
                    if (card.type == "minon" || card.category == "minion")
                    {
                        Minion m = new Minion(card);
                        minions.Add(m);
                        if (card.race == "demon")
                        {
                            demons.Add(m);
                        }
                    }
                    if (card.type == "spell" || card.category == "spell")
                    {
                        spells.Add(new Spell(card));
                    }
                }
            }
            // code to write a list of spell names with non-letter characters removed to a text file
            // used to copy generate the Spells class
            /*using (StreamWriter writer = new StreamWriter(@"spellnames.txt"))
            {
                foreach (Spell spell in spells)
                {
                    // remove non-word characters from spell name to form its function name.
                    writer.WriteLine(Regex.Replace(spell.Name, @"\W", ""));
                }
            }*/

            // code to write a list of spell function names with spell descriptions
            // used as a reference to make spell functions
            /*using (StreamWriter writer = new StreamWriter(@"C:\users\nrqm\desktop\spells.txt"))
            {
                foreach (Spell spell in spells)
                {
                    // remove non-word characters from spell name to form its function name.
                    writer.WriteLine(Regex.Replace(spell.Name, @"\W", ""));
                    writer.WriteLine(spell.Description);
                    writer.WriteLine();
                }
            }*/
        }

        public static Minion GetRandomMinion()
        {
            return new Minion(minions[Simulation.Rng.Next(minions.Count)]);
        }

        public static Spell GetRandomSpell()
        {
            return spells[Simulation.Rng.Next(spells.Count)];   // we don't modify spells, so they don't need to be copied.
        }

        public static Minion GetRandomDemon()
        {
            return new Minion(demons[Simulation.Rng.Next(demons.Count)]);
        }

        public static Minion GetOneOne()
        {
            return new Minion("OneOne", 1, 1, 0);
        }
    }
}
