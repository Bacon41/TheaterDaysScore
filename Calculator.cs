using Avalonia;
using Avalonia.Platform;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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

    class ActivationInstance {

        int[] scoreUp;
        int[] comboUp;
        int[] doubleScore;
        int[] doubleCombo;

        public ActivationInstance(double songLen) {
            int numSeconds = (int)Math.Ceiling(songLen) + 1;
            scoreUp = new int[numSeconds];
            comboUp = new int[numSeconds];
            doubleScore = new int[numSeconds];
            doubleCombo = new int[numSeconds];
        }

        public void AddIntervals(List<int> starts, Card.Skill skill) {
            foreach (int start in starts) {
                for (int x = start; x < scoreUp.Length && x < start + skill.duration; x++) {
                    if (skill.effectId == Card.Skill.Type.doubleBoost) {
                        if (skill.ScoreBonus() > doubleScore[x]) {
                            doubleScore[x] = skill.ScoreBonus();
                        }
                        if (skill.ComboBonus() > doubleCombo[x]) {
                            doubleCombo[x] = skill.ComboBonus();
                        }
                    } else {
                        if (skill.ScoreBonus() > scoreUp[x]) {
                            scoreUp[x] = skill.ScoreBonus();
                        }
                        if (skill.ComboBonus() > comboUp[x]) {
                            comboUp[x] = skill.ComboBonus();
                        }
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
                holdScore += this.ScoreBoostAt(x) * (Math.Min(startTime + length, x + 1) - Math.Max(startTime, x));
            }
            return holdScore;
        }
    }

    class Calculator {
        public Calculator() {
        }

        public int GetScore(int songNum, int totalAppeal, int guestId, int[] cardIds, int[] skillLevels) {
            var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
            StreamReader reader = new StreamReader(assets.Open(new Uri($"avares://TheaterDaysScore/res/songlist.json")));
            List<Song> songs = JsonSerializer.Deserialize<List<Song>>(reader.ReadToEnd());
            Song song = songs[songNum];

            StreamReader cardReader = new StreamReader(assets.Open(new Uri($"avares://TheaterDaysScore/res/cardlist.json")));
            List<Card> allCards = JsonSerializer.Deserialize<List<Card>>(cardReader.ReadToEnd());

            List<Card> cards = new List<Card>();

            Card guest = allCards.Find(card => card.id == guestId);
            for (int x = 0; x < 5; x++) {
                Card nextCard = allCards.Find(card => card.id == cardIds[x]);
                nextCard.SetLevel(skillLevels[x]);
                cards.Add(nextCard);
            }

            double baseScore = totalAppeal * (33f + song.level) / 20;
            double notesAndHolds = song.noteWeight + 2 * song.holdLength;

            double scoreScale = 0.7 * baseScore / notesAndHolds;
            double comboScale = 0.3 * baseScore / (2 * song.noteCount - 66);

            List<double> scores = new List<double>();

            for (int x = 0; x < 1; x++) {
                ActivationInstance ai = new ActivationInstance(song.songLength);
                foreach (Card c in cards) {
                    ai.AddIntervals(c.GetActivations(song, guest.centerEffect, cards[2].centerEffect), c.skill[0]);
                }

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
                    score += scoreScale * note.size * accuracy * ai.ScoreBoostAt(noteTime) + comboScale * comboState * ai.ComboBoostAt(noteTime);
                    if (note.holdQuarterBeats != 0) {
                        double holdTime = song.QuarterBeatsToSeconds(note.holdQuarterBeats);
                        score += 2 * scoreScale * ai.HoldOver(noteTime, holdTime);
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
