using DynamicData;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using TheaterDaysScore.Models;
using TheaterDaysScore.Services;

namespace TheaterDaysScore.ViewModels {
    [DataContract]
    public class MainWindowViewModel : ReactiveObject, IScreen {
        private RoutingState _router = new RoutingState();

        private SongInfoViewModel songInfo = new SongInfoViewModel();
        [DataMember]
        SongInfoViewModel SongInfo {
            get => songInfo;
            set => this.RaiseAndSetIfChanged(ref songInfo, value);
        }

        private DeckBuilderViewModel deckBuilder = new DeckBuilderViewModel();
        [DataMember]
        DeckBuilderViewModel DeckBuilder {
            get => deckBuilder;
            set => this.RaiseAndSetIfChanged(ref deckBuilder, value);
        }

        public ReactiveCommand<System.Reactive.Unit, IRoutableViewModel> EditDeck { get; }

        public MainWindowViewModel() {
            Router.Navigate.Execute(SongInfo);
            EditDeck = ReactiveCommand.CreateFromObservable(() => Router.Navigate.Execute(DeckBuilder));
        }

        public RoutingState Router {
            get => _router;
            set => this.RaiseAndSetIfChanged(ref _router, value);
        }
    }
}
