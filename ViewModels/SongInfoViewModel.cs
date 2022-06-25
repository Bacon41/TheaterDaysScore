using ReactiveUI;
using Splat;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.Serialization;
using TheaterDaysScore.Models;
using TheaterDaysScore.Services;

namespace TheaterDaysScore.ViewModels {
    [DataContract]
    public class SongInfoViewModel : ReactiveObject, IRoutableViewModel {
        private Calculator calc;

        private Unit unit;
        public Unit Unit {
            get => unit;
            set => this.RaiseAndSetIfChanged(ref unit, value);
        }

        private Song song;
        public Song Song {
            get => song;
            set => this.RaiseAndSetIfChanged(ref song, value);
        }

        private Song.Difficulty difficulty;
        [DataMember]
        public Song.Difficulty Difficulty {
            get => difficulty;
            set => this.RaiseAndSetIfChanged(ref difficulty, value);
        }

        private Calculator.BoostType boostType;
        [DataMember]
        public Calculator.BoostType BoostType {
            get => boostType;
            set => this.RaiseAndSetIfChanged(ref boostType, value);
        }

        private string scoreIdeal = "NaN";
        public string ScoreIdeal {
            get => scoreIdeal;
            set => this.RaiseAndSetIfChanged(ref scoreIdeal, value);
        }
        private string score001 = "NaN";
        public string Score001 {
            get => score001;
            set => this.RaiseAndSetIfChanged(ref score001, value);
        }
        private string score01 = "NaN";
        public string Score01 {
            get => score01;
            set => this.RaiseAndSetIfChanged(ref score01, value);
        }
        private string score1 = "NaN";
        public string Score1 {
            get => score1;
            set => this.RaiseAndSetIfChanged(ref score1, value);
        }
        private string score10 = "NaN";
        public string Score10 {
            get => score10;
            set => this.RaiseAndSetIfChanged(ref score10, value);
        }
        private string score50 = "NaN";
        public string Score50 {
            get => score50;
            set => this.RaiseAndSetIfChanged(ref score50, value);
        }
        private string scoreBase = "NaN";
        public string ScoreBase {
            get => scoreBase;
            set => this.RaiseAndSetIfChanged(ref scoreBase, value);
        }

        readonly ObservableAsPropertyHelper<string> appeal;
        public string Appeal => appeal.Value;

        readonly ObservableAsPropertyHelper<List<Card>> supports;
        public List<Card> Supports => supports.Value;

        public IScreen HostScreen { get; }

        public string UrlPathSegment => "songinfo";

        public SongInfoViewModel(IScreen screen = null) {
            HostScreen = screen ?? Locator.Current.GetService<IScreen>();

            calc = new Calculator();

            Calculate = ReactiveCommand.Create(() => {
                Calculator.Results results = calc.GetResults(Song, Difficulty, BoostType, Unit, 10000);
                if (results != null) {
                    ScoreIdeal = results.Ideal.ToString();
                    Score001 = results.Percentile(0.01).ToString();
                    Score01 = results.Percentile(0.1).ToString();
                    Score1 = results.Percentile(1).ToString();
                    Score10 = results.Percentile(10).ToString();
                    Score50 = results.Percentile(50).ToString();
                    ScoreBase = results.Base.ToString();
                }
            });

            appeal = this.WhenAnyValue(x => x.Song, x => x.Unit, x => x.BoostType)
                .Select(x => {
                    if (Unit == null || Song == null) {
                        return "N/A";
                    }
                    return calc.GetAppeal(Song.Type, BoostType, Unit).ToString();
                })
                .ToProperty(this, x => x.Appeal);

            supports = this.WhenAnyValue(x => x.Song, x => x.Unit, x => x.BoostType)
                .Select(x => {
                    if (Unit == null || Song == null) {
                        return null;
                    }
                    return Unit.TopSupport(Song.Type, BoostType);
                })
                .ToProperty(this, x => x.Supports);
        }

        public ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit> Calculate { get; }
        public ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit> GetAppeal { get; }
    }
}
