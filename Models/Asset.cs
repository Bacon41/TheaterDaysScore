namespace TheaterDaysScore.Models {
    public class Asset {
        public string Name { get; }
        public string RemoteName { get; }
        public string Hash { get; }

        public Asset(string name, string remoteName, string hash) {
            Name = name;
            RemoteName = remoteName;
            Hash = hash;
        }
    }
}
