using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using TheaterDaysScore.Models;

namespace TheaterDaysScore.ViewModels {
    [DataContract]
    public class SongInfoViewModel : ReactiveObject, IRoutableViewModel {
        private Calculator calc;

        public Unit Unit;

        private int songNum = 0;
        [DataMember]
        public int SongNum {
            get => songNum;
            set => this.RaiseAndSetIfChanged(ref songNum, value);
        }

        private string score = "NaN";
        public string Score {
            get => score;
            set => this.RaiseAndSetIfChanged(ref score, value);
        }

        public IScreen HostScreen { get; }

        public string UrlPathSegment => "songinfo";

        public SongInfoViewModel(IScreen screen = null) {
            HostScreen = screen ?? Locator.Current.GetService<IScreen>();

            Unit = new Unit("031tom0164", "031tom0164", "007ior0084", "020meg0084", "038chz0034", "009rit0084");

            calc = new Calculator();

            Calculate = ReactiveCommand.Create(() => {
                Score = calc.GetScore(SongNum, Unit).ToString();
            });
        }

        public ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit> Calculate { get; }
    }
}
