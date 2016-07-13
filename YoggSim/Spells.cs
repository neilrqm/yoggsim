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
            m.Effects.OtherEffectValue += Simulation.WindfuryManaModifier;    // doesn't handle situation where minion already has windfury
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
            if (b.PlayerMinions.Contains(m)) b.PlayerMinions[b.PlayerMinions.IndexOf(m)] = CardFactory.GetOneOne();
            else if (b.OpponentMinions.Contains(m)) b.OpponentMinions[b.OpponentMinions.IndexOf(m)] = CardFactory.GetOneOne();
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
            b.Player.SecretValue += Simulation.MageSecretManaModifier;
        }
        
        /// <summary>
        /// Secret: When one of your minions dies, return it to life with 1 Health.
        /// </summary>
        public static void Redemption(Board b)
        {
            b.Player.SecretValue += Simulation.PaladinSecretManaModifier;
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
            m.Effects.OtherEffectValue += Simulation.TauntManaModifier;
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
                m.Effects.OtherEffectValue += Simulation.TauntManaModifier;
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
            b.Player.SecretValue += Simulation.MageSecretManaModifier;
        }
        
        /// <summary>
        /// Secret: When your opponent plays a minion, summon a copy of it.
        /// </summary>
        public static void MirrorEntity(Board b)
        {
            b.Player.SecretValue += Simulation.MageSecretManaModifier;
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
            m.Effects.OtherEffectValue += Simulation.TauntManaModifier;
        }
        
        /// <summary>
        /// Gain control of an enemy minion with 3 or less Attack until end of turn.
        /// </summary>
        public static void ShadowMadness(Board b)
        {

        }
        
        /// <summary>
        /// Secret: When your opponent plays a minion, reduce its Health to 1.
        /// </summary>
        public static void Repentance(Board b)
        {

        }
        
        /// <summary>
        /// Destroy a minion. Your opponent draws 2 cards.
        /// </summary>
        public static void Naturalize(Board b)
        {

        }
        
        /// <summary>
        /// Summon two 2/3 Spirit Wolves with Taunt. Overload: (2)
        /// </summary>
        public static void FeralSpirit(Board b)
        {

        }
        
        /// <summary>
        /// Give a friendly character +3 Attack this turn.
        /// </summary>
        public static void RockbiterWeapon(Board b)
        {

        }
        
        /// <summary>
        /// Deal 3 damage. Draw a card.
        /// </summary>
        public static void HammerofWrath(Board b)
        {

        }
        
        /// <summary>
        /// Gain 2 Mana Crystals this turn only.
        /// </summary>
        public static void Innervate(Board b)
        {

        }
        
        /// <summary>
        /// Give a minion +2 Attack. Combo: +4 Attack instead.
        /// </summary>
        public static void ColdBlood(Board b)
        {

        }
        
        /// <summary>
        /// Deal 2 damage.
        /// </summary>
        public static void HolySmite(Board b)
        {

        }
        
        /// <summary>
        /// An enemy minion deals its damage to the minions next to it.
        /// </summary>
        public static void Betrayal(Board b)
        {

        }
        
        /// <summary>
        /// Secret: When a minion attacks your hero, destroy it.
        /// </summary>
        public static void Vaporize(Board b)
        {

        }
        
        /// <summary>
        /// Restore #6 Health.
        /// </summary>
        public static void HolyLight(Board b)
        {

        }
        
        /// <summary>
        /// Deal 3 damage to two random enemy minions.
        /// </summary>
        public static void MultiShot(Board b)
        {

        }
        
        /// <summary>
        /// Deal 3 damage. If you have a Beast, deal 5 damage instead.
        /// </summary>
        public static void KillCommand(Board b)
        {

        }
        
        /// <summary>
        /// Deal 2 damage to 2 random enemy minions. Overload: (2)
        /// </summary>
        public static void ForkedLightning(Board b)
        {

        }
        
        /// <summary>
        /// Deal 6 damage.
        /// </summary>
        public static void Fireball(Board b)
        {

        }
        
        /// <summary>
        /// Give a friendly minion +2 Attack and Charge.
        /// </summary>
        public static void Charge(Board b)
        {

        }
        
        /// <summary>
        /// Destroy an enemy minion.
        /// </summary>
        public static void Assassinate(Board b)
        {

        }
        
        /// <summary>
        /// Return a friendly minion to your hand. It costs (2) less.
        /// </summary>
        public static void Shadowstep(Board b)
        {

        }
        
        /// <summary>
        /// Secret: When an enemy casts a spell on a minion, summon a 1/3 as the new target.
        /// </summary>
        public static void Spellbender(Board b)
        {

        }
        
        /// <summary>
        /// Change a minion's Attack to be equal to its Health.
        /// </summary>
        public static void InnerFire(Board b)
        {

        }
        
        /// <summary>
        /// Give your minions "Deathrattle: Summon a 2/2 Treant."
        /// </summary>
        public static void SouloftheForest(Board b)
        {

        }
        
        /// <summary>
        /// Deal 5 damage to a minion and 2 damage to adjacent ones.
        /// </summary>
        public static void ExplosiveShot(Board b)
        {

        }
        
        /// <summary>
        /// Draw a card for each damaged friendly character.
        /// </summary>
        public static void BattleRage(Board b)
        {

        }
        
        /// <summary>
        /// Choose a minion. When that minion is destroyed, return it to the battlefield.
        /// </summary>
        public static void AncestralSpirit(Board b)
        {

        }
        
        /// <summary>
        /// Freeze a minion and the minions next to it, and deal 1 damage to them.
        /// </summary>
        public static void ConeofCold(Board b)
        {

        }
        
        /// <summary>
        /// Draw a card and deal damage equal to its cost.
        /// </summary>
        public static void HolyWrath(Board b)
        {

        }
        
        /// <summary>
        /// Summon a random Beast Companion.
        /// </summary>
        public static void AnimalCompanion(Board b)
        {

        }
        
        /// <summary>
        /// Deal 1 damage to all enemy minions.
        /// </summary>
        public static void ArcaneExplosion(Board b)
        {

        }
        
        /// <summary>
        /// Secret: When one of your minions is attacked, summon three 1/1 Snakes.
        /// </summary>
        public static void SnakeTrap(Board b)
        {

        }
        
        /// <summary>
        /// Deal 2 damage to all enemy minions and Freeze them.
        /// </summary>
        public static void Blizzard(Board b)
        {

        }
        
        /// <summary>
        /// Give your weapon +2 Attack.
        /// </summary>
        public static void DeadlyPoison(Board b)
        {

        }
        
        /// <summary>
        /// Return an enemy minion to its owner's hand.
        /// </summary>
        public static void Sap(Board b)
        {

        }
        
        /// <summary>
        /// Secret: When your hero takes damage, deal that much damage to the enemy hero.
        /// </summary>
        public static void EyeforanEye(Board b)
        {

        }
        
        /// <summary>
        /// Deal 1 damage.
        /// </summary>
        public static void Moonfire(Board b)
        {

        }
        
        /// <summary>
        /// Deal 2 damage to all enemies.
        /// </summary>
        public static void Consecration(Board b)
        {

        }
        
        /// <summary>
        /// Deal damage equal to your hero's Attack to a minion.
        /// </summary>
        public static void Savagery(Board b)
        {

        }
        
        /// <summary>
        /// Summon three 2/2 Treants with Charge that die at the end of the turn.
        /// </summary>
        public static void ForceofNature(Board b)
        {

        }
        
        /// <summary>
        /// Choose One - Give your minions +1/+1; or Summon a 3/2 Panther.
        /// </summary>
        public static void PoweroftheWild(Board b)
        {

        }
        
        /// <summary>
        /// Deal 3 damage. Overload: (1)
        /// </summary>
        public static void LightningBolt(Board b)
        {

        }
        
        /// <summary>
        /// If you have a weapon, give it +1/+1. Otherwise equip a 1/3 weapon.
        /// </summary>
        public static void Upgrade(Board b)
        {

        }
        
        /// <summary>
        /// Secret: When an enemy minion attacks, return it to its owner's hand and it costs (2) more.
        /// </summary>
        public static void FreezingTrap(Board b)
        {

        }
        
        /// <summary>
        /// Deal 5 damage to the enemy hero.
        /// </summary>
        public static void MindBlast(Board b)
        {

        }
        
        /// <summary>
        /// Deal 1 damage to a minion for each Armor you have.
        /// </summary>
        public static void ShieldSlam(Board b)
        {

        }
        
        /// <summary>
        /// Draw 2 cards.
        /// </summary>
        public static void ArcaneIntellect(Board b)
        {

        }
        
        /// <summary>
        /// Deal 3 damage randomly split among enemy characters.
        /// </summary>
        public static void ArcaneMissiles(Board b)
        {

        }
        
        /// <summary>
        /// Deal 1 damage. Draw a card.
        /// </summary>
        public static void Shiv(Board b)
        {

        }
        
        /// <summary>
        /// Give your hero +4 Attack this turn and 4 Armor.
        /// </summary>
        public static void Bite(Board b)
        {

        }
        
        /// <summary>
        /// Secret: When an enemy attacks, summon a 2/1 Defender as the new target.
        /// </summary>
        public static void NobleSacrifice(Board b)
        {

        }
        
        /// <summary>
        /// Secret: When your hero is attacked, deal 2 damage to all enemies.
        /// </summary>
        public static void ExplosiveTrap(Board b)
        {

        }
        
        /// <summary>
        /// Freeze all enemy minions.
        /// </summary>
        public static void FrostNova(Board b)
        {

        }
        
        /// <summary>
        /// Restore #8 Health. Draw 3 cards.
        /// </summary>
        public static void LayonHands(Board b)
        {

        }
        
        /// <summary>
        /// Give a minion +2 Health. Draw a card.
        /// </summary>
        public static void PowerWordShield(Board b)
        {

        }
        
        /// <summary>
        /// Secret: As soon as your hero is attacked, gain 8 Armor.
        /// </summary>
        public static void IceBarrier(Board b)
        {

        }
        
        /// <summary>
        /// Deal 2-3 damage to all enemy minions. Overload: (2)
        /// </summary>
        public static void LightningStorm(Board b)
        {

        }
        
        /// <summary>
        /// Draw 4 cards.
        /// </summary>
        public static void Sprint(Board b)
        {

        }
        
        /// <summary>
        /// Deal 1 damage to ALL minions.
        /// </summary>
        public static void Whirlwind(Board b)
        {

        }
        
        /// <summary>
        /// Deal 3 damage to a character and Freeze it.
        /// </summary>
        public static void Frostbolt(Board b)
        {

        }
        
        /// <summary>
        /// Deal 1 damage to all enemy minions. Draw a card.
        /// </summary>
        public static void FanofKnives(Board b)
        {

        }
        
        /// <summary>
        /// Draw cards until you have as many in hand as your opponent.
        /// </summary>
        public static void DivineFavor(Board b)
        {

        }
        
        /// <summary>
        /// Deal 2 damage to the enemy hero. Combo: Return this to your hand next turn.
        /// </summary>
        public static void Headcrack(Board b)
        {

        }
        
        /// <summary>
        /// Deal 3 damage to the enemy hero.
        /// </summary>
        public static void SinisterStrike(Board b)
        {

        }
        
        /// <summary>
        /// Give a minion Divine Shield.
        /// </summary>
        public static void HandofProtection(Board b)
        {

        }
        
        /// <summary>
        /// Give your characters +2 Attack this turn.
        /// </summary>
        public static void SavageRoar(Board b)
        {

        }
        
        /// <summary>
        /// Change the Health of ALL minions to 1.
        /// </summary>
        public static void Equality(Board b)
        {

        }
        
        /// <summary>
        /// Transform a minion into a 0/1 Frog with Taunt.
        /// </summary>
        public static void Hex(Board b)
        {

        }
        
        /// <summary>
        /// Silence a minion, then deal 1 damage to it.
        /// </summary>
        public static void EarthShock(Board b)
        {

        }
        
        /// <summary>
        /// Restore #8 Health.
        /// </summary>
        public static void HealingTouch(Board b)
        {

        }
        
        /// <summary>
        /// Destroy a damaged enemy minion.
        /// </summary>
        public static void Execute(Board b)
        {

        }
        
        /// <summary>
        /// Deal 4 damage. If your hero has 12 or less Health, deal 6 damage instead.
        /// </summary>
        public static void MortalStrike(Board b)
        {

        }
        
        /// <summary>
        /// Secret: When your opponent plays a minion, deal 4 damage to it.
        /// </summary>
        public static void Snipe(Board b)
        {

        }
        
        /// <summary>
        /// Draw a card. That card costs (3) less.
        /// </summary>
        public static void FarSight(Board b)
        {

        }
        
        /// <summary>
        /// Deal 5 damage. Draw a card.
        /// </summary>
        public static void Starfire(Board b)
        {

        }
        
        /// <summary>
        /// Give your Totems +2 Health.
        /// </summary>
        public static void TotemicMight(Board b)
        {

        }
        
        /// <summary>
        /// Choose One - Deal 3 damage to a minion; or 1 damage and draw a card.
        /// </summary>
        public static void Wrath(Board b)
        {

        }
        
        /// <summary>
        /// Deal 2 damage to all enemies. Restore #2 Health to all friendly characters.
        /// </summary>
        public static void HolyNova(Board b)
        {

        }
        
        /// <summary>
        /// Give a friendly minion +4/+4 until end of turn. Then, it dies. Horribly.
        /// </summary>
        public static void PowerOverwhelming(Board b)
        {

        }
        
        /// <summary>
        /// Change a minion's Attack to 1.
        /// </summary>
        public static void Humility(Board b)
        {

        }
        
        /// <summary>
        /// Destroy all minions.
        /// </summary>
        public static void TwistingNether(Board b)
        {

        }
        
        /// <summary>
        /// Put 2 random Demons from your deck into your hand.
        /// </summary>
        public static void SenseDemons(Board b)
        {

        }
        
        /// <summary>
        /// Deal 5 damage. Overload: (2)
        /// </summary>
        public static void LavaBurst(Board b)
        {

        }
        
        /// <summary>
        /// Deal 2 damage.
        /// </summary>
        public static void ArcaneShot(Board b)
        {

        }
        
        /// <summary>
        /// All minions lose Stealth. Destroy all enemy Secrets. Draw a card.
        /// </summary>
        public static void Flare(Board b)
        {

        }
        
        /// <summary>
        /// Give a Beast +2 Attack and Immune this turn.
        /// </summary>
        public static void BestialWrath(Board b)
        {

        }
        
        /// <summary>
        /// Deal 2 damage. Combo: Deal 4 damage instead.
        /// </summary>
        public static void Eviscerate(Board b)
        {

        }
        
        /// <summary>
        /// Deal 4 damage to a minion.
        /// </summary>
        public static void ShadowBolt(Board b)
        {

        }
        
        /// <summary>
        /// Deal 2 damage. Restore #2 Health to your hero.
        /// </summary>
        public static void DrainLife(Board b)
        {

        }
        
        /// <summary>
        /// Deal 2 damage to two random enemy minions.
        /// </summary>
        public static void Cleave(Board b)
        {

        }
        
        /// <summary>
        /// Give a minion +4/+4. (+4 Attack/+4 Health)
        /// </summary>
        public static void BlessingofKings(Board b)
        {

        }
        
        /// <summary>
        /// Deal 3 damage to ALL characters.
        /// </summary>
        public static void Hellfire(Board b)
        {

        }
        
        /// <summary>
        /// Deal 1 damage to an enemy character and Freeze it.
        /// </summary>
        public static void FrostShock(Board b)
        {

        }
        
        /// <summary>
        /// Deal 4 damage. Discard a random card.
        /// </summary>
        public static void Soulfire(Board b)
        {

        }
        
        /// <summary>
        /// Choose an enemy minion. At the start of your turn, destroy it.
        /// </summary>
        public static void Corruption(Board b)
        {

        }
        
        /// <summary>
        /// Give your minions Stealth until your next turn.
        /// </summary>
        public static void Conceal(Board b)
        {

        }
        
        /// <summary>
        /// Deal 4 damage to all enemy minions.
        /// </summary>
        public static void Flamestrike(Board b)
        {

        }
        
        /// <summary>
        /// Give your hero +4 Attack this turn.
        /// </summary>
        public static void HeroicStrike(Board b)
        {

        }
        
        /// <summary>
        /// Gain 5 Armor. Draw a card.
        /// </summary>
        public static void ShieldBlock(Board b)
        {

        }
        
        /// <summary>
        /// Your minions can't be reduced below 1 Health this turn. Draw a card.
        /// </summary>
        public static void CommandingShout(Board b)
        {

        }
        
        /// <summary>
        /// Look at the top three cards of your deck. Draw one and discard the others.
        /// </summary>
        public static void Tracking(Board b)
        {

        }
        
        /// <summary>
        /// Give your hero +2 Attack this turn and 2 Armor.
        /// </summary>
        public static void Claw(Board b)
        {

        }
        
        /// <summary>
        /// Destroy your weapon and deal its damage to all enemies.
        /// </summary>
        public static void BladeFlurry(Board b)
        {

        }
        
        /// <summary>
        /// Deal 2 damage to a minion. If it survives, draw a card.
        /// </summary>
        public static void Slam(Board b)
        {

        }
        
        /// <summary>
        /// Summon two 0/2 minions with Taunt.
        /// </summary>
        public static void MirrorImage(Board b)
        {

        }
        
        /// <summary>
        /// Deal 10 damage.
        /// </summary>
        public static void Pyroblast(Board b)
        {

        }
        
        /// <summary>
        /// Secret: When a character attacks your hero, instead he attacks another random character.
        /// </summary>
        public static void Misdirection(Board b)
        {

        }
        
        /// <summary>
        /// Deal 1 damage to a minion. If that kills it, draw a card.
        /// </summary>
        public static void MortalCoil(Board b)
        {

        }
        
        /// <summary>
        /// Destroy a random enemy minion.
        /// </summary>
        public static void DeadlyShot(Board b)
        {

        }
        
        /// <summary>
        /// Put a copy of a random card in your opponent's hand into your hand.
        /// </summary>
        public static void MindVision(Board b)
        {

        }
        
        /// <summary>
        /// Destroy a minion. Restore #3 Health to your hero.
        /// </summary>
        public static void SiphonSoul(Board b)
        {

        }
        
        /// <summary>
        /// Give a damaged minion +3/+3.
        /// </summary>
        public static void Rampage(Board b)
        {

        }
        
        /// <summary>
        /// Gain an empty Mana Crystal.
        /// </summary>
        public static void WildGrowth(Board b)
        {

        }
        
        /// <summary>
        /// Deal 2 damage to a minion. If it’s a friendly Demon, give it +2/+2 instead.
        /// </summary>
        public static void Demonfire(Board b)
        {

        }
        
        /// <summary>
        /// The next spell you cast this turn costs (3) less.
        /// </summary>
        public static void Preparation(Board b)
        {

        }
        
        /// <summary>
        /// Give your minions +3 Attack this turn.
        /// </summary>
        public static void Bloodlust(Board b)
        {

        }
        
        /// <summary>
        /// Deal 8 damage randomly split among enemy characters.
        /// </summary>
        public static void AvengingWrath(Board b)
        {

        }
        
        /// <summary>
        /// Silence a minion.
        /// </summary>
        public static void Silence(Board b)
        {

        }
        
        /// <summary>
        /// For each enemy minion, summon a 1/1 Hound with Charge.
        /// </summary>
        public static void UnleashtheHounds(Board b)
        {

        }
        
        /// <summary>
        /// Double a minion's Health.
        /// </summary>
        public static void DivineSpirit(Board b)
        {

        }
        
        /// <summary>
        /// Restore #4 Health to ALL minions.
        /// </summary>
        public static void CircleofHealing(Board b)
        {

        }
        
        /// <summary>
        /// Destroy a minion with an Attack of 5 or more.
        /// </summary>
        public static void ShadowWordDeath(Board b)
        {

        }
        
        /// <summary>
        /// Deal 5 damage. Restore 5 Health to your hero.
        /// </summary>
        public static void HolyFire(Board b)
        {

        }
        
        /// <summary>
        /// Silence all enemy minions. Draw a card.
        /// </summary>
        public static void MassDispel(Board b)
        {

        }
        
        /// <summary>
        /// Destroy a minion with 3 or less Attack.
        /// </summary>
        public static void ShadowWordPain(Board b)
        {

        }
        
        /// <summary>
        /// Your Hero Power becomes 'Deal 2 damage'. If already in Shadowform: 3 damage.
        /// </summary>
        public static void Shadowform(Board b)
        {

        }
        
        /// <summary>
        /// Choose a minion. Whenever it attacks, draw a card.
        /// </summary>
        public static void BlessingofWisdom(Board b)
        {

        }
        
        /// <summary>
        /// Double a minion's Attack.
        /// </summary>
        public static void BlessedChampion(Board b)
        {

        }
        
        /// <summary>
        /// Draw 2 cards. Costs (1) less for each minion that died this turn.
        /// </summary>
        public static void SolemnVigil(Board b)
        {

        }
        
        /// <summary>
        /// Deal 4 damage. Costs (1) less for each minion that died this turn.
        /// </summary>
        public static void DragonsBreath(Board b)
        {

        }
        
        /// <summary>
        /// Deal 2 damage to all non-Demon minions.
        /// </summary>
        public static void Demonwrath(Board b)
        {

        }
        
        /// <summary>
        /// Choose a minion. Shuffle 3 copies of it into your deck.
        /// </summary>
        public static void GangUp(Board b)
        {

        }
        
        /// <summary>
        /// Deal 2 damage. Unlock your Overloaded Mana Crystals.
        /// </summary>
        public static void LavaShock(Board b)
        {

        }
        
        /// <summary>
        /// Deal 3 damage. If your hand is empty, draw a card.
        /// </summary>
        public static void QuickShot(Board b)
        {

        }
        
        /// <summary>
        /// Deal 1 damage to all minions. If you have 12 or less Health, deal 3 damage instead.
        /// </summary>
        public static void Revenge(Board b)
        {

        }
        
        /// <summary>
        /// Summon a random friendly minion that died this game.
        /// </summary>
        public static void Resurrect(Board b)
        {

        }
        
        /// <summary>
        /// Give your minions +2/+2. Costs (1) less for each Murloc you control.
        /// </summary>
        public static void EveryfinisAwesome(Board b)
        {

        }
        
        /// <summary>
        /// Give a minion +1/+1 and "Deathrattle: Add an Explorer's Hat to your hand."
        /// </summary>
        public static void ExplorersHat(Board b)
        {

        }
        
        /// <summary>
        /// Choose one - Discover a minion; or Discover a spell.
        /// </summary>
        public static void RavenIdol(Board b)
        {

        }
        
        /// <summary>
        /// Secret: When an opposing Hero Power is used, deal 5 damage to a random enemy.
        /// </summary>
        public static void DartTrap(Board b)
        {

        }
        
        /// <summary>
        /// Deal 3 damage. Shuffle a 'Roaring Torch' into your deck that deals 6 damage.
        /// </summary>
        public static void ForgottenTorch(Board b)
        {

        }
        
        /// <summary>
        /// Summon 7 Murlocs that died this game.
        /// </summary>
        public static void AnyfinCanHappen(Board b)
        {

        }
        
        /// <summary>
        /// Secret: When your opponent has at least 3 minions and plays another, destroy it.
        /// </summary>
        public static void SacredTrial(Board b)
        {

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
