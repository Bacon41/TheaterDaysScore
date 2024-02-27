using System;
using System.Collections.Generic;
using System.Linq;
using TheaterDaysScore.JsonModels;
using TheaterDaysScore.Services;

namespace TheaterDaysScore.Models {
    public class Unit {
        public Card Guest { get; }
        public Card Center { get; }
        public Card[] Members { get; }

        private string[] memberIDs;

        private Random rand;

        public Unit(string guestID, int guestRank, string centerID, string member1ID, string member2ID, string member3ID, string member4ID) {
            Guest = Database.DB.GetCard(guestID).Copy();
            Guest.MasterRank = guestRank;
            Center = Database.DB.GetCard(centerID);

            memberIDs = new string[5] { member1ID, member2ID, centerID, member3ID, member4ID };
            Members = new Card[5];
            for (int x = 0; x < 5; x++) {
                Members[x] = Database.DB.GetCard(memberIDs[x]);
            }

            rand = new Random();
        }

        private double ActivationOdds(Types songType, Types cardType, Card.Skill skill) {
            int extraRate = 0;
            if (Guest.Center != null) {
                extraRate += Guest.Center.GetActivationIncrease(songType, cardType, this);
            }
            if (Center.Center != null) {
                extraRate += Center.Center.GetActivationIncrease(songType, cardType, this);
            }
            if (ComboUpCount() >= 2) {
                extraRate += skill.FusionComboRate;
            }
            return skill.Probability * ((100f + extraRate) / 100);
        }

        public IActivation GetActivations(Song song, bool alwaysActivate = false) {
            double length = song.TickToTime(song.LastNoteTick) - song.SkillStartTime;
            Activation activation = new Activation(length);

            foreach (Card card in Members) {
                foreach (Card.Skill skill in card.Skills) {
                    double activationThreshold = ActivationOdds(song.Type, card.Type, skill);
                    if (alwaysActivate) {
                        activationThreshold = 100;
                    }

                    int start = skill.Interval;
                    while (start < length) {
                        if (!song.IsDuringAppeal(start)) {
                            if (rand.NextDouble() * 100 < activationThreshold) {
                                activation.AddInterval(start, song.Type, card.Type, skill, this);
                            }
                        }
                        start += skill.Interval;
                    }
                }
            }

            return activation;
        }

        public interface IActivation {
            public double ScoreBoostAt(double time, Song.Note.Accuracy tapJudgement);
            public double ComboBoostAt(double time);
            public double HoldOver(double startTime, double length);
            public Song.Note.Accuracy AccuracyAt(double time, Song.Note.Accuracy tapJudgement);
            public bool ComboProtectedAt(double time, Song.Note.Accuracy tapJudgement);
        }

        private class Activation : IActivation {
            private int[] scoreUpPerfect;
            private int[] scoreUpGreat;
            private int[] scoreUpGood;
            private int[] comboUp;
            private int[] boostScorePerfect;
            private int[] boostScoreGreat;
            private int[] boostScoreGood;
            private int[] boostCombo;
            private int[] effectScore;
            private int[] effectCombo;
            private Song.Note.Accuracy[] lowestJudgementBoosted;
            private Song.Note.Accuracy[] lowestComboProtection;

            public Activation(double songLen) {
                int numSeconds = (int)Math.Ceiling(songLen) + 1;
                scoreUpPerfect = new int[numSeconds];
                scoreUpGreat = new int[numSeconds];
                scoreUpGood = new int[numSeconds];
                comboUp = new int[numSeconds];
                boostScorePerfect = new int[numSeconds];
                boostScoreGreat = new int[numSeconds];
                boostScoreGood = new int[numSeconds];
                boostCombo = new int[numSeconds];
                effectScore = new int[numSeconds];
                effectCombo = new int[numSeconds];
                lowestJudgementBoosted = new Song.Note.Accuracy[numSeconds];
                lowestComboProtection = new Song.Note.Accuracy[numSeconds];
                for (int x = 0; x < numSeconds; x++) {
                    // Good is the lowest score to not break combo by default
                    lowestComboProtection[x] = Song.Note.Accuracy.good;
                }
            }

