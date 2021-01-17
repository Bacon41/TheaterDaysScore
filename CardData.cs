using SQLite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.Json.Serialization;

namespace TheaterDaysScore {
    public class CardData {
        public class Skill {
            public Type effectId { get; set; }
            public int duration { get; set; }
            public int interval { get; set; }
            public int probability { get; set; }
            public int[] value { get; set; }
            public int level { get; set; }

            public enum Type {
                scoreUp = 1,
                comboBonus,
                lifeRestore,
                damageGuard,
                comboProtect,
                judgementBoost,
                doubleBoost,
                multiUp,
                overClock,
                overRondo,
            };
        }

        public class CenterEffect {
            public Type attribute { get; set; }
            public Types idolType { get; set; }
            public Types specificIdolType { get; set; }
            public Types songType { get; set; }
            public int value { get; set; }
            public Type attribute2 { get; set; }
            public int value2 { get; set; }

            public enum Type {
                vocalUp = 1,
                danceUp,
                visualUp,
                allUp,
                lifeUp,
                skillBoost,
            };
        }

        public string resourceId { get; set; }
        public int id { get; set; }
        public int idolId { get; set; }
        public string colour { get; set; }
        public Types idolType { get; set; }

        public int masterRankMax { get; set; }
        public int skillLevelMax { get; set; }

        public int vocalMaxAwakened { get; set; }
        public int vocalMasterBonus { get; set; }
        public int danceMaxAwakened { get; set; }
        public int danceMasterBonus { get; set; }
        public int visualMaxAwakened { get; set; }
        public int visualMasterBonus { get; set; }

        public CenterEffect centerEffect { get; set; }
        public List<Skill> skill { get; set; }
    }
}
