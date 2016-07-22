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
using System.Net;

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

    public abstract class CardFactory
    {
        private static List<Minion> minions = new List<Minion>();
        private static List<Spell> spells = new List<Spell>();
        private static List<Minion> demons = new List<Minion>();
        static CardFactory()
        {
            JsonCardList cards;
            // Assumes that 
            string cardDataFilePath = "./all-collectibles.json";
            string[] standardSets = new string[] { "basic", "expert", "brm", "loe", "wtog" };
            if (!File.Exists(cardDataFilePath))
            {
                Console.Write("Downloading card data file from https://github.com/pdyck/hearthstone-db/... ");
                new WebClient().DownloadFile("https://raw.githubusercontent.com/pdyck/hearthstone-db/master/cards/all-collectibles.json", cardDataFilePath);
                Console.WriteLine("Done");
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
                    else if (card.type == "spell" || card.category == "spell")
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
    }
}
