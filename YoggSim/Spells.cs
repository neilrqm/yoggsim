using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoggSim
{
    abstract class Spells
    {
        /// <summary>
        /// Take control of an enemy minion.
        /// </summary>
        public static void MindControl(Board b)
        {
            Minion m = b.OpponentMinions[Simulation.Rng.Next(b.OpponentMinions.Count)];
            b.OpponentMinions.Remove(m);
            if (b.PlayerMinions.Count < 7)
            {
                b.PlayerMinions.Add(m);
            }
        }
        
        /// <summary>
        /// Deal 1 damage to a minion and give it +2 Attack.
        /// </summary>
        public static void InnerRage(Board b)
        {
            Minion m = b.GetRandomMinion();
            if (m == null) return;
            m.ModifyHealth(1);
            m.Effects.AttackModifier += 2;
        }
        
        /// <summary>
        /// Deal 2 damage to a character. If that kills it, summon a random Demon.
        /// </summary>
        public static void BaneofDoom(Board b)
        {
            Character c = b.GetRandomCharacter();
            c.ModifyHealth(-2);
            if (c.Damage >= c.ActualHealth && c is Minion)
            {
                // if a minion died...
                if (b.PlayerMinions.Contains(c) || b.PlayerMinions.Count < 7)
                {
                    // and there's room to summon a new demon, then do so
                    Minion m = CardFactory.GetRandomDemon();
                    m.CreatedByYogg = true;
                    b.PlayerMinions.Add(m);
                }
            }
        }
        
        /// <summary>
        /// Copy 2 cards from your opponent's deck and put them into your hand.
        /// </summary>
        public static void Thoughtsteal(Board b)
        {
            b.Player.AddCards(2);
        }
        
        /// <summary>
        /// Give a minion Windfury.
        /// </summary>
        public static void Windfury(Board b)
        {
            Minion m = b.GetRandomMinion();
            if (m == null) return;
            m.Effects.OtherEffectValue += Simulation.WindfuryManaValue;    // doesn't handle situation where minion already has windfury
        }
        
        /// <summary>
        /// Deal 4 damage to an enemy and 1 damage to all other enemies.
        /// </summary>
        public static void Swipe(Board b)
        {
            Character target = b.GetRandomOpponent();
            target.ModifyHealth(-4);
            if (target != b.Opponent) b.Opponent.ModifyHealth(-1);
            foreach (Minion m in b.OpponentMinions)
            {
                if (m != target)
                {
                    m.ModifyHealth(-1);
                }
            }
        }
        
        /// <summary>
        /// Give a minion +3 Attack.
        /// </summary>
        public static void BlessingofMight(Board b)
        {
            Minion m = b.GetRandomMinion();
            if (m == null) return;
            m.Effects.AttackModifier += 3;
        }
        
        /// <summary>
        /// Destroy all minions except one. (chosen randomly)
        /// </summary>
        public static void Brawl(Board b)
        {
            Minion m = b.GetRandomMinion();
            if (m == null) return;
            bool players = false;
            if (b.PlayerMinions.Contains(m)) players = true;
            b.PlayerMinions.Clear();   // shortcut: this doesn't capture deathrattle effects for cards like Sylvanas, Cairne, etc.
            b.OpponentMinions.Clear();
            if (players) b.PlayerMinions.Add(m);
            else b.OpponentMinions.Add(m);
        }
        
        /// <summary>
        /// Transform a minion into a 1/1 Sheep.
        /// </summary>
        public static void Polymorph(Board b)
        {
            Minion m = b.GetRandomMinion();
            if (m == null) return;
            if (b.PlayerMinions.Contains(m)) b.PlayerMinions[b.PlayerMinions.IndexOf(m)] = new Minion("Sheep", 1, 1, 0, 0);
            else if (b.OpponentMinions.Contains(m)) b.OpponentMinions[b.OpponentMinions.IndexOf(m)] = new Minion("Sheep", 1, 1, 0, 0);
            else throw new Exception("Got a random minion that isn't on the board.");
        }
        
        /// <summary>
        /// Choose One - Deal 5 damage to a minion; or 2 damage to all enemy minions.
        /// </summary>
        public static void Starfall(Board b)
        {
            int coinFlip = Simulation.Rng.Next(2);
            if (coinFlip == 0)
            {
                // Pick a minion and deal 5 damage to it
                Minion m = b.GetRandomMinion();
                if (m == null) return;
                m.ModifyHealth(-5);
            }
            else if (coinFlip == 1)
            {
                // Deal 2 damage to all enemy minions
                foreach (Minion m in b.OpponentMinions)
                {
                    m.ModifyHealth(-2);
                }
            }
            else
            {
                throw new Exception("I don't know how to use the Random class.");
            }
        }
        
        /// <summary>
        /// Choose One - Gain 2 Mana Crystals; or Draw 3 cards.
        /// </summary>
        public static void Nourish(Board b)
        {
            int coinFlip = Simulation.Rng.Next(2);
            if (coinFlip == 0)
            {
                b.Player.Draw(3);
            }
            else
            {
                // ignoring addition of mana crystals
                // shortcut: this doesn't capture casting Yogg with Innervate or multiple Thaurisson ticks
            }
        }
        
        /// <summary>
        /// Secret: When your opponent casts a spell, Counter it.
        /// </summary>
        public static void Counterspell(Board b)
        {
            b.Player.AddSecretValue(Simulation.MageSecretManaValue);
        }
        
        /// <summary>
        /// Secret: When one of your minions dies, return it to life with 1 Health.
        /// </summary>
        public static void Redemption(Board b)
        {
            b.Player.AddSecretValue(Simulation.PaladinSecretManaValue);
        }
        
        /// <summary>
        /// Change a minion's Health to 1.
        /// </summary>
        public static void HuntersMark(Board b)
        {
            Minion m = b.GetRandomMinion();
            if (m == null) return;
            m.Damage = 0;   // silencing this effect will heal the minion to full, which appears to be correct
            m.Effects.HealthModifier = 1 - m.TotalHealth;
        }
        
        /// <summary>
        /// Put a copy of a random minion from your opponent's deck into the battlefield.
        /// </summary>
        public static void Mindgames(Board b)
        {
            if (b.PlayerMinions.Count < 7)
            {
                // shortcut: just put a random minion into the battlefield
                b.PlayerMinions.Add(CardFactory.GetRandomMinion());
            }
        }
        
        /// <summary>
        /// Destroy a friendly minion and deal its Attack damage to all enemy minions.
        /// </summary>
        public static void Shadowflame(Board b)
        {
            if (b.PlayerMinions.Count == 0) return;
            Minion m = b.GetRandomFriendlyMinion();
            foreach (Minion oppMinion in b.OpponentMinions)
            {
                oppMinion.ModifyHealth(-m.Attack);
            }
            b.PlayerMinions.Remove(m);
        }
        
        /// <summary>
        /// Restore a minion to full Health and give it Taunt.
        /// </summary>
        public static void AncestralHealing(Board b)
        {
            Minion m = b.GetRandomMinion();
            if (m == null) return;
            m.Damage = 0;
            m.Effects.OtherEffectValue += Simulation.TauntManaValue;
        }
        
        /// <summary>
        /// Choose One - Give a minion +4 Attack; or +4 Health and Taunt.
        /// </summary>
        public static void MarkofNature(Board b)
        {
            int coinFlip = Simulation.Rng.Next(2);
            Minion m = b.GetRandomMinion();
            if (m == null) return;
            if (coinFlip == 0)
            {
                m.Effects.AttackModifier += 4;
            }
            else if (coinFlip == 1)
            {
                m.Effects.HealthModifier += 4;
                m.Effects.OtherEffectValue += Simulation.TauntManaValue;
            }
        }
        
        /// <summary>
        /// Destroy a Demon. Restore #5 Health to your hero.
        /// </summary>
        public static void SacrificialPact(Board b)
        {
            Minion m = b.GetRandomDemon();
            if (m != null)
            {
                if (b.PlayerMinions.Contains(m)) b.PlayerMinions.Remove(m);
                else if (b.OpponentMinions.Contains(m)) b.OpponentMinions.Remove(m);
                else throw new Exception("Found a minion that isn't on the board.");
                b.Player.ModifyHealth(5);
            }
        }
        
        /// <summary>
        /// Freeze a character. If it was already Frozen, deal 4 damage instead.
        /// </summary>
        public static void IceLance(Board b)
        {
            Character c = b.GetRandomCharacter();
            if (c.Frozen)
            {
                c.ModifyHealth(-4);
                // shortcut: just realized we're not accounting for spell damage buffs.  I don't think we should?
                // Yogg is usually not played with spell damage on board, and accounting for it would result in an 
                // error swing of some magnitude or another.  Should probably add it as an OtherEffectValue when 
                // loading minions though.  Might be interesting to do a separate simulation with spell damage
                // counting fully.
            }
            c.Frozen = true;
        }
        
        /// <summary>
        /// Deal 2 damage to an undamaged minion.
        /// </summary>
        public static void Backstab(Board b)
        {
            Character c = b.GetRandomUndamagedCharacter(true);
            if (c == null) return;
            c.ModifyHealth(-2);
        }
        
        /// <summary>
        /// Secret: When your hero takes fatal damage, prevent it and become Immune this turn.
        /// </summary>
        public static void IceBlock(Board b)
        {
            b.Player.AddSecretValue(Simulation.MageSecretManaValue);
        }
        
        /// <summary>
        /// Secret: When your opponent plays a minion, summon a copy of it.
        /// </summary>
        public static void MirrorEntity(Board b)
        {
            b.Player.AddSecretValue(Simulation.MageSecretManaValue);
        }
        
        /// <summary>
        /// Return all minions to their owner's hand.
        /// </summary>
        public static void Vanish(Board b)
        {
            b.Player.AddCards(b.PlayerMinions.Count);
            b.Opponent.AddCards(b.OpponentMinions.Count);
            b.PlayerMinions.Clear();
            b.OpponentMinions.Clear();
        }
        
        /// <summary>
        /// Give a minion Taunt and +2/+2. (+2 Attack/+2 Health)
        /// </summary>
        public static void MarkoftheWild(Board b)
        {
            Minion m = b.GetRandomMinion();
            if (m == null) return;
            m.Effects.AttackModifier += 2;
            m.Effects.HealthModifier += 2;
            m.Effects.OtherEffectValue += Simulation.TauntManaValue;
        }
        
        /// <summary>
        /// Gain control of an enemy minion with 3 or less Attack until end of turn.
        /// </summary>
        public static void ShadowMadness(Board b)
        {
            List<Minion> qualifyingMinions = new List<Minion>();
            if (b.PlayerMinions.Count >= 7) return; // board full
            foreach (Minion m in b.OpponentMinions)
            {
                if (m.ActualAttack <= 3)
                {
                    qualifyingMinions.Add(m);
                }
            }
            if (qualifyingMinions.Count == 0) return; // no qualifying minions
            Minion selectedMinion = qualifyingMinions[Simulation.Rng.Next(qualifyingMinions.Count)];
            selectedMinion.ReturnToOpponent = true;
            b.OpponentMinions.Remove(selectedMinion);
            b.PlayerMinions.Add(selectedMinion);
        }
        
        /// <summary>
        /// Secret: When your opponent plays a minion, reduce its Health to 1.
        /// </summary>
        public static void Repentance(Board b)
        {
            b.Player.AddSecretValue(Simulation.PaladinSecretManaValue);
        }
        
        /// <summary>
        /// Destroy a minion. Your opponent draws 2 cards.
        /// </summary>
        public static void Naturalize(Board b)
        {
            Minion m = b.GetRandomMinion();
            if (m == null) return;
            if (b.PlayerMinions.Contains(m))
            {
                b.PlayerMinions.Remove(m);
                b.Player.ActivatedDeathrattleValue += m.Effects.DeathrattleValue;
            }
            if (b.OpponentMinions.Contains(m))
            {
                b.OpponentMinions.Remove(m);
                b.Opponent.ActivatedDeathrattleValue += m.Effects.DeathrattleValue;
            }
            b.Opponent.Draw(2);
        }
        
        /// <summary>
        /// Summon two 2/3 Spirit Wolves with Taunt. Overload: (2)
        /// </summary>
        public static void FeralSpirit(Board b)
        {
            for (int i = 0; i < 2 && b.PlayerMinions.Count < 7; i++)
            {
                b.PlayerMinions.Add(new Minion("FeralSpirit Wolf", 2, 3, 2, Simulation.TauntManaValue));
            }
        }
        
        /// <summary>
        /// Give a friendly character +3 Attack this turn.
        /// </summary>
        public static void RockbiterWeapon(Board b)
        {
            Character c = b.GetRandomFriendlyCharacter();
            if (c is Player)
            {
                ((Player)c).HeroAttackBuff += 3;
            }
            // shortcut: ignoring the effect of Rockbiter on minions since it's unlikely to fall on a minion that can attack.
        }
        
        /// <summary>
        /// Deal 3 damage. Draw a card.
        /// </summary>
        public static void HammerofWrath(Board b)
        {
            Character c = b.GetRandomCharacter();
            c.ModifyHealth(-3);
            b.Player.Draw(1);
        }
        
        /// <summary>
        /// Gain 2 Mana Crystals this turn only.
        /// </summary>
        public static void Innervate(Board b)
        {
            // This isn't going to have much of an effect usually.  It will allow the player to get
            // in a hero power, so I'm giving it a value of 1 in terms of mana swing.  You could justify
            // increasing this slightly to account for the ability to play a 2-mana minion.
            b.Player.OtherEffectManaValue += 1;
        }
        
        /// <summary>
        /// Give a minion +2 Attack. Combo: +4 Attack instead.
        /// </summary>
        public static void ColdBlood(Board b)
        {
            Minion m = b.GetRandomMinion();
            if (m == null) return;
            m.Effects.AttackModifier += 4;  // always comboed with Yogg, I guess?
        }
        
        /// <summary>
        /// Deal 2 damage.
        /// </summary>
        public static void HolySmite(Board b)
        {
            Character c = b.GetRandomCharacter();
            c.ModifyHealth(-2);
        }
        
        /// <summary>
        /// An enemy minion deals its damage to the minions next to it.
        /// </summary>
        public static void Betrayal(Board b)
        {
            Minion m = b.GetRandomOpponentMinion();
            int i = b.OpponentMinions.IndexOf(m);
            if (i > 0) b.OpponentMinions[i - 1].ModifyHealth(-m.ActualAttack);
            if (i < b.OpponentMinions.Count - 1) b.OpponentMinions[i + 1].ModifyHealth(-m.ActualAttack);
        }
        
        /// <summary>
        /// Secret: When a minion attacks your hero, destroy it.
        /// </summary>
        public static void Vaporize(Board b)
        {
            b.Player.AddSecretValue(Simulation.MageSecretManaValue);
        }
        
        /// <summary>
        /// Restore #6 Health.
        /// </summary>
        public static void HolyLight(Board b)
        {
            Character c = b.GetRandomCharacter();
            c.ModifyHealth(6);
        }
        
        /// <summary>
        /// Deal 3 damage to two random enemy minions.
        /// </summary>
        public static void MultiShot(Board b)
        {
            // I'm guessing this fizzles if the opponent has only 1 minion on board.
            if (b.OpponentMinions.Count >= 2)
            {
                Minion m1 = b.GetRandomOpponentMinion();
                Minion m2 = b.GetRandomOpponentMinion();
                while (m2 != m1) m2 = b.GetRandomOpponentMinion();
                m1.ModifyHealth(-3);
                m2.ModifyHealth(-3);
            }
        }
        
        /// <summary>
        /// Deal 3 damage. If you have a Beast, deal 5 damage instead.
        /// </summary>
        public static void KillCommand(Board b)
        {
            Character c = b.GetRandomCharacter();
            bool hasBeast = false;
            foreach (Minion m in b.PlayerMinions)
            {
                if (m.Race == "beast") hasBeast = true;
            }
            // Guessing that beasts are relatively unlikely to be played compared to their population
            // in the card pool (but more likely than demons), so I'm weighting the likelihood of a random
            // minion being marked as a beast erroneously as 2/3.
            if (hasBeast && Simulation.Rng.Next(9) < 3)
            {
                c.ModifyHealth(-5);
            }
            else
            {
                c.ModifyHealth(-3);
            }
        }
        
        /// <summary>
        /// Deal 2 damage to 2 random enemy minions. Overload: (2)
        /// </summary>
        public static void ForkedLightning(Board b)
        {
            // I'm guessing this fizzles if the opponent has only 1 minion on board.
            if (b.OpponentMinions.Count >= 2)
            {
                Minion m1 = b.GetRandomOpponentMinion();
                Minion m2 = b.GetRandomOpponentMinion();
                while (m2 != m1) m2 = b.GetRandomOpponentMinion();
                m1.ModifyHealth(-2);
                m2.ModifyHealth(-2);
            }
        }
        
        /// <summary>
        /// Deal 6 damage.
        /// </summary>
        public static void Fireball(Board b)
        {
            Character c = b.GetRandomCharacter();
            c.ModifyHealth(-6);
        }
        
        /// <summary>
        /// Give a friendly minion +2 Attack and Charge.
        /// </summary>
        public static void Charge(Board b)
        {
            Minion m = b.GetRandomFriendlyMinion();
            m.Effects.AttackModifier += 2;
            // not sure what the value of charge is, but it seems to be based on the minion's attack
            // value.  I'm assigning it a weight of 0.4 * the attack value, based on the attack mana modifier
            // of 0.5 reduced by the possibility of the minion only being able to attack face (but this might
            // be too cute, who knows).
            if (m.CreatedByYogg) m.Effects.OtherEffectValue += (double)m.ActualAttack * Simulation.ChargeManaModifier;
        }
        
        /// <summary>
        /// Destroy an enemy minion.
        /// </summary>
        public static void Assassinate(Board b)
        {
            Minion m = b.GetRandomOpponentMinion();
            if (m == null) return;
            b.OpponentMinions.Remove(m);
        }
        
        /// <summary>
        /// Return a friendly minion to your hand. It costs (2) less.
        /// </summary>
        public static void Shadowstep(Board b)
        {
            Minion m = b.GetRandomFriendlyMinion();
            if (m == null) return;
            b.PlayerMinions.Remove(m);
            b.Player.AddCards(1);
            // Player gets 2 extra mana of value, but it's probably going to be 2 turns before the 
            // card can affect the board again, so I'm weighting the value.
            b.Player.OtherEffectManaValue += Simulation.CardDiscountManaModifier * 2;
        }
        
        /// <summary>
        /// Secret: When an enemy casts a spell on a minion, summon a 1/3 as the new target.
        /// </summary>
        public static void Spellbender(Board b)
        {
            b.Player.AddSecretValue(Simulation.MageSecretManaValue);
        }
        
        /// <summary>
        /// Change a minion's Attack to be equal to its Health.
        /// </summary>
        public static void InnerFire(Board b)
        {
            Minion m = b.GetRandomMinion();
            if (m == null) return;
            m.Effects.AttackModifier = m.ActualHealth - m.Attack;

        }
        
        /// <summary>
        /// Give your minions "Deathrattle: Summon a 2/2 Treant."
        /// </summary>
        public static void SouloftheForest(Board b)
        {
            foreach (Minion m in b.PlayerMinions)
            {
                m.Effects.DeathrattleValue += 1;
            }
        }
        
        /// <summary>
        /// Deal 5 damage to a minion and 2 damage to adjacent ones.
        /// </summary>
        public static void ExplosiveShot(Board b)
        {
            Minion m = b.GetRandomMinion();
            if (m == null) return;
            m.ModifyHealth(-5);
            if (b.OpponentMinions.Contains(m))
            {
                int i = b.OpponentMinions.IndexOf(m);
                if (i > 0) b.OpponentMinions[i - 1].ModifyHealth(-2);
                if (i < b.OpponentMinions.Count - 1) b.OpponentMinions[i + 1].ModifyHealth(-2);
            }
            else if (b.PlayerMinions.Contains(m))
            {
                int i = b.PlayerMinions.IndexOf(m);
                if (i > 0) b.PlayerMinions[i - 1].ModifyHealth(-2);
                if (i < b.PlayerMinions.Count - 1) b.PlayerMinions[i + 1].ModifyHealth(-2);
            }
        }
        
        /// <summary>
        /// Draw a card for each damaged friendly character.
        /// </summary>
        public static void BattleRage(Board b)
        {
            int totalDraws = 0;
            if (b.Player.Damage > 0) totalDraws++;
            foreach (Minion m in b.PlayerMinions)
            {
                if (m.Damage > 0) totalDraws++;
            }
            b.Player.Draw(totalDraws);
        }
        
        /// <summary>
        /// Choose a minion. When that minion is destroyed, return it to the battlefield.
        /// </summary>
        public static void AncestralSpirit(Board b)
        {
            Minion m = b.GetRandomMinion();
            if (m == null) return;
            // Hey, free mana!  But not liquid.  Takes a while to cash in.
            m.Effects.DeathrattleValue += m.Mana * Simulation.ResummonMinionDeathrattleModifier;
        }
        
        /// <summary>
        /// Freeze a minion and the minions next to it, and deal 1 damage to them.
        /// </summary>
        public static void ConeofCold(Board b)
        {
            Minion m = b.GetRandomMinion();
            if (m == null) return;
            m.ModifyHealth(-1);
            m.Frozen = true;
            if (b.OpponentMinions.Contains(m))
            {
                int i = b.OpponentMinions.IndexOf(m);
                if (i > 0)
                {
                    b.OpponentMinions[i - 1].ModifyHealth(-1);
                    b.OpponentMinions[i - 1].Frozen = true;
                }
                if (i < b.OpponentMinions.Count - 1)
                {
                    b.OpponentMinions[i + 1].ModifyHealth(-1);
                    b.OpponentMinions[i + 1].Frozen = true;
                }
            }
            else if (b.PlayerMinions.Contains(m))
            {
                int i = b.PlayerMinions.IndexOf(m);
                if (i > 0)
                {
                    b.PlayerMinions[i - 1].ModifyHealth(-1);
                    b.PlayerMinions[i - 1].Frozen = true;
                }
                if (i < b.PlayerMinions.Count - 1)
                {
                    b.PlayerMinions[i + 1].ModifyHealth(-1);
                    b.PlayerMinions[i + 1].Frozen = true;
                }
            }
        }
        
        /// <summary>
        /// Draw a card and deal damage equal to its cost.
        /// </summary>
        public static void HolyWrath(Board b)
        {
            // let's just draw a random minion and hope it's close enough to a real situation.
            Character c = b.GetRandomCharacter();
            c.ModifyHealth(-CardFactory.GetRandomMinion().Mana);
        }
        
        /// <summary>
        /// Summon a random Beast Companion.
        /// </summary>
        public static void AnimalCompanion(Board b)
        {
            if (b.PlayerMinions.Count >= 7) return;
            switch (Simulation.Rng.Next(3))
            {
                case 0:
                    //summon huffer
                    b.PlayerMinions.Add(new Minion("Huffer", 4, 2, 3, Simulation.ChargeManaModifier * 4));
                    break;
                case 1:
                    // summon huffer
                    b.PlayerMinions.Add(new Minion("Leokk", 2, 4, 3, Simulation.AttackBuffOtherMinionsModifier * 1));
                    break;
                case 2:
                    // summon huffer
                    b.PlayerMinions.Add(new Minion("Misha", 4, 4, 3, Simulation.TauntManaValue));
                    break;
            }
        }
        
        /// <summary>
        /// Deal 1 damage to all enemy minions.
        /// </summary>
        public static void ArcaneExplosion(Board b)
        {
            foreach (Minion m in b.OpponentMinions)
            {
                m.ModifyHealth(-1);
            }
        }
        
        /// <summary>
        /// Secret: When one of your minions is attacked, summon three 1/1 Snakes.
        /// </summary>
        public static void SnakeTrap(Board b)
        {
            b.Player.AddSecretValue(Simulation.HunterSecretManaValue);
        }
        
        /// <summary>
        /// Deal 2 damage to all enemy minions and Freeze them.
        /// </summary>
        public static void Blizzard(Board b)
        {
            foreach (Minion m in b.OpponentMinions)
            {
                m.ModifyHealth(-2);
                m.Frozen = true;
            }
        }
        
        /// <summary>
        /// Give your weapon +2 Attack.
        /// </summary>
        public static void DeadlyPoison(Board b)
        {
            // shortcut: Imma just ignore weapons for now.
        }
        
        /// <summary>
        /// Return an enemy minion to its owner's hand.
        /// </summary>
        public static void Sap(Board b)
        {
            Minion m = b.GetRandomOpponentMinion();
            if (m == null) return;
            b.OpponentMinions.Remove(m);
            b.Opponent.AddCards(1);
        }
        
        /// <summary>
        /// Secret: When your hero takes damage, deal that much damage to the enemy hero.
        /// </summary>
        public static void EyeforanEye(Board b)
        {
            b.Player.AddSecretValue(Simulation.PaladinSecretManaValue);
        }
        
        /// <summary>
        /// Deal 1 damage.
        /// </summary>
        public static void Moonfire(Board b)
        {
            Character c = b.GetRandomCharacter();
            c.ModifyHealth(-1);
        }
        
        /// <summary>
        /// Deal 2 damage to all enemies.
        /// </summary>
        public static void Consecration(Board b)
        {
            b.Opponent.ModifyHealth(-2);
            foreach (Minion m in b.OpponentMinions)
            {
                m.ModifyHealth(-2);
            }
        }
        
        /// <summary>
        /// Deal damage equal to your hero's Attack to a minion.
        /// </summary>
        public static void Savagery(Board b)
        {
            Minion m = b.GetRandomMinion();
            if (m == null) return;
            m.ModifyHealth(b.Player.ActualAttack);
        }
        
        /// <summary>
        /// Summon three 2/2 Treants
        /// </summary>
        public static void ForceofNature(Board b)
        {
            for (int i = 0; i < 3 && b.PlayerMinions.Count < 7; i++)
            {
                b.PlayerMinions.Add(new Minion("FON Treant", 2, 2, 2, 0));
            }
        }
        
        /// <summary>
        /// Choose One - Give your minions +1/+1; or Summon a 3/2 Panther.
        /// </summary>
        public static void PoweroftheWild(Board b)
        {
            int coinFlip = Simulation.Rng.Next(2);
            if (coinFlip == 0)
            {
                foreach (Minion m in b.PlayerMinions)
                {
                    m.Effects.AttackModifier++;
                    m.Effects.HealthModifier++;
                }
            }
            else if (coinFlip == 1)
            {
                if (b.PlayerMinions.Count < 7)
                {
                    b.PlayerMinions.Add(new Minion("PotW Panther", 3, 2, 2, 0));
                }
            }
        }
        
        /// <summary>
        /// Deal 3 damage. Overload: (1)
        /// </summary>
        public static void LightningBolt(Board b)
        {
            Character c = b.GetRandomCharacter();
            c.ModifyHealth(-3);
        }
        
        /// <summary>
        /// If you have a weapon, give it +1/+1. Otherwise equip a 1/3 weapon.
        /// </summary>
        public static void Upgrade(Board b)
        {
            // shortcut: ignoring weapons for now
        }
        
        /// <summary>
        /// Secret: When an enemy minion attacks, return it to its owner's hand and it costs (2) more.
        /// </summary>
        public static void FreezingTrap(Board b)
        {
            b.Player.AddSecretValue(Simulation.HunterSecretManaValue);
        }
        
        /// <summary>
        /// Deal 5 damage to the enemy hero.
        /// </summary>
        public static void MindBlast(Board b)
        {
            b.Opponent.ModifyHealth(-5);
        }
        
        /// <summary>
        /// Deal 1 damage to a minion for each Armor you have.
        /// </summary>
        public static void ShieldSlam(Board b)
        {
            Minion m = b.GetRandomMinion();
            if (m == null) return;
            m.ModifyHealth(-b.Player.Armour);
        }
        
        /// <summary>
        /// Draw 2 cards.
        /// </summary>
        public static void ArcaneIntellect(Board b)
        {
            b.Player.Draw(2);
        }
        
        /// <summary>
        /// Deal 3 damage randomly split among enemy characters.
        /// </summary>
        public static void ArcaneMissiles(Board b)
        {
            for (int i = 0; i < 3; i++)
            {
                Character c = b.GetRandomOpponent();
                c.ModifyHealth(-1);
            }
        }
        
        /// <summary>
        /// Deal 1 damage. Draw a card.
        /// </summary>
        public static void Shiv(Board b)
        {
            Character c = b.GetRandomCharacter();
            c.ModifyHealth(-1);
            b.Player.Draw(1);
        }
        
        /// <summary>
        /// Give your hero +4 Attack this turn and 4 Armor.
        /// </summary>
        public static void Bite(Board b)
        {
            b.Player.HeroAttackBuff += 4;
            b.Player.Armour += 4;
        }
        
        /// <summary>
        /// Secret: When an enemy attacks, summon a 2/1 Defender as the new target.
        /// </summary>
        public static void NobleSacrifice(Board b)
        {
            b.Player.AddSecretValue(Simulation.PaladinSecretManaValue);
        }
        
        /// <summary>
        /// Secret: When your hero is attacked, deal 2 damage to all enemies.
        /// </summary>
        public static void ExplosiveTrap(Board b)
        {
            b.Player.AddSecretValue(Simulation.HunterSecretManaValue);
        }
        
        /// <summary>
        /// Freeze all enemy minions.
        /// </summary>
        public static void FrostNova(Board b)
        {
            foreach (Minion m in b.OpponentMinions)
            {
                m.Frozen = true;
            }
        }
        
        /// <summary>
        /// Restore #8 Health. Draw 3 cards.
        /// </summary>
        public static void LayonHands(Board b)
        {
            Character c = b.GetRandomCharacter();
            c.ModifyHealth(8);
            b.Player.Draw(3);
        }
        
        /// <summary>
        /// Give a minion +2 Health. Draw a card.
        /// </summary>
        public static void PowerWordShield(Board b)
        {
            Minion m = b.GetRandomMinion();
            if (m == null) return;
            m.Effects.HealthModifier += 2;
            b.Player.Draw(1);
        }
        
        /// <summary>
        /// Secret: As soon as your hero is attacked, gain 8 Armor.
        /// </summary>
        public static void IceBarrier(Board b)
        {
            b.Player.AddSecretValue(Simulation.MageSecretManaValue);
        }
        
        /// <summary>
        /// Deal 2-3 damage to all enemy minions. Overload: (2)
        /// </summary>
        public static void LightningStorm(Board b)
        {
            foreach (Minion m in b.OpponentMinions)
            {
                int damage = 2 + Simulation.Rng.Next(2);
                if (damage != 2 && damage != 3) throw new Exception("Invalid result from RNG source");
                m.ModifyHealth(-damage);
            }
        }
        
        /// <summary>
        /// Draw 4 cards.
        /// </summary>
        public static void Sprint(Board b)
        {
            b.Player.Draw(4);
        }
        
        /// <summary>
        /// Deal 1 damage to ALL minions.
        /// </summary>
        public static void Whirlwind(Board b)
        {
            foreach (Minion m in b.PlayerMinions)
            {
                m.ModifyHealth(-1);
            }
            foreach (Minion m in b.OpponentMinions)
            {
                m.ModifyHealth(-1);
            }
        }
        
        /// <summary>
        /// Deal 3 damage to a character and Freeze it.
        /// </summary>
        public static void Frostbolt(Board b)
        {
            Character c = b.GetRandomCharacter();
            c.ModifyHealth(-3);
            c.Frozen = true;
        }
        
        /// <summary>
        /// Deal 1 damage to all enemy minions. Draw a card.
        /// </summary>
        public static void FanofKnives(Board b)
        {
            foreach (Minion m in b.OpponentMinions)
            {
                m.ModifyHealth(-1);
            }
            b.Player.Draw(1);
        }
        
        /// <summary>
        /// Draw cards until you have as many in hand as your opponent.
        /// </summary>
        public static void DivineFavor(Board b)
        {
            if (b.Opponent.HandSize > b.Player.HandSize)
            {
                b.Player.Draw(b.Opponent.HandSize - b.Player.HandSize);
            }
        }
        
        /// <summary>
        /// Deal 2 damage to the enemy hero. Combo: Return this to your hand next turn.
        /// </summary>
        public static void Headcrack(Board b)
        {
            b.Opponent.ModifyHealth(-2);
            // shortcut: ignoring return to hand because it's low value and more likely to incorrectly
            // block a draw this turn than it is to incorrectly block a draw next turn.
        }
        
        /// <summary>
        /// Deal 3 damage to the enemy hero.
        /// </summary>
        public static void SinisterStrike(Board b)
        {
            b.Opponent.ModifyHealth(-3);
        }
        
        /// <summary>
        /// Give a minion Divine Shield.
        /// </summary>
        public static void HandofProtection(Board b)
        {
            Minion m = b.GetRandomMinion();
            if (m == null) return;
            m.Effects.OtherEffectValue += Simulation.DivineShieldManaValue;
        }
        
        /// <summary>
        /// Give your characters +2 Attack this turn.
        /// </summary>
        public static void SavageRoar(Board b)
        {
            b.Player.HeroAttackBuff += 2;
            foreach (Minion m in b.PlayerMinions)
            {
                m.Effects.AttackModifier += 2;
            }
        }
        
        /// <summary>
        /// Change the Health of ALL minions to 1.
        /// </summary>
        public static void Equality(Board b)
        {
            foreach (Minion m in b.PlayerMinions)
            {
                m.Effects.HealthModifier = 1 - m.TotalHealth;
                m.Damage = 0;
            }
            foreach (Minion m in b.OpponentMinions)
            {
                m.Effects.HealthModifier = 1 - m.TotalHealth;
                m.Damage = 0;
            }
        }
        
        /// <summary>
        /// Transform a minion into a 0/1 Frog with Taunt.
        /// </summary>
        public static void Hex(Board b)
        {
            Minion m = b.GetRandomMinion();
            if (m == null) return;
            List<Minion> side;
            if (b.OpponentMinions.Contains(m)) side = b.OpponentMinions;
            else if (b.PlayerMinions.Contains(m)) side = b.PlayerMinions;
            else throw new Exception("Found minion that's not on the board.");
            side.Insert(side.IndexOf(m), new Minion("Hex Frog", 0, 1, 0, Simulation.TauntManaValue));
            side.Remove(m);
        }
        
        /// <summary>
        /// Silence a minion, then deal 1 damage to it.
        /// </summary>
        public static void EarthShock(Board b)
        {
            Minion m = b.GetRandomMinion();
            if (m == null) return;
            m.Silence();
            m.ModifyHealth(-1);
        }
        
        /// <summary>
        /// Restore #8 Health.
        /// </summary>
        public static void HealingTouch(Board b)
        {
            Character c = b.GetRandomCharacter();
            c.ModifyHealth(8);
        }
        
        /// <summary>
        /// Destroy a damaged enemy minion.
        /// </summary>
        public static void Execute(Board b)
        {
            List<Minion> damagedMinions = new List<Minion>();
            foreach (Minion m in b.OpponentMinions)
            {
                if (m.Damage > 0)
                {
                    damagedMinions.Add(m);
                }
            }
            if (damagedMinions.Count > 0)
            {
                Minion m = damagedMinions[Simulation.Rng.Next(damagedMinions.Count)];
                b.OpponentMinions.Remove(m);
            }
        }
        
        /// <summary>
        /// Deal 4 damage. If your hero has 12 or less Health, deal 6 damage instead.
        /// </summary>
        public static void MortalStrike(Board b)
        {
            Character c = b.GetRandomCharacter();
            c.ModifyHealth(b.Player.ActualHealth <= 12 ? -6 : -4);
        }
        
        /// <summary>
        /// Secret: When your opponent plays a minion, deal 4 damage to it.
        /// </summary>
        public static void Snipe(Board b)
        {
            b.Player.AddSecretValue(Simulation.HunterSecretManaValue);
        }
        
        /// <summary>
        /// Draw a card. That card costs (3) less.
        /// </summary>
        public static void FarSight(Board b)
        {
            b.Player.Draw(1);
            b.Player.OtherEffectManaValue += Simulation.CardDiscountManaModifier * 3.0;
        }
        
        /// <summary>
        /// Deal 5 damage. Draw a card.
        /// </summary>
        public static void Starfire(Board b)
        {
            b.GetRandomCharacter().ModifyHealth(-5);
            b.Player.Draw(1);
        }
        
        /// <summary>
        /// Give your Totems +2 Health.
        /// </summary>
        public static void TotemicMight(Board b)
        {
            // shortcut: assuming that we don't have any totems.
        }
        
        /// <summary>
        /// Choose One - Deal 3 damage to a minion; or 1 damage and draw a card.
        /// </summary>
        public static void Wrath(Board b)
        {
            int coinFlip = Simulation.Rng.Next(2);
            if (coinFlip == 0)
            {
                b.GetRandomMinion().ModifyHealth(-3);
            }
            else
            {
                b.GetRandomMinion().ModifyHealth(-1);
                b.Player.Draw(1);
            }
        }
        
        /// <summary>
        /// Deal 2 damage to all enemies. Restore #2 Health to all friendly characters.
        /// </summary>
        public static void HolyNova(Board b)
        {
            b.Opponent.ModifyHealth(-2);
            b.Player.ModifyHealth(2);
            foreach (Minion m in b.OpponentMinions)
            {
                m.ModifyHealth(-2);
            }
            foreach (Minion m in b.PlayerMinions)
            {
                m.ModifyHealth(2);
            }
        }
        
        /// <summary>
        /// Give a friendly minion +4/+4 until end of turn. Then, it dies. Horribly.
        /// </summary>
        public static void PowerOverwhelming(Board b)
        {
            Minion m = b.GetRandomFriendlyMinion();
            if (m == null) return;
            m.Effects.AttackModifier += 4;
            m.Effects.HealthModifier += 4;
            m.Effects.DestroyAtEndOfTurn = true;
        }
        
        /// <summary>
        /// Change a minion's Attack to 1.
        /// </summary>
        public static void Humility(Board b)
        {
            Minion m = b.GetRandomMinion();
            if (m == null) return;
            m.Effects.AttackModifier = 1 - m.Attack;
        }
        
        /// <summary>
        /// Destroy all minions.
        /// </summary>
        public static void TwistingNether(Board b)
        {
            b.PlayerMinions.Clear();
            b.OpponentMinions.Clear();
        }
        
        /// <summary>
        /// Put 2 random Demons from your deck into your hand.
        /// </summary>
        public static void SenseDemons(Board b)
        {
            b.Player.AddCards(2);   // shortcut: ignoring the possibility of the player actually having demons in their deck.
        }
        
        /// <summary>
        /// Deal 5 damage. Overload: (2)
        /// </summary>
        public static void LavaBurst(Board b)
        {
            b.GetRandomCharacter().ModifyHealth(-5);
        }
        
        /// <summary>
        /// Deal 2 damage.
        /// </summary>
        public static void ArcaneShot(Board b)
        {
            b.GetRandomCharacter().ModifyHealth(-2);
        }
        
        /// <summary>
        /// All minions lose Stealth. Destroy all enemy Secrets. Draw a card.
        /// </summary>
        public static void Flare(Board b)
        {
            foreach (Minion m in b.PlayerMinions)
            {
                m.Effects.Stealth = false;
            }
            foreach (Minion m in b.OpponentMinions)
            {
                m.Effects.Stealth = false;
            }
            b.Player.Draw(1);
        }
        
        /// <summary>
        /// Give a Beast +2 Attack and Immune this turn.
        /// </summary>
        public static void BestialWrath(Board b)
        {
            List<Minion> beasts = new List<Minion>();
            foreach (Minion m in b.PlayerMinions)
            {
                if (m.Race == "beast") beasts.Add(m);
            }
            foreach (Minion m in b.OpponentMinions)
            {
                if (m.Race == "beast") beasts.Add(m);
            }
            if (beasts.Count == 0) return;
            Minion beast = beasts[Simulation.Rng.Next(beasts.Count)];
            if (beast.CreatedByYogg || Simulation.Rng.Next(9) < 3)
            {
                beast.Effects.AttackModifier += 2;
                // shortcut: ignoring immunity, the only time it will come into effect is when we summon huffer and it survives.
            }
        }
        
        /// <summary>
        /// Deal 2 damage. Combo: Deal 4 damage instead.
        /// </summary>
        public static void Eviscerate(Board b)
        {
            b.GetRandomCharacter().ModifyHealth(-4);
        }
        
        /// <summary>
        /// Deal 4 damage to a minion.
        /// </summary>
        public static void ShadowBolt(Board b)
        {
            Minion m = b.GetRandomMinion();
            if (m == null) return;
            m.ModifyHealth(-4);
        }
        
        /// <summary>
        /// Deal 2 damage. Restore #2 Health to your hero.
        /// </summary>
        public static void DrainLife(Board b)
        {
            b.GetRandomCharacter().ModifyHealth(-2);
            b.Player.ModifyHealth(2);
        }
        
        /// <summary>
        /// Deal 2 damage to two random enemy minions.
        /// </summary>
        public static void Cleave(Board b)
        {
            if (b.OpponentMinions.Count >= 2)
            {
                Minion firstMinion = b.GetRandomMinion();
                firstMinion.ModifyHealth(-2);
                Minion secondMinion = b.GetRandomMinion();
                while (secondMinion == firstMinion) secondMinion = b.GetRandomMinion();
                secondMinion.ModifyHealth(-2);
            }
        }
        
        /// <summary>
        /// Give a minion +4/+4. (+4 Attack/+4 Health)
        /// </summary>
        public static void BlessingofKings(Board b)
        {
            Minion m = b.GetRandomMinion();
            if (m == null) return;
            m.Effects.AttackModifier += 4;
            m.Effects.HealthModifier += 4;
        }
        
        /// <summary>
        /// Deal 3 damage to ALL characters.
        /// </summary>
        public static void Hellfire(Board b)
        {
            b.Player.ModifyHealth(-3);
            b.Opponent.ModifyHealth(-3);
            foreach (Minion m in b.PlayerMinions)
            {
                m.ModifyHealth(-3);
            }
            foreach (Minion m in b.OpponentMinions)
            {
                m.ModifyHealth(-3);
            }
        }
        
        /// <summary>
        /// Deal 1 damage to an enemy character and Freeze it.
        /// </summary>
        public static void FrostShock(Board b)
        {
            Character c = b.GetRandomOpponent();
            c.ModifyHealth(-1);
            c.Frozen = true;
        }
        
        /// <summary>
        /// Deal 4 damage. Discard a random card.
        /// </summary>
        public static void Soulfire(Board b)
        {
            b.GetRandomCharacter().ModifyHealth(-4);
            b.Player.Draw(-1);
        }
        
        /// <summary>
        /// Choose an enemy minion. At the start of your turn, destroy it.
        /// </summary>
        public static void Corruption(Board b)
        {
            Minion m = b.GetRandomOpponentMinion();
            if (m == null) return;
            m.Effects.DestroyAtStartOfNextTurn = true;
        }
        
        /// <summary>
        /// Give your minions Stealth until your next turn.
        /// </summary>
        public static void Conceal(Board b)
        {
            foreach (Minion m in b.PlayerMinions)
            {
                m.Effects.Stealth = true;
            }
        }
        
        /// <summary>
        /// Deal 4 damage to all enemy minions.
        /// </summary>
        public static void Flamestrike(Board b)
        {
            foreach (Minion m in b.OpponentMinions)
            {
                m.ModifyHealth(-4);
            }
        }
        
        /// <summary>
        /// Give your hero +4 Attack this turn.
        /// </summary>
        public static void HeroicStrike(Board b)
        {
            b.Player.HeroAttackBuff += 4;
        }
        
        /// <summary>
        /// Gain 5 Armor. Draw a card.
        /// </summary>
        public static void ShieldBlock(Board b)
        {
            b.Player.Armour += 5;
            b.Player.Draw(1);
        }
        
        /// <summary>
        /// Your minions can't be reduced below 1 Health this turn. Draw a card.
        /// </summary>
        public static void CommandingShout(Board b)
        {
            foreach (Minion m in b.PlayerMinions)
            {
                m.Effects.CommandingShout = true;
            }
        }
        
        /// <summary>
        /// Look at the top three cards of your deck. Draw one and discard the others.
        /// </summary>
        public static void Tracking(Board b)
        {
            b.Player.Draw(1);   // shortcut: not simulating removing cards from the deck (other than drawing)
        }
        
        /// <summary>
        /// Give your hero +2 Attack this turn and 2 Armor.
        /// </summary>
        public static void Claw(Board b)
        {
            b.Player.HeroAttackBuff += 2;
            b.Player.Armour += 2;
        }
        
        /// <summary>
        /// Destroy your weapon and deal its damage to all enemies.
        /// </summary>
        public static void BladeFlurry(Board b)
        {
            // shortcut: not dealing with weapons yet.
        }
        
        /// <summary>
        /// Deal 2 damage to a minion. If it survives, draw a card.
        /// </summary>
        public static void Slam(Board b)
        {
            Minion m = b.GetRandomMinion();
            if (m == null) return;
            m.ModifyHealth(-2);
            if (m.ActualHealth > 0) b.Player.Draw(1);
        }
        
        /// <summary>
        /// Summon two 0/2 minions with Taunt.
        /// </summary>
        public static void MirrorImage(Board b)
        {
            for (int i = 0; i < 2 && b.PlayerMinions.Count < 7; i++)
            {
                b.PlayerMinions.Add(new Minion("Mirror Image", 0, 2, 1, Simulation.TauntManaValue));
            }
        }
        
        /// <summary>
        /// Deal 10 damage.
        /// </summary>
        public static void Pyroblast(Board b)
        {
            b.GetRandomCharacter().ModifyHealth(-10);
        }
        
        /// <summary>
        /// Secret: When a character attacks your hero, instead he attacks another random character.
        /// </summary>
        public static void Misdirection(Board b)
        {
            b.Player.AddSecretValue(Simulation.HunterSecretManaValue);
        }
        
        /// <summary>
        /// Deal 1 damage to a minion. If that kills it, draw a card.
        /// </summary>
        public static void MortalCoil(Board b)
        {
            Minion m = b.GetRandomMinion();
            if (m == null) return;
            m.ModifyHealth(-1);
            if (m.ActualHealth <= 0) b.Player.Draw(1);
        }
        
        /// <summary>
        /// Destroy a random enemy minion.
        /// </summary>
        public static void DeadlyShot(Board b)
        {
            Minion m = b.GetRandomOpponentMinion();
            if (m == null) return;
            b.OpponentMinions.Remove(m);
        }
        
        /// <summary>
        /// Put a copy of a random card in your opponent's hand into your hand.
        /// </summary>
        public static void MindVision(Board b)
        {
            b.Player.AddCards(1);
        }
        
        /// <summary>
        /// Destroy a minion. Restore #3 Health to your hero.
        /// </summary>
        public static void SiphonSoul(Board b)
        {
            Minion m = b.GetRandomMinion();
            if (b.PlayerMinions.Contains(m))
            {
                b.PlayerMinions.Remove(m);
            }
            else if (b.OpponentMinions.Contains(m))
            {
                b.OpponentMinions.Remove(m);
            }
            else
            {
                throw new Exception("Found a minion that's not on the board.");
            }
            b.Player.ModifyHealth(3);
        }
        
        /// <summary>
        /// Give a damaged minion +3/+3.
        /// </summary>
        public static void Rampage(Board b)
        {
            List<Minion> damagedMinions = new List<Minion>();
            foreach (Minion m in b.OpponentMinions)
            {
                if (m.Damage > 0)
                {
                    damagedMinions.Add(m);
                }
            }
            foreach (Minion m in b.PlayerMinions)
            {
                if (m.Damage > 0)
                {
                    damagedMinions.Add(m);
                }
            }
            if (damagedMinions.Count > 0)
            {
                Minion m = damagedMinions[Simulation.Rng.Next(damagedMinions.Count)];
                m.Effects.AttackModifier += 3;
                m.Effects.HealthModifier += 3;
            }
        }
        
        /// <summary>
        /// Gain an empty Mana Crystal.
        /// </summary>
        public static void WildGrowth(Board b)
        {
            // shorcut: probably at 10 mana... you could argue that this should be AddCards() instead of Draw()
            b.Player.Draw(1);
        }
        
        /// <summary>
        /// Deal 2 damage to a minion. If it’s a friendly Demon, give it +2/+2 instead.
        /// </summary>
        public static void Demonfire(Board b)
        {
            Minion m = b.GetRandomMinion();
            if (m == null) return;
            if (m.Race == "demon" && m.CreatedByYogg)
            {
                // shortcut: assuming that the player didn't have demons on board before Yogging.
                m.Effects.AttackModifier += 2;
                m.Effects.HealthModifier += 2;
            }
            else
            {
                m.ModifyHealth(-2);
            }
        }
        
        /// <summary>
        /// The next spell you cast this turn costs (3) less.
        /// </summary>
        public static void Preparation(Board b)
        {
            // no guarantee that the player will have a useful spell that they can play after this.
            b.Player.OtherEffectManaValue += 3 * Simulation.CardDiscountManaModifier;
        }
        
        /// <summary>
        /// Give your minions +3 Attack this turn.
        /// </summary>
        public static void Bloodlust(Board b)
        {
            // shortcut: assuming that minions have already attacked and negligible expected value from summoning 
            // a minion with charge
        }
        
        /// <summary>
        /// Deal 8 damage randomly split among enemy characters.
        /// </summary>
        public static void AvengingWrath(Board b)
        {
            for (int i = 0; i < 8; i++)
            {
                b.GetRandomCharacter().ModifyHealth(-1);
            }
        }
        
        /// <summary>
        /// Silence a minion.
        /// </summary>
        public static void Silence(Board b)
        {
            Minion m = b.GetRandomMinion();
            if (m == null) return;
            m.Silence();
        }
        
        /// <summary>
        /// For each enemy minion, summon a 1/1 Hound with Charge.
        /// </summary>
        public static void UnleashtheHounds(Board b)
        {
            for (int i = 0; i < b.OpponentMinions.Count && b.PlayerMinions.Count < 7; i++)
            {
                b.PlayerMinions.Add(new Minion("Hound", 1, 1, 0, 1 * Simulation.ChargeManaModifier));
            }
        }
        
        /// <summary>
        /// Double a minion's Health.
        /// </summary>
        public static void DivineSpirit(Board b)
        {
            Minion m = b.GetRandomMinion();
            if (m == null) return;
            m.Effects.HealthModifier = 2 * m.ActualHealth;
        }
        
        /// <summary>
        /// Restore #4 Health to ALL minions.
        /// </summary>
        public static void CircleofHealing(Board b)
        {
            foreach (Minion m in b.PlayerMinions)
            {
                m.ModifyHealth(4);
            }
            foreach (Minion m in b.OpponentMinions)
            {
                m.ModifyHealth(4);
            }
        }
        
        /// <summary>
        /// Destroy a minion with an Attack of 5 or more.
        /// </summary>
        public static void ShadowWordDeath(Board b)
        {
            List<Minion> eligibleMinions = new List<Minion>();
            foreach (Minion m in b.PlayerMinions)
            {
                if (m.ActualAttack >= 5)
                {
                    eligibleMinions.Add(m);
                }
            }
            foreach (Minion m in b.OpponentMinions)
            {
                if (m.ActualAttack >= 5)
                {
                    eligibleMinions.Add(m);
                }
            }
            if (eligibleMinions.Count == 0) return;
            Minion minionToKill = eligibleMinions[Simulation.Rng.Next(eligibleMinions.Count)];
            b.PlayerMinions.Remove(minionToKill);
            b.OpponentMinions.Remove(minionToKill);
        }
        
        /// <summary>
        /// Deal 5 damage. Restore 5 Health to your hero.
        /// </summary>
        public static void HolyFire(Board b)
        {
            b.GetRandomCharacter().ModifyHealth(-5);
            if (b.Player.ActualHealth <= 0)
            {
                // player died
                return;
            }
            b.Player.ModifyHealth(5);
        }
        
        /// <summary>
        /// Silence all enemy minions. Draw a card.
        /// </summary>
        public static void MassDispel(Board b)
        {
            foreach (Minion m in b.OpponentMinions)
            {
                m.Silence();
            }
            b.Player.Draw(1);
        }
        
        /// <summary>
        /// Destroy a minion with 3 or less Attack.
        /// </summary>
        public static void ShadowWordPain(Board b)
        {
            List<Minion> eligibleMinions = new List<Minion>();
            foreach (Minion m in b.PlayerMinions)
            {
                if (m.ActualAttack <= 3)
                {
                    eligibleMinions.Add(m);
                }
            }
            foreach (Minion m in b.OpponentMinions)
            {
                if (m.ActualAttack <= 3)
                {
                    eligibleMinions.Add(m);
                }
            }
            if (eligibleMinions.Count == 0) return;
            Minion minionToKill = eligibleMinions[Simulation.Rng.Next(eligibleMinions.Count)];
            b.PlayerMinions.Remove(minionToKill);
            b.OpponentMinions.Remove(minionToKill);
        }
        
        /// <summary>
        /// Your Hero Power becomes 'Deal 2 damage'. If already in Shadowform: 3 damage.
        /// </summary>
        public static void Shadowform(Board b)
        {
            // ???
            b.Player.OtherEffectManaValue += 1;
        }
        
        /// <summary>
        /// Choose a minion. Whenever it attacks, draw a card.
        /// </summary>
        public static void BlessingofWisdom(Board b)
        {
            Minion m = b.GetRandomMinion();
            if (m == null) return;
            if (b.PlayerMinions.Contains(m))
            {
                m.Effects.OtherEffectValue += 1;
            }
            else
            {
                // subtracting value from the opponent adds it to the player!
                m.Effects.OtherEffectValue -= 1;
            }
        }
        
        /// <summary>
        /// Double a minion's Attack.
        /// </summary>
        public static void BlessedChampion(Board b)
        {
            Minion m = b.GetRandomMinion();
            if (m == null) return;
            m.Effects.AttackModifier = m.ActualAttack * 2 - m.Attack;
        }
        
        /// <summary>
        /// Draw 2 cards. Costs (1) less for each minion that died this turn.
        /// </summary>
        public static void SolemnVigil(Board b)
        {
            b.Player.Draw(2);
        }
        
        /// <summary>
        /// Deal 4 damage. Costs (1) less for each minion that died this turn.
        /// </summary>
        public static void DragonsBreath(Board b)
        {
            b.GetRandomCharacter().ModifyHealth(-4);
        }
        
        /// <summary>
        /// Deal 2 damage to all non-Demon minions.
        /// </summary>
        public static void Demonwrath(Board b)
        {
            foreach (Minion m in b.OpponentMinions)
            {
                if (m.Race != "demon" || (!m.CreatedByYogg && Simulation.Rng.Next(9) != 0))
                {
                    // This should be true most of the time, and we're removing some minions that are incorrectly identified by demons
                    m.ModifyHealth(-2);
                }
            }
        }
        
        /// <summary>
        /// Choose a minion. Shuffle 3 copies of it into your deck.
        /// </summary>
        public static void GangUp(Board b)
        {
            // shortcut: not really sure how to account for the value added here; will probably just ignore it.
            b.Player.CardsAddedToDeck += 3;
        }
        
        /// <summary>
        /// Deal 2 damage. Unlock your Overloaded Mana Crystals.
        /// </summary>
        public static void LavaShock(Board b)
        {
            // no value in unlocking overloaded crystals unless Ben Brode gets his way.
            b.GetRandomCharacter().ModifyHealth(-2);
        }
        
        /// <summary>
        /// Deal 3 damage. If your hand is empty, draw a card.
        /// </summary>
        public static void QuickShot(Board b)
        {
            b.GetRandomCharacter().ModifyHealth(-3);
            if (b.Player.HandSize == 0)
            {
                b.Player.Draw(1);
            }
        }
        
        /// <summary>
        /// Deal 1 damage to all minions. If you have 12 or less Health, deal 3 damage instead.
        /// </summary>
        public static void Revenge(Board b)
        {
            foreach (Minion m in b.PlayerMinions)
            {
                m.ModifyHealth(b.Player.ActualHealth > 12 ? -1 : -3);
            }
            foreach (Minion m in b.OpponentMinions)
            {
                m.ModifyHealth(b.Player.ActualHealth > 12 ? -1 : -3);
            }
        }
        
        /// <summary>
        /// Summon a random friendly minion that died this game.
        /// </summary>
        public static void Resurrect(Board b)
        {
            // shortcut: just putting a random minion into the battlefield
            if (b.PlayerMinions.Count < 7)
            {
                b.PlayerMinions.Add(CardFactory.GetRandomMinion());
            }
        }
        
        /// <summary>
        /// Give your minions +2/+2. Costs (1) less for each Murloc you control.
        /// </summary>
        public static void EveryfinisAwesome(Board b)
        {
            foreach (Minion m in b.PlayerMinions)
            {
                m.Effects.AttackModifier += 2;
                m.Effects.HealthModifier += 2;
            }
        }
        
        /// <summary>
        /// Give a minion +1/+1 and "Deathrattle: Add an Explorer's Hat to your hand."
        /// </summary>
        public static void ExplorersHat(Board b)
        {
            Minion m = b.GetRandomMinion();
            if (m == null) return;
            m.Effects.AttackModifier++;
            m.Effects.HealthModifier++;
            m.Effects.DeathrattleValue += 1.0;  // man, I dunno
        }
        
        /// <summary>
        /// Choose one - Discover a minion; or Discover a spell.
        /// </summary>
        public static void RavenIdol(Board b)
        {
            b.Player.AddCards(1);
        }
        
        /// <summary>
        /// Secret: When an opposing Hero Power is used, deal 5 damage to a random enemy.
        /// </summary>
        public static void DartTrap(Board b)
        {
            b.Player.AddSecretValue(Simulation.HunterSecretManaValue);
        }
        
        /// <summary>
        /// Deal 3 damage. Shuffle a 'Roaring Torch' into your deck that deals 6 damage.
        /// </summary>
        public static void ForgottenTorch(Board b)
        {
            b.GetRandomCharacter().ModifyHealth(-3);
            b.Player.CardsAddedToDeck += 1;
        }
        
        /// <summary>
        /// Summon 7 Murlocs that died this game.
        /// </summary>
        public static void AnyfinCanHappen(Board b)
        {
            // yeah, I'm just gonna ignore this.
        }
        
        /// <summary>
        /// Secret: When your opponent has at least 3 minions and plays another, destroy it.
        /// </summary>
        public static void SacredTrial(Board b)
        {
            b.Player.AddSecretValue(Simulation.PaladinSecretManaValue);
        }
        
        /// <summary>
        /// Deal 3 damage to all minions. Shuffle this card into your opponent's deck.
        /// </summary>
        public static void ExcavatedEvil(Board b)
        {

        }
        
        /// <summary>
        /// Choose an enemy minion. Shuffle it into your deck.
        /// </summary>
        public static void Entomb(Board b)
        {
            
        }
        
        /// <summary>
        /// Give your opponent a 'Cursed!' card. While they hold it, they take 2 damage on their turn.
        /// </summary>
        public static void CurseofRafaam(Board b)
        {

        }
        
        /// <summary>
        /// Give a minion +1/+1 for each of your Totems.
        /// </summary>
        public static void PrimalFusion(Board b)
        {

        }
        
        /// <summary>
        /// Transform your minions into random minions that cost (1) more.
        /// </summary>
        public static void Evolve(Board b)
        {

        }
        
        /// <summary>
        /// Give your minions "Deathrattle: Add a random Beast to your hand."
        /// </summary>
        public static void Infest(Board b)
        {

        }
        
        /// <summary>
        /// Choose One - Give your hero +4 Attack this turn; or Gain 8 Armor.
        /// </summary>
        public static void FeralRage(Board b)
        {

        }
        
        /// <summary>
        /// Give a minion +2/+2. If it's a Beast, draw a card.
        /// </summary>
        public static void MarkofYShaarj(Board b)
        {

        }
        
        /// <summary>
        /// Deal 1 damage. Summon a 1/1 Mastiff.
        /// </summary>
        public static void OntheHunt(Board b)
        {

        }
        
        /// <summary>
        /// Discover a Deathrattle card.
        /// </summary>
        public static void JourneyBelow(Board b)
        {

        }
        
        /// <summary>
        /// Draw a card. Add 2 extra copies of it to your hand.
        /// </summary>
        public static void ThistleTea(Board b)
        {

        }
        
        /// <summary>
        /// Destroy a Frozen minion.
        /// </summary>
        public static void Shatter(Board b)
        {

        }
        
        /// <summary>
        /// Spend all your Mana. Deal that much damage to a minion.
        /// </summary>
        public static void ForbiddenFlame(Board b)
        {

        }
        
        /// <summary>
        /// Add 3 random Mage spells to your hand.
        /// </summary>
        public static void CabalistsTome(Board b)
        {

        }
        
        /// <summary>
        /// Give a minion +2/+6.
        /// </summary>
        public static void PowerWordTentacles(Board b)
        {

        }
        
        /// <summary>
        /// Destroy all minions with 2 or less Attack.
        /// </summary>
        public static void ShadowWordHorror(Board b)
        {

        }
        
        /// <summary>
        /// Spend all your Mana. Summon a random minion that costs that much.
        /// </summary>
        public static void ForbiddenShaping(Board b)
        {

        }
        
        /// <summary>
        /// This turn, your healing effects deal damage instead.
        /// </summary>
        public static void EmbracetheShadow(Board b)
        {

        }
        
        /// <summary>
        /// Spend all your Mana. Summon that many 1/1 Tentacles.
        /// </summary>
        public static void ForbiddenRitual(Board b)
        {

        }
        
        /// <summary>
        /// Deal 9 damage randomly split among ALL characters.
        /// </summary>
        public static void SpreadingMadness(Board b)
        {

        }
        
        /// <summary>
        /// Replace your Hero Power and Warlock cards with another class's. The cards cost (1) less.
        /// </summary>
        public static void RenounceDarkness(Board b)
        {

        }
        
        /// <summary>
        /// Deal 5 damage to an undamaged character.
        /// </summary>
        public static void ShadowStrike(Board b)
        {

        }
        
        /// <summary>
        /// Choose One - Summon seven 1/1 Wisps; or Give your minions +2/+2.
        /// </summary>
        public static void WispsoftheOldGods(Board b)
        {

        }
        
        /// <summary>
        /// Spend all your Mana. Restore twice that much Health.
        /// </summary>
        public static void ForbiddenHealing(Board b)
        {

        }
        
        /// <summary>
        /// Deal 4 damage to a minion. Overload: (1)
        /// </summary>
        public static void Stormcrack(Board b)
        {

        }
        
        /// <summary>
        /// Summon all three Animal Companions.
        /// </summary>
        public static void CalloftheWild(Board b)
        {

        }
        
        /// <summary>
        /// Give a minion +1/+2.
        /// </summary>
        public static void DivineStrength(Board b)
        {

        }
        
        /// <summary>
        /// Destroy all minions. Draw a card for each.
        /// </summary>
        public static void DOOM(Board b)
        {

        }
        
        /// <summary>
        /// Summon five 1/1 Silver Hand Recruits.
        /// </summary>
        public static void StandAgainstDarkness(Board b)
        {

        }
        
        /// <summary>
        /// Add a copy of each damaged friendly minion to your hand.
        /// </summary>
        public static void BloodWarriors(Board b)
        {

        }
        
        /// <summary>
        /// Discover a minion. Give it +1/+1.
        /// </summary>
        public static void ALightintheDarkness(Board b)
        {

        }
        
        /// <summary>
        /// Deal 1 damage to a minion. If it survives, summon a 2/2 Slime.
        /// </summary>
        public static void BloodToIchor(Board b)
        {

        }
        
    }
}
