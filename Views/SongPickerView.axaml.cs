using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using DynamicData;
using ReactiveUI;
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using TheaterDaysScore.Models;
using TheaterDaysScore.ViewModels;

namespace TheaterDaysScore.Views {
    public class SongPickerView : ReactiveUserControl<SongPickerViewModel> {

        private CheckBox princessCheck => this.FindControl<CheckBox>("typePrincess");
        private CheckBox fairyCheck => this.FindControl<CheckBox>("typeFairy");
        private CheckBox angelCheck => this.FindControl<CheckBox>("typeAngel");
        private CheckBox princessFairyCheck => this.FindControl<CheckBox>("typePrincessFairy");
        private CheckBox fairyAngelCheck => this.FindControl<CheckBox>("typeFairyAngel");
        private CheckBox angelPrincessCheck => this.FindControl<CheckBox>("typeAngelPrincess");
        private CheckBox allCheck => this.FindControl<CheckBox>("typeAll");

        private ListBox songChoice => this.FindControl<ListBox>("songChoice");

        public SongPickerView() {
            this.InitializeComponent();

            this.WhenActivated(disposables => {
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
                this.Bind(ViewModel, vm => vm.Types, v => v.princessFairyCheck.IsChecked, set => {
                    return set.Contains(Types.PrincessFairy);
                }, isChecked => {
                    SetType(isChecked, Types.PrincessFairy);
                    return ViewModel.Types;
                }).DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.Types, v => v.fairyAngelCheck.IsChecked, set => {
                    return set.Contains(Types.FairyAngel);
                }, isChecked => {
                    SetType(isChecked, Types.FairyAngel);
                    return ViewModel.Types;
                }).DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.Types, v => v.angelPrincessCheck.IsChecked, set => {
                    return set.Contains(Types.AngelPrincess);
                }, isChecked => {
                    SetType(isChecked, Types.AngelPrincess);
                    return ViewModel.Types;
                }).DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.Types, v => v.allCheck.IsChecked, set => {
                    return set.Contains(Types.All);
                }, isChecked => {
                    SetType(isChecked, Types.All);
                    return ViewModel.Types;
                }).DisposeWith(disposables);

                // Card selection
                songChoice.GetObservable(ListBox.SelectedItemProperty)
                    .Subscribe(x => ViewModel.SetSongID(x as Song))
                    .DisposeWith(disposables);

                ViewModel.FilterSongs();
            });
        }

        private void SetType(bool? isChecked, Types type) {
            if (isChecked ?? true) {
                ViewModel.Types.Add(type);
            } else {
                ViewModel.Types.Remove(type);
            }
            ViewModel.FilterSongs();
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
