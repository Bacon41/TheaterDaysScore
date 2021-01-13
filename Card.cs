using SQLite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.Json.Serialization;

namespace TheaterDaysScore {
    class Card {

        public class Skill {
            public Type effectId { get; set; }
            public int duration { get; set; }
            public int interval { get; set; }
            public int probability { get; set; }
            public int[] value { get; set; }
            public int level { get; set; }

            public int leveledProbability;

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

            public Skill(Type effectId, int duration, int interval, int probability, int[] value, int level) {
                this.effectId = effectId;
                this.duration = duration;
                this.interval = interval;
                this.probability = probability;
                this.value = value;
                this.level = level;

                if (this.level <= 10) {
                    leveledProbability = this.probability + this.level;
                } else {
                    leveledProbability = this.probability + 10 + (this.level - 10) * 5;
                }
            }

            public void SetLevel(int level) {
                if (level <= 10) {
                    this.leveledProbability = this.probability + level;
                } else {
                    this.leveledProbability = this.probability + 10 + (level - 10) * 5;
                }
            }

            public int ScoreBonus() {
                switch (this.effectId) {
                    case Type.scoreUp:
                        return this.value[0];
                    case Type.doubleBoost:
                        return this.value[0];
                    case Type.multiUp:
                        return this.value[0];
                    case Type.overClock:
                        return this.value[0];
                }
                return 0;
            }

            public int ComboBonus() {
                switch (this.effectId) {
                    case Type.comboBonus:
                        return this.value[0];
                    case Type.doubleBoost:
                        return this.value[1];
                    case Type.overRondo:
                        return this.value[0];
                }
                return 0;
            }
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

            public CenterEffect(Type attribute, Types idolType, Types specificIdolType, Types songType, int value, Type attribute2, int value2) {
                this.attribute = attribute;
                this.idolType = idolType;
                this.specificIdolType = specificIdolType;
                this.songType = songType;
                this.value = value;
                this.attribute2 = attribute2;
                this.value2 = value2;
            }
        }

        public int id { get; set; }
        public int idolId { get; set; }
        public string colour { get; set; }
        public Types idolType { get; set; }

        public int vocalMaxAwakened { get; set; }
        public int vocalMasterBonus { get; set; }
        public int danceMaxAwakened { get; set; }
        public int danceMasterBonus { get; set; }
        public int visualMaxAwakened { get; set; }
        public int visualMasterBonus { get; set; }


        public CenterEffect centerEffect { get; set; }
        public List<Skill> skill { get; set; }

        private Random random;

        public Card(int id, int idolId, Types idolType,
            int vocalMaxAwakened, int vocalMasterBonus, int danceMaxAwakened, int danceMasterBonus, int visualMaxAwakened, int visualMasterBonus,
            CenterEffect centerEffect, List<Skill> skill) {

            this.id = id;
            this.idolId = idolId;
            this.idolType = idolType;

            this.vocalMaxAwakened = vocalMaxAwakened;
            this.vocalMasterBonus = vocalMasterBonus;
            this.danceMaxAwakened = danceMaxAwakened;
            this.danceMasterBonus = danceMasterBonus;
            this.visualMaxAwakened = visualMaxAwakened;
            this.visualMasterBonus = visualMasterBonus;

            this.centerEffect = centerEffect;
            this.skill = skill;

            random = new Random();

            // https://storage.matsurihi.me/mltd/card/017kth0054_0_a.png
            // https://storage.matsurihi.me/mltd/icon_l/017kth0054_1.png
        }

        public Vector3 GetStats(int level) {
            return new Vector3(this.vocalMaxAwakened + this.vocalMasterBonus * level,
                    this.danceMaxAwakened + this.danceMasterBonus * level,
                    this.visualMaxAwakened + this.visualMasterBonus * level);
        }

        public Vector3 GetCenter() {
            if (this.centerEffect == null) {
                return new Vector3(0);
            }
            switch (this.centerEffect.attribute) {
                case Card.CenterEffect.Type.vocalUp:
                    return new Vector3((float)this.centerEffect.value / 100, 0, 0);
                case Card.CenterEffect.Type.danceUp:
                    return new Vector3(0, (float)(this.centerEffect.value + this.centerEffect.value2) / 100, 0);
                case Card.CenterEffect.Type.visualUp:
                    return new Vector3(0, 0, (float)this.centerEffect.value / 100);
                case Card.CenterEffect.Type.allUp:
                    return new Vector3((float)this.centerEffect.value / 100);
            }
            return new Vector3(0);
        }

        public void SetLevel(int level) {
            foreach (Skill skill in this.skill) {
                skill.SetLevel(level);
            }
        }

        public List<int> GetActivations(Song song, CenterEffect guestEffect, CenterEffect centerEffect) {
            List<int> activations = new List<int>();
            if (this.skill == null) {
                return activations;
            }

            foreach (Skill skill in this.skill) {
                int start = skill.interval;
                double activationThreshold = skill.leveledProbability;
                if (guestEffect != null && guestEffect.attribute == CenterEffect.Type.skillBoost) {
                    if (guestEffect.idolType == this.idolType) {
                        activationThreshold += skill.leveledProbability * (double)guestEffect.value / 100;
                    }
                }
                if (centerEffect != null && centerEffect.attribute == CenterEffect.Type.skillBoost) {
                    if (centerEffect.idolType == this.idolType) {
                        activationThreshold += skill.leveledProbability * (double)centerEffect.value / 100;
                    }
                }

                double appealTime = song.SecondsSinceFirst(song.notes.First(note => note.size == 10));
                while (start < song.songLength) {
                    if (!song.IsDuringAppeal(start)) {
                        if (random.NextDouble() * 100 < activationThreshold) {
                            activations.Add(start);
                        }
                    }
                    start += skill.interval;
                }
            }
            return activations;
        }
    }
}
