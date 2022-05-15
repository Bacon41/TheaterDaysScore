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
    public class DeckBuilderViewModel : ReactiveObject, IRoutableViewModel {

        private bool filterIdol;
        [DataMember]
        public bool FilterIdol {
            get => filterIdol;
            set => this.RaiseAndSetIfChanged(ref filterIdol, value);
        }

        private int selectedIdol;
        [DataMember]
        public int SelectedIdol {
            get => selectedIdol;
            set => this.RaiseAndSetIfChanged(ref selectedIdol, value);
        }

        private HashSet<CardData.Rarities> rarities;
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

        public IScreen HostScreen { get; }

        public string UrlPathSegment => "deckbuilder";

        public DeckBuilderViewModel(IScreen screen = null) {
            HostScreen = screen ?? Locator.Current.GetService<IScreen>();

            Items = new ObservableCollection<Card>();
            Idols = Database.DB.GetIdols();

            Rarities = new HashSet<CardData.Rarities>();

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

            SelectAll = ReactiveCommand.Create(() => {
                foreach (Card card in Items) {
                    card.IsHeld = true;
                }
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
            Items.AddRange(Database.DB.AllCards()
                .Where(card => !FilterIdol || (Database.DB.GetIdols()[SelectedIdol].ID == card.IdolID))
                .Where(card => Rarities.Contains(card.Rarity))
                .Where(card => Types.Contains(card.Type))
                .Where(card => Categories.Contains(card.Category))
                .Where(card => CenterTypes.Contains(card.CenterType) || CenterTypes.Contains(card.CenterType2))
                .Where(card => SkillTypes.Contains(card.SkillType))
                );
        }

        public ObservableCollection<Card> Items { get; }

        public List<Idol> Idols { get; }

        public ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit> Update { get; }
        public ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit> Save { get; }
        public ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit> Load { get; }

        public ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit> SelectAll { get; }
        public ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit> MaxRank { get; }
        public ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit> MaxLevel { get; }

        public ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit> AllRarities { get; }
        public ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit> AllTypes { get; }
        public ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit> AllCategories { get; }
        public ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit> AllCenters { get; }
        public ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit> AllSkills { get; }
    }
}