            public void AddInterval(int start, Types songType, Types cardType, Card.Skill skill, Unit unit) {
                for (int x = start; x < scoreUpPerfect.Length && x < start + skill.Duration; x++) {
                    // Score/Combo
                    if (skill.Effect == CardData.Skill.Type.doubleBoost) {
                        if (skill.ScoreBoost > boostScorePerfect[x]) {
                            if (skill.LowestScoreBoostJudgement >= Song.Note.Accuracy.perfect) {
                                boostScorePerfect[x] = skill.ScoreBoost + unit.GetCenterDoubleBoost(songType, cardType);
                            }
                        }
                        if (skill.ScoreBoost > boostScoreGreat[x]) {
                            if (skill.LowestScoreBoostJudgement >= Song.Note.Accuracy.great) {
                                boostScoreGreat[x] = skill.ScoreBoost + unit.GetCenterDoubleBoost(songType, cardType);
                            }
                        }
                        if (skill.ScoreBoost > boostScoreGood[x]) {
                            if (skill.LowestScoreBoostJudgement >= Song.Note.Accuracy.good) {
                                boostScoreGood[x] = skill.ScoreBoost + unit.GetCenterDoubleBoost(songType, cardType);
                            }
                        }
                        if (skill.ComboBoost > boostCombo[x]) {
                            boostCombo[x] = skill.ComboBoost + unit.GetCenterDoubleBoost(songType, cardType);
                        }
                    } else if (skill.Effect == CardData.Skill.Type.doubleEffect) {
                        if (skill.ScoreBoost > effectScore[x]) {
                            effectScore[x] = skill.ScoreBoost + unit.GetCenterDoubleEffect(songType, cardType);
                        }
                        if (skill.ComboBoost > effectCombo[x]) {
                            effectCombo[x] = skill.ComboBoost + unit.GetCenterDoubleEffect(songType, cardType);
                        }
                    } else if (skill.Effect == CardData.Skill.Type.fusionScore) {
                        int scoreBoost = skill.ScoreBoost;
                        Song.Note.Accuracy lowestJudgement = skill.LowestScoreBoostJudgement;
                        if (unit.ScoreUpCount() >= 2) {
                            scoreBoost = skill.FusionScoreBoost;
                            lowestJudgement = skill.LowestFusionScoreBoostJudgement;
                        }
                        if (lowestJudgement >= Song.Note.Accuracy.perfect) {
                            if (scoreBoost > scoreUpPerfect[x]) {
                                scoreUpPerfect[x] = scoreBoost;
                            }
                        }
                        if (lowestJudgement >= Song.Note.Accuracy.great) {
                            if (scoreBoost > scoreUpGreat[x]) {
                                scoreUpGreat[x] = scoreBoost;
                            }
                        }
                        if (lowestJudgement >= Song.Note.Accuracy.good) {
                            if (scoreBoost > scoreUpGood[x]) {
                                scoreUpGood[x] = scoreBoost;
                            }
                        }
                    } else {
                        if (skill.LowestScoreBoostJudgement >= Song.Note.Accuracy.perfect) {
                            if (skill.ScoreBoost > scoreUpPerfect[x]) {
                                scoreUpPerfect[x] = skill.ScoreBoost;
                            }
                        }
                        if (skill.LowestScoreBoostJudgement >= Song.Note.Accuracy.great) {
                            if (skill.ScoreBoost > scoreUpGreat[x]) {
                                scoreUpGreat[x] = skill.ScoreBoost;
                            }
                        }
                        if (skill.LowestScoreBoostJudgement >= Song.Note.Accuracy.good) {
                            if (skill.ScoreBoost > scoreUpGood[x]) {
                                scoreUpGood[x] = skill.ScoreBoost;
                            }
                        }
                        if (skill.ComboBoost > comboUp[x]) {
                            comboUp[x] = skill.ComboBoost;
                        }
                    }
                    // Judgement
                    if (skill.Effect == CardData.Skill.Type.perfectLock) {
                        if (skill.LowestJudgementBoost > lowestJudgementBoosted[x]) {
                            lowestJudgementBoosted[x] = skill.LowestJudgementBoost;
                        }
                    } else if (skill.Effect == CardData.Skill.Type.fusionScore) {
                        if (unit.IsFused()) {
                            if (skill.LowestJudgementBoost > lowestJudgementBoosted[x]) {
                                lowestJudgementBoosted[x] = skill.LowestJudgementBoost;
                            }
                        }
                    } else if (skill.Effect == CardData.Skill.Type.fusionCombo) {
                        if (unit.IsFused()) {
                            if (skill.LowestJudgementBoost > lowestJudgementBoosted[x]) {
                                lowestJudgementBoosted[x] = skill.LowestJudgementBoost;
                            }
                        }
                    }
                    // Combo protection
                    if (skill.Effect == CardData.Skill.Type.comboGuard) {
                        if (skill.LowestComboProtect > lowestComboProtection[x]) {
                            lowestComboProtection[x] = skill.LowestComboProtect;
                        }
                    }
                }
            }

