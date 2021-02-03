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

        private string score = "NaN";
        public string Score {
            get => score;
            set => this.RaiseAndSetIfChanged(ref score, value);
        }

        readonly ObservableAsPropertyHelper<string> appeal;
        public string Appeal => appeal.Value;

        public IScreen HostScreen { get; }

        public string UrlPathSegment => "songinfo";

        public SongInfoViewModel(IScreen screen = null) {
            HostScreen = screen ?? Locator.Current.GetService<IScreen>();

            calc = new Calculator();

            Calculate = ReactiveCommand.Create(() => {
                Score = calc.GetScore(SongNum, BoostType, Unit).ToString();
            });

            appeal = this.WhenAnyValue(x => x.SongNum, x => x.Unit, x => x.BoostType)
                .Select(x => {
                    if (Unit == null) {
                        return "N/A";
                    }
                    return "Appeal: " + calc.GetAppeal(Database.DB.GetSong(SongNum).Type, BoostType, Unit).ToString();
                })
                .ToProperty(this, x => x.Appeal);
        }

        public ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit> Calculate { get; }
        public ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit> GetAppeal { get; }
    }
}
