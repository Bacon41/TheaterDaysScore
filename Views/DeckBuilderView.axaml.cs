using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using DynamicData;
using ReactiveUI;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using TheaterDaysScore.JsonModels;
using TheaterDaysScore.Models;
using TheaterDaysScore.Services;
using TheaterDaysScore.ViewModels;

namespace TheaterDaysScore.Views {
    public class DeckBuilderView : ReactiveUserControl<DeckBuilderViewModel> {

        private CheckBox ssrCheck => this.FindControl<CheckBox>("raritySSR");
        private CheckBox srCheck => this.FindControl<CheckBox>("raritySR");
        private CheckBox rCheck => this.FindControl<CheckBox>("rarityR");
        private CheckBox nCheck => this.FindControl<CheckBox>("rarityN");

        private CheckBox princessCheck => this.FindControl<CheckBox>("typePrincess");
        private CheckBox fairyCheck => this.FindControl<CheckBox>("typeFairy");
        private CheckBox angelCheck => this.FindControl<CheckBox>("typeAngel");
        private CheckBox exCheck => this.FindControl<CheckBox>("typeEX");

        private CheckBox permCheck => this.FindControl<CheckBox>("categoryPerm");
        private CheckBox limCheck => this.FindControl<CheckBox>("categoryLim");
        private CheckBox fesCheck => this.FindControl<CheckBox>("categoryFes");
        private CheckBox pstCheck => this.FindControl<CheckBox>("categoryPST");
        private CheckBox colleCheck => this.FindControl<CheckBox>("categoryColle");
        private CheckBox annCheck => this.FindControl<CheckBox>("categoryAnn");
        private CheckBox otherCheck => this.FindControl<CheckBox>("categoryOther");

        public DeckBuilderView() {
            this.InitializeComponent();

            this.WhenActivated(disposables => {
                // Rarity filters
                this.Bind(ViewModel, vm => vm.Rarities, v => v.ssrCheck.IsChecked, set => {
                    return set.Contains(CardData.Rarities.SSR);
                }, isChecked => {
                    SetRarity(isChecked, CardData.Rarities.SSR);
                    return ViewModel.Rarities;
                }).DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.Rarities, v => v.srCheck.IsChecked, set => {
                    return set.Contains(CardData.Rarities.SR);
                }, isChecked => {
                    SetRarity(isChecked, CardData.Rarities.SR);
                    return ViewModel.Rarities;
                }).DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.Rarities, v => v.rCheck.IsChecked, set => {
                    return set.Contains(CardData.Rarities.R);
                }, isChecked => {
                    SetRarity(isChecked, CardData.Rarities.R);
                    return ViewModel.Rarities;
                }).DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.Rarities, v => v.nCheck.IsChecked, set => {
                    return set.Contains(CardData.Rarities.N);
                }, isChecked => {
                    SetRarity(isChecked, CardData.Rarities.N);
                    return ViewModel.Rarities;
                }).DisposeWith(disposables);

                // Type filters
                this.Bind(ViewModel, vm => vm.Types, v => v.princessCheck.IsChecked, set => {
                    return set.Contains(Types.Princess);
                }, isChecked => {
                    SetType(isChecked, Types.Princess);
                    return ViewModel.Types;
                }).DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.Types, v => v.fairyCheck.IsChecked, set => {
                    return set.Contains(Types.Fairy);
                }, isChecked => {
                    SetType(isChecked, Types.Fairy);
                    return ViewModel.Types;
                }).DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.Types, v => v.angelCheck.IsChecked, set => {
                    return set.Contains(Types.Angel);
                }, isChecked => {
                    SetType(isChecked, Types.Angel);
                    return ViewModel.Types;
                }).DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.Types, v => v.exCheck.IsChecked, set => {
                    return set.Contains(Types.EX);
                }, isChecked => {
                    SetType(isChecked, Types.EX);
                    return ViewModel.Types;
                }).DisposeWith(disposables);

                // Category filters
                this.Bind(ViewModel, vm => vm.Categories, v => v.permCheck.IsChecked, set => {
                    return set.Contains(Card.Categories.PermanentGasha);
                }, isChecked => {
                    SetCategory(isChecked, Card.Categories.PermanentGasha);
                    return ViewModel.Categories;
                }).DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.Categories, v => v.limCheck.IsChecked, set => {
                    return set.Contains(Card.Categories.LimitedGasha);
                }, isChecked => {
                    SetCategory(isChecked, Card.Categories.LimitedGasha);
                    return ViewModel.Categories;
                }).DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.Categories, v => v.fesCheck.IsChecked, set => {
                    return set.Contains(Card.Categories.Fes);
                }, isChecked => {
                    SetCategory(isChecked, Card.Categories.Fes);
                    return ViewModel.Categories;
                }).DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.Categories, v => v.pstCheck.IsChecked, set => {
                    return set.Contains(Card.Categories.PST);
                }, isChecked => {
                    SetCategory(isChecked, Card.Categories.PST);
                    return ViewModel.Categories;
                }).DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.Categories, v => v.colleCheck.IsChecked, set => {
                    return set.Contains(Card.Categories.MiliColle);
                }, isChecked => {
                    SetCategory(isChecked, Card.Categories.MiliColle);
                    return ViewModel.Categories;
                }).DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.Categories, v => v.annCheck.IsChecked, set => {
                    return set.Contains(Card.Categories.Anniversary);
                }, isChecked => {
                    SetCategory(isChecked, Card.Categories.Anniversary);
                    return ViewModel.Categories;
                }).DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.Categories, v => v.otherCheck.IsChecked, set => {
                    return set.Contains(Card.Categories.Other);
                }, isChecked => {
                    SetCategory(isChecked, Card.Categories.Other);
                    return ViewModel.Categories;
                }).DisposeWith(disposables);

                // Initial filtering on boot
                ViewModel.FilterCards();
            });
        }

        private void SetRarity(bool? isChecked, CardData.Rarities rarity) {
            if (isChecked ?? true) {
                ViewModel.Rarities.Add(rarity);
            } else {
                ViewModel.Rarities.Remove(rarity);
            }
            ViewModel.FilterCards();
        }

        private void SetType(bool? isChecked, Types type) {
            if (isChecked ?? true) {
                ViewModel.Types.Add(type);
            } else {
                ViewModel.Types.Remove(type);
            }
            ViewModel.FilterCards();
        }

        private void SetCategory(bool? isChecked, Card.Categories category) {
            if (isChecked ?? true) {
                ViewModel.Categories.Add(category);
            } else {
                ViewModel.Categories.Remove(category);
            }
            ViewModel.FilterCards();
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
