using Avalonia.Controls;
using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
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

        private int songNum = 0;
        [DataMember]
        public int SongNum {
            get => songNum;
            set => this.RaiseAndSetIfChanged(ref songNum, value);
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
                Calculator.Results results = calc.GetResults(SongNum, BoostType, Unit, 10000);
                if (results != null) {
                    ScoreIdeal = results.Ideal.ToString();
                    Score001 = results.Percentile(0.01).ToString();
                    Score01 = results.Percentile(0.1).ToString();
                    Score1 = results.Percentile(1).ToString();
                    Score10 = results.Percentile(10).ToString();
                    Score50 = results.Percentile(50).ToString();
                }
            });

            appeal = this.WhenAnyValue(x => x.SongNum, x => x.Unit, x => x.BoostType)
                .Select(x => {
                    if (Unit == null) {
                        return "N/A";
                    }
                    return calc.GetAppeal(Database.DB.GetSong(SongNum).Type, BoostType, Unit).ToString();
                })
                .ToProperty(this, x => x.Appeal);

            supports = this.WhenAnyValue(x => x.SongNum, x => x.Unit, x => x.BoostType)
                .Select(x => {
                    if (Unit == null) {
                        return null;
                    }
                    return Unit.TopSupport(Database.DB.GetSong(SongNum).Type, BoostType);
                })
                .ToProperty(this, x => x.Supports);
        }

        public ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit> Calculate { get; }
        public ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit> GetAppeal { get; }
    }
}
