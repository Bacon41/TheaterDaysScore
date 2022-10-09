using System;
using System.Collections.Generic;
using System.Linq;
using TheaterDaysScore.JsonModels;

namespace TheaterDaysScore.Models {
    public class Song {
        public enum Difficulty {
            TwoMix,
            TwoMixPlus,
            FourMix,
            SixMix,
            MillionMix,
            OverMix,
        }

        // Divide this by the time signature denominator to get ticks per beat
        private const int tickAdjustment = 1920;

        public string Name { get; }
        public string Asset { get; }
        public Types Type { get; }
        public Dictionary<Difficulty, int> Level { get; }

        public List<Beat> Beats { get; }
        public Dictionary<Difficulty, List<Note>> Notes { get; }
        public List<TimeSignature> TimeSignatures { get; }

        public int FirstNoteTick { get; }
        public int LastNoteTick { get; }
        public int TotalTicks { get; }
        public double SkillStartTime { get; }

        public Dictionary<Difficulty, int> TotalNoteScore { get; }
        public Dictionary<Difficulty, double> TotalHoldSeconds { get; }

        public class Note : IComparable<Note> {
            public int Tick;
            public float Lane;
            public InteractType Type;
            public int Size;
            public int HoldTicks;
            public InteractType EndType;
            public float EndLane;
            public Accuracy TapAccuracy;

            public List<Waypoint> Waypoints;

            public enum InteractType {
                tap,
                leftFlick,
                upFlick,
                rightFlick,
            }

            public enum Accuracy {
                perfect,
                great,
                good,
                fastSlow,
                miss,
            }

            public class Waypoint {
                public int Tick;
                public float Lane;

                public Waypoint(SongData.Waypoint data, int startTick) {
                    Tick = startTick + data.subtick;
                    Lane = data.posx;
                }

                public Waypoint(int tick, float lane) {
                    Tick = tick;
                    Lane = lane;
                }
            }

            public Note(SongData.Event data) {
                Tick = data.tick;

                TapAccuracy = Accuracy.perfect;

                float centerLane = 0f;
                Lane = data.track;
                if (1 <= Lane && Lane <= 2) { // 2M
                    Lane = data.track - 1;
                    centerLane = 0.5f;
                } else if (3 <= Lane && Lane <= 4) { // 2M+
                    Lane = data.track - 3;
                    centerLane = 0.5f;
                } else if (9 <= Lane && Lane <= 12) { // 4M
                    Lane = data.track - 9;
                    centerLane = 1.5f;
                } else if (25 <= Lane && Lane <= 30) { // 6M
                    Lane = data.track - 25;
                    centerLane = 2.5f;
                } else if (31 <= Lane && Lane <= 36) { // MM
                    Lane = data.track - 31;
                    centerLane = 2.5f;
                } else if (37 <= Lane && Lane <= 42) { // OM
                    Lane = data.track - 37;
                    centerLane = 2.5f;
                }

                Type = InteractType.tap;
                Size = 1;
                switch (data.type) {
                    case 1:
                        Size = 2;
                        break;
                    case 2:
                        Type = InteractType.leftFlick;
                        break;
                    case 3:
                        Type = InteractType.upFlick;
                        break;
                    case 4:
                        Type = InteractType.rightFlick;
                        break;
                    case 7:
                        Size = 2;
                        break;
                    case 8:
                        Size = 10;
                        Lane = centerLane;
                        break;
                }

                // Hold info
                Waypoints = new List<Waypoint>();
                if (5 <= data.type && data.type <= 7) {
                    HoldTicks = data.duration;

                    if (data.poly.Count > 0) {
                        foreach (SongData.Waypoint w in data.poly) {
                            Waypoints.Add(new Waypoint(w, Tick));
                        }
                    } else {
                        Waypoints.Add(new Waypoint(Tick, Lane));
                        Waypoints.Add(new Waypoint(Tick + HoldTicks, Lane));
                    }
                    EndLane = Waypoints[Waypoints.Count - 1].Lane;

                    switch (data.endType) {
                        case 0:
                            EndType = InteractType.tap;
                            break;
                        case 1:
                            EndType = InteractType.leftFlick;
                            break;
                        case 2:
                            EndType = InteractType.upFlick;
                            break;
                        case 3:
                            EndType = InteractType.rightFlick;
                            break;
                    }
                }
            }

