using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Numerics;
using TheaterDaysScore.JsonModels;

namespace TheaterDaysScore.Models {
    public class Card {
        private CardData data;
        private Idol idol;

        public enum Categories {
            PermanentGacha,
            LimitedGacha,
            SHSGasha,
            Fes,
            Linkage,
            PST,
            MiliColle,
            PremiumPickup,
            Anniversary,
            PR,
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
        public int IdolID { get; }
        public Types Type { get; }
        public CardData.Rarities Rarity { get; }
        public Categories Category { get; }
        public Color Color { get; }
        public List<Skill> Skills { get; }
        public CardData.Skill.Type SkillType { get; }
        public CenterEffect Center { get; }
        public CardData.CenterEffect.Type CenterBoostType { get; }
        public CardData.CenterEffect.Type CenterBoostType2 { get; }
        public Types CenterReqType { get; }

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
            public Song.Note.Accuracy LowestScoreBoostJudgement { get; }
            public int FusionScoreBoost { get; }
            public Song.Note.Accuracy LowestFusionScoreBoostJudgement { get; }
            public int ComboBoost { get; }
            public int FusionComboRate { get; }
            public Song.Note.Accuracy LowestJudgementBoost { get; }
            public Song.Note.Accuracy LowestComboProtect { get; }

            public Skill(CardData.Skill data, int level) {
                this.data = data;
                Level = level;

                Interval = this.data.interval;
                Duration = this.data.duration;
                baseProbability = this.data.probability;

                LowestJudgementBoost = Song.Note.Accuracy.perfect;
                LowestComboProtect = Song.Note.Accuracy.good;

                Effect = this.data.effectId;
                switch (this.data.effectId) {
                    case CardData.Skill.Type.scoreBonus:
                        ScoreBoost = this.data.values[0];
                        LowestScoreBoostJudgement = LowestJudgementInRange(this.data.evaluationTypes[0]);
                        break;
                    case CardData.Skill.Type.multiUp:
                        ScoreBoost = this.data.values[0];
                        break;
                    case CardData.Skill.Type.overClock:
                        ScoreBoost = this.data.values[0];
                        break;
                    case CardData.Skill.Type.fusionScore:
                        ScoreBoost = this.data.values[0];
                        LowestScoreBoostJudgement = LowestJudgementInRange(this.data.evaluationTypes[0]);
                        FusionScoreBoost = this.data.values[1];
                        LowestFusionScoreBoostJudgement = LowestJudgementInRange(this.data.evaluationTypes[1]);
                        LowestJudgementBoost = LowestJudgementInRange(this.data.evaluationTypes[2]);
                        break;
                    case CardData.Skill.Type.comboBonus:
                        ComboBoost = this.data.values[0];
                        break;
                    case CardData.Skill.Type.multiBonus:
                        ComboBoost = this.data.values[0];
                        break;
                    case CardData.Skill.Type.overRondo:
                        ComboBoost = this.data.values[0];
                        break;
                    case CardData.Skill.Type.fusionCombo:
                        ComboBoost = this.data.values[0];
                        FusionComboRate = this.data.values[1];
                        break;
                    case CardData.Skill.Type.doubleBoost:
                        ScoreBoost = this.data.values[0];
                        ComboBoost = this.data.values[1];
                        break;
                    case CardData.Skill.Type.doubleEffect:
                        ScoreBoost = this.data.values[0];
                        ComboBoost = this.data.values[0];
                        break;
                    case CardData.Skill.Type.perfectLock:
                        LowestJudgementBoost = LowestJudgementInRange(this.data.evaluationTypes[0]);
                        break;
                    case CardData.Skill.Type.comboGuard:
                        LowestComboProtect = LowestJudgementInRange(this.data.evaluationTypes[0]);
                        break;
                }
            }

