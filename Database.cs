using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using SQLite;

namespace TheaterDaysScore {
    class Database {
        private const string appName = "MirishitaScore";
        private const string dbName = "tdData.db";

        private SQLiteConnection conn;

        public Database() {
            // https://jimrich.sk/environment-specialfolder-on-windows-linux-and-os-x/
            string appDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), appName);
            Directory.CreateDirectory(appDir);
            string dbLoc = Path.Combine(appDir, dbName);
            if (File.Exists(dbLoc)) {
                conn = new SQLiteConnection(dbLoc, SQLiteOpenFlags.ReadWrite);
            } else {
                conn = new SQLiteConnection(dbLoc, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex);
            }
            conn.CreateTable<Card.CenterEffect>();
            conn.CreateTable<Card.Skill>();
            conn.CreateTable<Card>();
            // https://github.com/praeclarum/sqlite-net/wiki/Synchronous-API
            // https://alialhaddad.medium.com/how-to-fetch-data-in-c-net-core-ea1ab720e3f9
        }
    }
}
