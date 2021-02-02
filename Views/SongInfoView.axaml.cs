using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using DynamicData;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using TheaterDaysScore.JsonModels;
using TheaterDaysScore.Models;
using TheaterDaysScore.Services;
using TheaterDaysScore.ViewModels;

namespace TheaterDaysScore.Views {
    public class SongInfoView : ReactiveUserControl<SongInfoViewModel> {
        private ListBox songSelection;

        private DrawCanvas scoreCanvas;
        private DrawIntervals intervalCanvas;

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
            });
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);

            songSelection = this.FindControl<ListBox>("songSelection");
            scoreCanvas = this.FindControl<DrawCanvas>("scoreCanvas");
            intervalCanvas = this.FindControl<DrawIntervals>("intervalCanvas");
        }
    }
}
