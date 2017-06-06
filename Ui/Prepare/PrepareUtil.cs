using System;
using System.Threading.Tasks;
using MyCC.Core;
using MyCC.Core.Currencies;
using MyCC.Core.Helpers;
using MyCC.Core.Settings;
using MyCC.Ui.Helpers;
using Xamarin.Forms;
using MyCC.Core.Preperation;

namespace MyCC.Ui.Prepare
{
    public class PrepareUtil : IPrepareUtil
    {
        public bool PreparingNeeded => !ApplicationSettings.AppInitialised ||
                                        Core.Preperation.Prepare.PreparingNeeded ||
                                        Core.Preperation.Prepare.AsyncExecutePreperations != null ||
                                        Migrate.MigrationsNeeded;

        public async Task Prepare(Action<(double progress, string infoText)> onProgress)
        {
            await PrepareAndMigrate();

            if (ApplicationSettings.AppInitialised) return;
            await PrepareFirstStart(onProgress);
        }

        private static async Task PrepareFirstStart(Action<(double progress, string infoText)> onProgress)
        {
            try
            {
                void SetProgress(double progress, string source)
                {
                    onProgress((0.8 * progress, string.Format(DependencyService.Get<ITextResolver>().LoadingCurrenciesFrom, source)));
                }

                await CurrencyStorage.Instance.LoadOnline(SetProgress);
                await MyccUtil.Rates.FetchNeededButNotLoaded(progress => onProgress((0.8 + progress * 0.2, DependencyService.Get<ITextResolver>().LoadingRates)));

                UiUtils.Update.CreateAssetsData();
                UiUtils.Update.CreateRatesData();
                ApplicationSettings.AppInitialised = true;
            }
            catch (Exception e)
            {
                DependencyService.Get<IErrorDialog>().Display(e);
                e.LogError();
            }
        }

        private static async Task PrepareAndMigrate()
        {
            if (Core.Preperation.Prepare.PreparingNeeded)
            {
                Core.Preperation.Prepare.ExecutePreperations();
                if (Core.Preperation.Prepare.AsyncExecutePreperations != null) await Core.Preperation.Prepare.AsyncExecutePreperations;
            }
            if (Migrate.MigrationsNeeded) await Migrate.ExecuteMigratations();
        }
    }
}