namespace TheaterDaysScore.JsonModels {
    public class VersionInfo {
        public App app { get; set; }
        public Resource res { get; set; }

        public class App {
            public string version { get; set; }
            public string updateTime { get; set; }
            public int revision { get; set; }
        }
        public class Resource {
            public int version { get; set; }
            public string updateTime { get; set; }
            public string indexName { get; set; }
        }
    }
}
