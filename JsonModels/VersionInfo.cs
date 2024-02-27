namespace TheaterDaysScore.JsonModels {
    public class VersionInfo {
        public App app { get; set; }
        public Asset asset { get; set; }

        public class App {
            public string version { get; set; }
            public string updatedAt { get; set; }
            public int? revision { get; set; }
        }
        public class Asset {
            public int version { get; set; }
            public string updatedAt { get; set; }
            public string indexName { get; set; }
        }
    }
}