            // Create a note to represent the end of the hold
            public Note(Note startNote) {
                Tick = startNote.Tick + startNote.HoldTicks;
                Lane = startNote.EndLane;
                Type = startNote.EndType;
                Size = startNote.Size;
            }

            public int CompareTo(Note otherNote) {
                if (Tick == otherNote.Tick) {
                    if (Lane == otherNote.Lane) {
                        return 0;
                    } else if (Lane < otherNote.Lane) {
                        return -1;
                    }
                    return 1;
                } else if (Tick < otherNote.Tick) {
                    return -1;
                }
                return 1;
            }
        }

        private List<Note> GetNotes(SongData.Event data) {
            List<Note> notes = new List<Note>();
            notes.Add(new Note(data));


            // Hold info
            if (notes[0].HoldTicks != 0) {
                notes.Add(new Note(notes[0]));
            }
            return notes;
        }

        public class Beat {
            public int Tick;
            public double Second;
            public int MeasureNum;
            public int BeatNum;
            public bool MeasureStart;

            public Beat(SongData.Event data) {
                Tick = data.tick;
                Second = data.absTime;
                MeasureNum = data.measure;
                BeatNum = data.beat;
                MeasureStart = data.type == -1;
            }
        }

        public class TimeSignature {
            public int Numerator;
            public int Denominator;
            public float Tempo;
            public int StartTick;
            public double StartTime;

            public int TicksPerBeat;
            public double SecondsPerBeat;
            public double TicksPerSecond;
            public double SecondsPerTick;

            public TimeSignature(SongData.Conductor data) {
                Numerator = data.tsigNumerator;
                Denominator = data.tsigDenominator;
                Tempo = data.tempo;
                StartTick = data.tick;
                StartTime = data.absTime;

                TicksPerBeat = tickAdjustment / Denominator;
                // BPM / sec/min = BPS. BPS == quarters/sec. quarters/sec / beats/quarter = beats/sec
                SecondsPerBeat = 240.0 / (Tempo * Denominator);
                TicksPerSecond = (tickAdjustment * Tempo) / 240;
                SecondsPerTick = 240.0 / (Tempo * tickAdjustment);
            }
        }

