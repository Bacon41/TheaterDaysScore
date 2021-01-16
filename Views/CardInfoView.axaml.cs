using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace TheaterDaysScore.Views {
    public class CardInfoView : UserControl {
        public string CardID { get; set; }
        public CardInfoView() {
            this.InitializeComponent();
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);

            CardID = "031tom0164";
        }
    }
}
