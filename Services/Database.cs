using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using SQLite;
using Avalonia;
using Avalonia.Platform;
using System.Net;
using System.Text.Json;
using TheaterDaysScore.Models;
using TheaterDaysScore.JsonModels;

namespace TheaterDaysScore.Services {
    public class Database {
        private const string appName = "TheaterDaysScore";
        private const string settingsFileName = "appstate.json";
        private const string dbFileName = "tdData.db";
        private const string cardsDirName = "cards";
        private const string cardsFileName = "cards.json";
        private const string songsDirName = "songs";
        private const string songsFileName = "songs.json";
        private const string versionFileName = "version.json";
        private const string bn765AssetPath = "/production/2018/Android/";

        private string appDir;
        private string settingsFilePath;
        private string dbFilePath;
        private string cardsDirPath;
        private string cardsFilePath;
        private string songsDirPath;
        private string songsFilePath;
        private string versionFilePath;

        private const string matsuriAPI = "https://api.matsurihi.me/mltd/v1/cards/";
        private const string matsuriStorage = "https://storage.matsurihi.me/mltd/icon_l/";
        private const string m_ltdAPI = "https://api.39m.ltd/api/fetch/all_song_info";
        private const string matsuriVerionAPI = "https://api.matsurihi.me/mltd/v1/version/latest";
        private const string bn765API = "https://td-assets.bn765.com";

        private Dictionary<string, Card> allCards;
        private List<Song> allSongs;
        private List<Idol> allIdols;

        private Song2 harmonics;

        private static readonly Lazy<Database> _db = new Lazy<Database>(() => new Database());
        public static Database DB { get => _db.Value; }

        private Database() {
            // https://jimrich.sk/environment-specialfolder-on-windows-linux-and-os-x/
            appDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), appName);
            Directory.CreateDirectory(appDir);
            settingsFilePath = Path.Combine(appDir, settingsFileName);
            dbFilePath = Path.Combine(appDir, dbFileName);
            cardsDirPath = Path.Combine(appDir, cardsDirName);
            Directory.CreateDirectory(cardsDirPath);
            cardsFilePath = Path.Combine(cardsDirPath, cardsFileName);
            songsDirPath = Path.Combine(appDir, songsDirName);
            Directory.CreateDirectory(songsDirPath);
            songsFilePath = Path.Combine(songsDirPath, songsFileName);
            versionFilePath = Path.Combine(appDir, versionFileName);

            InitData();
        }

        private List<CardData> PopulateCards() {
            List<CardData> cards = new List<CardData>();
            try {
                using WebClient client = new WebClient {
                    Proxy = null
                };
                string cardInfo = client.DownloadString(matsuriAPI);

                // Validate format and save
                cards = JsonSerializer.Deserialize<List<CardData>>(cardInfo);
                File.WriteAllText(cardsFilePath, cardInfo);
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

        private List<SongList> PopulateSongs() {
            List<SongList> songs = new List<SongList>();
            try {
                using WebClient client = new WebClient {
                    Proxy = null
                };
                string songInfo = client.DownloadString(m_ltdAPI);

                // Validate format and save
                songs = JsonSerializer.Deserialize<List<SongList>>(songInfo);
                File.WriteAllText(songsFilePath, songInfo);
            } catch (Exception exception) {
                Console.WriteLine("Could not get song data: " + exception);
            }

            return songs;
        }

        private VersionInfo PopulateAssets() {
            VersionInfo currentVersion = null;
            if (File.Exists(versionFilePath)) {
                StreamReader versionReader = new StreamReader(File.OpenRead(versionFilePath));
                currentVersion = JsonSerializer.Deserialize<VersionInfo>(versionReader.ReadToEnd());
            }
            VersionInfo latestVersion;
            try {
                using WebClient client = new WebClient {
                    Proxy = null
                };
                // Get the latest version
                string indexInfo = client.DownloadString(matsuriVerionAPI);

                // Validate format and save
                latestVersion = JsonSerializer.Deserialize<VersionInfo>(indexInfo);
                string versionIndexFile = Path.Combine(appDir, latestVersion.res.indexName);

                // If the newest is not downloaded, fetch it
                if (!File.Exists(versionIndexFile)) {
                    Uri versionIndexURL = new Uri(bn765API + "/" + latestVersion.res.version.ToString() + bn765AssetPath + latestVersion.res.indexName);
                    client.DownloadFile(versionIndexURL, versionIndexFile);
                    // Save newest version info
                    File.WriteAllText(versionFilePath, indexInfo);

                    // Clean up the old version index
                    if (currentVersion != null) {
                        string prevVersionIndexFile = Path.Combine(appDir, currentVersion.res.indexName);
                        if (File.Exists(prevVersionIndexFile)) {
                            File.Delete(prevVersionIndexFile);
                        }
                    }
                }
                currentVersion = latestVersion;
            } catch (Exception exception) {
                Console.WriteLine("Could not get index info: " + exception);
            }

            return currentVersion;
        }

        private void InitData() {
            SQLiteConnection conn;
            if (File.Exists(dbFilePath)) {
                conn = new SQLiteConnection(dbFilePath, SQLiteOpenFlags.ReadWrite);
            } else {
                conn = new SQLiteConnection(dbFilePath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex);
                conn.CreateTable<HeldCard>();
            }

            // https://github.com/praeclarum/sqlite-net/wiki/Synchronous-API

            List<CardData> allCardData = new List<CardData>();
            if (!File.Exists(cardsFilePath)) {
                allCardData = PopulateCards();
            } else {
                StreamReader cardReader = new StreamReader(File.OpenRead(cardsFilePath));
                allCardData = JsonSerializer.Deserialize<List<CardData>>(cardReader.ReadToEnd());
            }

            List<HeldCard> allHeldCards = conn.Table<HeldCard>().ToList();

            conn.Close();

            // Storing idols
            var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
            StreamReader idolReader = new StreamReader(assets.Open(new Uri($"avares://TheaterDaysScore/Assets/idollist.json")));
            allIdols = new List<Idol>();
            List<IdolData> readIdols = JsonSerializer.Deserialize<List<IdolData>>(idolReader.ReadToEnd());
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
            List<SongData> readSongs = JsonSerializer.Deserialize<List<SongData>>(songReader.ReadToEnd());
            foreach (SongData song in readSongs) {
                allSongs.Add(new Song(song));
            }

            StreamReader songReader2 = new StreamReader(assets.Open(new Uri($"avares://TheaterDaysScore/Assets/harmon_fumen_sobj.json")));
            SongData2 readHarmonics = JsonSerializer.Deserialize<SongData2>(songReader2.ReadToEnd());
            harmonics = new Song2(readHarmonics);

            List<SongList> allSongData = new List<SongList>();
            if (!File.Exists(songsFilePath)) {
                allSongData = PopulateSongs();
            } else {
                StreamReader songsReader = new StreamReader(File.OpenRead(songsFilePath));
                allSongData = JsonSerializer.Deserialize<List<SongList>>(songsReader.ReadToEnd());
            }

            PopulateAssets();
        }

        public string CardImagePath(string resourceId) {
            return Path.Combine(cardsDirPath, resourceId + "_1.png");
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
            SQLiteConnection conn = new SQLiteConnection(dbFilePath, SQLiteOpenFlags.ReadWrite);

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

        public Song2 GetSong2() {
            return harmonics;
        }

        public string SettingsLoc() {
            return settingsFilePath;
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
