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
        public Skill skill;
        


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

        public Card(int id, Types type, Skill skill) {
            this.id = id;
            this.type = type;
            this.skill = skill;
            
            // https://storage.matsurihi.me/mltd/card/017kth0054_0_a.png
        }

        public List<int> GetActivations(double songLen) {
            List<int> activations = new List<int>();
            if (this.skill == null) {
                return activations;
            }
            int start = this.skill.interval;
            while (start < songLen) {
                activations.Add(start);
                start += this.skill.interval;
            }
            return activations;
        }
    }

    class ActivationInstance {

        int[] scoreUp;
        int[] comboUp;

        public ActivationInstance(double songLen) {
            scoreUp = new int[(int)Math.Ceiling(songLen)];
            comboUp = new int[(int)Math.Ceiling(songLen)];
        }

        public void AddIntervals(List<int> starts, Card.Skill skill) {
            foreach (int start in starts) {
                for (int x = start; x < scoreUp.Length && x < start + skill.duration; x++) {
                    if (skill.ScoreBonus() > scoreUp[x]) {
                        scoreUp[x] = skill.ScoreBonus();
                    }
                    if (skill.ComboBonus() > comboUp[x]) {
                        comboUp[x] = skill.ComboBonus();
                    }
                }
            }
        }

        public double ScoreBoostAt(double time) {
            return (100.0 + scoreUp[(int)Math.Floor(time)]) / 100;
        }

        public double ComboBoostAt(double time) {
            return (100.0 + 3 * comboUp[(int)Math.Floor(time)]) / 100;
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
            Song song = songs[0];

            ActivationInstance ai = new ActivationInstance(song.songLength);

            //Card guest = new Card(256, Types.Fairy);
            Card center = new Card(286, Types.Princess, new Card.Skill(Card.Skill.Type.comboBonus, 6, 13, 35, new int[] { 26 }, 5));
            //Card center = new Card(464, Types.Princess, new Card.Skill(Card.Skill.Type.scoreUp, 4, 7, 30, new int[] { 30 }, 5));
            //ai.AddIntervals(center.GetActivations(song.songLength), center.skill);

            Card mem1 = new Card(250, Types.Angel, new Card.Skill(Card.Skill.Type.comboBonus, 4, 7, 30, new int[] { 28 }, 6));
            //ai.AddIntervals(mem1.GetActivations(song.songLength), mem1.skill);

            Card mem2 = new Card(391, Types.Princess, new Card.Skill(Card.Skill.Type.comboBonus, 7, 13, 30, new int[] { 26 }, 7));
            //ai.AddIntervals(mem2.GetActivations(song.songLength), mem2.skill);

            Card mem3 = new Card(269, Types.Fairy, new Card.Skill(Card.Skill.Type.comboBonus, 7, 13, 30, new int[] { 26 }, 8));
            //ai.AddIntervals(mem3.GetActivations(song.songLength), mem3.skill);

            Card mem4 = new Card(178, Types.Angel, new Card.Skill(Card.Skill.Type.comboBonus, 7, 13, 30, new int[] { 26 }, 9));
            //ai.AddIntervals(mem4.GetActivations(song.songLength), mem4.skill);

            List<Card> cards = new List<Card>() {
                new Card(409, Types.Fairy, new Card.Skill(Card.Skill.Type.comboBonus, 6, 11, 30, new int[] { 28 }, 10)),
                new Card(368, Types.Fairy, new Card.Skill(Card.Skill.Type.scoreUp, 5, 10, 30, new int[] { 30 }, 12)),
                new Card(868, Types.Fairy, new Card.Skill(Card.Skill.Type.multiUp, 6, 11, 30, new int[] { 32 }, 10)),
                new Card(159, Types.Fairy, new Card.Skill(Card.Skill.Type.scoreUp, 7, 13, 30, new int[] { 30 }, 12)),
                new Card(432, Types.Fairy, new Card.Skill(Card.Skill.Type.comboBonus, 5, 9, 30, new int[] { 26 }, 12))
            };
            foreach (Card c in cards) {
                ai.AddIntervals(c.GetActivations(song.songLength), c.skill);
            }

            //int totalAppeal = 320000;
            int totalAppeal = 377515;

            double baseScore = totalAppeal * (33f + song.level) / 20;
            double notesAndHolds = song.noteWeight + 2 * song.holdLength;

            double scoreScale = 0.7 * baseScore / notesAndHolds;
            double comboScale = 0.3 * baseScore / (2 * song.noteCount - 66);

            double score = 0;
            int combo = 0;
            foreach (Song.Note note in song.notes) {
                combo++;
                double noteTime = (double)(note.measure * 16 + note.quarterBeat) / 4 / song.bpm * 60;
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
                    double holdTime = (double)note.holdQuarterBeats / 4 / song.bpm * 60;
                    score += 2 * scoreScale * ai.HoldOver(noteTime, holdTime);
                }
            }
            
            return (int)score;
        }
    }
}
