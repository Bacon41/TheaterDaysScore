using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using SQLite;
using Avalonia;
using Avalonia.Platform;
using Newtonsoft.Json;
using System.Net;
using TheaterDaysScore.Models;
using TheaterDaysScore.JsonModels;

namespace TheaterDaysScore.Services {
    public class Database {
        private const string appName = "TheaterDaysScore";
        private const string settingsFile = "appstate.json";
        private const string dbName = "tdData.db";
        private const string cardDir = "cards";
        private const string cardFile = "cards.json";

        private string appDir;
        private string settingsLoc;
        private string dbLoc;
        private string cardsDir;
        private string cardsFile;

        private const string matsuriAPI = "https://api.matsurihi.me/mltd/v1/cards/";
        private const string matsuriStorage = "https://storage.matsurihi.me/mltd/icon_l/";

        private Dictionary<string, Card> allCards;
        private List<Song> allSongs;
        private List<Idol> allIdols;

        private static readonly Lazy<Database> _db = new Lazy<Database>(() => new Database());
        public static Database DB { get => _db.Value; }

        private Database() {
            // https://jimrich.sk/environment-specialfolder-on-windows-linux-and-os-x/
            appDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), appName);
            Directory.CreateDirectory(appDir);
            settingsLoc = Path.Combine(appDir, settingsFile);
            dbLoc = Path.Combine(appDir, dbName);
            cardsDir = Path.Combine(appDir, cardDir);
            Directory.CreateDirectory(cardsDir);
            cardsFile = Path.Combine(cardsDir, cardFile);

            InitData();
        }

        private List<CardData> PopulateCards() {
            string cardsFile = Path.Combine(cardsDir, cardFile);

            List<CardData> cards = new List<CardData>();
            try {
                using WebClient client = new WebClient {
                    Proxy = null
                };
                string cardInfo = client.DownloadString(matsuriAPI);

                // Validate format and save
                cards = JsonConvert.DeserializeObject<List<CardData>>(cardInfo);
                File.WriteAllText(cardsFile, cardInfo);
            } catch (Exception exception) {
                Console.WriteLine("Could not get card data: " + exception);
            }

            foreach (CardData card in cards) {
                try {
                    string imageFile = CardImagePath(card.resourceId);
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

        private void InitData() {
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
                allCardData = PopulateCards();
            } else {
                StreamReader cardReader = new StreamReader(File.OpenRead(cardsFile));
                allCardData = JsonConvert.DeserializeObject<List<CardData>>(cardReader.ReadToEnd());
            }

            List<HeldCard> allHeldCards = conn.Table<HeldCard>().ToList();

            conn.Close();

            // Storing idols
            var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
            StreamReader idolReader = new StreamReader(assets.Open(new Uri($"avares://TheaterDaysScore/Assets/idollist.json")));
            allIdols = new List<Idol>();
            List<IdolData> readIdols = JsonConvert.DeserializeObject<List<IdolData>>(idolReader.ReadToEnd());
            foreach (IdolData idolData in readIdols) {
                allIdols.Add(new Idol(idolData));
            }

            // Storing cards
            allCards = new Dictionary<string, Card>();
            foreach (CardData cardData in allCardData) {
                HeldCard heldCard = allHeldCards.Find(card => card.ID == cardData.resourceId);
                Idol idol = allIdols.Find(idol => idol.ID == cardData.idolId);
                Card card = null;
                if (heldCard != null) {
                    card = new Card(cardData, idol, true, heldCard.MasterRank, heldCard.SkillLevel);
                } else {
                    card = new Card(cardData, idol, false, 0, 0);
                }
                allCards.Add(card.ID, card);
            }

            // Song info
            allSongs = new List<Song>();
            StreamReader songReader = new StreamReader(assets.Open(new Uri($"avares://TheaterDaysScore/Assets/songlist.json")));
            List<SongData> readSongs = JsonConvert.DeserializeObject<List<SongData>>(songReader.ReadToEnd());
            foreach (SongData song in readSongs) {
                allSongs.Add(new Song(song));
            }
        }

        public string CardImagePath(string resourceId) {
            return Path.Combine(cardsDir, resourceId + "_1.png");
        }

        public List<Card> UpdateCards() {
            PopulateCards();

            InitData();

            return AllCards();
        }

        public List<Card> LoadHeld() {
            InitData();

            return AllCards();
        }

        public void SaveHeld() {
            SQLiteConnection conn = new SQLiteConnection(dbLoc, SQLiteOpenFlags.ReadWrite);

            conn.DeleteAll<HeldCard>();

            List<Card> toSave = allCards.Select(keyVal => keyVal.Value)
                .Where(card => card.IsHeld)
                .ToList();
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

        public List<Card> TopAppeal(Types songType, Calculator.BoostType eventType, int topCount, params string[] skipIds) {
            return allCards.Select(keyVal => keyVal.Value)
                .Where(card => card.IsHeld)
                .Where(card => !skipIds.Contains(card.ID))
                .OrderByDescending(card => card.TotalAppeal(songType, eventType))
                .Take(topCount)
                .ToList();
        }

        public List<Idol> GetIdols() {
            return allIdols;
        }

        public Song GetSong(int num) {
            return allSongs[num];
        }

        public string SettingsLoc() {
            return settingsLoc;
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
