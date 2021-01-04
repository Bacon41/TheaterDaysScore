using Avalonia;
using Avalonia.Platform;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.Json;

namespace TheaterDaysScore {

    public enum Types {
        Princess,
        Fairy,
        Angel,
        All,
        EX,
    };

    class Card {
        public int id;
        public Types type;
        public CenterEffect centerEffect;
        public Skill skill;

        private Random random;

        public class Skill {
            public Type type;
            public int duration;
            public int interval;
            public int probability;
            public int[] value;
            public int level;

            public int leveledProbability;

            public enum Type {
                filler,
                scoreUp,
                comboBonus,
                lifeRestore,
                damageGuard,
                comboProtect,
                judgementBoost,
                doubleBoost,
                multiUp,
                overClock,
                overRondo,
            };

            public Skill(Type type, int duration, int interval, int probability, int[] value, int level) {
                this.type = type;
                this.duration = duration;
                this.interval = interval;
                this.probability = probability;
                this.value = value;
                this.level = level;

                if (this.level <= 10) {
                    leveledProbability = this.probability + this.level;
                } else {
                    leveledProbability = this.probability + 10 + (this.level - 10) * 5;
                }
            }

            public int ScoreBonus() {
                switch (this.type) {
                    case Type.scoreUp:
                        return this.value[0];
                    case Type.doubleBoost:
                        return this.value[0];
                    case Type.multiUp:
                        return this.value[0];
                    case Type.overClock:
                        return this.value[0];
                }
                return 0;
            }

            public int ComboBonus() {
                switch (this.type) {
                    case Type.comboBonus:
                        return this.value[0];
                    case Type.doubleBoost:
                        return this.value[1];
                    case Type.overRondo:
                        return this.value[0];
                }
                return 0;
            }
        }

        public class CenterEffect {
            public Type type;
            public Types idolType;
            public Types songType;
            public int value;

            public enum Type {
                filler,
                vocalUp,
                danceUp,
                visualUp,
                allUp,
                lifeUp,
                skillBoost,
            };

            public CenterEffect(Type type, Types idolType, Types songType, int value) {
                this.type = type;
                this.idolType = idolType;
                this.songType = songType;
                this.value = value;
            }
        }

        public Card(int id, Types type, CenterEffect centerEffect, Skill skill) {
            this.id = id;
            this.type = type;
            this.centerEffect = centerEffect;
            this.skill = skill;

            random = new Random();

            // https://storage.matsurihi.me/mltd/card/017kth0054_0_a.png
        }

        public List<int> GetActivations(double songLen, CenterEffect guestEffect, CenterEffect centerEffect) {
            List<int> activations = new List<int>();
            if (this.skill == null) {
                return activations;
            }

            int start = this.skill.interval;
            double activationThreshold = this.skill.leveledProbability;
            if (guestEffect != null && guestEffect.type == CenterEffect.Type.skillBoost) {
                if (guestEffect.idolType == this.type) {
                    activationThreshold += this.skill.leveledProbability * (double)guestEffect.value / 100;
                }
            }
            if (centerEffect != null && centerEffect.type == CenterEffect.Type.skillBoost) {
                if (centerEffect.idolType == this.type) {
                    activationThreshold += this.skill.leveledProbability * (double)centerEffect.value / 100;
                }
            }

            while (start < songLen) {
                if (random.NextDouble() * 100 < activationThreshold) {
                    activations.Add(start);
                }
                start += this.skill.interval;
            }
            return activations;
        }
    }

    class ActivationInstance {

        int[] scoreUp;
        int[] comboUp;
        int[] doubleScore;
        int[] doubleCombo;

        public ActivationInstance(double songLen) {
            int numSeconds = (int)Math.Ceiling(songLen);
            scoreUp = new int[numSeconds];
            comboUp = new int[numSeconds];
            doubleScore = new int[numSeconds];
            doubleCombo = new int[numSeconds];
        }

