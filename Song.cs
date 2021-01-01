using System;
using System.Collections.Generic;
using System.Text;

namespace TheaterDaysScore {
    public class Song {
        public string name { get; set; }
        public Types type;
        public int bpm;
        public int measures { get; set; }
        public int level;
        public int noteCount;
        public int bigNotes;
        public int appealNotes;
        public float holdBeats;

        public List<Note> notes { get; set; }

        public float songLength; // sec
        public int noteWeight;
        public float holdLength; // sec

        public class Note {
            public int beat { get; set; }
            public int lane { get; set; }
            public int size { get; set; }

            public Note() { }

            public Note(int b, int c, int s) {
                beat = b;
                lane = c;
                size = s;
            }
        }

        public Song() {
            notes = new List<Note>();
        }

        public Song(string n, Types t, int bp, int m, int l, int c, int b, int a, float h) {
            name = n;
            type = t;
            bpm = bp;
            measures = m;
            level = l;
            noteCount = c;
            bigNotes = b;
            appealNotes = a;
            holdBeats = h;

            songLength = (float)measures * 4 / bpm * 60;
            noteWeight = noteCount + bigNotes + appealNotes * 9;
            holdLength = holdBeats / bpm * 60;

            // https://api.megmeg.work/mltd/v1/songDesc/
        }
    }
}
