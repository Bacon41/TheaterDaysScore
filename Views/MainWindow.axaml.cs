using Avalonia;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ReactiveUI;
using System;
using System.Reactive.Disposables;
using TheaterDaysScore.ViewModels;

namespace TheaterDaysScore.Views {
    public class MainWindow : ReactiveWindow<MainWindowViewModel> {

        public MainWindow() {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif

            this.WhenActivated(disposables => {
                // Pass the unit from the builder to the song
                this.WhenAnyValue(x => x.ViewModel.UnitBuilder.Unit)
                    .Subscribe(x => ViewModel.SongInfo.Unit = x)
                    .DisposeWith(disposables);

                // Re-build the unit whenever the page changes, to pick up level increases
                this.WhenAnyObservable(x => x.ViewModel.Router.NavigationChanged)
                    .Subscribe(x => ViewModel.UnitBuilder.SetUnit())
                    .DisposeWith(disposables);

                // Fill in the unit on boot
                ViewModel.UnitBuilder.SetUnit();
            });
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
