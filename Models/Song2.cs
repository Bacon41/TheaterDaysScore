using System;
using System.Collections.Generic;
using System.Linq;
using TheaterDaysScore.JsonModels;

namespace TheaterDaysScore.Models {
    public class Song2 {
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
        public Dictionary<Difficulty, List<Note2>> Notes { get; }
        public List<TimeSignature> TimeSignatures { get; }

        public int FirstNoteTick { get; }
        public int LastNoteTick { get; }
        public int TotalTicks { get; }

        public class Note2 {
            public int Tick;
            public double Second;
            public float Lane;
            public InteractType Type;
            public int PointsValue;
            public int HoldTicks;
            public InteractType EndType;
            public float EndLane;

            public List<Waypoint> Waypoints;

            public enum InteractType {
                tap,
                leftFlick,
                upFlick,
                rightFlick,
            }

            public class Waypoint {
                public int Tick;
                public float Lane;

                public Waypoint(SongData2.Waypoint data, int startTick) {
                    Tick = startTick + data.subtick;
                    Lane = data.posx;
                }

                public Waypoint(int tick, float lane) {
                    Tick = tick;
                    Lane = lane;
                }
            }

            public Note2(SongData2.Event data) {
                Tick = data.tick;
                Second = data.absTime;

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
                PointsValue = 1;
                switch (data.type) {
                    case 1:
                        PointsValue = 2;
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
                        PointsValue = 2;
                        break;
                    case 8:
                        PointsValue = 10;
                        Lane = centerLane;
                        break;
                }

                // Hold info
                Waypoints = new List<Waypoint>();
                if (5 <= data.type && data.type <= 7) {
                    HoldTicks = data.duration;

                    if (data.poly.Count > 0) {
                        foreach (SongData2.Waypoint w in data.poly) {
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
        }

        public class Beat {
            public int Tick;
            public double Second;
            public int MeasureNum;
            public int BeatNum;
            public bool MeasureStart;

            public Beat(SongData2.Event data) {
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

            public TimeSignature(SongData2.Conductor data) {
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

        public Song2(SongList songData, SongData2 beatmapData) {
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
            Notes = new Dictionary<Difficulty, List<Note2>>();
            foreach (Difficulty d in Enum.GetValues(typeof(Difficulty))) {
                Notes[d] = new List<Note2>();
            }
            TimeSignatures = new List<TimeSignature>();

            foreach (SongData2.Conductor cond in beatmapData.ct) {
                TimeSignatures.Add(new TimeSignature(cond));
            }

            foreach (SongData2.Event evt in beatmapData.evts) {
                if (evt.track == -1) {
                    Beats.Add(new Beat(evt));
                } else if (1 <= evt.track && evt.track <= 2) {
                    Notes[Difficulty.TwoMix].Add(new Note2(evt));
                } else if (3 <= evt.track && evt.track <= 4) {
                    Notes[Difficulty.TwoMixPlus].Add(new Note2(evt));
                } else if (9 <= evt.track && evt.track <= 12) {
                    Notes[Difficulty.FourMix].Add(new Note2(evt));
                } else if (25 <= evt.track && evt.track <= 30) {
                    Notes[Difficulty.SixMix].Add(new Note2(evt));
                } else if (31 <= evt.track && evt.track <= 36) {
                    Notes[Difficulty.MillionMix].Add(new Note2(evt));
                } else if (37 <= evt.track && evt.track <= 42) {
                    Notes[Difficulty.OverMix].Add(new Note2(evt));
                }
            }
            // Not sure why, but songs without OM still have a handful of entries sometimes
            if (Notes[Difficulty.OverMix].Count < Notes[Difficulty.MillionMix].Count) {
                Notes[Difficulty.OverMix].Clear();
            }
            // There are 5 minutes of beats, but ~2:20 of actual song usually, so throw out the excess
            var lastNote = beatmapData.evts.FindLast(x => x.track != -1);
            int lastBeatIdx = Beats.FindIndex(x => x.Tick > lastNote.tick + lastNote.duration && x.MeasureStart) + 1;
            TimeSignature lastTS = TimeSignatureAtTick(Beats[lastBeatIdx].BeatNum);
            Beats.RemoveRange(lastBeatIdx, Beats.Count - lastBeatIdx);

            FirstNoteTick = Notes[Difficulty.TwoMix][0].Tick;
            LastNoteTick = Notes[Difficulty.TwoMix][Notes[Difficulty.TwoMix].Count - 1].Tick + Notes[Difficulty.TwoMix][Notes[Difficulty.TwoMix].Count - 1].HoldTicks;
            TotalTicks = Beats[Beats.Count - 1].Tick;
        }

        public TimeSignature TimeSignatureAtSecond(double second) {
            return TimeSignatures.FindLast(x => x.StartTime <= second);
        }

        public TimeSignature TimeSignatureAtTick(int tick) {
            return TimeSignatures.FindLast(x => x.StartTick <= tick);
        }

        public int TimeToTick(double time) {
            TimeSignature currentSig = TimeSignatureAtSecond(time);
            return currentSig.StartTick + (int)((time - currentSig.StartTime) * currentSig.TicksPerSecond);
        }

        public double TickToTime(int tick) {
            TimeSignature currentSig = TimeSignatureAtTick(tick);
            return currentSig.StartTime + (int)((tick - currentSig.StartTick) * currentSig.SecondsPerTick);
        }

        public bool IsDuringAppeal(double second) {
            List<Note2> notes = Notes[Difficulty.TwoMix];
            int firstAppealIdx = notes.FindIndex(n => n.PointsValue == 10);
            if (notes[firstAppealIdx].Second <= second && (firstAppealIdx == notes.Count - 1 || second <= notes[firstAppealIdx + 1].Second)) {
                return true;
            }
            int secondAppealIdx = notes.FindLastIndex(n => n.PointsValue == 10);
            if (firstAppealIdx != secondAppealIdx) {
                if (notes[secondAppealIdx].Second <= second && (firstAppealIdx == notes.Count - 1 || second <= notes[secondAppealIdx + 1].Second)) {
                    return true;
                }
            }
            return false;
        }
    }
}
