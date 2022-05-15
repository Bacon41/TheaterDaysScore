using TheaterDaysScore.JsonModels;

namespace TheaterDaysScore.Models {
    public class Idol {
        private IdolData data;

        public int ID { get; set; }
        public string Name { get; set; }
        public string NameKanji { get; set; }
        public string Colour { get; set; }

        public Idol(IdolData data) {
            this.data = data;

            ID = this.data.id;
            Name = this.data.name_eng;
            NameKanji = this.data.name;
            Colour = "#" + this.data.colour;
        }
    }
}