            private Song.Note.Accuracy LowestJudgementInRange(CardData.Skill.Evaluations evalRange) {
                switch (evalRange) {
                    case CardData.Skill.Evaluations.perfect:
                        return Song.Note.Accuracy.perfect;
                    case CardData.Skill.Evaluations.perfectGreat:
                        return Song.Note.Accuracy.great;
                    case CardData.Skill.Evaluations.great:
                        return Song.Note.Accuracy.great;
                    case CardData.Skill.Evaluations.perfectGreatGood:
                        return Song.Note.Accuracy.good;
                    case CardData.Skill.Evaluations.greatGood:
                        return Song.Note.Accuracy.good;
                    case CardData.Skill.Evaluations.perfectGreatGoodFastSlow:
                        return Song.Note.Accuracy.fastSlow;
                    case CardData.Skill.Evaluations.greatGoodFastSlow:
                        return Song.Note.Accuracy.fastSlow;
                    default:
                        return Song.Note.Accuracy.miss;
                }
            }
        }

        public class CenterEffect {
            private CardData.CenterEffect data;

            public CenterEffect(CardData.CenterEffect data) {
                this.data = data;
            }

            public int ActivationBoost(Types cardType, Unit unit) {
                if (data.attributes != null && data.attributes.Length > 0) {
                    switch (data.attributes[0]) {
                        case CardData.CenterEffect.Type.skillActivationUp:
                            if (data.idolType == cardType) {
                                return data.values[0];
                            }
                            break;
                    }
                }
                if (data.attributes != null && data.attributes.Length > 1) {
                    switch (data.attributes[1]) {
                        case CardData.CenterEffect.Type.skillActivationUp:
                            switch (data.specificIdolType) {
                                case Types.Princess:
                                    if (unit.IsMonocolour(data.specificIdolType)) {
                                        return data.values[1];
                                    }
                                    break;
                                case Types.Fairy:
                                    if (unit.IsMonocolour(data.specificIdolType)) {
                                        return data.values[1];
                                    }
                                    break;
                                case Types.Angel:
                                    if (unit.IsMonocolour(data.specificIdolType)) {
                                        return data.values[1];
                                    }
                                    break;
                                case Types.All:
                                    if (unit.IsTricolour()) {
                                        return data.values[1];
                                    }
                                    break;
                            }
                            break;
                    }
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
                if (data.attributes != null && data.attributes.Length > 0) {
                    switch (data.attributes[0]) {
                        case CardData.CenterEffect.Type.vocalUp:
                            boost += new Vector3((float)data.values[0] / 100, 0, 0);
                            break;
                        case CardData.CenterEffect.Type.danceUp:
                            boost += new Vector3(0, (float)data.values[0] / 100, 0);
                            break;
                        case CardData.CenterEffect.Type.visualUp:
                            boost += new Vector3(0, 0, (float)data.values[0] / 100);
                            break;
                        case CardData.CenterEffect.Type.allUp:
                            boost += new Vector3((float)data.values[0] / 100);
                            break;
                    }
                }
                if (data.attributes != null && data.attributes.Length > 1) {
                    if (data.songType == songType || data.songType == Types.All) {
                        switch (data.attributes[1]) {
                            case CardData.CenterEffect.Type.vocalUp:
                                boost += new Vector3((float)data.values[1] / 100, 0, 0);
                                break;
                            case CardData.CenterEffect.Type.danceUp:
                                boost += new Vector3(0, (float)data.values[1] / 100, 0);
                                break;
                            case CardData.CenterEffect.Type.visualUp:
                                boost += new Vector3(0, 0, (float)data.values[1] / 100);
                                break;
                            case CardData.CenterEffect.Type.allUp:
                                boost += new Vector3((float)data.values[1] / 100);
                                break;
                        }
                    }
                }
                return boost;
            }
        }

