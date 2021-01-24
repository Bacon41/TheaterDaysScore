using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using TheaterDaysScore.JsonModels;

namespace TheaterDaysScore.Models {
    public class Song {
        public string Name { get; }
        public Types Type { get; }
        public int BPM { get; }
        public int Level { get; }
        public List<Note> Notes { get; }

        public int DisplayMeasures { get; }
        public double SkillStartOffset { get; }

        public double Length { get; } // sec
        public int NoteWeight { get; }
        public double HoldLength { get; } // sec

        public int MeasuresForSkillStart { get; }

        private int firstQuarterBeat;
        private double quarterBeatsToSeconds;
        private int holdQuarterBeats;

        public class Note {
            public int Measure { get; }
            public int QuarterBeat { get; }
            public int Lane { get; }
            public int Size { get; }
            public SongData.Note.Type Type { get; }
            public List<Note> Waypoints { get; }

            public int TotalQuarterBeats { get; }
            public int HoldQuarterBeats { get; }

            public Note(SongData.Note data) {
                Measure = data.measure;
                QuarterBeat = data.quarterBeat;
                Lane = data.lane;
                Size = data.size;
                Type = data.type;

                if (data.waypoints != null) {
                    Waypoints = new List<Note>();
                    foreach (SongData.Note point in data.waypoints) {
                        Waypoints.Add(new Note(point));
                    }
                }

                TotalQuarterBeats = Measure * 16 + QuarterBeat;

                if (Waypoints != null) {
                    Note lastWaypoint = Waypoints.Last();
                    HoldQuarterBeats = (lastWaypoint.Measure - Measure) * 16 + lastWaypoint.QuarterBeat - QuarterBeat;
                }
            }
        }

        public Song(SongData data) {
            Name = data.name;
            Type = data.type;
            BPM = data.bpm;
            Level = data.level;

            Notes = new List<Note>();
            foreach (SongData.Note note in data.notes) {
                Notes.Add(new Note(note));
            }

            SkillStartOffset = data.skillStartOffset;

            Note lastNote = Notes.Last();
            int lastQuarterBeat = lastNote.Measure * 16 + lastNote.QuarterBeat;
            firstQuarterBeat = Notes[0].Measure * 16 + Notes[0].QuarterBeat;

            MeasuresForSkillStart = (int)Math.Ceiling((SkillStartOffset - firstQuarterBeat) / 16);

            DisplayMeasures = lastNote.Measure + 1 + MeasuresForSkillStart;
            holdQuarterBeats = Notes.Sum(note => note.HoldQuarterBeats);

            quarterBeatsToSeconds = 60.0 / (4 * BPM);

            Length = (lastQuarterBeat - firstQuarterBeat + SkillStartOffset) * quarterBeatsToSeconds;
            NoteWeight = Notes.Sum(note => note.Size);
            HoldLength = holdQuarterBeats * quarterBeatsToSeconds;

            // https://million.hyrorre.com/
            // https://api.megmeg.work/mltd/v1/songDesc/
        }

        public bool IsDuringAppeal(double second) {
            int i = 0;
            while (i != -1) {
                i = Notes.FindIndex(i, Notes.Count - i, note => note.Size == 10);
                if (i >= 0) {
                    double appealStart = SecondsSinceFirst(Notes[i]);
                    if (appealStart < second) {
                        if (i < Notes.Count - 1) {
                            double appealEnd = SecondsSinceFirst(Notes[i + 1]);
                            if (appealEnd > second) {
                                return true;
                            }
                        } else {
                            return true;
                        }
                    }
                    i++;
                }
            }
            return false;
        }

        public double SecondsSinceFirst(Note note) {
            return (note.TotalQuarterBeats - firstQuarterBeat + SkillStartOffset) * quarterBeatsToSeconds;
        }

        public double QuarterBeatsToSeconds(int quarterBeats) {
            return quarterBeats * quarterBeatsToSeconds;
        }
    }
}
