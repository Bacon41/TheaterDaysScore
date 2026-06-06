using Avalonia.Media;
using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
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
            SHSGacha,
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
        public int SortOrder { get; }
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

        public Bitmap? TypeImage { get; }
        public string SkillTypeShort { get {
                switch (SkillType) {
                    case CardData.Skill.Type.none:
                        return "";
                    case CardData.Skill.Type.scoreBonus:
                        return "SB";
                    case CardData.Skill.Type.comboBonus:
                        return "CB";
                    case CardData.Skill.Type.healer:
                        return "LI";
                    case CardData.Skill.Type.lifeGuard:
                        return "LG";
                    case CardData.Skill.Type.comboGuard:
                        return "CG";
                    case CardData.Skill.Type.perfectLock:
                        return "JB";
                    case CardData.Skill.Type.doubleBoost:
                        return "DB";
                    case CardData.Skill.Type.multiUp:
                        return "MU";
                    case CardData.Skill.Type.multiBonus:
                        return "MB";
                    case CardData.Skill.Type.overClock:
                        return "OC";
                    case CardData.Skill.Type.overRondo:
                        return "OR";
                    case CardData.Skill.Type.doubleEffect:
                        return "DE";
                    case CardData.Skill.Type.fusionScore:
                        return "FS";
                    case CardData.Skill.Type.fusionCombo:
                        return "FC";
                    case CardData.Skill.Type.overEffect:
                        return "OE";
                    case CardData.Skill.Type.pureBoost:
                        return "PB";
                    default:
                        return "";
                }
            } }

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
            public int TypeBonus { get; }

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
                        LowestScoreBoostJudgement = LowestJudgementInRange(this.data.evaluationTypes[0]);
                        break;
                    case CardData.Skill.Type.pureBoost:
                        ScoreBoost = this.data.values[0];
                        ComboBoost = this.data.values[1];
                        TypeBonus = this.data.values[2];
                        LowestScoreBoostJudgement = LowestJudgementInRange(this.data.evaluationTypes[0]);
                        break;
                    case CardData.Skill.Type.doubleEffect:
                        ScoreBoost = this.data.values[0];
                        ComboBoost = this.data.values[0];
                        break;
                    case CardData.Skill.Type.overEffect:
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

            // Percentage increase of given card's skill activation increase
            public int GetActivationIncrease(Types songType, Types cardType, Unit unit) {
                // Ensure unit conditions are met
                if (data.specificIdolType != Types.None && !unit.IsMonocolour(data.specificIdolType)) {
                    return 0;
                }
                // Ensure boost applies to given card
                if (!Calculator.CompareType(data.idolType, cardType)) {
                    return 0;
                }
                // Ensure song condition is met
                if (!Calculator.CompareType(data.songType, songType)) {
                    return 0;
                }
                // Search for an activation boost effect
                if (data.attributes != null) {
                    for (int x = 0; x < data.attributes.Length; x++) {
                        if (data.attributes[x] == CardData.CenterEffect.Type.skillActivationUp) {
                            return data.values[x];
                        }
                    }
                }
                return 0;
            }

            // Percentage to increase each stat of given card
            public Vector3 GetStatIncrease(Types songType, Types cardType, Unit unit) {
                Vector3 boost = Vector3.Zero;
                if (data == null) {
                    return boost;
                }
                // Ensure unit conditions are met
                if (data.specificIdolType != Types.None && !unit.IsMonocolour(data.specificIdolType)) {
                    return boost;
                }
                // Ensure boost applies to given card
                if (!Calculator.CompareType(data.idolType, cardType)) {
                    return boost;
                }
                // Add in base effect
                if (data.attributes != null && data.attributes.Length > 0) {
                    boost += AttributeVector(data.attributes[0], data.values[0]);
                }
                // Add in secondary effect (with additional restrictions)
                if (data.attributes != null && data.attributes.Length > 1) {
                    if (Calculator.CompareType(data.songType, songType)) {
                        boost += AttributeVector(data.attributes[1], data.values[1]);
                    }
                }
                return boost;
            }

            // Percentage increase of given card's double boost strength
            public int GetDoubleBoostIncrease(Types songType, Types cardType, Unit unit) {
                // Ensure unit conditions are met
                if (data.specificIdolType != Types.None && !unit.IsMonocolour(data.specificIdolType)) {
                    return 0;
                }
                // Ensure boost applies to given card
                if (!Calculator.CompareType(data.idolType, cardType)) {
                    return 0;
                }
                // Ensure song condition is met
                if (!Calculator.CompareType(data.songType, songType)) {
                    return 0;
                }
                // Search for a double boost increase effect
                if (data.attributes != null) {
                    for (int x = 0; x < data.attributes.Length; x++) {
                        if (data.attributes[x] == CardData.CenterEffect.Type.boostSkillUp) {
                            return data.values[x];
                        }
                    }
                }
                return 0;
            }

            // Percentage increase of given card's double effect strength
            public int GetDoubleEffectIncrease(Types songType, Types cardType, Unit unit) {
                // Ensure unit conditions are met
                if (data.specificIdolType != Types.None && !unit.IsMonocolour(data.specificIdolType)) {
                    return 0;
                }
                // Ensure boost applies to given card
                if (!Calculator.CompareType(data.idolType, cardType)) {
                    return 0;
                }
                // Ensure song condition is met
                if (!Calculator.CompareType(data.songType, songType)) {
                    return 0;
                }
                // Search for a double effect increase effect
                if (data.attributes != null) {
                    for (int x = 0; x < data.attributes.Length; x++) {
                        if (data.attributes[x] == CardData.CenterEffect.Type.effectSkillUp) {
                            return data.values[x];
                        }
                    }
                }
                return 0;
            }

            // Convert given number to a percentage vector of each stat to increase
            private Vector3 AttributeVector(CardData.CenterEffect.Type type, float value) {
                switch (type) {
                    case CardData.CenterEffect.Type.vocalUp:
                        return new Vector3(value / 100, 0, 0);
                    case CardData.CenterEffect.Type.danceUp:
                        return new Vector3(0, value / 100, 0);
                    case CardData.CenterEffect.Type.visualUp:
                        return new Vector3(0, 0, value / 100);
                    case CardData.CenterEffect.Type.allUp:
                        return new Vector3(value / 100);
                    default: // Shouldn't happen
                        return Vector3.Zero;
                }
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
            SortOrder = this.data.id;
            Type = this.data.idolType;
            IdolID = this.data.idolId;

            switch (this.data.idolType) {
                case Types.Princess:
                    TypeImage = new Bitmap(AvaloniaLocator.Current.GetService<IAssetLoader>().Open(new Uri($"avares://TheaterDaysScore/Assets/princess.png")));
                    break;
                case Types.Fairy:
                    TypeImage = new Bitmap(AvaloniaLocator.Current.GetService<IAssetLoader>().Open(new Uri($"avares://TheaterDaysScore/Assets/fairy.png")));
                    break;
                case Types.Angel:
                    TypeImage = new Bitmap(AvaloniaLocator.Current.GetService<IAssetLoader>().Open(new Uri($"avares://TheaterDaysScore/Assets/angel.png")));
                    break;
            }
            
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
                    Category = Categories.SHSGacha;
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

        public Vector3 SplitAppeal(bool forceMax) {
            int masterRank = MasterRank;
            if (forceMax) {
                masterRank = data.masterRankMax;
            }
            return new Vector3(data.parameters.vocal.afterAwakened.max + data.parameters.vocal.masterBonus * masterRank,
                    data.parameters.dance.afterAwakened.max + data.parameters.dance.masterBonus * masterRank,
                    data.parameters.visual.afterAwakened.max + data.parameters.visual.masterBonus * masterRank);
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

            Vector3 splitAppeal = SplitAppeal(false);
            Vector3 splitAppealType = floor(splitAppeal * 0.3f);
            if (!Calculator.CompareType(Type, songType)) {
                splitAppealType = Vector3.Zero;
            }
            Vector3 splitAppealEvent = floor(splitAppeal * eventBoost);
            return Vector3.Dot(new Vector3(1), floor(splitAppeal / 2) + floor(splitAppealType / 2) + floor(splitAppealEvent / 2));
        }
    }
}
