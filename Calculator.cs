using System;
using System.Collections.Generic;
using System.Numerics;
using TheaterDaysScore.Models;
using TheaterDaysScore.Services;

namespace TheaterDaysScore {

    public enum Types {
        Princess = 1,
        Fairy,
        Angel,
        All,
        EX,
    };

    public class Calculator {
        public enum BoostType {
            none,
            vocal,
            dance,
            visual
        }

        public Calculator() {
        }

        public int GetAppeal(Types songType, BoostType eventType, Unit unit) {
            if (unit == null) {
                return 0;
            }

            // https://megmeg.work/basic_information/formula/appealvalue/

            // Support
            Vector3 supportStatus = new Vector3(0);
            Vector3 supportType = new Vector3(0);
            Vector3 supportEvent = new Vector3(0);

            List<Card> supportCards = unit.TopSupport(songType, eventType);
            foreach (Card card in supportCards) {
                Vector3 stats = card.SplitAppeal();

                supportStatus += stats;

                if (card.Type == songType || card.Type == Types.EX || songType == Types.All) {
                    supportType += floor(stats * 0.3f);
                }

                switch (eventType) {
                    case BoostType.vocal:
                        supportEvent += floor(stats * new Vector3(1.2f, 0, 0));
                        break;
                    case BoostType.dance:
                        supportEvent += floor(stats * new Vector3(0, 1.2f, 0));
                        break;
                    case BoostType.visual:
                        supportEvent += floor(stats * new Vector3(0, 0, 1.2f));
                        break;
                }
            }
            float supportAppeal = Vector3.Dot(new Vector3(1), floor(supportStatus / 2) + floor(supportType / 2) + floor(supportEvent / 2));

            // Guest
            Vector3 guestStatus = new Vector3(0);
            Vector3 guestType = new Vector3(0);
            Vector3 guestCenter = new Vector3(0);
            {
                Vector3 stats = unit.Guest.SplitAppeal();

                guestStatus = stats;

                if (unit.Guest.Type == songType || unit.Guest.Type == Types.EX || songType == Types.All) {
                    guestType += floor(stats * 0.3f);
                }

                Vector3 guestCenterEffect = unit.Guest.Center.GetBoost(songType, unit.Guest.Type, unit);
                Vector3 centerCenterEffect = unit.Center.Center.GetBoost(songType, unit.Guest.Type, unit);
                guestCenter += floor(stats * (guestCenterEffect + centerCenterEffect));
            }
            float guestAppeal = Vector3.Dot(new Vector3(1), guestStatus + guestType + guestCenter);

            // Unit
            Vector3[] unitStatus = new Vector3[4] { new Vector3(), new Vector3(), new Vector3(), new Vector3() };
            Vector3[] unitType = new Vector3[4] { new Vector3(), new Vector3(), new Vector3(), new Vector3() };
            Vector3[] unitCenter = new Vector3[4] { new Vector3(), new Vector3(), new Vector3(), new Vector3() };
            Vector3[] unitEvent = new Vector3[4] { new Vector3(), new Vector3(), new Vector3(), new Vector3() };
            foreach (Card card in unit.Members) {
                Vector3 stats = card.SplitAppeal();
                int cardType = -1;
                switch (card.Type) {
                    case Types.Princess:
                        cardType = 0;
                        break;
                    case Types.Fairy:
                        cardType = 1;
                        break;
                    case Types.Angel:
                        cardType = 2;
                        break;
                    case Types.EX:
                        cardType = 3;
                        break;
                }

                unitStatus[cardType] += stats;

                if (card.Type == songType || card.Type == Types.EX || songType == Types.All) {
                    unitType[cardType] += stats * 0.3f;
                }

                Vector3 guestCenterEffect = unit.Guest.Center.GetBoost(songType, card.Type, unit);
                Vector3 centerCenterEffect = unit.Center.Center.GetBoost(songType, card.Type, unit);
                unitCenter[cardType] += (guestCenterEffect + centerCenterEffect) * stats;

                switch (eventType) {
                    case BoostType.vocal:
                        unitEvent[cardType] += stats * new Vector3(1.2f, 0, 0);
                        break;
                    case BoostType.dance:
                        unitEvent[cardType] += stats * new Vector3(0, 1.2f, 0);
                        break;
                    case BoostType.visual:
                        unitEvent[cardType] += stats * new Vector3(0, 0, 1.2f);
                        break;
                }
            }
            float unitAppeal = Vector4.Dot(new Vector4(
                    Vector3.Dot(new Vector3(1), floor(unitStatus[0]) + floor(unitType[0]) + floor(unitCenter[0]) + floor(unitEvent[0])),
                    Vector3.Dot(new Vector3(1), floor(unitStatus[1]) + floor(unitType[1]) + floor(unitCenter[1]) + floor(unitEvent[1])),
                    Vector3.Dot(new Vector3(1), floor(unitStatus[2]) + floor(unitType[2]) + floor(unitCenter[2]) + floor(unitEvent[2])),
                    Vector3.Dot(new Vector3(1), floor(unitStatus[3]) + floor(unitType[3]) + floor(unitCenter[3]) + floor(unitEvent[3]))
                ), new Vector4(1));

            // Total
            float totalAppeal = supportAppeal + guestAppeal + unitAppeal;
            return (int)totalAppeal;
        }