            public double ScoreBoostAt(double time, Song.Note.Accuracy tapJudgement) {
                int currentSecond = (int)Math.Floor(time);
                int rateUp = 0;
                if (tapJudgement == Song.Note.Accuracy.perfect) {
                    if (scoreUpPerfect[currentSecond] > 0) {
                        rateUp += scoreUpPerfect[currentSecond] + effectScore[currentSecond];
                    }
                    if (boostScorePerfect[currentSecond] > 0) {
                        rateUp += boostScorePerfect[currentSecond] + effectScore[currentSecond];
                    }
                } else if (tapJudgement == Song.Note.Accuracy.great) {
                    if (scoreUpGreat[currentSecond] > 0) {
                        rateUp += scoreUpGreat[currentSecond] + effectScore[currentSecond];
                    }
                    if (boostScoreGreat[currentSecond] > 0) {
                        rateUp += boostScoreGreat[currentSecond] + effectScore[currentSecond];
                    }
                } else if (tapJudgement == Song.Note.Accuracy.good) {
                    if (scoreUpGood[currentSecond] > 0) {
                        rateUp += scoreUpGood[currentSecond] + effectScore[currentSecond];
                    }
                    if (boostScoreGood[currentSecond] > 0) {
                        rateUp += boostScoreGood[currentSecond] + effectScore[currentSecond];
                    }
                }
                return (100.0 + rateUp) / 100;
            }

            public double ComboBoostAt(double time) {
                int currentSecond = (int)Math.Floor(time);
                int rateUp = 0;
                if (comboUp[currentSecond] > 0) {
                    rateUp += comboUp[currentSecond] + effectCombo[currentSecond];
                }
                if (boostCombo[currentSecond] > 0) {
                    rateUp += boostCombo[currentSecond] + effectCombo[currentSecond];
                }
                return (100.0 + 3 * rateUp) / 100;
            }

            public double HoldOver(double startTime, double length) {
                int startScore = (int)Math.Floor(startTime);
                int endScore = (int)Math.Ceiling(startTime + length);
                double holdScore = 0;
                for (int x = startScore; x < endScore; x++) {
                    holdScore += ScoreBoostAt(x, Song.Note.Accuracy.perfect) * (Math.Min(startTime + length, x + 1) - Math.Max(startTime, x));
                }
                return holdScore;
            }

            public Song.Note.Accuracy AccuracyAt(double time, Song.Note.Accuracy tapJudgement) {
                int currentSecond = (int)Math.Floor(time);
                if (lowestJudgementBoosted[currentSecond] >= tapJudgement) {
                    return Song.Note.Accuracy.perfect;
                }
                return tapJudgement;
            }

            public bool ComboProtectedAt(double time, Song.Note.Accuracy tapJudgement) {
                int currentSecond = (int)Math.Floor(time);
                if (lowestComboProtection[currentSecond] >= tapJudgement) {
                    return true;
                }
                return false;
            }
        }

        public int GetCenterDoubleBoost(Types songType, Types cardType) {
            int boost = 0;
            boost += Center.Center.GetDoubleBoostIncrease(songType, cardType, this);
            boost += Center.Center.GetDoubleBoostIncrease(songType, cardType, this);
            return boost;
        }

        public int GetCenterDoubleEffect(Types songType, Types cardType) {
            int boost = 0;
            boost += Center.Center.GetDoubleEffectIncrease(songType, cardType, this);
            boost += Center.Center.GetDoubleEffectIncrease(songType, cardType, this);
            return boost;
        }

        public bool IsTricolour() {
            int unitTypes = Members.Select(card => card.Type).Distinct().Count();
            return unitTypes >= 3;
        }

        public bool IsMonocolour(Types matchType) {
            if (matchType == Types.All)
                return IsTricolour();

            int unitTypes = Members.Where(card => card.Type == matchType).Count();
            return unitTypes == 5;
        }

        public int ScoreUpCount() {
            int scoreCards = Members.Where(card =>
                card.SkillType == CardData.Skill.Type.scoreBonus ||
                card.SkillType == CardData.Skill.Type.multiUp ||
                card.SkillType == CardData.Skill.Type.overClock ||
                card.SkillType == CardData.Skill.Type.fusionScore ||
                card.SkillType == CardData.Skill.Type.doubleBoost).Count();
            return scoreCards;
        }

        public int ComboUpCount() {
            int comboCards = Members.Where(card =>
                card.SkillType == CardData.Skill.Type.comboBonus ||
                card.SkillType == CardData.Skill.Type.multiUp ||
                card.SkillType == CardData.Skill.Type.overRondo ||
                card.SkillType == CardData.Skill.Type.fusionCombo ||
                card.SkillType == CardData.Skill.Type.doubleBoost).Count();
            return comboCards;
        }

        public bool IsFused() {
            int fusionScores = Members.Where(card => card.SkillType == CardData.Skill.Type.fusionScore).Count();
            int fusionCombos = Members.Where(card => card.SkillType == CardData.Skill.Type.fusionCombo).Count();
            return fusionScores >= 1 && fusionCombos >= 1;
        }

        public List<Card> TopSupport(Types songType, Calculator.BoostType eventType) {
            return Database.DB.TopAppeal(songType, eventType, 10, memberIDs);
        }
    }
}
