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

        private HashSet<Types> types = new HashSet<Types>();
        [DataMember]
        public HashSet<Types> Types {
            get => types;
            set => this.RaiseAndSetIfChanged(ref types, value);
        }

        private HashSet<Card.Categories> categories = new HashSet<Card.Categories>();
        [DataMember]
        public HashSet<Card.Categories> Categories {
            get => categories;
            set => this.RaiseAndSetIfChanged(ref categories, value);
        }

        public IScreen HostScreen { get; }

        public string UrlPathSegment => "deckbuilder";

        public DeckBuilderViewModel(IScreen screen = null) {
            HostScreen = screen ?? Locator.Current.GetService<IScreen>();

            Items = new ObservableCollection<Card>();

            Update = ReactiveCommand.Create(() => {
                Database.DB.UpdateCards();
                FilterCards();
            });
            Save = ReactiveCommand.Create(() => {
                Database.DB.SaveHeld();
            });
            Load = ReactiveCommand.Create(() => {
                Database.DB.LoadHeld();
                FilterCards();
            });

            MaxRank = ReactiveCommand.Create(() => {
                foreach (Card card in Items) {
                    card.MasterRank = card.MasterRanks.Last();
                }
                FilterCards();
            });
            MaxLevel = ReactiveCommand.Create(() => {
                foreach (Card card in Items) {
                    card.SkillLevel = card.SkillLevels.Last();
                }
                FilterCards();
            });
        }

        public void FilterCards() {
            Items.Clear();
            Items.AddRange(Database.DB.AllCards()
                .Where(card => Rarities.Contains(card.Rarity))
                .Where(card => Types.Contains(card.Type))
                .Where(card => Categories.Contains(card.Category))
                );
        }

        public ObservableCollection<Card> Items { get; }

        public ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit> Update { get; }
        public ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit> Save { get; }
        public ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit> Load { get; }

        public ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit> MaxRank { get; }
        public ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit> MaxLevel { get; }
    }
}