        private Vector3 floor(Vector3 input) {
            return new Vector3((float)Math.Floor(input.X), (float)Math.Floor(input.Y), (float)Math.Floor(input.Z));
        }

        public class Results {
            public int Ideal { get; }

            private List<int> scores;

            public Results(int ideal, List<int> scores) {
                Ideal = ideal;
                this.scores = scores;
                this.scores.Sort();
            }

            public int Percentile(double percent) {
                return scores[(int)(scores.Count * (1 - percent / 100))];
            }
        }

        public Results GetResults(Song song, Song.Difficulty difficulty, BoostType boostType, Unit unit, int numRuns) {
            if (unit == null) {
                return null;
            }
            if (song.Level[difficulty] == 0) {
                return null;
            }

            // https://megmeg.work/basic_information/formula/score/

            int totalAppeal = GetAppeal(song.Type, boostType, unit);

            double baseScore = totalAppeal * (33f + song.Level[difficulty]) / 20;
            double notesAndHolds = song.TotalNoteScore[difficulty] + 2 * song.TotalHoldSeconds[difficulty];

            double scoreScale = 0.7 * baseScore / notesAndHolds;
            double comboScale = 0.3 * baseScore / (2 * song.Notes[difficulty].Count - 66);

            int ideal = GetScore(unit.GetActivations(song, true), song, difficulty, scoreScale, comboScale);
            List<int> scores = new List<int>();
            for (int x = 0; x < numRuns; x++) {
                scores.Add(GetScore(unit.GetActivations(song), song, difficulty, scoreScale, comboScale));
            }

            return new Results(ideal, scores);
        }

        private int GetScore(Unit.IActivation activations, Song song, Song.Difficulty difficulty, double scoreScale, double comboScale) {
            double score = 0;
            int combo = 0;
            foreach (Song.Note note in song.Notes[difficulty]) {
                combo++;
                double comboMultiplier = 0;
                if (combo >= 100) {
                    comboMultiplier = 2;
                } else if (combo >= 70) {
                    comboMultiplier = 1.8;
                } else if (combo >= 50) {
                    comboMultiplier = 1.6;
                } else if (combo >= 30) {
                    comboMultiplier = 1.3;
                } else if (combo >= 10) {
                    comboMultiplier = 1;
                }

                double accuracyMultiplier = 1.0;

                double noteTime = song.TickToTime(note.Tick) - song.SkillStartTime;

                score += scoreScale * note.Size * accuracyMultiplier * activations.ScoreBoostAt(noteTime) + comboScale * comboMultiplier * activations.ComboBoostAt(noteTime);
                if (note.HoldTicks != 0) {
                    double holdTime = song.TickToTime(note.Tick + note.HoldTicks) - song.TickToTime(note.Tick);
                    score += 2 * scoreScale * activations.HoldOver(noteTime, holdTime);
                }
            }
            return (int)score;
        }
    }
}
