using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Numerics;
using TheaterDaysScore.JsonModels;

namespace TheaterDaysScore.Models {
    public class Card {
        private CardData data;
        private IdolData idol;

        public enum Categories {
            PermanentGasha,
            LimitedGasha,
            SHSGasha,
            Fes,
            PST,
            MiliColle,
            PremiumPickup,
            Anniversary,
            Other
        }

        public bool IsHeld { get; set; }

        public int MasterRank { get; set; }
        public List<int> MasterRanks { get; set; }

        private int skillLevel;
        public int SkillLevel {
            get => skillLevel;
            set {
                skillLevel = value;
                if (Skills != null) {
                    foreach (Skill s in Skills) {
                        s.Level = skillLevel;
                    }
                }
            }
        }
        public List<int> SkillLevels { get; set; }

        public string ID { get; }
        public Types Type { get; }
        public CardData.Rarities Rarity { get; }
        public Categories Category { get; }
        public Color Color { get; }
        public List<Skill> Skills { get; }
        public CardData.Skill.Type SkillType { get; }
        public CenterEffect Center { get; }
        public CardData.CenterEffect.Type CenterType { get; }
        public CardData.CenterEffect.Type CenterType2 { get; }

        public class Skill {
            private CardData.Skill data;
            public int Level { get; set; }

            public int Interval { get; }
            public int Duration { get; }

            private int baseProbability;
            public int Probability { get {
                    if (Level <= 10) {
                        return baseProbability + Level;
                    }
                    return baseProbability + 10 + (Level - 10) * 5;
                }
            }

            public CardData.Skill.Type Effect { get; }
            public int ScoreBoost { get; }
            public int ComboBoost { get; }

