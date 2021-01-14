using Avalonia;
using Avalonia.Platform;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.Json;

namespace TheaterDaysScore {

    public enum Types {
        Princess = 1,
        Fairy,
        Angel,
        All,
        EX,
    };

    class Calculator {
        private List<Song> songs;

        public enum BoostType {
            none,
            vocal,
            dance,
            visual
        }

        public Calculator() {
            var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
            StreamReader reader = new StreamReader(assets.Open(new Uri($"avares://TheaterDaysScore/res/songlist.json")));
            songs = JsonSerializer.Deserialize<List<Song>>(reader.ReadToEnd());
        }

        public int GetAppeal(Types songType, BoostType eventType, Unit unit) {
            // https://megmeg.work/basic_information/formula/appealvalue/

            // Princess song:
            // Guest: Guest: 745 (4)
            // Team: 432 (5, 12), 868 (4, 10), 572 (0, 10), 409 (4, 10), 732 (4, 5)
            // Supports: 1043 (0, 10), 462 (0, 10), 303 (0, 10), 869 (0, 10), 808 (0, 10), 745 (0, 10), 514 (0, 10), 639 (0, 5), 772 (0, 5), 806 (4, 5)
            // Appeal: 353775 (17), Support: 112543, Vocal: 57911(+11386), Dance: 86311(+131122), Visual: 56187(+10858)
            // Fairy song:
            // Guest: 868 (4)
            // Team: 409 (4, 10), 368 (5, 12), 868 (4, 10), 159 (5, 12), 432 (5, 12)
            // Supports: 1044 (0, 10), 461 (0, 10), 981 (0, 10), 924 (0, 5), 685 (0, 10), 573 (0, 10), 515 (0, 10), 733 (4, 10), 253 (5, 12), 806 (4, 5)
            // Appeal: 377527 (17), Support: 114829, Vocal: 61334(+18397), Dance: 81319(+137974), Visual: 60390(+18113)
            // Angel song:
            // Guest: 982 (10)
            // Team: 468 (4, 10), 287 (5, 12), 982 (0, 10), 519 (4, 10), 516 (4, 10)
            // Supports: 305 (0, 10), 746 (0, 10), 574 (0, 10), 766 (4, 10), 732 (4, 5), 627 (0, 5), 754 (0, 5), 727 (0, 5), 486 (4, 5), 806 (4, 5)
            // Appeal: 362909 (17), Support: 114913, Vocal: 62234(+18666), Dance: 81159(+125598), Visual: 57889(+17363)
            // All song:
            // Guest 745 (4)
            // Team: 432 (5, 12), 868 (4, 10), 572 (0, 10), 409 (4, 10), 732 (4, 5)
            // Supports: 486 (4, 5), 468 (4, 10), 287 (5, 12), 519 (4, 10), 766 (4, 10), 516 (4, 10), 733 (4, 10), 253 (5, 12), 159 (5, 12), 368 (5, 12)
            // Appeal: 386402 (17), Support: 122057, Vocal: 59498(+17844), Dance: 92430(+144093), Visual: 55801(+16736)

            /*Types songType = Types.Princess;
            BoostType eventType = BoostType.none;
            int guestId = 745;
            int guestLevel = 4;
            int[] cardIds = new int[5] { 432, 868, 572, 409, 732 };
            int[] cardLevel = new int[5] { 5, 4, 0, 4, 4 };
            int[] supportIds = new int[10] { 1043, 462, 303, 869, 808, 745, 514, 639, 772, 806 };
            int[] supportLevel = new int[10] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 4 };*/
            /*Types songType = Types.Fairy;
            BoostType eventType = BoostType.none;
            int guestId = 868;
            int guestLevel = 4;
            int[] cardIds = new int[5] { 409, 368, 868, 159, 432 };
            int[] cardLevel = new int[5] { 4, 5, 4, 5, 5 };
            int[] supportIds = new int[10] { 1044, 461, 981, 924, 685, 573, 515, 733, 253, 806 };
            int[] supportLevel = new int[10] { 0, 0, 0, 0, 0, 0, 0, 4, 5, 4 };*/
            /*Types songType = Types.Angel;
            BoostType eventType = BoostType.none;
            int guestId = 982;
            int guestLevel = 4;
            int[] cardIds = new int[5] { 468, 287, 982, 519, 516 };
            int[] cardLevel = new int[5] { 4, 5, 0, 4, 4 };
            int[] supportIds = new int[10] { 305, 746, 574, 766, 732, 627, 754, 727, 486, 806 };
            int[] supportLevel = new int[10] { 0, 0, 0, 4, 4, 0, 0, 0, 4, 4 };*/
            /*Types songType = Types.All;
            BoostType eventType = BoostType.none;
            int guestId = 745;
            int guestLevel = 4;
            int[] cardIds = new int[5] { 432, 868, 572, 409, 732 };
            int[] cardLevel = new int[5] { 5, 4, 0, 4, 4 };
            int[] supportIds = new int[10] { 486, 468, 287, 519, 766, 516, 733, 253, 159, 368 };
            int[] supportLevel = new int[10] { 4, 4, 5, 4, 4, 4, 4, 5, 5, 5 };*/

            Vector3 guestCenterEffect = unit.Guest.Center.GetBoost(songType, unit);
            Vector3 centerCenterEffect = unit.Center.Center.GetBoost(songType, unit);

            // Support
            Vector3 supportStatus = new Vector3(0);
            Vector3 supportType = new Vector3(0);
            Vector3 supportEvent = new Vector3(0);

            List<Card> supportCards = unit.TopSupport(songType);
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

        public int GetScore(int songNum, int totalAppeal, Unit unit) {
            // https://megmeg.work/basic_information/formula/score/

            Song song = songs[songNum];

            totalAppeal = GetAppeal(song.type, BoostType.none, unit);

            double baseScore = totalAppeal * (33f + song.level) / 20;
            double notesAndHolds = song.noteWeight + 2 * song.holdLength;

            double scoreScale = 0.7 * baseScore / notesAndHolds;
            double comboScale = 0.3 * baseScore / (2 * song.noteCount - 66);

            List<double> scores = new List<double>();
            for (int x = 0; x < 1; x++) {
                Unit.IActivation activations = unit.GetActivations(song, true);

                double score = 0;
                int combo = 0;
                foreach (Song.Note note in song.notes) {
                    combo++;
                    double noteTime = song.SecondsSinceFirst(note);
                    double comboState = 0;
                    if (combo >= 100) {
                        comboState = 2;
                    } else if (combo >= 70) {
                        comboState = 1.8;
                    } else if (combo >= 50) {
                        comboState = 1.6;
                    } else if (combo >= 30) {
                        comboState = 1.3;
                    } else if (combo >= 10) {
                        comboState = 1;
                    }
                    double accuracy = 1.0;
                    score += scoreScale * note.size * accuracy * activations.ScoreBoostAt(noteTime) + comboScale * comboState * activations.ComboBoostAt(noteTime);
                    if (note.holdQuarterBeats != 0) {
                        double holdTime = song.QuarterBeatsToSeconds(note.holdQuarterBeats);
                        score += 2 * scoreScale * activations.HoldOver(noteTime, holdTime);
                    }
                }
                scores.Add(score);
            }

            // 50th percentile
            scores.Sort();
            return (int)scores[scores.Count / 2];
        }
    }
}
