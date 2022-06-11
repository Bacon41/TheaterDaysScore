using System.Collections.Generic;

namespace TheaterDaysScore.JsonModels {
    public class SongData2 {
        public string m_Name { get; set; }
        public List<Event> evts { get; set; }
        public List<Conductor> ct { get; set; }

        public class Event {
            public double absTime { get; set; }
            public int tick { get; set; }
            public int measure { get; set; }
            public int beat { get; set; }
            public int track { get; set; }
            public int type { get; set; }
            public int duration { get; set; }
            public List<Waypoint> poly { get; set; }
            public int endType { get; set; }
        }

        public class Waypoint {
            public int subtick { get; set; }
            public float posx { get; set; }
        }

        public class Conductor {
            public double absTime { get; set; }
            public int tick { get; set; }
            public float tempo { get; set; }
            public int tsigNumerator { get; set; }
            public int tsigDenominator { get; set; }
        }
    }
}
