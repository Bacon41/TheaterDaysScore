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
using AssetStudio;
using MessagePack;
using System.Text.Encodings.Web;
using System.ComponentModel;

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

        private const string matsuriAPI = "https://api.matsurihi.me/api/mltd/v2/cards/?includeParameters=true&includeSkills=true";
        private const string matsuriStorage = "https://storage.matsurihi.me/mltd/icon_l/";
        private const string m_ltdSongAPI = "https://api.39m.ltd/api/fetch/all_song_info";
        private const string m_ltdImage = "https://storage.39m.ltd/img/";
        private const string matsuriVerionAPI = "https://api.matsurihi.me/api/mltd/v2/version/latest";
        private const string bn765API = "https://td-assets.bn765.com";

        private Dictionary<string, Card> allCards;
        private Dictionary<string, Song> allSongs;
        private List<Idol> allIdols;

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

        private void DownloadCards() {
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
        }

        private void DownloadSongs() {
            // Download song beatmaps
            DownloadAssets();

            // Get song info
            List<SongList> songs = new List<SongList>();
            try {
                using WebClient client = new WebClient {
                    Proxy = null
                };
                string songInfo = client.DownloadString(m_ltdSongAPI);

                // Validate format and save
                songs = JsonSerializer.Deserialize<List<SongList>>(songInfo);
                string jsonOutput = JsonSerializer.Serialize(songs, new JsonSerializerOptions { WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping });
                File.WriteAllText(songsFilePath, jsonOutput);
            } catch (Exception exception) {
                Console.WriteLine("Could not get song data: " + exception);
            }

            foreach (SongList song in songs) {
                try {
                    string imageFile = SongImagePath(song.asset);
                    string imageURL = m_ltdImage + "jacket_" + song.asset + ".png";

                    if (!File.Exists(imageFile)) {
                        using WebClient client = new WebClient {
                            Proxy = null
                        };
                        client.DownloadFileAsync(new Uri(imageURL), imageFile);
                    }
                } catch (Exception exception) {
                    Console.WriteLine("Could not get song image: " + exception);
                }
            }
        }

        private void DownloadAssets() {
            // Ensure most recent index is downloaded
            string assetBaseURL;
            string versionIndexFile;
            try {
                using WebClient client = new WebClient {
                    Proxy = null
                };
                // Get the latest version
                string indexInfo = client.DownloadString(matsuriVerionAPI);

                // Validate format and save
                VersionInfo latestVersion = JsonSerializer.Deserialize<VersionInfo>(indexInfo);
                assetBaseURL = bn765API + "/" + latestVersion.asset.version.ToString() + bn765AssetPath;
                versionIndexFile = Path.Combine(appDir, latestVersion.asset.indexName);

                // If the newest is not downloaded, fetch it
                if (!File.Exists(versionIndexFile)) {
                    Uri versionIndexURL = new Uri(assetBaseURL + latestVersion.asset.indexName);
                    client.DownloadFile(versionIndexURL, versionIndexFile);

                    // Clean up the old version index
                    if (File.Exists(versionFilePath)) {
                        StreamReader versionReader = new StreamReader(File.OpenRead(versionFilePath));
                        VersionInfo currentVersion = JsonSerializer.Deserialize<VersionInfo>(versionReader.ReadToEnd());
                        versionReader.Close();

                        string prevVersionIndexFile = Path.Combine(appDir, currentVersion.asset.indexName);
                        if (File.Exists(prevVersionIndexFile)) {
                            File.Delete(prevVersionIndexFile);
                        }
                    }

                    // Save newest version info
                    File.WriteAllText(versionFilePath, indexInfo);
                }
            } catch (Exception exception) {
                Console.WriteLine("Could not get index info: " + exception);
                return;
            }

            // Extract relevant assets from index
            List<Asset> assetList = new List<Asset>();
            try {
                // https://github.com/OpenMLTD/MLTDTools/blob/master/src/MiriTore.Database/AssetInfoList.Parsing.cs

                byte[] data = File.ReadAllBytes(versionIndexFile);
                object[] deserialized = MessagePackSerializer.Deserialize<object[]>(data);

                var dict = deserialized[0] as IDictionary<object, object>;

                foreach (var kv in dict) {
                    string resourceName = kv.Key as string;
                    object[] arr = kv.Value as object[];

                    string contentHash = arr[0] as string;
                    string remoteName = arr[1] as string;

                    // Limit to just beatmaps
                    if (resourceName.Contains("scrobj")) {
                        var assetInfo = new Asset(resourceName, contentHash, remoteName);
                        assetList.Add(assetInfo);
                    }
                }
            } catch (Exception exception) {
                Console.WriteLine("Could not extract asset info: " + exception);
                return;
            }

            try {
                using WebClient client = new WebClient {
                    Proxy = null
                };

                // Download encoded data
                foreach (Asset asset in assetList) {
                    string assetPath = Path.Combine(songsDirPath, asset.Name);
                    if (!File.Exists(assetPath)) {
                        Uri versionAssetURL = new Uri(assetBaseURL + asset.Hash);
                        client.DownloadFile(versionAssetURL, assetPath);
                    }
                }

                // https://github.com/OpenMLTD/MLTDTools/blob/master/src/ExtractAcb/Program.cs

                // Decode downloaded file and save info as json
                AssetsManager manager = new AssetsManager();
                foreach (Asset asset in assetList) {
                    manager.Clear();

                    string assetPath = Path.Combine(songsDirPath, asset.Name);
                    manager.LoadFiles(assetPath);
                    foreach (var assetFile in manager.assetsFileList) {
                        foreach (var obj in assetFile.Objects) {
                            if (obj.type != ClassIDType.MonoBehaviour) {
                                continue;
                            }
                            MonoBehaviour behaviourObj = obj as MonoBehaviour;

                            // This one contains the beatmap, skip the others
                            if (!behaviourObj.m_Name.Contains("fumen")) {
                                continue;
                            }

                            string songFileName = behaviourObj.m_Name + ".json";
                            string songFilePath = Path.Combine(songsDirPath, songFileName);

                            if (!File.Exists(songFilePath)) {
                                // https://github.com/Perfare/AssetStudio/blob/master/AssetStudioGUI/Exporter.cs

                                var fullObjectData = behaviourObj.ToType();
                                string jsonOutput = JsonSerializer.Serialize(fullObjectData, new JsonSerializerOptions { WriteIndented = true });
                                File.WriteAllText(songFilePath, jsonOutput);
                            }
                        }
                    }
                }
            } catch (Exception exception) {
                Console.WriteLine("Could not download/extract asset info: " + exception);
                return;
            }
        }

        // Load cached/saved data, but do not make network calls
        private void InitData() {
            // Database for saving owned cards and their levels
            // https://github.com/praeclarum/sqlite-net/wiki/Synchronous-API
            SQLiteConnection conn;
            if (File.Exists(dbFilePath)) {
                conn = new SQLiteConnection(dbFilePath, SQLiteOpenFlags.ReadWrite);
            } else {
                conn = new SQLiteConnection(dbFilePath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex);
                conn.CreateTable<HeldCard>();
            }
            List<HeldCard> allHeldCards = conn.Table<HeldCard>().ToList();
            conn.Close();

            // Load cached card info
            List<CardData> allCardData = new List<CardData>();
            if (File.Exists(cardsFilePath)) {
                StreamReader cardReader = new StreamReader(File.OpenRead(cardsFilePath));
                allCardData = JsonSerializer.Deserialize<List<CardData>>(cardReader.ReadToEnd());
                cardReader.Close();
            }

            // Load stored idol info
            var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
            StreamReader idolReader = new StreamReader(assets.Open(new Uri($"avares://TheaterDaysScore/Assets/idollist.json")));
            allIdols = new List<Idol>();
            List<IdolData> readIdols = JsonSerializer.Deserialize<List<IdolData>>(idolReader.ReadToEnd());
            idolReader.Close();
            foreach (IdolData idolData in readIdols) {
                allIdols.Add(new Idol(idolData));
            }

            // Configure cards according to info from database
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

            // Load cached song info
            List<SongList> allSongData = new List<SongList>();
            if (File.Exists(songsFilePath)) {
                StreamReader songsReader = new StreamReader(File.OpenRead(songsFilePath));
                allSongData = JsonSerializer.Deserialize<List<SongList>>(songsReader.ReadToEnd());
                songsReader.Close();
            }

            // Confiugre songs according to downloaded data
            allSongs = new Dictionary<string, Song>();
            foreach(SongList songData in allSongData) {
                string beatmapFile = Path.Combine(songsDirPath, songData.asset + "_fumen_sobj.json");
                if (File.Exists(beatmapFile)) {
                    StreamReader songReader = new StreamReader(File.OpenRead(beatmapFile));
                    SongData data = JsonSerializer.Deserialize<SongData>(songReader.ReadToEnd());
                    songReader.Close();
                    Song song = new Song(songData, data);
                    allSongs.Add(songData.asset, song);
                }
            }
        }

        public string CardImagePath(string resourceId) {
            return Path.Combine(cardsDirPath, resourceId + "_1.png");
        }

        public string SongImagePath(string resourceId) {
            return Path.Combine(songsDirPath, "jacket_" + resourceId + ".png");
        }

        public List<Card> UpdateCards() {
            DownloadCards();

            InitData();

            return AllCards();
        }

        public List<Song> UpdateSongs() {
            DownloadSongs();

            InitData();

            return AllSongs();
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

        public Song GetSong(string id) {
            return allSongs[id];
        }

        public List<Song> AllSongs() {
            return allSongs.Select(keyVal => keyVal.Value).ToList();
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
