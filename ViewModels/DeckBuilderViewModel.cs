using Avalonia.Controls;
using DynamicData;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheaterDaysScore.JsonModels;
using TheaterDaysScore.Models;
using TheaterDaysScore.Services;

namespace TheaterDaysScore.ViewModels {
    public class DeckBuilderViewModel : ViewModelBase {
        private HashSet<CardData.Rarities> rarities;
        private HashSet<Types> types;

        public DeckBuilderViewModel() {
            Items = new ObservableCollection<Card>();
            rarities = new HashSet<CardData.Rarities>();
            types = new HashSet<Types>();
            FilterCards();

            SSRFilter = ReactiveCommand.Create((bool isChecked) => {
                if (isChecked) {
                    rarities.Add(CardData.Rarities.SSR);
                } else {
                    rarities.Remove(CardData.Rarities.SSR);
                }
                FilterCards();
            });
            SRFilter = ReactiveCommand.Create((bool isChecked) => {
                if (isChecked) {
                    rarities.Add(CardData.Rarities.SR);
                } else {
                    rarities.Remove(CardData.Rarities.SR);
                }
                FilterCards();
            });
            RFilter = ReactiveCommand.Create((bool isChecked) => {
                if (isChecked) {
                    rarities.Add(CardData.Rarities.R);
                } else {
                    rarities.Remove(CardData.Rarities.R);
                }
                FilterCards();
            });
            NFilter = ReactiveCommand.Create((bool isChecked) => {
                if (isChecked) {
                    rarities.Add(CardData.Rarities.N);
                } else {
                    rarities.Remove(CardData.Rarities.N);
                }
                FilterCards();
            });

            PrincessFilter = ReactiveCommand.Create((bool isChecked) => {
                if (isChecked) {
                    types.Add(Types.Princess);
                } else {
                    types.Remove(Types.Princess);
                }
                FilterCards();
            });
            FairyFilter = ReactiveCommand.Create((bool isChecked) => {
                if (isChecked) {
                    types.Add(Types.Fairy);
                } else {
                    types.Remove(Types.Fairy);
                }
                FilterCards();
            });
            AngelFilter = ReactiveCommand.Create((bool isChecked) => {
                if (isChecked) {
                    types.Add(Types.Angel);
                } else {
                    types.Remove(Types.Angel);
                }
                FilterCards();
            });
            EXFilter = ReactiveCommand.Create((bool isChecked) => {
                if (isChecked) {
                    types.Add(Types.EX);
                } else {
                    types.Remove(Types.EX);
                }
                FilterCards();
            });

            Update = ReactiveCommand.Create(() => { });
            Save = ReactiveCommand.Create(() => { });
            Close = ReactiveCommand.Create(() => { });
        }

        private void FilterCards() {
            Items.Clear();
            Items.AddRange(Database.DB.AllCards()
                .Where(card => rarities.Contains(card.Rarity))
                .Where(card => types.Contains(card.Type))
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
        public ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit> Close { get; }
    }
}
