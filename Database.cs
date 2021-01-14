using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using SQLite;
using Avalonia;
using Avalonia.Platform;
using System.Text.Json;

namespace TheaterDaysScore {
    class Database {
        private const string appName = "MirishitaScore";
        private const string dbName = "tdData.db";

        private Dictionary<string, Card> allCards;
        private Dictionary<string, Card> heldCards;

        private static readonly Lazy<Database> _db = new Lazy<Database>(() => new Database());
        public static Database DB { get => _db.Value; }

        private Database() {
            // https://jimrich.sk/environment-specialfolder-on-windows-linux-and-os-x/
            string appDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), appName);
            Directory.CreateDirectory(appDir);
            string dbLoc = Path.Combine(appDir, dbName);

            SQLiteConnection conn;
            if (File.Exists(dbLoc)) {
                conn = new SQLiteConnection(dbLoc, SQLiteOpenFlags.ReadWrite);
            } else {
                conn = new SQLiteConnection(dbLoc, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex);
                conn.CreateTable<HeldCard>();
            }

            // https://github.com/praeclarum/sqlite-net/wiki/Synchronous-API

            /*conn.Insert(new HeldCard { ID = "031tom0164", MasterRank = 4, SkillLevel = 10 });
            conn.Insert(new HeldCard { ID = "007ior0084", MasterRank = 4, SkillLevel = 10 });
            conn.Insert(new HeldCard { ID = "020meg0084", MasterRank = 5, SkillLevel = 12 });
            conn.Insert(new HeldCard { ID = "038chz0034", MasterRank = 5, SkillLevel = 12 });
            conn.Insert(new HeldCard { ID = "009rit0084", MasterRank = 5, SkillLevel = 12 });*/
            /*conn.Insert(new HeldCard { ID = "044miz0224", MasterRank = 0, SkillLevel = 5 });
            conn.Insert(new HeldCard { ID = "033sih0114", MasterRank = 0, SkillLevel = 5 });
            conn.Insert(new HeldCard { ID = "046rio0204", MasterRank = 0, SkillLevel = 5 });
            conn.Insert(new HeldCard { ID = "051tmg0164", MasterRank = 0, SkillLevel = 5 });
            conn.Insert(new HeldCard { ID = "049mom0144", MasterRank = 0, SkillLevel = 5 });
            conn.Insert(new HeldCard { ID = "020meg0114", MasterRank = 0, SkillLevel = 5 });
            conn.Insert(new HeldCard { ID = "008tak0094", MasterRank = 0, SkillLevel = 5 });
            conn.Insert(new HeldCard { ID = "009rit0124", MasterRank = 4, SkillLevel = 10 });
            conn.Insert(new HeldCard { ID = "009rit0064", MasterRank = 5, SkillLevel = 12 });
            conn.Insert(new HeldCard { ID = "202xxx0023", MasterRank = 4, SkillLevel = 10 });*/
            //conn.Delete<HeldCard>(1);

            List<HeldCard> allHeldCards = conn.Table<HeldCard>().ToList();

            conn.Close();
            
            // https://alialhaddad.medium.com/how-to-fetch-data-in-c-net-core-ea1ab720e3f9

            var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
            StreamReader cardReader = new StreamReader(assets.Open(new Uri($"avares://TheaterDaysScore/res/cardlist.json")));
            List<CardData> allCardData = JsonSerializer.Deserialize<List<CardData>>(cardReader.ReadToEnd());

            allCards = new Dictionary<string, Card>();
            heldCards = new Dictionary<string, Card>();
            foreach (CardData cardData in allCardData) {
                HeldCard heldCard = allHeldCards.Find(card => card.ID == cardData.resourceId);
                Card card = null;
                if (heldCard != null) {
                    card = new Card(cardData, heldCard.MasterRank, heldCard.SkillLevel);
                    heldCards.Add(card.ID, card);
                } else {
                    card = new Card(cardData, 0, 0);
                }
                allCards.Add(card.ID, card);
            }
        }

        public List<Card> AllCards() {
            return allCards.Select(keyVal => keyVal.Value).ToList();
        }

        public Card GetCard(string id) {
            return allCards[id];
        }

        public List<Card> TopAppeal(Types songType, int topCount, params string[] skipIds) {
            return heldCards.Select(keyVal => keyVal.Value)
                .Where(card => !skipIds.Contains(card.ID))
                .OrderByDescending(card => card.TotalAppeal(songType))
                .Take(topCount)
                .ToList();
        }

        [Table("held_cards")]
        public class HeldCard {
            [PrimaryKey]
            [Column("id")]
            public string ID { get; set; }

            [Column("master_rank")]
            public int MasterRank { get; set; }

            [Column("skill_level")]
            public int SkillLevel { get; set; }
        }
    }
}
