﻿using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ReactiveUI;
using TheaterDaysScore.Models;
using TheaterDaysScore.Services;
using TheaterDaysScore.ViewModels;

namespace TheaterDaysScore.Views {
    public class SongInfoView : ReactiveUserControl<SongInfoViewModel> {
        private TextBlock appealDisp;
        private TextBlock scoreDisp;
        private Calculator calc;

        private DrawCanvas scoreCanvas;
        private DrawIntervals intervalCanvas;

        private int songNum;
        public Unit unit;

        public SongInfoView() {
            this.InitializeComponent();

            songNum = 0;
            unit = new Unit("031tom0164", "031tom0164", "007ior0084", "020meg0084", "038chz0034", "009rit0084");

            scoreCanvas.Draw(songNum);
            intervalCanvas.Draw(songNum, unit);
            calc = new Calculator();

            this.FindControl<RadioButton>("song0").IsChecked = true;

            this.WhenActivated(disposables => { });
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);

            appealDisp = this.FindControl<TextBlock>("appealDisplay");
            scoreDisp = this.FindControl<TextBlock>("scoreDisplay");
            scoreCanvas = this.FindControl<DrawCanvas>("scoreCanvas");
            intervalCanvas = this.FindControl<DrawIntervals>("intervalCanvas");
        }

        public void SongSelect(object sender, RoutedEventArgs e) {
            RadioButton rb = sender as RadioButton;
            switch (rb.Name) {
                case "song0":
                    songNum = 0;
                    break;
                case "song1":
                    songNum = 1;
                    break;
                case "song2":
                    songNum = 2;
                    break;
                case "song3":
                    songNum = 3;
                    break;
                case "song4":
                    songNum = 4;
                    break;
            }
            scoreCanvas.Draw(songNum);
            intervalCanvas.Draw(songNum, unit);
            scoreCanvas.InvalidateVisual();
            intervalCanvas.InvalidateVisual();
            
            appealDisp.Text = "Appeal: " + calc.GetAppeal(Database.DB.GetSong(songNum).Type, Calculator.BoostType.none, unit);
        }
    }
}
