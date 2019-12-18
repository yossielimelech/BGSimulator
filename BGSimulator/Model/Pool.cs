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

        private static Pool instance;
        public static Pool Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Pool();
                }

                return instance;
            }
        }

        private Pool()
        {
            Initialize();
        }


        private void Initialize()
        {
            allMinions = new List<IMinion>()
            {
                //Tier 1
                new MinionBase() { MinionType = MinionType.Beast, Name = "Alleycat", Attack = 1, Health = 1, MinionTier = MinionTier.Ranks[1], Tags = MinionTag.BattleCry, OnPlayed = (tp) => tp.Board.Summon("Tabbycat",tp.Index)  },

                new MinionBase() { MinionType = MinionType.Beast, Name = "Tabbycat", Attack = 1, Health = 1, MinionTier = MinionTier.Ranks[1], PoolMinion = false },

                new MinionBase() { MinionType = MinionType.Beast, Name = "Dire Wolf Alpha", Attack = 2, Health = 2, MinionTier = MinionTier.Ranks[1] }, //*****

                new MinionBase() { MinionType = MinionType.Mech, Name = "Mecharoo", Attack = 1, Health = 1, Attributes = Attribute.DeathRattle, MinionTier = MinionTier.Ranks[1], OnDeath = (tp) => tp.Board.Summon("Jo-E Bot",tp.Index, Direction.InPlace) },

                new MinionBase() { MinionType = MinionType.Mech, Name = "Jo-E Bot", Attack = 1, Health = 1, MinionTier = MinionTier.Ranks[1], PoolMinion = false },

                new MinionBase() { MinionType = MinionType.Mech, Name = "Micro Machine", Attack = 1, Health = 2, MinionTier = MinionTier.Ranks[1], OnTurnStart = (tp) => { tp.Board.Buff(minion: tp.Activator, attack: 1); } },

                new MinionBase() { MinionType = MinionType.Murloc, Name = "Murloc Tidecaller", Attack = 1, Health = 2, MinionTier = MinionTier.Ranks[1], OnMinionSummon = (tp) => { if(tp.Summon.MinionType == MinionType.Murloc) { tp.Board.Buff(minion: tp.Activator, attack: 1); } } },

                new MinionBase() { MinionType = MinionType.Murloc, Name = "Murloc Tidehunter", Attack = 2, Health = 1, MinionTier = MinionTier.Ranks[1], Tags = MinionTag.BattleCry, OnPlayed = (tp) => tp.Board.Summon("Murloc Scout",tp.Index) },

                new MinionBase() { MinionType = MinionType.Murloc, Name = "Murloc Scout", Attack = 1, Health = 1, MinionTier = MinionTier.Ranks[1], PoolMinion = false },

                new MinionBase() { MinionType = MinionType.Neutral, Name = "Righteous Protector", Attack = 1, Health = 1, MinionTier = MinionTier.Ranks[1] },

                new MinionBase() { MinionType = MinionType.Mech, Name = "Rockpool Hunter", Attack = 2, Health = 3, MinionTier = MinionTier.Ranks[1], Tags = MinionTag.Targeted | MinionTag.BattleCry, ValidTargets = MinionType.Murloc, OnPlayed =(tp) => { if(tp.Target == null) return; tp.Board.Buff(minion: tp.Target, attack: 1, health: 1); } },

                new MinionBase() { MinionType = MinionType.Neutral, Name = "Selfless Hero", Attack = 2, Health = 1, MinionTier = MinionTier.Ranks[1], OnDeath = (tp) => { tp.Board.BuffRandom(attributes: Attribute.DivineShield); } },

                new MinionBase() { MinionType = MinionType.Demon, Name = "Voidwalker", Attack = 1, Health = 3 , MinionTier = MinionTier.Ranks[1], Attributes = Attribute.Taunt },

                new MinionBase() { MinionType = MinionType.Demon, Name = "Vulgar Homunculus", Attack = 2,Health = 4 , MinionTier = MinionTier.Ranks[1], Attributes = Attribute.Taunt, Tags = MinionTag.BattleCry, OnPlayed = (tp) => { tp.Player.TakeDamage(2); } },

                new MinionBase() { MinionType = MinionType.Neutral, Name = "Wrath Weaver", Attack = 1, Health = 1, MinionTier = MinionTier.Ranks[1], OnMinionSummon = (tp) => { if (tp.Summon.MinionType.HasFlag(MinionType.Demon)) { tp.Player.TakeDamage(1); } } },

                //Tier 2
                new MinionBase() { MinionType = MinionType.Mech, Name = "Annoy-o-Tron", Attack = 1, Health = 2, Attributes = Attribute.Taunt | Attribute.DivineShield, MinionTier = MinionTier.Ranks[2] },

                new MinionBase() { MinionType = MinionType.Murloc, Name = "Coldlight Seer", Attack = 2, Health = 3, MinionTier = MinionTier.Ranks[2], Tags = MinionTag.BattleCry, ValidTargets = MinionType.Murloc, OnPlayed = (tp) => { tp.Board.BuffAllOfType(type:tp.Activator.ValidTargets, health: 2); } },

                new MinionBase() { MinionType = MinionType.Mech, Name = "Harvest Golem", Attack = 2, Health = 3, MinionTier = MinionTier.Ranks[2], Attributes = Attribute.DeathRattle, OnDeath = (tp) => {tp.Board.Summon("Damaged Golem", tp.Index, Direction.InPlace); } },

                new MinionBase() { MinionType = MinionType.Mech, Name = "Damaged Golem", Attack = 2, Health = 1, MinionTier = MinionTier.Ranks[1], PoolMinion = false },

                new MinionBase() { MinionType = MinionType.Mech, Name = "Kaboom Bot", Attack = 2, Health = 2, MinionTier = MinionTier.Ranks[2], OnDeath = (tp) => { var minion = tp.RivalBoard.GetRandomMinion(); if(minion != null) { tp.RivalBoard.MinionTakeDamage(minion, 4); } } },

                new MinionBase() { MinionType = MinionType.Beast, Name = "Kindly Grandmother", Attack = 1, Health = 1, MinionTier = MinionTier.Ranks[2], Attributes = Attribute.DeathRattle, OnDeath = (tp) => {tp.Board.Summon("Big Bad Wolf", tp.Index, Direction.InPlace); } },

                new MinionBase() { MinionType = MinionType.Beast, Name = "Big Bad Wolf", Attack = 3, Health = 2, MinionTier = MinionTier.Ranks[1], PoolMinion = false },

                new MinionBase() { MinionType = MinionType.Mech, Name = "Metaltooth Leaper", Attack = 3, Health = 3, MinionTier = MinionTier.Ranks[2], Tags = MinionTag.BattleCry, OnPlayed = (tp) => { tp.Board.BuffAllOfType(MinionType.Mech, 2); } },

                new MinionBase() { MinionType = MinionType.Beast, Name = "Mounted Raptor", Attack = 3, Health = 2, MinionTier = MinionTier.Ranks[2], Attributes = Attribute.DeathRattle, OnDeath = (tp) => { var minion = GetRandomMinionFromRank(1); tp.Board.Summon(minion, tp.Index, Direction.InPlace); } },

                new MinionBase() { MinionType = MinionType.Murloc, Name = "Murloc Warleader", Attack = 3, Health = 3, MinionTier = MinionTier.Ranks[2],  }, // **** 

                new MinionBase() { MinionType = MinionType.Demon, Name = "Nathrezim Overseer", Attack = 2,Health = 4 , MinionTier = MinionTier.Ranks[2], Tags = MinionTag.Targeted | MinionTag.BattleCry, ValidTargets = MinionType.Demon, OnPlayed = (tp) => { if(tp.Target ==null) return; tp.Board.Buff(tp.Target,2,2); } },

                new MinionBase() { MinionType = MinionType.Amalgam, Name = "Nightmare Amalgam", Attack = 3,Health = 4 , MinionTier = MinionTier.Ranks[2]},

                new MinionBase() { MinionType = MinionType.Murloc, Name = "Old Murk-Eye", Attack = 2, Health = 4, MinionTier = MinionTier.Ranks[2] },  //***

                new MinionBase() { MinionType = MinionType.Mech, Name = "Pogo-Hopper", Attack = 1, Health = 1, MinionTier = MinionTier.Ranks[2] }, //***

                new MinionBase() { MinionType = MinionType.Beast, Name = "Rat Pack", Attack = 2, Health = 2, MinionTier = MinionTier.Ranks[2], Attributes = Attribute.DeathRattle, OnDeath = (tp) => { tp.Board.Summon("Rat", tp.Index, Direction.InPlace, tp.Activator.Attack); } },

                new MinionBase() { MinionType = MinionType.Beast, Name = "Rat", Attack = 1, Health = 1, MinionTier = MinionTier.Ranks[1], PoolMinion = false },

                new MinionBase() { MinionType = MinionType.Beast, Name = "Scavenging Hyena", Attack = 2, Health = 2, MinionTier = MinionTier.Ranks[2], OnMinionDied = (tp) => { if(tp.Target.MinionType == MinionType.Beast) { tp.Board.Buff(tp.Activator, 2, 1); } } },

                new MinionBase() { MinionType = MinionType.Mech, Name = "Shielded Minibot", Attack = 1, Health = 1, MinionTier = MinionTier.Ranks[2], Attributes = Attribute.DivineShield },

                new MinionBase() { MinionType = MinionType.Neutral, Name = "Spawn of N'Zoth", Attack = 2, Health = 2, MinionTier = MinionTier.Ranks[2], Attributes = Attribute.DeathRattle, OnDeath = (tp) => { tp.Board.BuffAllOfType(MinionType.All, 1, 1); } },

                new MinionBase() { MinionType = MinionType.Mech, Name = "Zoobot", Attack = 3, Health = 3, MinionTier = MinionTier.Ranks[2], Tags = MinionTag.BattleCry, OnPlayed = (tp) => { tp.Board.BuffRandomUnique(new List<MinionType>(){ MinionType.Murloc, MinionType.Beast, MinionType.Dragon }, 1, 1); } },

                ////Tier 3
                new MinionBase() { MinionType = MinionType.Mech, Name = "Cobalt Guardian", Attack = 6, Health = 3, MinionTier = MinionTier.Ranks[3], OnMinionSummon = (tp) => { if(tp.Summon == null || tp.Summon.MinionType != MinionType.Mech) return; tp.Board.Buff(tp.Activator, attributes: Attribute.DivineShield); } },

                new MinionBase() { MinionType = MinionType.Neutral, Name = "Crowd Favorite", Attack = 4, Health = 4, MinionTier = MinionTier.Ranks[3], OnMinionSummon = (tp) => { if(tp.Summon == null || !tp.Summon.Tags.HasFlag(MinionTag.BattleCry)) return; tp.Board.Buff(tp.Activator, 1, 1); } },

                new MinionBase() { MinionType = MinionType.Neutral, Name = "Crystalweaver", Attack = 5, Health = 4, MinionTier = MinionTier.Ranks[3], Tags = MinionTag.Targeted | MinionTag.BattleCry, ValidTargets = MinionType.Demon, OnPlayed = (tp) => { if(tp.Target == null) return; tp.Board.Buff(tp.Target, 1, 1); } },

                new MinionBase() { MinionType = MinionType.Neutral, Name = "Houndmaster", Attack = 4, Health = 3, MinionTier = MinionTier.Ranks[3], Tags = MinionTag.Targeted | MinionTag.BattleCry, ValidTargets = MinionType.Beast, OnPlayed = (tp) => { if(tp.Target == null) return; tp.Board.Buff(tp.Target, 2, 2); } },

                new MinionBase() { MinionType = MinionType.Demon, Name = "Imp Gang Boss", Attack = 2, Health = 4 , MinionTier = MinionTier.Ranks[3], OnDamage = (tp) => { tp.Board.Summon("Imp", tp.Index); } },

                new MinionBase() { MinionType = MinionType.Demon, Name = "Imp", Attack = 1, Health = 1 , MinionTier = MinionTier.Ranks[1], PoolMinion = false },

                new MinionBase() { MinionType = MinionType.Beast, Name = "Infested Wolf", Attack = 3, Health = 3, MinionTier = MinionTier.Ranks[3], Attributes = Attribute.DeathRattle, OnDeath = (tp) => { tp.Board.Summon("Spider", tp.Index, Direction.InPlace, 2); } },

                new MinionBase() { MinionType = MinionType.Beast, Name = "Spider", Attack = 1, Health = 1, MinionTier = MinionTier.Ranks[1], },

                new MinionBase() { MinionType = MinionType.Neutral, Name = "Khadgar", Attack = 2, Health = 2, MinionTier = MinionTier.Ranks[3] }, // ***

                new MinionBase() { MinionType = MinionType.Neutral, Name = "Pack Leader", Attack = 3, Health = 3, MinionTier = MinionTier.Ranks[3], OnMinionSummon = (tp) => { if(tp.Summon == null || tp.Summon.MinionType != MinionType.Beast) return; tp.Board.Buff(tp.Summon, 3); } },

                new MinionBase() { MinionType = MinionType.Neutral, Name = "Phalanx Commander", Attack = 4, Health = 5, MinionTier = MinionTier.Ranks[3] }, //***

                new MinionBase() { MinionType = MinionType.Mech, Name = "Piloted Shredder", Attack = 4, Health = 3, MinionTier = MinionTier.Ranks[3], Attributes = Attribute.DeathRattle,  }, //***

                new MinionBase() { MinionType = MinionType.Mech, Name = "Psych-o-Tron", Attack = 3, Health = 4, MinionTier = MinionTier.Ranks[3], Attributes = Attribute.DivineShield | Attribute.Taunt },

                new MinionBase() { MinionType = MinionType.Mech, Name = "Replicating Menace", Attack = 3, Health = 1, MinionTier = MinionTier.Ranks[3], Tags = MinionTag.Magnetic, OnPlayed = (tp) => { tp.Board.TryMagnet(tp.Activator, tp.Index); } },

                new MinionBase() { MinionType = MinionType.Mech, Name = "Screwjank Clunker", Attack = 2, Health = 5, MinionTier = MinionTier.Ranks[3], Tags = MinionTag.Targeted | MinionTag.BattleCry, ValidTargets = MinionType.Mech, OnPlayed = (tp) => { if(tp.Target == null) return; tp.Board.Buff(tp.Target, 2, 2); } },

                new MinionBase() { MinionType = MinionType.Neutral, Name = "Shifter Zerus", Attack = 1, Health = 1, MinionTier = MinionTier.Ranks[3] }, //***

                new MinionBase() { MinionType = MinionType.Neutral, Name = "Soul Juggler", Attack = 3, Health = 3, MinionTier = MinionTier.Ranks[3], OnMinionDied = (tp) => { if(tp.Target.MinionType != MinionType.Demon) return; var target = tp.RivalBoard.GetRandomMinion(); if(target == null) return;  tp.RivalBoard.MinionTakeDamage(target, 3); } },

                new MinionBase() { MinionType = MinionType.Neutral, Name = "Tortollan Shellraiser", Attack = 2, Health = 6, MinionTier = MinionTier.Ranks[3], Attributes = Attribute.DeathRattle, OnDeath = (tp) => { tp.Board.BuffRandom(1, 1); } },

                ////Tier 4
                new MinionBase() { MinionType = MinionType.Mech, Name = "Annoy-o-Module", Attack = 2, Health = 4, MinionTier = MinionTier.Ranks[4], Attributes = Attribute.Taunt | Attribute.DivineShield, Tags = MinionTag.Magnetic, ValidTargets = MinionType.Mech, OnPlayed = (tp) => { tp.Board.TryMagnet(tp.Activator, tp.Index); } },

                new MinionBase() { MinionType = MinionType.Neutral, Name = "Bolvar, Fireblood", Attack = 1, Health = 7, MinionTier = MinionTier.Ranks[4], Attributes = Attribute.DivineShield, OnMinionLostDivineShield = (tp) => { tp.Board.Buff(tp.Activator, 2); } },

                new MinionBase() { MinionType = MinionType.Beast, Name = "Cave Hydra", Attack = 2, Health = 4, MinionTier = MinionTier.Ranks[4], OnAttack = (tp) => { tp.RivalBoard.CleaveAttack(tp.Activator, tp.Target); } },

                new MinionBase() { MinionType = MinionType.Neutral, Name = "Defender of Argus", Attack = 2, Health = 3, MinionTier = MinionTier.Ranks[4], Tags = MinionTag.BattleCry, ValidTargets = MinionType.All, OnPlayed = (tp) => { tp.Board.BuffAdjacent(tp.Activator, 1, 1, Attribute.Taunt); } },

                new MinionBase() { MinionType = MinionType.Neutral, Name = "Festeroot Hulk", Attack = 2, Health = 7, MinionTier = MinionTier.Ranks[4], OnMinionAttacked = (tp) => { tp.Board.Buff(tp.Activator, 1, 0); } },

                new MinionBase() { MinionType = MinionType.Mech, Name = "Iron Sensei", Attack = 2, Health = 2, MinionTier = MinionTier.Ranks[4], OnTurnEnd = (tp) => { tp.Board.BuffRandom(2, 2, type: MinionType.Mech); } },

                new MinionBase() { MinionType = MinionType.Neutral, Name = "Menagerie Magician", Attack = 4, Health = 4, MinionTier = MinionTier.Ranks[4], Tags = MinionTag.Targeted | MinionTag.BattleCry, OnPlayed = (tp) => { tp.Board.BuffRandomUnique(new List<MinionType>(){ MinionType.Beast, MinionType.Dragon, MinionType.Murloc }, 2, 2); } },

                new MinionBase() { MinionType = MinionType.Mech, Name = "Piloted Sky Golem", Attack = 6, Health = 4, MinionTier = MinionTier.Ranks[4], OnDeath = (tp) => { var minion = GetRandomMinionFromRank(4); tp.Board.Summon(minion, tp.Index, Direction.InPlace); } }, //*** (Got to here on check).

                new MinionBase() { MinionType = MinionType.Mech, Name = "Security Rover", Attack = 2, Health = 6, MinionTier = MinionTier.Ranks[4], OnDamage = (tp) => { tp.Board.Summon("Guard Bot", tp.Index); } },

                new MinionBase() { MinionType = MinionType.Mech, Name = "Guard Bot", Attack = 2, Health = 6, MinionTier = MinionTier.Ranks[1], Attributes = Attribute.Taunt, PoolMinion = false },

                new MinionBase() { MinionType = MinionType.Demon, Name = "Siegebreaker", Attack = 5,Health =8 , MinionTier = MinionTier.Ranks[4], Attributes = Attribute.Taunt, },

                new MinionBase() { MinionType = MinionType.Beast, Name = "The Beast", Attack = 9, Health = 7, MinionTier = MinionTier.Ranks[4], Attributes = Attribute.DeathRattle, OnDeath = (tp) => { tp.RivalBoard.Summon("Finkle Einhorn", tp.RivalBoard.PlayedMinions.Count - 1); } },

                new MinionBase() { MinionType = MinionType.Neutral, Name = "Finkle Einhorn", Attack = 3, Health = 3, MinionTier = MinionTier.Ranks[1], PoolMinion = false },

                new MinionBase() { MinionType = MinionType.Murloc, Name = "Toxfin", Attack = 1, Health = 2, MinionTier = MinionTier.Ranks[4] , Tags = MinionTag.Targeted | MinionTag.BattleCry, ValidTargets = MinionType.Murloc, OnPlayed = (tp) => { if(tp.Target ==null) return; tp.Board.Buff(tp.Target, attributes: Attribute.Poison); } },

                new MinionBase() { MinionType = MinionType.Neutral, Name = "Virmen Sensei", Attack = 4, Health = 5, MinionTier = MinionTier.Ranks[4], Tags = MinionTag.Targeted | MinionTag.BattleCry, ValidTargets = MinionType.Beast, OnPlayed = (tp) => { if(tp.Target ==null) return; tp.Board.Buff(tp.Target, 2 , 2); } },

                ////Tier 5
                new MinionBase() { MinionType = MinionType.Demon, Name = "Annihilan Battlemaster", Attack = 3, Health = 1 , MinionTier = MinionTier.Ranks[5], Tags = MinionTag.BattleCry, OnPlayed = (tp) => { tp.Board.Buff(tp.Activator, 0, tp.Player.MissingHealth); } },

                new MinionBase() { MinionType = MinionType.Neutral, Name = "Baron Rivendare", Attack = 1, Health = 7, MinionTier = MinionTier.Ranks[5] },

                new MinionBase() { MinionType = MinionType.Neutral, Name = "Brann Bronzebeard", Attack = 2, Health = 4, MinionTier = MinionTier.Ranks[5] },

                new MinionBase() { MinionType = MinionType.Beast, Name = "Goldrinn, the Great Wolf", Attack = 4, Health = 4, MinionTier = MinionTier.Ranks[5], Attributes = Attribute.DeathRattle, OnDeath = (tp) => { tp.Board.BuffAllOfType(MinionType.Beast, 4, 4); } },

                new MinionBase() { MinionType = MinionType.Beast, Name = "Ironhide Direhorn", Attack = 7, Health = 7, MinionTier = MinionTier.Ranks[5], OnAttack = (tp) => { /* if overkill summon x */ } },

                new MinionBase() { MinionType = MinionType.Mech, Name = "Junkbot", Attack = 1, Health = 5, MinionTier = MinionTier.Ranks[5], OnMinionDied = (tp) => { if(tp.Target.MinionType != MinionType.Mech) return; tp.Board.Buff(tp.Activator, 2, 2); } },

                new MinionBase() { MinionType = MinionType.Neutral, Name = "Lightfang Enforcer", Attack = 2, Health = 2, MinionTier = MinionTier.Ranks[5], OnTurnEnd = (tp) => { tp.Board.BuffRandomUnique(new List<MinionType>(){ MinionType.Beast, MinionType.Demon, MinionType.Mech, MinionType.Murloc }, 2, 2);  } },

                new MinionBase() { MinionType = MinionType.Demon, Name = "Mal'Ganis", Attack = 9,Health =7 , MinionTier = MinionTier.Ranks[5]},

                new MinionBase() { MinionType = MinionType.Mech, Name = "Mechano-Egg", Attack = 0, Health = 5, MinionTier = MinionTier.Ranks[5], Attributes = Attribute.DeathRattle, OnDeath = (tp) => { tp.Board.Summon("Robosaur", tp.Index, Direction.InPlace); } },

                new MinionBase() { MinionType = MinionType.Mech, Name = "Robosaur", Attack = 8, Health = 8, MinionTier = MinionTier.Ranks[1], PoolMinion = false },

                new MinionBase() { MinionType = MinionType.Murloc, Name = "Primalfin Lookout", Attack = 3, Health = 2, MinionTier = MinionTier.Ranks[5] },

                new MinionBase() { MinionType = MinionType.Beast, Name = "Sated Threshadon", Attack = 5, Health = 7, MinionTier = MinionTier.Ranks[5], Attributes = Attribute.DeathRattle, OnDeath = (tp) => { tp.Board.Summon("Primalfin", tp.Index, Direction.InPlace, 3); } },

                new MinionBase() { MinionType = MinionType.Murloc, Name = "Primalfin", Attack = 1, Health = 1, MinionTier = MinionTier.Ranks[1], PoolMinion = false },

                new MinionBase() { MinionType = MinionType.Beast, Name = "Savannah Highmane", Attack = 6, Health = 5, MinionTier = MinionTier.Ranks[5], Attributes = Attribute.DeathRattle, OnDeath = (tp) => { tp.Board.Summon("Hyena", tp.Index, Direction.InPlace); } },

                new MinionBase() { MinionType = MinionType.Beast, Name = "Hyena", Attack = 2, Health = 2, MinionTier = MinionTier.Ranks[1], PoolMinion = false },

                new MinionBase() { MinionType = MinionType.Neutral, Name = "Strongshell Scavenger", Attack = 2, Health = 3, MinionTier = MinionTier.Ranks[5], Tags = MinionTag.BattleCry, OnPlayed = (tp) => { tp.Board.BuffAllWithAttribute(Attribute.Taunt, 2, 2); } },

                new MinionBase() { MinionType = MinionType.Neutral, Name = "The Boogeymonster", Attack = 6, Health = 7, MinionTier = MinionTier.Ranks[5] },

                new MinionBase() { MinionType = MinionType.Demon, Name = "Voidlord", Attack = 3,Health =9 , MinionTier = MinionTier.Ranks[5], Attributes = Attribute.DeathRattle, OnDeath = (tp) => { tp.Board.Summon("Voidwalker", tp.Index, Direction.InPlace, 3); } },

                ////Tier 6
                new MinionBase() { MinionType = MinionType.Mech, Name = "Foe Reaper 4000", Attack = 6, Health = 9, MinionTier = MinionTier.Ranks[6], OnAttack = (tp) => { tp.RivalBoard.CleaveAttack(tp.Activator, tp.Target); } },

                new MinionBase() { MinionType = MinionType.Beast, Name = "Gentle Megasaur", Attack = 5, Health = 4, MinionTier = MinionTier.Ranks[6], Tags = MinionTag.BattleCry, OnPlayed = (tp) => { var adapt = tp.Player.ChooseAdapt(); tp.Board.BuffAdapt(adapt, tp.Index); } },

                new MinionBase() { MinionType = MinionType.Neutral, Name = "Plant", Attack = 1, Health = 1, MinionTier = MinionTier.Ranks[1], },

                new MinionBase() { MinionType = MinionType.Beast, Name = "Ghastcoiler", Attack = 7, Health = 7, MinionTier = MinionTier.Ranks[6] },

                new MinionBase() { MinionType = MinionType.Neutral, Name = "Kangor's Apprentice", Attack = 3, Health = 6, MinionTier = MinionTier.Ranks[6], Attributes = Attribute.DeathRattle, OnDeath = (tp) => { tp.Board.SummonFromGraveyard(MinionType.Mech, tp.Index, amount: 2); } },

                new MinionBase() { MinionType = MinionType.Beast, Name = "Maexxna", Attack = 2, Health = 8, MinionTier = MinionTier.Ranks[6], Attributes = Attribute.Poison },

                new MinionBase() { MinionType = MinionType.Beast, Name = "Mama Bear", Attack = 4, Health = 4, MinionTier = MinionTier.Ranks[6], OnMinionSummon = (tp) => { if(tp.Summon == null || tp.Summon.MinionType != MinionType.Beast) return; tp.Board.Buff(tp.Summon, 4, 4); } },

                new MinionBase() { MinionType = MinionType.Mech, Name = "Sneed's Old Shredder", Attack = 5, Health = 7, MinionTier = MinionTier.Ranks[6]},

                new MinionBase() { MinionType = MinionType.Neutral, Name = "Zapp Slywick", Attack = 7, Health = 10, MinionTier = MinionTier.Ranks[6],  Attributes = Attribute.WindFury, OnAttack = (tp) => { /*attack minion with least attack*/ } },

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

        private string GetRandomMinionFromRank(int rank)
        {
            var rankMinions = allMinions.Where(m => m.MinionTier.Tier == rank && m.PoolMinion).ToList();
            rankMinions.Shuffle();
            return rankMinions.First().Name;
        }

        private IMinion GetRandomMinion(int maxRank)
        {
            int rank = GetRandomRank(maxRank);
            lock (poolMinions)
            {
                var minionsFromRank = poolMinions.Where(m => m.MinionTier.Tier <= rank).ToList();
                minionsFromRank.Shuffle();
                var minion = minionsFromRank[rank];
                poolMinions.Remove(minion);
                return minion;
            }
        }

        public void Return(IMinion minion)
        {
            if (!minion.PoolMinion)
                return;

            lock (poolMinions)
            {
                poolMinions.Add(GetFreshCopy(minion.Name));
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

        public IMinion GetFreshCopy(string minionName)
        {
            return allMinions.First(m => m.Name == minionName)?.Clone();
        }
    }
}
