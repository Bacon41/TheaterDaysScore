using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TheaterDaysScore.JsonModels {
    public class CardData {
        public enum Rarities {
            N = 1,
            R,
            SR,
            SSR
        }

        public class Skill {
            public Type effectId { get; set; }
            public int duration { get; set; }
            public int interval { get; set; }
            public int probability { get; set; }
            public Evaluations[] evaluationTypes { get; set; }
            public int[] values { get; set; }

            public enum Type {
                none,
                scoreBonus = 1,
                comboBonus,
                healer,
                lifeGuard,
                comboGuard,
                perfectLock,
                doubleBoost,
                multiUp,
                multiBonus,
                overClock,
                overRondo,
                doubleEffect,
                fusionScore = 17,
                fusionCombo,
                overEffect,
            };

            public enum Evaluations {
                all,
                perfect,
                perfectGreat,
                great,
                greatGoodFastSlow,
                perfectGreatGood,
                perfectGreatGoodFastSlow,
                greatGood,
            };
        }

        public class CenterEffect {
            public int id { get; set; }
            public Types idolType { get; set; } // To apply effect to
            public Types specificIdolType { get; set; } // Requirement for application
            public Types songType { get; set; } // Requirement for application
            public Type[] attributes { get; set; } // To apply effect to
            public int[] values { get; set; }

            public enum Type {
                none,
                vocalUp = 1,
                danceUp,
                visualUp,
                allUp,
                lifeUp,
                skillActivationUp,
                boostSkillUp,
                effectSkillUp,
                affectionPointsUp,
            };
        }
        
        public int id { get; set; }
        public int idolId { get; set; }
        public Types idolType { get; set; }
        public Rarities rarity { get; set; }
        public string resourceId { get; set; }

        public enum ExTypes {
            none = 0,
            eventRanking = 2,
            eventPoints,
            fes,
            firstAnniversary,
            extra,
            secondAnniversary,
            eventRankingExtra,
            eventPointsExtra,
            thirdAnniversary,
            eventRankingExtra2,
            eventPointsExtra2,
            fourthAnniversary,
            secondHairstyle,
            specialEventSale,
            fifthAnniversary,
            pr,
            tenthFranchiseAnniversary,
            sixthAnniversary,
            linkage
        };
        public ExTypes exType { get; set; }
        public enum Categories {
            unknown = 0,
            normal = 10,
            perminantGacha = 20,
            limitedGacha,
            fesGacha,
            premiumPickupGacha = 24,
            secondHairstyleGacha,
            linkageGacha,
            millicolleSR = 30,
            eventReward,
            eventReward2,
            anniversaryEvent,
            theaterBoostExtra,
            millicolleR,
            franchiseAnniversaryEvent,
            theaterChallengeExtra,
            extraIdol = 41,
            extraIdolShop,
            importedFromKRTW,
            pr,
            other = 99
        };
        public Categories category { get; set; }

        public int masterRankMax { get; set; }
        public int skillLvMax { get; set; }
        public class Parameter {
            public class StatInfo {
                [JsonPropertyName("base")]
                public int baseNum { get; set; }
                public class AwakeningStats {
                    public float diff { get; set; }
                    public int max { get; set; }
                }
                public AwakeningStats beforeAwakened { get; set; }
                public AwakeningStats afterAwakened { get; set; }
                public int masterBonus { get; set; }
            }
            public StatInfo vocal { get; set; }
            public StatInfo dance { get; set; }
            public StatInfo visual { get; set; }
            public class AwakeningDiff {
                public int beforeAwakened { get; set; }
                public int afterAwakened { get; set; }
            }
            public AwakeningDiff lvMax { get; set; }
            public AwakeningDiff life { get; set; }
        }
        public Parameter parameters { get; set; }

        public CenterEffect centerEffect { get; set; }
        public List<Skill> skills { get; set; }
    }
}
