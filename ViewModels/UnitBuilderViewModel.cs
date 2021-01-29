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
        public int PlacementIndex = -1;

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

        private Unit unit = null;
        public Unit Unit {
            get => unit;
            set => this.RaiseAndSetIfChanged(ref unit, value);
        }

        public IScreen HostScreen { get; }

        public string UrlPathSegment => "unitbuilder";

        public UnitBuilderViewModel(IScreen screen = null) {
            HostScreen = screen ?? Locator.Current.GetService<IScreen>();

            Items = new ObservableCollection<Card>();
            Rarities = new HashSet<CardData.Rarities>();
            Types = new HashSet<Types>();
        }

        public void FilterCards() {
            Items.Clear();
            Items.AddRange(Database.DB.AllCards()
                .Where(card => card.IsHeld)
                .Where(card => Rarities.Contains(card.Rarity))
                .Where(card => Types.Contains(card.Type))
                );
        }

        public void SetCard(Card card) {
            if (card == null) {
                return;
            }
            string[] usedCards = { Member1, Member2, Center, Member3, Member4 };
            switch (PlacementIndex) {
                case 0:
                    Guest = card.ID;
                    break;
                case 1:
                    if (usedCards.Contains(card.ID)) {
                        return;
                    }
                    Member1 = card.ID;
                    break;
                case 2:
                    if (usedCards.Contains(card.ID)) {
                        return;
                    }
                    Member2 = card.ID;
                    break;
                case 3:
                    if (usedCards.Contains(card.ID)) {
                        return;
                    }
                    Center = card.ID;
                    break;
                case 4:
                    if (usedCards.Contains(card.ID)) {
                        return;
                    }
                    Member3 = card.ID;
                    break;
                case 5:
                    if (usedCards.Contains(card.ID)) {
                        return;
                    }
                    Member4 = card.ID;
                    break;
            }
            SetUnit();
        }

        public void SetUnit() {
            Unit = new Unit(Guest, Center, Member1, Member2, Member3, Member4);
        }

        public ObservableCollection<Card> Items { get; }
    }
}
