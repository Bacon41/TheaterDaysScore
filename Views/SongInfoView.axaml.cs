using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ReactiveUI;
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using TheaterDaysScore.ViewModels;

namespace TheaterDaysScore.Views {
    public class SongInfoView : ReactiveUserControl<SongInfoViewModel> {
        private DrawCanvas scoreCanvas;
        private DrawIntervals intervalCanvas;
        private DrawCanvas2 scoreCanvas2;
        private DrawIntervals2 intervalCanvas2;

        public SongInfoView() {
            this.InitializeComponent();

            this.WhenActivated(disposables => {
                this.WhenAnyValue(x => x.ViewModel.SongNum)
                    .Subscribe(x => {
                        scoreCanvas.Draw(x);
                        scoreCanvas.InvalidateVisual();
                        intervalCanvas.Draw(x, ViewModel.Unit);
                        intervalCanvas.InvalidateVisual();
                    })
                    .DisposeWith(disposables);
                this.WhenAnyValue(x => x.ViewModel.Song, x => x.ViewModel.Difficulty)
                    .Subscribe(x => {
                        scoreCanvas2.Draw(x.Item1, x.Item2);
                        scoreCanvas2.InvalidateVisual();
                        intervalCanvas2.Draw(x.Item1, x.Item2, ViewModel.Unit);
                        intervalCanvas2.InvalidateVisual();
                    })
                    .DisposeWith(disposables);
            });
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);

            scoreCanvas = this.FindControl<DrawCanvas>("scoreCanvas");
            intervalCanvas = this.FindControl<DrawIntervals>("intervalCanvas");
            scoreCanvas2 = this.FindControl<DrawCanvas2>("scoreCanvas2");
            intervalCanvas2 = this.FindControl<DrawIntervals2>("intervalCanvas2");
        }
    }
}
