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

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
