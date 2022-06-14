using DynamicData;
using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using TheaterDaysScore.Models;
using TheaterDaysScore.Services;

namespace TheaterDaysScore.ViewModels {
    [DataContract]
    public class SongPickerViewModel : ReactiveObject, IRoutableViewModel {

        // Filters
        private HashSet<Types> types = new HashSet<Types>();
        [DataMember]
        public HashSet<Types> Types {
            get => types;
            set => this.RaiseAndSetIfChanged(ref types, value);
        }

        // Song
        private string songID = "";
        [DataMember]
        public string SongID {
            get => songID;
            set => this.RaiseAndSetIfChanged(ref songID, value);
        }

        private Song song = null;
        public Song Song {
            get => song;
            set => this.RaiseAndSetIfChanged(ref song, value);
        }

        public IScreen HostScreen { get; }

        public string UrlPathSegment => "songpicker";

        public SongPickerViewModel(IScreen screen = null) {
            HostScreen = screen ?? Locator.Current.GetService<IScreen>();

            Items = new ObservableCollection<Song>();

            Update = ReactiveCommand.Create(() => {
                Database.DB.UpdateSongs();
                FilterSongs();
            });

            var allTypes = (Types[])Enum.GetValues(typeof(Types));
            AllTypes = ReactiveCommand.Create(() => {
                if (Types.Count == allTypes.Length) {
                    Types = new HashSet<Types>();
                } else {
                    Types = new HashSet<Types>(allTypes);
                }
            });
        }

        public void FilterSongs() {
            Items.Clear();
            Items.AddRange(Database.DB.AllSongs()
                .Where(card => Types.Contains(card.Type))
                );
        }

        public void SetSongID(Song song) {
            if (song == null) {
                return;
            }
            SongID = song.Asset;
            SetSong();
        }

        public void SetSong() {
            if (SongID == "") {
                return;
            }
            Song = Database.DB.GetSong(SongID);
        }

        public ObservableCollection<Song> Items { get; }

        public ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit> Update { get; }

        public ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit> AllTypes { get; }
    }
}