        public Song(SongList songData, SongData beatmapData) {
            Name = songData.song_name;
            Asset = songData.asset;
            Type = (Types)songData.song_type;

            Level = new Dictionary<Difficulty, int>();
            Level[Difficulty.TwoMix] = songData.two_mix_lv;
            Level[Difficulty.TwoMixPlus] = songData.two_mixplus_lv;
            Level[Difficulty.FourMix] = songData.four_mix_lv;
            Level[Difficulty.SixMix] = songData.six_mix_lv;
            Level[Difficulty.MillionMix] = songData.million_mix_lv;
            Level[Difficulty.OverMix] = songData.over_mix_lv;

            // Beatmap info
            Beats = new List<Beat>();
            Notes = new Dictionary<Difficulty, List<Note>>();
            foreach (Difficulty d in Enum.GetValues(typeof(Difficulty))) {
                Notes[d] = new List<Note>();
            }
            TimeSignatures = new List<TimeSignature>();

            foreach (SongData.Conductor cond in beatmapData.ct) {
                TimeSignatures.Add(new TimeSignature(cond));
            }

            foreach (SongData.Event evt in beatmapData.evts) {
                if (evt.track == -1) {
                    Beats.Add(new Beat(evt));
                } else if (1 <= evt.track && evt.track <= 2) {
                    Notes[Difficulty.TwoMix].AddRange(GetNotes(evt));
                } else if (3 <= evt.track && evt.track <= 4) {
                    Notes[Difficulty.TwoMixPlus].AddRange(GetNotes(evt));
                } else if (9 <= evt.track && evt.track <= 12) {
                    Notes[Difficulty.FourMix].AddRange(GetNotes(evt));
                } else if (25 <= evt.track && evt.track <= 30) {
                    Notes[Difficulty.SixMix].AddRange(GetNotes(evt));
                } else if (31 <= evt.track && evt.track <= 36) {
                    Notes[Difficulty.MillionMix].AddRange(GetNotes(evt));
                } else if (37 <= evt.track && evt.track <= 42) {
                    Notes[Difficulty.OverMix].AddRange(GetNotes(evt));
                }
            }
            // Not sure why, but songs without OM still have a handful of entries sometimes
            if (Notes[Difficulty.OverMix].Count < Notes[Difficulty.MillionMix].Count) {
                Notes[Difficulty.OverMix].Clear();
            }
            // Sort to account for hold notes added
            Notes[Difficulty.TwoMix].Sort();
            Notes[Difficulty.TwoMixPlus].Sort();
            Notes[Difficulty.FourMix].Sort();
            Notes[Difficulty.SixMix].Sort();
            Notes[Difficulty.MillionMix].Sort();
            Notes[Difficulty.OverMix].Sort();
            // There are 5 minutes of beats, but ~2:20 of actual song usually, so throw out the excess
            var lastNote = beatmapData.evts.FindLast(x => x.track != -1);
            int lastBeatIdx = Beats.FindIndex(x => x.Tick > lastNote.tick + lastNote.duration && x.MeasureStart) + 1;
            TimeSignature lastTS = TimeSignatureAtTick(Beats[lastBeatIdx].BeatNum);
            Beats.RemoveRange(lastBeatIdx, Beats.Count - lastBeatIdx);

            FirstNoteTick = Notes[Difficulty.TwoMix][0].Tick;
            LastNoteTick = Notes[Difficulty.TwoMix][Notes[Difficulty.TwoMix].Count - 1].Tick + Notes[Difficulty.TwoMix][Notes[Difficulty.TwoMix].Count - 1].HoldTicks;
            TotalTicks = Beats[Beats.Count - 1].Tick;
            // Many songs seem to be at roughly 2 seconds before the notes, but not sure how to get the real value
            SkillStartTime = Math.Max(0, TickToTime(Notes[Difficulty.TwoMix][0].Tick) - 2);

            TotalNoteScore = new Dictionary<Difficulty, int>();
            TotalNoteScore[Difficulty.TwoMix] = Notes[Difficulty.TwoMix].Sum(n => n.Size);
            TotalNoteScore[Difficulty.TwoMixPlus] = Notes[Difficulty.TwoMixPlus].Sum(n => n.Size);
            TotalNoteScore[Difficulty.FourMix] = Notes[Difficulty.FourMix].Sum(n => n.Size);
            TotalNoteScore[Difficulty.SixMix] = Notes[Difficulty.SixMix].Sum(n => n.Size);
            TotalNoteScore[Difficulty.MillionMix] = Notes[Difficulty.MillionMix].Sum(n => n.Size);
            TotalNoteScore[Difficulty.OverMix] = Notes[Difficulty.OverMix].Sum(n => n.Size);

            TotalHoldSeconds = new Dictionary<Difficulty, double>();
            TotalHoldSeconds[Difficulty.TwoMix] = Notes[Difficulty.TwoMix].Sum(n => TickToTime(n.Tick + n.HoldTicks) - TickToTime(n.Tick));
            TotalHoldSeconds[Difficulty.TwoMixPlus] = Notes[Difficulty.TwoMixPlus].Sum(n => TickToTime(n.Tick + n.HoldTicks) - TickToTime(n.Tick));
            TotalHoldSeconds[Difficulty.FourMix] = Notes[Difficulty.FourMix].Sum(n => TickToTime(n.Tick + n.HoldTicks) - TickToTime(n.Tick));
            TotalHoldSeconds[Difficulty.SixMix] = Notes[Difficulty.SixMix].Sum(n => TickToTime(n.Tick + n.HoldTicks) - TickToTime(n.Tick));
            TotalHoldSeconds[Difficulty.MillionMix] = Notes[Difficulty.MillionMix].Sum(n => TickToTime(n.Tick + n.HoldTicks) - TickToTime(n.Tick));
            TotalHoldSeconds[Difficulty.OverMix] = Notes[Difficulty.OverMix].Sum(n => TickToTime(n.Tick + n.HoldTicks) - TickToTime(n.Tick));
        }

