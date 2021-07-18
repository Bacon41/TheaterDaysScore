using System.Collections.Generic;

namespace TheaterDaysScore.JsonModels {
    public class SongData {
        public string name { get; set; }
        public Types type { get; set; }
        public int bpm { get; set; }
        public int level { get; set; }
        public double skillStartOffset { get; set; }
        public List<Note> notes { get; set; }

        public class Note {
            public enum Type {
                tap,
                leftFlick,
                rightFlick,
                upFlick
            }

            public int measure { get; set; }
            public int quarterBeat { get; set; }
            public int lane { get; set; }
            public int size { get; set; }
            public Type type { get; set; }
            public List<Note> waypoints { get; set; }
        }
    }
}
