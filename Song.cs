using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;

namespace TheaterDaysScore {
    public class Song {
        public string name { get; set; }
        public Types type { get; set; }
        public int bpm { get; set; }
        public int level { get; set; }
        public List<Note> notes { get; set; }

        public int noteCount;
        public int bigNotes;
        public int appealNotes;
        public int measures;
        public int holdQuarterBeats;

        public double songLength; // sec
        public int noteWeight;
        public double holdLength; // sec

        public class Note {
            public enum NoteType {
                tap,
                leftFlick,
                rightFlick,
                upFlick
            }

            public int measure { get; set; }
            [JsonPropertyName("quarterBeat")]
            public int quarterBeat { get; set; }
            public int lane { get; set; }
            public int size { get; set; }
            public NoteType type { get; set; }
            public List<Note> waypoints { get; set; }

            public int holdQuarterBeats;


            [JsonConstructor]
            public Note(int measure, int quarterBeat, int lane, int size, NoteType type, List<Note> waypoints) {
                this.measure = measure;
                this.quarterBeat = quarterBeat;
                this.lane = lane;
                this.size = size;
                this.type = type;
                this.waypoints = waypoints;

                if (this.waypoints != null) {
                    Note lastWaypoint = this.waypoints.Last();
                    holdQuarterBeats = (lastWaypoint.measure - this.measure) * 16 + lastWaypoint.quarterBeat - this.quarterBeat;
                }
            }
        }

        public Song(string name, Types type, int bpm, int level, List<Note> notes) {
            this.name = name;
            this.type = type;
            this.bpm = bpm;
            this.level = level;
            this.notes = notes;
            noteCount = this.notes.Count;
            bigNotes = this.notes.Where(note => note.size == 2).Count();
            appealNotes = this.notes.Where(note => note.size == 10).Count();
            Note lastNote = this.notes.Last();
            int lastQuarterBeat = lastNote.measure * 16 + lastNote.quarterBeat;
            measures = lastNote.measure + 2;
            holdQuarterBeats = this.notes.Sum(note => note.holdQuarterBeats);

            songLength = (double)lastQuarterBeat / 4 / this.bpm * 60;
            noteWeight = this.notes.Sum(note => note.size);
            holdLength = (double)holdQuarterBeats / 4 / this.bpm * 60;

            // https://api.megmeg.work/mltd/v1/songDesc/
        }
    }
}
