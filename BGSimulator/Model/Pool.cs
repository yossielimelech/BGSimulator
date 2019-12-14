using BGSimulator.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static BGSimulator.Utils.RandomUtils;

namespace BGSimulator.Model
{
    public class Pool
    {

        List<IMinion> allMinions;
        List<IMinion> poolMinions;
        public Pool()
        {
            Initialize();
        }


        private void Initialize()
        {
            allMinions = new List<IMinion>()
            {
                //Tier 1
                new MinionBase() { MinionType = MinionType.Beast, Name = "Alleycat", Attack = 1, Health = 1, MinionTier = MinionTier.Ranks[1], OnPlayed = (tp) => tp.Board.Summon("Tabbycat",tp.Index)  },

                new MinionBase() { MinionType = MinionType.Beast, Name = "Tabbycat", Attack = 1, Health = 1, MinionTier = MinionTier.Ranks[1], PoolMinion = false },

                new MinionBase() { MinionType = MinionType.Beast, Name = "Dire Wolf Alpha", Attack = 2, Health = 2, MinionTier = MinionTier.Ranks[1] },

                new MinionBase() { MinionType = MinionType.Mech, Name = "Mecharoo", Attack = 1, Health = 1, Attributes = Attribute.DeathRattle, MinionTier = MinionTier.Ranks[1], OnDeath = (tp) => tp.Board.Summon("Jo-E Bot",tp.Index) },

                new MinionBase() { MinionType = MinionType.Mech, Name = "Jo-E Bot", Attack = 1, Health = 1, MinionTier = MinionTier.Ranks[1], PoolMinion = false },

                new MinionBase() { MinionType = MinionType.Mech, Name = "Micro Machine", Attack = 1, Health = 2, MinionTier = MinionTier.Ranks[1], OnTurnStart = (tp) => { tp.Board.Buff(minion: tp.Activator, attack: 1); } },

                new MinionBase() { MinionType = MinionType.Murloc, Name = "Murlock Tidecaller", Attack = 1, Health = 2, MinionTier = MinionTier.Ranks[1], OnMinionSummon = (tp) => { if(tp.Summon.MinionType == MinionType.Murloc) { tp.Board.Buff(minion: tp.Activator, attack: 1); } } },

                new MinionBase() { MinionType = MinionType.Murloc, Name = "Murlock Tidehunter", Attack = 2, Health = 1, MinionTier = MinionTier.Ranks[1], OnPlayed = (tp) => tp.Board.Summon("Murlock Scout",tp.Index) },

                new MinionBase() { MinionType = MinionType.Murloc, Name = "Murlock Scout", Attack = 1, Health = 1, MinionTier = MinionTier.Ranks[1], PoolMinion = false },

                new MinionBase() { MinionType = MinionType.Neutral, Name = "Righteos Protector", Attack = 1, Health = 1, MinionTier = MinionTier.Ranks[1] },

                new MinionBase() { MinionType = MinionType.Mech, Name = "Rockpool Hunter", Attack = 2, Health = 3, MinionTier = MinionTier.Ranks[1], Tags = MinionTag.Targeted, OnPlayed =(tp) => { tp.Board.Buff(minion: tp.Target, attack: 1, health: 1); } },

                new MinionBase() { MinionType = MinionType.Neutral, Name = "Selfless Hero", Attack = 2, Health = 1, MinionTier = MinionTier.Ranks[1], OnDeath = (tp) => { tp.Board.BuffRandom(attributes: Attribute.DivineShield); } },

                new MinionBase() { MinionType = MinionType.Demon, Name = "Voidwalker", Attack = 1,Health =3 , MinionTier = MinionTier.Ranks[1]},

                new MinionBase() { MinionType = MinionType.Demon, Name = "Vulgar Homunculus", Attack = 2,Health =4 , MinionTier =MinionTier.Ranks[1]},

                new MinionBase() { MinionType = MinionType.Neutral, Name = "Wrath Weaver", Attack = 1, Health = 1, MinionTier = MinionTier.Ranks[1] },

                //Tier 2
                new MinionBase() { MinionType = MinionType.Mech, Name = "Annoy-o-Tron", Attack = 1, Health = 2, Attributes = Attribute.Taunt | Attribute.DivineShield, MinionTier = MinionTier.Ranks[2] },

                new MinionBase() { MinionType = MinionType.Murloc, Name = "Coldlight Seer", Attack = 2, Health = 3, MinionTier = MinionTier.Ranks[2] },

                new MinionBase() { MinionType = MinionType.Mech, Name = "Harvest Golem", Attack = 2, Health = 3, MinionTier = MinionTier.Ranks[2] },

                new MinionBase() { MinionType = MinionType.Mech, Name = "Kaboom Bot", Attack = 2, Health = 2, MinionTier = MinionTier.Ranks[2] },

                new MinionBase() { MinionType = MinionType.Beast, Name = "Kindly Grandmother", Attack = 1, Health = 1, MinionTier = MinionTier.Ranks[2] },

                new MinionBase() { MinionType = MinionType.Mech, Name = "Metaltooth Leaper", Attack = 3, Health = 3, MinionTier = MinionTier.Ranks[2] },

                new MinionBase() { MinionType = MinionType.Beast, Name = "Mounted Raptor", Attack = 3, Health = 2, MinionTier = MinionTier.Ranks[2] },

                new MinionBase() { MinionType = MinionType.Murloc, Name = "Murloc Warleader", Attack = 3, Health = 3, MinionTier = MinionTier.Ranks[2] },

                new MinionBase() { MinionType = MinionType.Demon, Name = "Nathrezim Overseer", Attack = 2,Health = 4 , MinionTier = MinionTier.Ranks[2]},

                new MinionBase() { MinionType = MinionType.Amalgam, Name = "Nightmare Amalgam", Attack = 3,Health = 4 , MinionTier = MinionTier.Ranks[2]},

                new MinionBase() { MinionType = MinionType.Murloc, Name = "Old Murk-Eye", Attack = 2, Health = 4, MinionTier = MinionTier.Ranks[2] },

                new MinionBase() { MinionType = MinionType.Mech, Name = "Pogo-Hopper", Attack = 1, Health = 1, MinionTier = MinionTier.Ranks[2] },

                new MinionBase() { MinionType = MinionType.Beast, Name = "Rat Pack", Attack = 2, Health = 2, MinionTier = MinionTier.Ranks[2] },

                new MinionBase() { MinionType = MinionType.Beast, Name = "Scavenging Hyena", Attack = 2, Health = 2, MinionTier = MinionTier.Ranks[2] },

                new MinionBase() { MinionType = MinionType.Mech, Name = "Shielded Minibot", Attack = 1, Health = 1, MinionTier = MinionTier.Ranks[2] },

                new MinionBase() { MinionType = MinionType.Neutral, Name = "Spawn of N'Zoth", Attack = 2, Health = 2, MinionTier = MinionTier.Ranks[2] },

                new MinionBase() { MinionType = MinionType.Mech, Name = "Zoobot", Attack = 3, Health = 3, MinionTier = MinionTier.Ranks[2] },

                ////Tier 3
                new MinionBase() { MinionType = MinionType.Mech, Name = "Cobalt Guardian", Attack = 6, Health = 3, MinionTier = MinionTier.Ranks[3] },

                new MinionBase() { MinionType = MinionType.Neutral, Name = "Crowd Favorite", Attack = 4, Health = 4, MinionTier = MinionTier.Ranks[3] },

                new MinionBase() { MinionType = MinionType.Neutral, Name = "Crystalweaver", Attack = 5, Health = 4, MinionTier = MinionTier.Ranks[3] },

                new MinionBase() { MinionType = MinionType.Neutral, Name = "Houndmaster", Attack = 4, Health = 3, MinionTier = MinionTier.Ranks[3] },

                new MinionBase() { MinionType = MinionType.Demon, Name = "Imp Gang Boss", Attack = 2,Health =4 , MinionTier =MinionTier.Ranks[1]},

                new MinionBase() { MinionType = MinionType.Beast, Name = "Infested Wolf", Attack = 3, Health = 3, MinionTier = MinionTier.Ranks[3] },

                new MinionBase() { MinionType = MinionType.Neutral, Name = "Khadgar", Attack = 2, Health = 2, MinionTier = MinionTier.Ranks[3] },

                new MinionBase() { MinionType = MinionType.Neutral, Name = "Pack Leader", Attack = 3, Health = 3, MinionTier = MinionTier.Ranks[3] },

                new MinionBase() { MinionType = MinionType.Neutral, Name = "Phalanx Commander", Attack = 4, Health = 5, MinionTier = MinionTier.Ranks[3] },

                new MinionBase() { MinionType = MinionType.Mech, Name = "Piloted Shredder", Attack = 4, Health = 3, MinionTier = MinionTier.Ranks[3] },

                new MinionBase() { MinionType = MinionType.Mech, Name = "Psych-o-Tron", Attack = 3, Health = 4, MinionTier = MinionTier.Ranks[3] },

                new MinionBase() { MinionType = MinionType.Mech, Name = "Replicating Menace", Attack = 3, Health = 1, MinionTier = MinionTier.Ranks[3] },

                new MinionBase() { MinionType = MinionType.Mech, Name = "Screwjank Clunker", Attack = 2, Health = 5, MinionTier = MinionTier.Ranks[3] },

                new MinionBase() { MinionType = MinionType.Neutral, Name = "Shifter Zerus", Attack = 1, Health = 1, MinionTier = MinionTier.Ranks[3] },

                new MinionBase() { MinionType = MinionType.Neutral, Name = "Soul Juggler", Attack = 3, Health = 3, MinionTier = MinionTier.Ranks[3] },

                new MinionBase() { MinionType = MinionType.Neutral, Name = "Tortollan Shellraiser", Attack = 2, Health = 6, MinionTier = MinionTier.Ranks[3] },

                ////Tier 4
                new MinionBase() { MinionType = MinionType.Mech, Name = "Annoy-o-Module", Attack = 2, Health = 4, MinionTier = MinionTier.Ranks[4] },

                new MinionBase() { MinionType = MinionType.Neutral, Name = "Bolvar, Fireblood", Attack = 1, Health = 7, MinionTier = MinionTier.Ranks[4] },

                new MinionBase() { MinionType = MinionType.Beast, Name = "Cave Hydra", Attack = 2, Health = 4, MinionTier = MinionTier.Ranks[4] },

                new MinionBase() { MinionType = MinionType.Neutral, Name = "Defender of Argus", Attack = 2, Health = 3, MinionTier = MinionTier.Ranks[4] },

                new MinionBase() { MinionType = MinionType.Neutral, Name = "Festeroot Hulk", Attack = 2, Health = 7, MinionTier = MinionTier.Ranks[4] },

                new MinionBase() { MinionType = MinionType.Mech, Name = "Iron Sensei", Attack = 2, Health = 2, MinionTier = MinionTier.Ranks[4] },

                new MinionBase() { MinionType = MinionType.Neutral, Name = "Menagerie Magician", Attack = 4, Health = 4, MinionTier = MinionTier.Ranks[4] },

                new MinionBase() { MinionType = MinionType.Mech, Name = "Piloted Sky Golem", Attack = 6, Health = 4, MinionTier = MinionTier.Ranks[4] },

                new MinionBase() { MinionType = MinionType.Murloc, Name = "Primalfin Lookout", Attack = 3, Health = 2, MinionTier = MinionTier.Ranks[4] },

                new MinionBase() { MinionType = MinionType.Mech, Name = "Security Rover", Attack = 2, Health = 6, MinionTier = MinionTier.Ranks[4] },

                new MinionBase() { MinionType = MinionType.Demon, Name = "Siegebreaker", Attack = 5,Health =8 , MinionTier =MinionTier.Ranks[4]},

                new MinionBase() { MinionType = MinionType.Beast, Name = "The Beast", Attack = 9, Health = 7, MinionTier = MinionTier.Ranks[4] },

                new MinionBase() { MinionType = MinionType.Murloc, Name = "Toxfin", Attack = 1, Health = 2, MinionTier = MinionTier.Ranks[4] },

                new MinionBase() { MinionType = MinionType.Neutral, Name = "Virmen Sensei", Attack = 4, Health = 5, MinionTier = MinionTier.Ranks[4] },

                ////Tier 5
                new MinionBase() { MinionType = MinionType.Demon, Name = "Annihilan Battlemaster", Attack = 3,Health =1 , MinionTier =MinionTier.Ranks[5]},

                new MinionBase() { MinionType = MinionType.Neutral, Name = "Baron Rivendare", Attack = 1, Health = 7, MinionTier = MinionTier.Ranks[5] },

                new MinionBase() { MinionType = MinionType.Neutral, Name = "Brann Bronzebeard", Attack = 2, Health = 4, MinionTier = MinionTier.Ranks[5] },

                new MinionBase() { MinionType = MinionType.Beast, Name = "Goldrinn, the Great Wolf", Attack = 4, Health = 4, MinionTier = MinionTier.Ranks[5] },

                new MinionBase() { MinionType = MinionType.Beast, Name = "Ironhide Direhorn", Attack =7, Health = 7, MinionTier = MinionTier.Ranks[5] },

                new MinionBase() { MinionType = MinionType.Mech, Name = "Junkbot", Attack = 1, Health = 5, MinionTier = MinionTier.Ranks[5] },

                new MinionBase() { MinionType = MinionType.Neutral, Name = "Lightfang Enforcer", Attack = 2, Health = 2, MinionTier = MinionTier.Ranks[5] },

                new MinionBase() { MinionType = MinionType.Demon, Name = "Mal'Ganis", Attack = 9,Health =7 , MinionTier =MinionTier.Ranks[5]},

                new MinionBase() { MinionType = MinionType.Mech, Name = "Mechano-Egg", Attack = 0, Health = 5, MinionTier = MinionTier.Ranks[5] },

                new MinionBase() { MinionType = MinionType.Beast, Name = "Sated Threshadon", Attack = 5, Health = 7, MinionTier = MinionTier.Ranks[5] },

                new MinionBase() { MinionType = MinionType.Beast, Name = "Savannah Highmane", Attack = 6, Health = 5, MinionTier = MinionTier.Ranks[5] },

                new MinionBase() { MinionType = MinionType.Neutral, Name = "Strongshell Scavenger", Attack = 2, Health = 3, MinionTier = MinionTier.Ranks[5] },

                new MinionBase() { MinionType = MinionType.Neutral, Name = "The Boogeymonster", Attack = 6, Health = 7, MinionTier = MinionTier.Ranks[5] },

                new MinionBase() { MinionType = MinionType.Demon, Name = "Voidlord", Attack = 3,Health =9 , MinionTier =MinionTier.Ranks[5]},

                ////Tier 6
                new MinionBase() { MinionType = MinionType.Mech, Name = "Foe Reaper 4000", Attack = 6, Health = 9, MinionTier = MinionTier.Ranks[6] },

                new MinionBase() { MinionType = MinionType.Beast, Name = "Gentle Megasaur", Attack = 5, Health = 4, MinionTier = MinionTier.Ranks[6] },

                new MinionBase() { MinionType = MinionType.Beast, Name = "Ghastcoiler", Attack = 7, Health = 7, MinionTier = MinionTier.Ranks[6] },

                new MinionBase() { MinionType = MinionType.Neutral, Name = "Kangor's Apprentice", Attack = 3, Health = 6, MinionTier = MinionTier.Ranks[6] },

                new MinionBase() { MinionType = MinionType.Beast, Name = "Maexxna", Attack = 2, Health = 8, MinionTier = MinionTier.Ranks[6] },

                new MinionBase() { MinionType = MinionType.Beast, Name = "Mama Bear", Attack = 4, Health = 4, MinionTier = MinionTier.Ranks[6] },

                new MinionBase() { MinionType = MinionType.Mech, Name = "Sneed's Old Shredder", Attack = 5, Health = 7, MinionTier = MinionTier.Ranks[6] },

                new MinionBase() { MinionType = MinionType.Neutral, Name = "Zapp Slywick", Attack = 7, Health = 10, MinionTier = MinionTier.Ranks[6] },

            };

            BuildPool();
        }


