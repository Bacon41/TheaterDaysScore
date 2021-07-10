using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using TheaterDaysScore.JsonModels;
using TheaterDaysScore.Models;
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

        private double ActivationOdds(Types cardType, Card.Skill skill) {
            int extraRate = 0;
            if (Guest.Center != null) {
                extraRate += Guest.Center.ActivationBoost(cardType, this);
            }
            if (Center.Center != null) {
                extraRate += Center.Center.ActivationBoost(cardType, this);
            }
            return skill.Probability * ((100f + extraRate) / 100);
        }

        public IActivation GetActivations(Song song, bool alwaysActivate = false) {
            Activation activation = new Activation(song.Length);

            foreach (Card card in Members) {
                foreach (Card.Skill skill in card.Skills) {
                    double activationThreshold = ActivationOdds(card.Type, skill);
                    if (alwaysActivate) {
                        activationThreshold = 100;
                    }

                    int start = skill.Interval;
                    while (start < song.Length) {
                        if (!song.IsDuringAppeal(start)) {
                            if (rand.NextDouble() * 100 < activationThreshold) {
                                activation.AddInterval(start, skill);
                            }
                        }
                        start += skill.Interval;
                    }
                }
            }

            return activation;
        }

        public interface IActivation {
            public double ScoreBoostAt(double time);
            public double ComboBoostAt(double time);
            public double HoldOver(double startTime, double length);
        }

        private class Activation : IActivation {
            private int[] scoreUp;
            private int[] comboUp;
            private int[] boostScore;
            private int[] boostCombo;
            private int[] effectScore;
            private int[] effectCombo;

            public Activation(double songLen) {
                int numSeconds = (int)Math.Ceiling(songLen) + 1;
                scoreUp = new int[numSeconds];
                comboUp = new int[numSeconds];
                boostScore = new int[numSeconds];
                boostCombo = new int[numSeconds];
                effectScore = new int[numSeconds];
                effectCombo = new int[numSeconds];
            }

            public void AddInterval(int start, Card.Skill skill) {
                for (int x = start; x < scoreUp.Length && x < start + skill.Duration; x++) {
                    if (skill.Effect == CardData.Skill.Type.doubleBoost) {
                        if (skill.ScoreBoost > boostScore[x]) {
                            boostScore[x] = skill.ScoreBoost;
                        }
                        if (skill.ComboBoost > boostCombo[x]) {
                            boostCombo[x] = skill.ComboBoost;
                        }
                    }
                    else if(skill.Effect == CardData.Skill.Type.doubleEffect) {
                        if (skill.ScoreBoost > effectScore[x]) {
                            effectScore[x] = skill.ScoreBoost;
                        }
                        if (skill.ComboBoost > effectCombo[x]) {
                            effectCombo[x] = skill.ComboBoost;
                        }
                    } else {
                        if (skill.ScoreBoost > scoreUp[x]) {
                            scoreUp[x] = skill.ScoreBoost;
                        }
                        if (skill.ComboBoost > comboUp[x]) {
                            comboUp[x] = skill.ComboBoost;
                        }
                    }
                }
            }

            public double ScoreBoostAt(double time) {
                int currentSecond = (int)Math.Floor(time);
                int rateUp = 0;
                if (scoreUp[currentSecond] > 0) {
                    rateUp += scoreUp[currentSecond] + effectScore[currentSecond];
                }
                if (boostScore[currentSecond] > 0) {
                    rateUp += boostScore[currentSecond] + effectScore[currentSecond];
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
                    holdScore += ScoreBoostAt(x) * (Math.Min(startTime + length, x + 1) - Math.Max(startTime, x));
                }
                return holdScore;
            }
        }

        public bool IsTricolour() {
            int unitTypes = Members.Select(card => card.Type).Distinct().Count();
            return unitTypes >= 3;
        }

        public bool IsMonocolour(Types matchType) {
            int unitTypes = Members.Where(card => card.Type == matchType).Count();
            return unitTypes == 5;
        }

        public List<Card> TopSupport(Types songType, Calculator.BoostType eventType) {
            return Database.DB.TopAppeal(songType, eventType, 10, memberIDs);
        }
    }
}
