using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ReactiveUI;
using System.Collections.Generic;
using System.Reactive.Disposables;
using TheaterDaysScore.ViewModels;

namespace TheaterDaysScore.Views {
    public class CardInfoView : UserControl {

        public CardInfoView() {
            this.InitializeComponent();
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
