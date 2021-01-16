using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using TheaterDaysScore.Services;

namespace TheaterDaysScore.Views {
    public class SongInfoView : UserControl {
        private TextBlock disp;
        private Calculator calc;

        private DrawCanvas scoreCanvas;
        private DrawIntervals intervalCanvas;

        private int songNum;
        private Unit unit;

        public SongInfoView() {
            this.InitializeComponent();

            songNum = 0;
            unit = new Unit("031tom0164", "031tom0164", "007ior0084", "020meg0084", "038chz0034", "009rit0084");

            scoreCanvas.Draw(songNum);
            intervalCanvas.Draw(songNum, unit);
            calc = new Calculator();

            calc.GetAppeal(Types.Fairy, Calculator.BoostType.none, unit);

            this.FindControl<RadioButton>("song0").IsChecked = true;

            var x = Database.DB.TopAppeal(Types.Fairy, 10, "031tom0164");
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);

            disp = this.FindControl<TextBlock>("scoreDisplay");
            scoreCanvas = this.FindControl<DrawCanvas>("scoreCanvas");
            intervalCanvas = this.FindControl<DrawIntervals>("intervalCanvas");
        }

        public void Calculate_Click(object sender, RoutedEventArgs e) {
            disp.Text = "50th Percentile: " + calc.GetScore(songNum, unit).ToString();
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
        }
    }
}
