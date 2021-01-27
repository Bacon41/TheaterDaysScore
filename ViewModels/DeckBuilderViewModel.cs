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
            FilterCards();

            SSRFilter = ReactiveCommand.Create((bool isChecked) => {
                if (isChecked) {
                    Rarities.Add(CardData.Rarities.SSR);
                } else {
                    Rarities.Remove(CardData.Rarities.SSR);
                }
                FilterCards();
            });
            SRFilter = ReactiveCommand.Create((bool isChecked) => {
                if (isChecked) {
                    Rarities.Add(CardData.Rarities.SR);
                } else {
                    Rarities.Remove(CardData.Rarities.SR);
                }
                FilterCards();
            });
            RFilter = ReactiveCommand.Create((bool isChecked) => {
                if (isChecked) {
                    Rarities.Add(CardData.Rarities.R);
                } else {
                    Rarities.Remove(CardData.Rarities.R);
                }
                FilterCards();
            });
            NFilter = ReactiveCommand.Create((bool isChecked) => {
                if (isChecked) {
                    Rarities.Add(CardData.Rarities.N);
                } else {
                    Rarities.Remove(CardData.Rarities.N);
                }
                FilterCards();
            });

            PrincessFilter = ReactiveCommand.Create((bool isChecked) => {
                if (isChecked) {
                    Types.Add(TheaterDaysScore.Types.Princess);
                } else {
                    Types.Remove(TheaterDaysScore.Types.Princess);
                }
                FilterCards();
            });
            FairyFilter = ReactiveCommand.Create((bool isChecked) => {
                if (isChecked) {
                    Types.Add(TheaterDaysScore.Types.Fairy);
                } else {
                    Types.Remove(TheaterDaysScore.Types.Fairy);
                }
                FilterCards();
            });
            AngelFilter = ReactiveCommand.Create((bool isChecked) => {
                if (isChecked) {
                    Types.Add(TheaterDaysScore.Types.Angel);
                } else {
                    Types.Remove(TheaterDaysScore.Types.Angel);
                }
                FilterCards();
            });
            EXFilter = ReactiveCommand.Create((bool isChecked) => {
                if (isChecked) {
                    Types.Add(TheaterDaysScore.Types.EX);
                } else {
                    Types.Remove(TheaterDaysScore.Types.EX);
                }
                FilterCards();
            });

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

        public ReactiveCommand<bool, System.Reactive.Unit> SSRFilter { get; }
        public ReactiveCommand<bool, System.Reactive.Unit> SRFilter { get; }
        public ReactiveCommand<bool, System.Reactive.Unit> RFilter { get; }
        public ReactiveCommand<bool, System.Reactive.Unit> NFilter { get; }

        public ReactiveCommand<bool, System.Reactive.Unit> PrincessFilter { get; }
        public ReactiveCommand<bool, System.Reactive.Unit> FairyFilter { get; }
        public ReactiveCommand<bool, System.Reactive.Unit> AngelFilter { get; }
        public ReactiveCommand<bool, System.Reactive.Unit> EXFilter { get; }

        public ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit> Update { get; }
        public ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit> Save { get; }
    }
}
