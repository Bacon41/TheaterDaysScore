using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;

namespace TheaterDaysScore {
    public class Idol {
        public int id { get; set; }
        public string colour { get; set; }

        public Idol(int id, string colour) {
            this.id = id;
            this.colour = colour;

            // http://mirishitadb.php.xdomain.jp/db/#profile
        }
    }
}
