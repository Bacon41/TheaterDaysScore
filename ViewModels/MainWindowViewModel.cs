using DynamicData;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            cardsView = new CardInfoViewModel();
            Content = songsView;
        }

        public ViewModelBase Content {
            get => content;
            private set => this.RaiseAndSetIfChanged(ref content, value);
        }

        public void ChooseCards() {
            var vm = cardsView;

            vm.Cancel.Subscribe(_ => {
                Content = songsView;
            });
            vm.Save.Subscribe(_ => {
                Database.DB.SaveHeld();
            });
            vm.Update.Subscribe(_ => {
                cardsView.Items.Clear();
                cardsView.Items.AddRange(Database.DB.UpdateCards());
            });

            Content = vm;
        }
    }
}
