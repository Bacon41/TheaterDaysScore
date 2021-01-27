using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using DynamicData;
using ReactiveUI;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
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
                this.OneWayBind(ViewModel, vm => vm.Rarities, v => v.ssrCheck.IsChecked, set => {
                    return set.Contains(JsonModels.CardData.Rarities.SSR);
                }).DisposeWith(disposables);
                this.OneWayBind(ViewModel, vm => vm.Rarities, v => v.srCheck.IsChecked, set => {
                    return set.Contains(JsonModels.CardData.Rarities.SR);
                }).DisposeWith(disposables);
                this.OneWayBind(ViewModel, vm => vm.Rarities, v => v.rCheck.IsChecked, set => {
                    return set.Contains(JsonModels.CardData.Rarities.R);
                }).DisposeWith(disposables);
                this.OneWayBind(ViewModel, vm => vm.Rarities, v => v.nCheck.IsChecked, set => {
                    return set.Contains(JsonModels.CardData.Rarities.N);
                }).DisposeWith(disposables);

                this.OneWayBind(ViewModel, vm => vm.Types, v => v.princessCheck.IsChecked, set => {
                    return set.Contains(Types.Princess);
                }).DisposeWith(disposables);
                this.OneWayBind(ViewModel, vm => vm.Types, v => v.fairyCheck.IsChecked, set => {
                    return set.Contains(Types.Fairy);
                }).DisposeWith(disposables);
                this.OneWayBind(ViewModel, vm => vm.Types, v => v.angelCheck.IsChecked, set => {
                    return set.Contains(Types.Angel);
                }).DisposeWith(disposables);
                this.OneWayBind(ViewModel, vm => vm.Types, v => v.exCheck.IsChecked, set => {
                    return set.Contains(Types.EX);
                }).DisposeWith(disposables);

                ViewModel.FilterCards();
            });
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
