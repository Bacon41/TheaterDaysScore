using TheaterDaysScore.JsonModels;

namespace TheaterDaysScore.Models {
    public class Idol {
        private IdolData data;

        public int ID { get; set; }
        public string Name { get; set; }
        public string Class { get; set; }
        public string Colour { get; set; }

        public Idol(IdolData data) {
            this.data = data;

            ID = this.data.id;
            Name = this.data.name;
            Class = this.data.classField;
            Colour = "#" + this.data.colour;
        }
    }
}