        public Card(CardData data, Idol idol, bool isHeld, int masterRank, int skillLevel) {
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
            for (int x = 0; x <= this.data.skillLvMax; x++) {
                SkillLevels.Add(x);
            }

            ID = this.data.resourceId;
            Type = this.data.idolType;
            IdolID = this.data.idolId;
            
            switch (this.data.category) {
                case CardData.Categories.normal: // Free with account
                    Category = Categories.Other;
                    break;
                case CardData.Categories.perminantGacha:
                    Category = Categories.PermanentGacha;
                    break;
                case CardData.Categories.limitedGacha:
                    Category = Categories.LimitedGacha;
                    break;
                case CardData.Categories.fesGacha:
                    Category = Categories.Fes;
                    break;
                case CardData.Categories.linkageGacha:
                    Category = Categories.Linkage;
                    break;
                case CardData.Categories.premiumPickupGacha:
                    Category = Categories.PremiumPickup;
                    break;
                case CardData.Categories.secondHairstyleGacha:
                    Category = Categories.SHSGasha;
                    break;
                case CardData.Categories.millicolleSR:
                    Category = Categories.MiliColle;
                    break;
                case CardData.Categories.millicolleR:
                    Category = Categories.MiliColle;
                    break;
                case CardData.Categories.eventReward: // Theater
                    Category = Categories.PST;
                    break;
                case CardData.Categories.eventReward2: // Tour
                    Category = Categories.PST;
                    break;
                case CardData.Categories.anniversaryEvent:
                    Category = Categories.Anniversary;
                    break;
                case CardData.Categories.franchiseAnniversaryEvent: // 10th Crossing
                    Category = Categories.Anniversary;
                    break;
                case CardData.Categories.pr: // Promotion with company
                    Category = Categories.PR;
                    break;
                case CardData.Categories.theaterBoostExtra: // Voting runner-up
                    Category = Categories.Other;
                    break;
                case CardData.Categories.theaterChallengeExtra: // Voting runner-up
                    Category = Categories.Other;
                    break;
                case CardData.Categories.extraIdol: // Idol Heroes, Leon
                    Category = Categories.Other;
                    break;
                case CardData.Categories.extraIdolShop: // Idol Heroes, Leon
                    Category = Categories.Other;
                    break;
                case CardData.Categories.importedFromKRTW: // Chihaya, Miki, Kotoha
                    Category = Categories.Other;
                    break;
                default: // Just in case
                    Category = Categories.Other;
                    break;
            }

            if (Type != Types.EX) {
                Color = Color.Parse(this.idol.Colour);
            } else {
                Color = Colors.LimeGreen;
            }
            Skills = new List<Skill>();
            if (this.data.skills != null) {
                foreach (CardData.Skill skill in this.data.skills) {
                    Skills.Add(new Skill(skill, skillLevel));
                    SkillType = skill.effectId;
                }
            } else {
                SkillType = CardData.Skill.Type.none;
            }
            if (this.data.centerEffect != null && this.data.centerEffect.id != 0) {
                Center = new CenterEffect(this.data.centerEffect);
                CenterBoostType = this.data.centerEffect.attributes[0];
                if (this.data.centerEffect.attributes.Length > 1)
                    CenterBoostType2 = this.data.centerEffect.attributes[1];
                CenterReqType = this.data.centerEffect.specificIdolType;
            } else {
                CenterBoostType = CardData.CenterEffect.Type.none;
                CenterBoostType2 = CardData.CenterEffect.Type.none;
                CenterReqType = Types.None;
            }

            // https://storage.matsurihi.me/mltd/card/017kth0054_0_a.png
            // https://storage.matsurihi.me/mltd/icon_l/017kth0054_1.png
        }

        public Card Copy() {
            return new Card(this.data, this.idol, IsHeld, MasterRank, SkillLevel);
        }

        public Vector3 SplitAppeal() {
            return new Vector3(data.parameters.vocal.afterAwakened.max + data.parameters.vocal.masterBonus * MasterRank,
                    data.parameters.dance.afterAwakened.max + data.parameters.dance.masterBonus * MasterRank,
                    data.parameters.visual.afterAwakened.max + data.parameters.visual.masterBonus * MasterRank);
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
