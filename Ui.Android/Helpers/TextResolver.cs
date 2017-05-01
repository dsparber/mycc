﻿using MyCC.Ui.Android.Helpers;
using MyCC.Ui.Helpers;
using Xamarin.Forms;
using Application = Android.App.Application;

[assembly: Dependency(typeof(TextResolver))]
namespace MyCC.Ui.Android.Helpers
{
    public class TextResolver : ITextResolver
    {
        private static string GetText(int id) => Application.Context.Resources.GetText(id);

        public string ManuallyAdded => GetText(Resource.String.ManuallyAdded);
        public string BittrexAdded => GetText(Resource.String.BittrexAdded);
        public string AddressAdded => GetText(Resource.String.AddressAdded);
        public string Amount => GetText(Resource.String.Amount);
        public string Currency => GetText(Resource.String.Currency);
        public string Name => GetText(Resource.String.Name);
        public string OneAccount => GetText(Resource.String.OneAccount);
        public string Accounts => GetText(Resource.String.Accounts);
        public string Currencies => GetText(Resource.String.Currencies);
        public string OneCurrency => GetText(Resource.String.OneCurrency);
        public string Further => GetText(Resource.String.Further);
        public string NoDataToDisplay => GetText(Resource.String.NoDataToDisplay);
        public string AsCurrency => GetText(Resource.String.AsCurrency);
        public string CoinProofOfWork => GetText(Resource.String.CoinProofOfWork);
        public string CoinProofOfStake => GetText(Resource.String.CoinProofOfStake);
        public string GHps => GetText(Resource.String.GHps);
        public string UnitSecond => GetText(Resource.String.UnitSecond);
    }
}