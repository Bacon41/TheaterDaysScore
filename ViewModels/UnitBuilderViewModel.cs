using DynamicData;
using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
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

        private HashSet<Card.Categories> categories = new HashSet<Card.Categories>();
        [DataMember]
        public HashSet<Card.Categories> Categories {
            get => categories;
            set => this.RaiseAndSetIfChanged(ref categories, value);
        }

        private HashSet<CardData.CenterEffect.Type> centerTypes = new HashSet<CardData.CenterEffect.Type>();
        [DataMember]
        public HashSet<CardData.CenterEffect.Type> CenterTypes {
            get => centerTypes;
            set => this.RaiseAndSetIfChanged(ref centerTypes, value);
        }

        private HashSet<CardData.Skill.Type> skillTypes = new HashSet<CardData.Skill.Type>();
        [DataMember]
        public HashSet<CardData.Skill.Type> SkillTypes {
            get => skillTypes;
            set => this.RaiseAndSetIfChanged(ref skillTypes, value);
        }

        // Unit
        private int placementIndex = -1;
        public int PlacementIndex {
            get => placementIndex;
            set => this.RaiseAndSetIfChanged(ref placementIndex, value);
        }

        private string guest = "";
        [DataMember]
        public string Guest {
            get => guest;
            set => this.RaiseAndSetIfChanged(ref guest, value);
        }

        private int guestRank = 0;
        [DataMember]
        public int GuestRank {
            get => guestRank;
            set => this.RaiseAndSetIfChanged(ref guestRank, value);
        }

        private List<int> guestRanks;
        public List<int> GuestRanks {
            get => guestRanks;
            set => this.RaiseAndSetIfChanged(ref guestRanks, value);
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

            var allRarities = (CardData.Rarities[])Enum.GetValues(typeof(CardData.Rarities));
            AllRarities = ReactiveCommand.Create(() => {
                if (Rarities.Count == allRarities.Length) {
                    Rarities = new HashSet<CardData.Rarities>();
                } else {
                    Rarities = new HashSet<CardData.Rarities>(allRarities);
                }
            });

            var allTypes = (Types[])Enum.GetValues(typeof(Types));
            AllTypes = ReactiveCommand.Create(() => {
                if (Types.Count == allTypes.Length) {
                    Types = new HashSet<Types>();
                } else {
                    Types = new HashSet<Types>(allTypes);
                }
            });

            var allCategories = (Card.Categories[])Enum.GetValues(typeof(Card.Categories));
            AllCategories = ReactiveCommand.Create(() => {
                if (Categories.Count == allCategories.Length) {
                    Categories = new HashSet<Card.Categories>();
                } else {
                    Categories = new HashSet<Card.Categories>(allCategories);
                }
            });

            var allCenters = (CardData.CenterEffect.Type[])Enum.GetValues(typeof(CardData.CenterEffect.Type));
            AllCenters = ReactiveCommand.Create(() => {
                if (CenterTypes.Count == allCenters.Length) {
                    CenterTypes = new HashSet<CardData.CenterEffect.Type>();
                } else {
                    CenterTypes = new HashSet<CardData.CenterEffect.Type>(allCenters);
                }
            });

            var allSkills = (CardData.Skill.Type[])Enum.GetValues(typeof(CardData.Skill.Type));
            AllSkills = ReactiveCommand.Create(() => {
                if (SkillTypes.Count == allSkills.Length) {
                    SkillTypes = new HashSet<CardData.Skill.Type>();
                } else {
                    SkillTypes = new HashSet<CardData.Skill.Type>(allSkills);
                }
            });
        }

        public void FilterCards() {
            Items.Clear();
            if (PlacementIndex >= 0) {
                Items.AddRange(Database.DB.AllCards()
                    .Where(card => PlacementIndex == 0 || card.IsHeld)
                    .Where(card => Rarities.Contains(card.Rarity))
                    .Where(card => Types.Contains(card.Type))
                    .Where(card => Categories.Contains(card.Category))
                    .Where(card => CenterTypes.Contains(card.CenterType))
                    .Where(card => SkillTypes.Contains(card.SkillType))
                    );
            }
        }

        public void SetCard(Card card) {
            if (card == null) {
                return;
            }
            if (PlacementIndex == 0) {
                Guest = card.ID;
            } else {
                string oldVal = "";
                
                switch (PlacementIndex) {
                    case 1:
                        oldVal = Member1;
                        Member1 = card.ID;
                        break;
                    case 2:
                        oldVal = Member2;
                        Member2 = card.ID;
                        break;
                    case 3:
                        oldVal = Center;
                        Center = card.ID;
                        break;
                    case 4:
                        oldVal = Member3;
                        Member3 = card.ID;
                        break;
                    case 5:
                        oldVal = Member4;
                        Member4 = card.ID;
                        break;
                }
                if (card.ID == Member1 && PlacementIndex != 1) {
                    Member1 = oldVal;
                }
                if (card.ID == Member2 && PlacementIndex != 2) {
                    Member2 = oldVal;
                }
                if (card.ID == Center && PlacementIndex != 3) {
                    Center = oldVal;
                }
                if (card.ID == Member3 && PlacementIndex != 4) {
                    Member3 = oldVal;
                }
                if (card.ID == Member4 && PlacementIndex != 5) {
                    Member4 = oldVal;
                }
            }
            SetUnit();
        }

        public void SetUnit() {
            if (Guest == "" || Center == "" || Member1 == "" || Member2 == "" || Member3 == "" || Member4 == "") {
                return;
            }
            Unit = new Unit(Guest, GuestRank, Center, Member1, Member2, Member3, Member4);
        }

        public ObservableCollection<Card> Items { get; }

        public ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit> AllRarities { get; }
        public ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit> AllTypes { get; }
        public ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit> AllCategories { get; }
        public ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit> AllCenters { get; }
        public ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit> AllSkills { get; }
    }
}
