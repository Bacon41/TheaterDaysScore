using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using System;
using System.Diagnostics;

namespace TheaterDaysScore {
    public class MainWindow : Window {

        private TextBlock disp;
        private Calculator calc;

        private DrawCanvas scoreCanvas;
        private DrawIntervals intervalCanvas;

        private int songNum;
        private int guestId;
        private int[] cardIds;
        private int[] skillLevels;
        private int totalAppeal;

        public MainWindow() {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif

            songNum = 0;
            /*guestId = 256;
            cardIds = new int[5] { 250, 391, 286, 269, 178 };
            skillLevels = new int[5] { 6, 7, 5, 8, 9 };
            totalAppeal = 320000;*/
            guestId = 868;
            cardIds = new int[5] { 409, 368, 868, 159, 432 };
            skillLevels = new int[5] { 12, 12, 10, 12, 12 };
            totalAppeal = 377515;
            /*guestId = 745;
            cardIds = new int[5] { 432, 868, 572, 409, 732 };
            skillLevels = new int[5] { 12, 10, 10, 10, 5 };
            totalAppeal = 386402;*/

            scoreCanvas.Draw(songNum);
            intervalCanvas.Draw(songNum, cardIds);
            calc = new Calculator();

            calc.GetAppeal();
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
            disp = this.FindControl<TextBlock>("scoreDisplay");
            scoreCanvas = this.FindControl<DrawCanvas>("scoreCanvas");
            intervalCanvas = this.FindControl<DrawIntervals>("intervalCanvas");
        }

        public void Calculate_Click(object sender, RoutedEventArgs e) {
            disp.Text = "50th Percentile: " + calc.GetScore(songNum, totalAppeal, guestId, cardIds, skillLevels).ToString();
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
            }
            scoreCanvas.Draw(songNum);
            intervalCanvas.Draw(songNum, cardIds);
            scoreCanvas.InvalidateVisual();
            intervalCanvas.InvalidateVisual();
        }
    }
}
