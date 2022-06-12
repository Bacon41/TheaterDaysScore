using ReactiveUI;
using System.Runtime.Serialization;

namespace TheaterDaysScore.ViewModels {
    [DataContract]
    public class MainWindowViewModel : ReactiveObject, IScreen {
        private RoutingState _router = new RoutingState();

        private SongInfoViewModel songInfo = new SongInfoViewModel();
        [DataMember]
        public SongInfoViewModel SongInfo {
            get => songInfo;
            set => this.RaiseAndSetIfChanged(ref songInfo, value);
        }

        private DeckBuilderViewModel deckBuilder = new DeckBuilderViewModel();
        [DataMember]
        public DeckBuilderViewModel DeckBuilder {
            get => deckBuilder;
            set => this.RaiseAndSetIfChanged(ref deckBuilder, value);
        }

        private SongPickerViewModel songPicker = new SongPickerViewModel();
        [DataMember]
        public SongPickerViewModel SongPicker {
            get => songPicker;
            set => this.RaiseAndSetIfChanged(ref songPicker, value);
        }

        private UnitBuilderViewModel unitBuilder = new UnitBuilderViewModel();
        [DataMember]
        public UnitBuilderViewModel UnitBuilder {
            get => unitBuilder;
            set => this.RaiseAndSetIfChanged(ref unitBuilder, value);
        }

        public ReactiveCommand<System.Reactive.Unit, IRoutableViewModel> EditDeck { get; }

        public ReactiveCommand<System.Reactive.Unit, IRoutableViewModel> PickSong { get; }

        public ReactiveCommand<System.Reactive.Unit, IRoutableViewModel> EditUnit { get; }

        public MainWindowViewModel() {
            Router.Navigate.Execute(SongInfo);
            EditDeck = ReactiveCommand.CreateFromObservable(() => Router.Navigate.Execute(DeckBuilder));
            PickSong = ReactiveCommand.CreateFromObservable(() => Router.Navigate.Execute(SongPicker));
            EditUnit = ReactiveCommand.CreateFromObservable(() => Router.Navigate.Execute(UnitBuilder));
        }

        public RoutingState Router {
            get => _router;
            set => this.RaiseAndSetIfChanged(ref _router, value);
        }
    }
}
