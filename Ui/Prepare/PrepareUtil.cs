using System;
using System.Threading.Tasks;
using MyCC.Core;
using MyCC.Core.Currencies;
using MyCC.Core.Helpers;
using MyCC.Core.Settings;
using MyCC.Ui.Helpers;
using Xamarin.Forms;

namespace MyCC.Ui.Prepare
{
    public class PrepareUtil : IPrepareUtil
    {
        public bool PreparingNeeded => !ApplicationSettings.AppInitialised;

        public async Task Prepare(Action<(double progress, string infoText)> onProgress)
        {
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
    }
}