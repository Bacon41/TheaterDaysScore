using Avalonia.Controls;
using DynamicData;
using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using TheaterDaysScore.JsonModels;
using TheaterDaysScore.Models;
using TheaterDaysScore.Services;

namespace TheaterDaysScore.ViewModels {
    [DataContract]
    public class UnitBuilderViewModel : ReactiveObject, IRoutableViewModel {

        // Filters
        private HashSet<CardData.Rarities> rarities = new HashSet<CardData.Rarities>();
        [DataMember]
        public HashSet<CardData.Rarities> Rarities {
            get => rarities;
            set => this.RaiseAndSetIfChanged(ref rarities, value);
        }

        private HashSet<Types> types = new HashSet<Types>();
        [DataMember]
        public HashSet<Types> Types {
            get => types;
            set => this.RaiseAndSetIfChanged(ref types, value);
        }

        // Unit
        private string guest = "";
        [DataMember]
        public string Guest {
            get => guest;
            set => this.RaiseAndSetIfChanged(ref guest, value);
        }

        private string center = "";
        [DataMember]
        public string Center {
            get => center;
            set => this.RaiseAndSetIfChanged(ref center, value);
        }

        private string member1 = "";
        [DataMember]
        public string Member1 {
            get => member1;
            set => this.RaiseAndSetIfChanged(ref member1, value);
        }

        private string member2 = "";
        [DataMember]
        public string Member2 {
            get => member2;
            set => this.RaiseAndSetIfChanged(ref member2, value);
        }

        private string member3 = "";
        [DataMember]
        public string Member3 {
            get => member3;
            set => this.RaiseAndSetIfChanged(ref member3, value);
        }

        private string member4 = "";
        [DataMember]
        public string Member4 {
            get => member4;
            set => this.RaiseAndSetIfChanged(ref member4, value);
        }

        public IScreen HostScreen { get; }

        public string UrlPathSegment => "deckbuilder";

        public UnitBuilderViewModel(IScreen screen = null) {
            HostScreen = screen ?? Locator.Current.GetService<IScreen>();

            Items = new ObservableCollection<Card>();
            Rarities = new HashSet<CardData.Rarities>();
            Types = new HashSet<Types>();
        }

        public void FilterCards() {
            Items.Clear();
            Items.AddRange(Database.DB.AllCards()
                .Where(card => Rarities.Contains(card.Rarity))
                .Where(card => Types.Contains(card.Type))
                );
        }

        public Unit BuildUnit() {
            return new Unit(Guest, Center, Member1, Member2, Member3, Member4);
        }

        public ObservableCollection<Card> Items { get; }
    }
}