            public Skill(CardData.Skill data, int level) {
                this.data = data;
                Level = level;

                Interval = this.data.interval;
                Duration = this.data.duration;
                baseProbability = this.data.probability;

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
                    case CardData.Skill.Type.doubleEffect:
                        ScoreBoost = this.data.value[0];
                        ComboBoost = this.data.value[0];
                        break;
                    case CardData.Skill.Type.comboBonus:
                        ComboBoost = this.data.value[0];
                        break;
                    case CardData.Skill.Type.multiBonus:
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

            public int ActivationBoost(Types cardType, Unit unit) {
                switch (data.attribute) {
                    case CardData.CenterEffect.Type.skillBoost:
                        if (data.idolType == cardType) {
                            return data.value;
                        }
                        break;
                }
                switch (data.attribute2) {
                    case CardData.CenterEffect.Type.skillBoost:
                        switch (data.specificIdolType) {
                            case Types.Princess:
                                if (unit.IsMonocolour(data.specificIdolType)) {
                                    return data.value2;
                                }
                                break;
                            case Types.Fairy:
                                if (unit.IsMonocolour(data.specificIdolType)) {
                                    return data.value2;
                                }
                                break;
                            case Types.Angel:
                                if (unit.IsMonocolour(data.specificIdolType)) {
                                    return data.value2;
                                }
                                break;
                            case Types.All:
                                if (unit.IsTricolour()) {
                                    return data.value2;
                                }
                                break;
                        }
                        break;
                }
                return 0;
            }

            public Vector3 GetBoost(Types songType, Types cardType, Unit unit) {
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
                switch (data.idolType) {
                    case Types.Princess:
                        if (cardType != Types.Princess && cardType != Types.EX) {
                            return boost;
                        }
                        break;
                    case Types.Fairy:
                        if (cardType != Types.Fairy && cardType != Types.EX) {
                            return boost;
                        }
                        break;
                    case Types.Angel:
                        if (cardType != Types.Angel && cardType != Types.EX) {
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

        public Card(CardData data, IdolData idol, bool isHeld, int masterRank, int skillLevel) {
            this.data = data;
            this.idol = idol;
            IsHeld = isHeld;

            Rarity = data.rarity;
            
            MasterRank = masterRank;
            MasterRanks = new List<int>();
            for (int x = 0; x <= this.data.masterRankMax; x++) {
                MasterRanks.Add(x);
            }
            SkillLevel = skillLevel;
            SkillLevels = new List<int>();
            for (int x = 0; x <= this.data.skillLevelMax; x++) {
                SkillLevels.Add(x);
            }

            ID = this.data.resourceId;
            Type = this.data.idolType;
            
            switch (this.data.category) {
                case "normal": // Free with account
                    Category = Categories.Other;
                    break;
                case "gasha0": // Perm
                    Category = Categories.PermanentGasha;
                    break;
                case "gasha1": // Lim
                    Category = Categories.LimitedGasha;
                    break;
                case "gasha2": // Fes
                    Category = Categories.Fes;
                    break;
                case "gasha4": // Premium Pickup SR
                    Category = Categories.PremiumPickup;
                    break;
                case "gasha5": // Second Hairstyle
                    Category = Categories.SHSGasha;
                    break;
                case "event0": // MiliColle SR
                    Category = Categories.MiliColle;
                    break;
                case "event1": // Theater
                    Category = Categories.PST;
                    break;
                case "event2": // Tour
                    Category = Categories.PST;
                    break;
                case "event3": // Anniversary
                    Category = Categories.Anniversary;
                    break;
                case "event4": // Voting runner-up
                    Category = Categories.Other;
                    break;
                case "event5": // MiliColle R
                    Category = Categories.MiliColle;
                    break;
                case "other": // Idol Heroes, Leon
                    Category = Categories.Other;
                    break;
                default: // Just in case
                    Category = Categories.Other;
                    break;
            }

            if (Type != Types.EX) {
                Color = Color.Parse("#" + this.idol.colour);
            } else {
                Color = Colors.LimeGreen;
            }
            Skills = new List<Skill>();
            if (this.data.skill != null) {
                foreach (CardData.Skill skill in this.data.skill) {
                    Skills.Add(new Skill(skill, skillLevel));
                    SkillType = skill.effectId;
                }
            } else {
                SkillType = CardData.Skill.Type.none;
            }
            if (this.data.centerEffect != null) {
                Center = new CenterEffect(this.data.centerEffect);
                CenterType = this.data.centerEffect.attribute;
                CenterType2 = this.data.centerEffect.attribute2;
            } else {
                CenterType = CardData.CenterEffect.Type.none;
                CenterType2 = CardData.CenterEffect.Type.none;
            }

            // https://storage.matsurihi.me/mltd/card/017kth0054_0_a.png
            // https://storage.matsurihi.me/mltd/icon_l/017kth0054_1.png
        }

        public Card Copy() {
            return new Card(this.data, this.idol, IsHeld, MasterRank, SkillLevel);
        }

        public Vector3 SplitAppeal() {
            return new Vector3(data.vocalMaxAwakened + data.vocalMasterBonus * MasterRank,
                    data.danceMaxAwakened + data.danceMasterBonus * MasterRank,
                    data.visualMaxAwakened + data.visualMasterBonus * MasterRank);
        }

        private Vector3 floor(Vector3 input) {
            return new Vector3((float)Math.Floor(input.X), (float)Math.Floor(input.Y), (float)Math.Floor(input.Z));
        }

        public float TotalAppeal(Types songType, Calculator.BoostType eventType) {
            Vector3 eventBoost = Vector3.Zero;
            switch (eventType) {
                case Calculator.BoostType.vocal:
                    eventBoost = new Vector3(1.2f, 0, 0);
                    break;
                case Calculator.BoostType.dance:
                    eventBoost = new Vector3(0, 1.2f, 0);
                    break;
                case Calculator.BoostType.visual:
                    eventBoost = new Vector3(0, 0, 1.2f);
                    break;
            }

            Vector3 splitAppeal = SplitAppeal();
            Vector3 splitAppealType = floor(splitAppeal * 0.3f);
            if (Type != songType && Type != Types.EX && songType != Types.All) {
                splitAppealType = Vector3.Zero;
            }
            Vector3 splitAppealEvent = floor(splitAppeal * eventBoost);
            return Vector3.Dot(new Vector3(1), floor(splitAppeal / 2) + floor(splitAppealType / 2) + floor(splitAppealEvent / 2));
        }
    }
}
