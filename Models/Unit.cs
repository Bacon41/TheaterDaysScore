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

        public Unit(string guestID, string centerID, string member1ID, string member2ID, string member3ID, string member4ID) {
            Guest = Database.DB.GetCard(guestID);
            Center = Database.DB.GetCard(centerID);

            memberIDs = new string[5] { member1ID, member2ID, centerID, member3ID, member4ID };
            Members = new Card[5];
            for (int x = 0; x < 5; x++) {
                Members[x] = Database.DB.GetCard(memberIDs[x]);
            }

            rand = new Random();
        }

        private double ActivationOdds(Types cardType, Card.Skill skill) {
            double activationThreshold = skill.Probability;
            if (Guest.Center != null) {
                activationThreshold += skill.Probability * Guest.Center.ActivationBoost(cardType);
            }
            if (Center.Center != null) {
                activationThreshold += skill.Probability * Center.Center.ActivationBoost(cardType);
            }
            return activationThreshold;
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
            private int[] doubleScore;
            private int[] doubleCombo;

            public Activation(double songLen) {
                int numSeconds = (int)Math.Ceiling(songLen) + 1;
                scoreUp = new int[numSeconds];
                comboUp = new int[numSeconds];
                doubleScore = new int[numSeconds];
                doubleCombo = new int[numSeconds];
            }

            public void AddInterval(int start, Card.Skill skill) {
                for (int x = start; x < scoreUp.Length && x < start + skill.Duration; x++) {
                    if (skill.Effect == CardData.Skill.Type.doubleBoost) {
                        if (skill.ScoreBoost > doubleScore[x]) {
                            doubleScore[x] = skill.ScoreBoost;
                        }
                        if (skill.ComboBoost > doubleCombo[x]) {
                            doubleCombo[x] = skill.ComboBoost;
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
                return (100.0 + scoreUp[currentSecond] + doubleScore[currentSecond]) / 100;
            }

            public double ComboBoostAt(double time) {
                int currentSecond = (int)Math.Floor(time);
                return (100.0 + 3 * (comboUp[currentSecond] + doubleCombo[currentSecond])) / 100;
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

        public List<Card> TopSupport(Types songType) {
            return Database.DB.TopAppeal(songType, 10, memberIDs);
        }
    }
}
