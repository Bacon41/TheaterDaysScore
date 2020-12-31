using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;

namespace TheaterDaysScore {
    public class MainWindow : Window {

        private TextBlock disp;
        private Calculator calc;

        private DrawingPresenter canvas;

        public MainWindow() {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif

            calc = new Calculator();
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
            disp = this.FindControl<TextBlock>("scoreDisplay");
            canvas = this.FindControl<DrawingPresenter>("noteMap");
        }

        public void Calculate_Click(object sender, RoutedEventArgs e) {
            disp.Text = calc.GetScore().ToString();
        }
    }
}
