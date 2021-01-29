using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.ReactiveUI;
using ReactiveUI;
using System;
using System.Diagnostics;
using System.Reactive.Disposables;
using TheaterDaysScore.Services;
using TheaterDaysScore.ViewModels;

namespace TheaterDaysScore.Views {
    public class MainWindow : ReactiveWindow<MainWindowViewModel> {

        public MainWindow() {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif

            this.WhenActivated(disposables => {
                ViewModel.UnitBuilder.SetUnit();
                ViewModel.SongInfo.Unit = ViewModel.UnitBuilder.Unit;

                this.WhenAnyValue(x => x.ViewModel.UnitBuilder.Unit)
                    .Subscribe(x => ViewModel.SongInfo.Unit = x)
                    .DisposeWith(disposables);
            });
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
