using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TheaterDaysScore.Models {
    public class Card {
        private CardData data;
        private Idol idol;
        private int masterRank;
        private int skillLevel;

        public string ID { get; }
        public Types Type { get; }
        public Color Color { get; }
        public List<Skill> Skills { get; }
        public CenterEffect Center { get; }

        public class Skill {
            private CardData.Skill data;
            private int level;

            public int Interval { get; }
            public int Duration { get; }
            public int Probability { get; }

            public CardData.Skill.Type Effect { get; }
            public int ScoreBoost { get; }
            public int ComboBoost { get; }

            public Skill(CardData.Skill data, int level) {
                this.data = data;
                this.level = level;

                Interval = this.data.interval;
                Duration = this.data.duration;
                Probability = this.data.probability + this.level;
                if (this.level > 10) {
                    Probability += (this.level - 10) * 5;
                }

                Effect = this.data.effectId;
                switch (this.data.effectId) {
                    case CardData.Skill.Type.scoreUp:
                        ScoreBoost = this.data.value[0];
                        break;
                    case CardData.Skill.Type.multiUp:
                        ScoreBoost = this.data.value[0];
                        break;
                    case CardData.Skill.Type.overClock:
                        ScoreBoost = this.data.value[0];
                        break;
                    case CardData.Skill.Type.doubleBoost:
                        ScoreBoost = this.data.value[0];
                        ComboBoost = this.data.value[1];
                        break;
                    case CardData.Skill.Type.comboBonus:
                        ComboBoost = this.data.value[0];
                        break;
                    case CardData.Skill.Type.overRondo:
                        ComboBoost = this.data.value[0];
                        break;
                }
            }
        }

        public class CenterEffect {
            private CardData.CenterEffect data;

            public CenterEffect(CardData.CenterEffect data) {
                this.data = data;
            }

            public double ActivationBoost(Types cardType) {
                switch (data.attribute) {
                    case CardData.CenterEffect.Type.skillBoost:
                        if (data.idolType == cardType) {
                            return (double)data.value / 100;
                        }
                        break;
                }
                return 0;
            }

            public Vector3 GetBoost(Types songType, Unit unit) {
                Vector3 boost = new Vector3(0);
                if (data == null) {
                    return boost;
                }
                switch (data.specificIdolType) {
                    case Types.Princess:
                        if (!unit.IsMonocolour(data.specificIdolType)) {
                            return boost;
                        }
                        break;
                    case Types.Fairy:
                        if (!unit.IsMonocolour(data.specificIdolType)) {
                            return boost;
                        }
                        break;
                    case Types.Angel:
                        if (!unit.IsMonocolour(data.specificIdolType)) {
                            return boost;
                        }
                        break;
                    case Types.All:
                        if (!unit.IsTricolour()) {
                            return boost;
                        }
                        break;
                }
                switch (data.attribute) {
                    case CardData.CenterEffect.Type.vocalUp:
                        boost += new Vector3((float)data.value / 100, 0, 0);
                        break;
                    case CardData.CenterEffect.Type.danceUp:
                        boost += new Vector3(0, (float)data.value / 100, 0);
                        break;
                    case CardData.CenterEffect.Type.visualUp:
                        boost += new Vector3(0, 0, (float)data.value / 100);
                        break;
                    case CardData.CenterEffect.Type.allUp:
                        boost += new Vector3((float)data.value / 100);
                        break;
                }
                if (data.songType == songType || data.songType == Types.All) {
                    switch (data.attribute2) {
                        case CardData.CenterEffect.Type.vocalUp:
                            boost += new Vector3((float)data.value2 / 100, 0, 0);
                            break;
                        case CardData.CenterEffect.Type.danceUp:
                            boost += new Vector3(0, (float)data.value2 / 100, 0);
                            break;
                        case CardData.CenterEffect.Type.visualUp:
                            boost += new Vector3(0, 0, (float)data.value2 / 100);
                            break;
                        case CardData.CenterEffect.Type.allUp:
                            boost += new Vector3((float)data.value2 / 100);
                            break;
                    }
                }
                return boost;
            }
        }

        public Card(CardData data, Idol idol, int masterRank, int skillLevel) {
            this.data = data;
            this.idol = idol;
            
            this.masterRank = masterRank;
            this.skillLevel = skillLevel;

            ID = data.resourceId;
            Type = data.idolType;
            if (Type != Types.EX) {
                Color = Color.Parse("#" + idol.colour);
            } else {
                Color = Colors.LimeGreen;
            }
            Skills = new List<Skill>();
            if (this.data.skill != null) {
                foreach (CardData.Skill skill in this.data.skill) {
                    Skills.Add(new Skill(skill, skillLevel));
                }
            }
            if (this.data.centerEffect != null) {
                Center = new CenterEffect(this.data.centerEffect);
            }

            // https://storage.matsurihi.me/mltd/card/017kth0054_0_a.png
            // https://storage.matsurihi.me/mltd/icon_l/017kth0054_1.png
        }

        public Vector3 SplitAppeal() {
            return new Vector3(data.vocalMaxAwakened + data.vocalMasterBonus * masterRank,
                    data.danceMaxAwakened + data.danceMasterBonus * masterRank,
                    data.visualMaxAwakened + data.visualMasterBonus * masterRank);
        }

        public int TotalAppeal(Types songType) {
            Vector3 splitAppeal = SplitAppeal();
            if (Type == songType || Type == Types.EX || songType == Types.All) {
                splitAppeal *= 1.3f;
            }
            return (int)(splitAppeal.X + splitAppeal.Y + splitAppeal.Z);
        }
    }
}
