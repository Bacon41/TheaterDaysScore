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
    public class DeckBuilderViewModel : ReactiveObject, IRoutableViewModel {

        private HashSet<CardData.Rarities> rarities = new HashSet<CardData.Rarities>();
        [DataMember]
        public HashSet<CardData.Rarities> Rarities {
            get => rarities;
            set => this.RaiseAndSetIfChanged(ref rarities, value);
        }

        [DataMember]
        private HashSet<Types> types = new HashSet<Types>();
        [DataMember]
        public HashSet<Types> Types {
            get => types;
            set => this.RaiseAndSetIfChanged(ref types, value);
        }

        public IScreen HostScreen { get; }

        public string UrlPathSegment => "deckbuilder";

        public DeckBuilderViewModel(IScreen screen = null) {
            HostScreen = screen ?? Locator.Current.GetService<IScreen>();

            Items = new ObservableCollection<Card>();
            Rarities = new HashSet<CardData.Rarities>();
            Types = new HashSet<Types>();

            Update = ReactiveCommand.Create(() => {
                Items.Clear();
                Items.AddRange(Database.DB.UpdateCards());
            });
            Save = ReactiveCommand.Create(() => {
                Database.DB.SaveHeld();
            });
        }

        public void FilterCards() {
            Items.Clear();
            Items.AddRange(Database.DB.AllCards()
                .Where(card => Rarities.Contains(card.Rarity))
                .Where(card => Types.Contains(card.Type))
                );
        }

        public ObservableCollection<Card> Items { get; }

        public ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit> Update { get; }
        public ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit> Save { get; }
    }
}