        public TimeSignature TimeSignatureAtSecond(double second) {
            if (second < 0) {
                return TimeSignatures[0];
            }
            return TimeSignatures.FindLast(x => x.StartTime <= second);
        }

        public TimeSignature TimeSignatureAtTick(int tick) {
            if (tick < 0) {
                return TimeSignatures[0];
            }
            return TimeSignatures.FindLast(x => x.StartTick <= tick);
        }

        public int TimeToTick(double time) {
            TimeSignature currentSig = TimeSignatureAtSecond(time);
            return currentSig.StartTick + (int)((time - currentSig.StartTime) * currentSig.TicksPerSecond);
        }

        public double TickToTime(int tick) {
            TimeSignature currentSig = TimeSignatureAtTick(tick);
            return currentSig.StartTime + ((tick - currentSig.StartTick) * currentSig.SecondsPerTick);
        }

        public bool IsDuringAppeal(double second) {
            List<Note> notes = Notes[Difficulty.TwoMix];
            int firstAppealIdx = notes.FindIndex(n => n.Size == 10);
            if (TickToTime(notes[firstAppealIdx].Tick) <= second && (firstAppealIdx == notes.Count - 1 || second <= TickToTime(notes[firstAppealIdx + 1].Tick))) {
                return true;
            }
            int secondAppealIdx = notes.FindLastIndex(n => n.Size == 10);
            if (firstAppealIdx != secondAppealIdx) {
                if (TickToTime(notes[secondAppealIdx].Tick) <= second && (firstAppealIdx == notes.Count - 1 || second <= TickToTime(notes[secondAppealIdx + 1].Tick))) {
                    return true;
                }
            }
            return false;
        }

        public void RandomizeAccuraciesPercent(Difficulty difficulty, double greatPercet, double goodPercent, double fastSlowPercent, double missPercent) {
            List<Note> notes = Notes[difficulty];
            int greatCount = (int)(notes.Count * greatPercet / 100);
            int goodCount = (int)(notes.Count * goodPercent / 100);
            int fastSlowCount = (int)(notes.Count * fastSlowPercent / 100);
            int missCount = (int)(notes.Count * missPercent / 100);

            RandomizeAccuraciesCount(difficulty, greatCount, goodCount, fastSlowCount, missCount);
        }

        public void RandomizeAccuraciesCount(Difficulty difficulty, int greatCount, int goodCount, int fastSlowCount, int missCount) {
            List<Note> notes = Notes[difficulty];

            Random randomizer = new Random();
            List<Note> randomizedNotes = notes.OrderBy(note => randomizer.Next()).ToList();
            for (int x = 0; x < randomizedNotes.Count; x++) {
                if (x < missCount) {
                    randomizedNotes[x].TapAccuracy = Note.Accuracy.miss;
                } else if (x < fastSlowCount) {
                    randomizedNotes[x].TapAccuracy = Note.Accuracy.fastSlow;
                } else if (x < goodCount) {
                    randomizedNotes[x].TapAccuracy = Note.Accuracy.good;
                } else if (x < greatCount) {
                    randomizedNotes[x].TapAccuracy = Note.Accuracy.great;
                } else {
                    randomizedNotes[x].TapAccuracy = Note.Accuracy.perfect;
                }
            }
        }

        public void ResetAccuracies() {
            foreach (var item in Notes) {
                foreach (Note note in item.Value) {
                    note.TapAccuracy = Note.Accuracy.perfect;
                }
            }
        }
    }
}
