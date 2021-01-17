using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using TheaterDaysScore.Models;
using TheaterDaysScore.Services;

namespace TheaterDaysScore.ViewModels {
    public class MainWindowViewModel : ViewModelBase {
        ViewModelBase content;

        SongInfoViewModel songsView;
        CardInfoViewModel cardsView;

        public MainWindowViewModel() {
            songsView = new SongInfoViewModel();
            cardsView = new CardInfoViewModel(Database.DB.AllCards());
            Content = songsView;
        }

        public ViewModelBase Content {
            get => content;
            private set => this.RaiseAndSetIfChanged(ref content, value);
        }

        public void ChooseCards() {
            var vm = cardsView;

            Observable.Merge(
                vm.Cancel.Select(_ => (Card)null))
                .Take(1)
                .Subscribe(model => {
                    Content = songsView;
                });

            Content = vm;
        }
    }
}
