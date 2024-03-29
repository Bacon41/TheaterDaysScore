﻿using Avalonia.Controls;
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

        public SongInfoView() {
            this.InitializeComponent();

            this.WhenActivated(disposables => {
                this.WhenAnyValue(x => x.ViewModel.Song, x => x.ViewModel.Difficulty)
                    .Subscribe(x => {
                        scoreCanvas.Draw(x.Item1, x.Item2);
                        scoreCanvas.InvalidateVisual();
                        intervalCanvas.Draw(x.Item1, x.Item2, ViewModel.Unit);
                        intervalCanvas.InvalidateVisual();
                    })
                    .DisposeWith(disposables);
            });
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);

            scoreCanvas = this.FindControl<DrawCanvas>("scoreCanvas");
            intervalCanvas = this.FindControl<DrawIntervals>("intervalCanvas");
        }
    }
}
