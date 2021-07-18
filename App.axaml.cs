using Avalonia;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ReactiveUI;
using Splat;
using TheaterDaysScore.Services;
using TheaterDaysScore.ViewModels;
using TheaterDaysScore.Views;

namespace TheaterDaysScore {
    public class App : Application {
        public override void Initialize() {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted() {
            // Create the AutoSuspendHelper
            var suspension = new AutoSuspendHelper(ApplicationLifetime);
            RxApp.SuspensionHost.CreateNewAppState = () => new MainWindowViewModel();
            RxApp.SuspensionHost.SetupDefaultSuspendResume(new SuspensionDriver(Database.DB.SettingsLoc()));
            suspension.OnFrameworkInitializationCompleted();

            // Load the saved view model state
            var state = RxApp.SuspensionHost.GetAppState<MainWindowViewModel>();
            Locator.CurrentMutable.RegisterConstant<IScreen>(state);

            // Register views.
            Locator.CurrentMutable.Register<IViewFor<DeckBuilderViewModel>>(() => new DeckBuilderView());
            Locator.CurrentMutable.Register<IViewFor<UnitBuilderViewModel>>(() => new UnitBuilderView());
            Locator.CurrentMutable.Register<IViewFor<SongInfoViewModel>>(() => new SongInfoView());

            new MainWindow { DataContext = state }.Show();
            base.OnFrameworkInitializationCompleted();
        }
    }
}
