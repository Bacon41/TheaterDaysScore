namespace TheaterDaysScore.JsonModels {
    public class IdolData {
        public int id { get; set; }
        public string colour { get; set; }

        public IdolData(int id, string colour) {
            this.id = id;
            this.colour = colour;

            // http://mirishitadb.php.xdomain.jp/db/#profile
        }
    }
}
