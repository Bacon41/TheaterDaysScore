using Avalonia.Controls;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheaterDaysScore.Models;
using TheaterDaysScore.Services;

namespace TheaterDaysScore.ViewModels {
    public class CardInfoViewModel : ViewModelBase {
        public CardInfoViewModel(IEnumerable<Card> items) {
            Items = new ObservableCollection<Card>(items);

            Cancel = ReactiveCommand.Create(() => { });
        }

        public ObservableCollection<Card> Items { get; }

        public ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit> Cancel { get; }
    }
}