        public List<IMinion> Roll(int amount, int maxRank)
        {
            List<IMinion> rollOffer = new List<IMinion>();
            for (int i = 0; i < amount; i++)
            {
                rollOffer.Add(GetRandomMinion(maxRank));
            }

            return rollOffer;
        }

        private IMinion GetRandomMinion(int maxRank)
        {
            int rank = GetRandomRank(maxRank);
            lock (poolMinions)
            {
                var minionsFromRank = poolMinions.Where(m => m.MinionTier.Tier <= rank).ToArray();
                minionsFromRank = Shuffle(minionsFromRank) as IMinion[];
                var minion = minionsFromRank[rank];
                poolMinions.Remove(minion);
                return minion;
            }
        }

        public void Return(IMinion minion)
        {
            lock (poolMinions)
            {
                poolMinions.Add(minion);
            }
        }

        public void Return(List<IMinion> lastOffer)
        {
            lock (poolMinions)
            {
                poolMinions.AddRange(lastOffer);
            }
        }

        private int GetRandomRank(int maxRank)
        {
            int sum = 0;
            int rank = 0;
            var total = Enumerable.Range(1, maxRank).Sum();
            int rand = RandomNumber(1, total);

            for (int i = 0; i < maxRank; i++)
            {
                sum += (maxRank - i);
                if (rand <= sum)
                {
                    rank = i + 1;
                }
            }

            return rank;
        }

        public void BuildPool()
        {
            poolMinions = new List<IMinion>();

            foreach (var minion in allMinions)
            {
                if (!minion.PoolMinion)
                    continue;
                for (int i = 0; i < minion.MinionTier.NumberOfCopies; i++)
                {
                    poolMinions.Add(minion.Clone());
                }
            }
        }

        public IMinion GetCopy(string minionName)
        {
            return allMinions.First(m => m.Name == minionName)?.Clone();
        }
    }
}
