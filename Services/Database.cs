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
using System.Net.Http;
using System.Net;
using TheaterDaysScore.Models;

namespace TheaterDaysScore.Services {
    public class Database {
        private const string appName = "MirishitaScore";
        private const string dbName = "tdData.db";
        private const string cardDir = "cards";
        private const string cardFile = "cards.json";

        private const string matsuriAPI = "https://api.matsurihi.me/mltd/v1/cards/";
        private const string matsuriStorage = "https://storage.matsurihi.me/mltd/icon_l/";

        private Dictionary<string, Card> allCards;

        private static readonly Lazy<Database> _db = new Lazy<Database>(() => new Database());
        public static Database DB { get => _db.Value; }

        private List<CardData> PopulateCards(string cardsDir) {
            string cardsFile = Path.Combine(cardsDir, cardFile);

            List<CardData> cards = new List<CardData>();
            try {
                using WebClient client = new WebClient {
                    Proxy = null
                };
                string cardInfo = client.DownloadString(matsuriAPI);

                // Validate format and save
                cards = JsonSerializer.Deserialize<List<CardData>>(cardInfo);
                File.WriteAllText(cardsFile, cardInfo);
            } catch (Exception exception) {
                Console.WriteLine("Could not get card data: " + exception);
            }

            foreach (CardData card in cards) {
                try {
                    string imageFile = Path.Combine(cardsDir, card.resourceId + ".png");
                    string imageURL = matsuriStorage + card.resourceId + "_1.png";

                    if (!File.Exists(imageFile)) {
                        using WebClient client = new WebClient {
                            Proxy = null
                        };
                        client.DownloadFileAsync(new Uri(imageURL), imageFile);
                    }
                } catch (Exception exception) {
                    Console.WriteLine("Could not get card image: " + exception);
                }
            }

            return cards;
        }

        private Database() {
            InitData();
        }

        private void InitData() {
            // https://jimrich.sk/environment-specialfolder-on-windows-linux-and-os-x/
            string appDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), appName);
            Directory.CreateDirectory(appDir);
            string dbLoc = Path.Combine(appDir, dbName);
            string cardsDir = Path.Combine(appDir, cardDir);
            Directory.CreateDirectory(cardsDir);
            string cardsFile = Path.Combine(cardsDir, cardFile);

            SQLiteConnection conn;
            if (File.Exists(dbLoc)) {
                conn = new SQLiteConnection(dbLoc, SQLiteOpenFlags.ReadWrite);
            } else {
                conn = new SQLiteConnection(dbLoc, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex);
                conn.CreateTable<HeldCard>();
            }

            // https://github.com/praeclarum/sqlite-net/wiki/Synchronous-API

            List<CardData> allCardData = new List<CardData>();
            if (!File.Exists(cardsFile)) {
                allCardData = PopulateCards(cardsDir);
            } else {
                StreamReader cardReader = new StreamReader(File.OpenRead(cardsFile));
                allCardData = JsonSerializer.Deserialize<List<CardData>>(cardReader.ReadToEnd());
            }

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

            // Idol info
            var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
            StreamReader idolReader = new StreamReader(assets.Open(new Uri($"avares://TheaterDaysScore/Assets/idollist.json")));
            List<Idol> idols = JsonSerializer.Deserialize<List<Idol>>(idolReader.ReadToEnd());

            // Storing cards
            allCards = new Dictionary<string, Card>();
            foreach (CardData cardData in allCardData) {
                HeldCard heldCard = allHeldCards.Find(card => card.ID == cardData.resourceId);
                Idol idolData = idols.Find(idol => idol.id == cardData.idolId);
                Card card = null;
                if (heldCard != null) {
                    card = new Card(cardData, idolData, true, heldCard.MasterRank, heldCard.SkillLevel);
                } else {
                    card = new Card(cardData, idolData, false, 0, 0);
                }
                allCards.Add(card.ID, card);
            }
        }

        public List<Card> UpdateCards() {
            string appDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), appName);
            string cardsDir = Path.Combine(appDir, cardDir);
            PopulateCards(cardsDir);

            InitData();

            return AllCards();
        }

        public void SaveHeld() {
            string appDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), appName);
            string dbLoc = Path.Combine(appDir, dbName);

            SQLiteConnection conn = new SQLiteConnection(dbLoc, SQLiteOpenFlags.ReadWrite);

            conn.DeleteAll<HeldCard>();

            List<Card> toSave = allCards.Select(keyVal => keyVal.Value)
                .Where(card => card.IsHeld).ToList();
            foreach(Card card in toSave) {
                conn.Insert(new HeldCard { ID = card.ID, MasterRank = card.MasterRank, SkillLevel = card.SkillLevel });
            }

            conn.Close();
        }

        public List<Card> AllCards() {
            return allCards.Select(keyVal => keyVal.Value).ToList();
        }

        public Card GetCard(string id) {
            return allCards[id];
        }

        public List<Card> TopAppeal(Types songType, int topCount, params string[] skipIds) {
            return allCards.Select(keyVal => keyVal.Value)
                .Where(card => card.IsHeld)
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