        public void AddIntervals(List<int> starts, Card.Skill skill) {
            foreach (int start in starts) {
                for (int x = start; x < scoreUp.Length && x < start + skill.duration; x++) {
                    if (skill.type == Card.Skill.Type.doubleBoost) {
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

        public int GetScore() {
            var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
            StreamReader reader = new StreamReader(assets.Open(new Uri($"avares://TheaterDaysScore/res/songlist.json")));
            List<Song> songs = JsonSerializer.Deserialize<List<Song>>(reader.ReadToEnd());
            Song song = songs[1];

            Card guest = new Card(256, Types.Fairy, new Card.CenterEffect(Card.CenterEffect.Type.skillBoost, Types.Fairy, Types.All, 20), null);
            //Card guest = new Card(868, Types.Fairy, new Card.CenterEffect(Card.CenterEffect.Type.danceUp, Types.Fairy, Types.Fairy, 90), new Card.Skill(Card.Skill.Type.multiUp, 6, 11, 30, new int[] { 32 }, 10));

            List<Card> cards = new List<Card>() {
                //new Card(286, Types.Princess, null, new Card.Skill(Card.Skill.Type.comboBonus, 6, 13, 35, new int[] { 26 }, 5)),
                new Card(250, Types.Angel, null, new Card.Skill(Card.Skill.Type.comboBonus, 4, 7, 30, new int[] { 28 }, 6)),
                new Card(391, Types.Princess, null, new Card.Skill(Card.Skill.Type.comboBonus, 7, 13, 30, new int[] { 26 }, 7)),

                new Card(256, Types.Fairy, new Card.CenterEffect(Card.CenterEffect.Type.skillBoost, Types.Fairy, Types.All, 20), new Card.Skill(Card.Skill.Type.judgementBoost, 3, 9, 25, new int[] { }, 5)),

                new Card(269, Types.Fairy, null, new Card.Skill(Card.Skill.Type.comboBonus, 7, 13, 30, new int[] { 26 }, 8)),
                new Card(178, Types.Angel, null, new Card.Skill(Card.Skill.Type.comboBonus, 7, 13, 30, new int[] { 26 }, 9))

                /*new Card(409, Types.Fairy, null, new Card.Skill(Card.Skill.Type.comboBonus, 6, 11, 30, new int[] { 28 }, 10)),
                new Card(368, Types.Fairy, null, new Card.Skill(Card.Skill.Type.scoreUp, 5, 10, 30, new int[] { 30 }, 12)),
                new Card(868, Types.Fairy, null, new Card.Skill(Card.Skill.Type.multiUp, 6, 11, 30, new int[] { 32 }, 10)),
                new Card(159, Types.Fairy, null, new Card.Skill(Card.Skill.Type.scoreUp, 7, 13, 30, new int[] { 30 }, 12)),
                new Card(432, Types.Fairy, null, new Card.Skill(Card.Skill.Type.comboBonus, 5, 9, 30, new int[] { 26 }, 12))*/

                /*new Card(432, Types.Fairy, null, new Card.Skill(Card.Skill.Type.comboBonus, 5, 9, 30, new int[] { 26 }, 12)),
                new Card(868, Types.Fairy, null, new Card.Skill(Card.Skill.Type.multiUp, 6, 11, 30, new int[] { 32 }, 10)),
                new Card(572, Types.Princess, null, new Card.Skill(Card.Skill.Type.doubleBoost, 4, 7, 30, new int[] { 10, 5 }, 10)),
                new Card(409, Types.Fairy, null, new Card.Skill(Card.Skill.Type.comboBonus, 6, 11, 30, new int[] { 28 }, 10)),
                new Card(732, Types.Angel, null, new Card.Skill(Card.Skill.Type.overClock, 5, 9, 30, new int[] { 32, 14 }, 5))*/
            };

            int totalAppeal = 320000;
            //int totalAppeal = 377515;
            //int totalAppeal = 386402;

            double baseScore = totalAppeal * (33f + song.level) / 20;
            double notesAndHolds = song.noteWeight + 2 * song.holdLength;

            double scoreScale = 0.7 * baseScore / notesAndHolds;
            double comboScale = 0.3 * baseScore / (2 * song.noteCount - 66);

            List<double> scores = new List<double>();

            for (int x = 0; x < 10000; x++) {
                ActivationInstance ai = new ActivationInstance(song.songLength);
                foreach (Card c in cards) {
                    ai.AddIntervals(c.GetActivations(song.songLength, guest.centerEffect, cards[2].centerEffect), c.skill);
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
