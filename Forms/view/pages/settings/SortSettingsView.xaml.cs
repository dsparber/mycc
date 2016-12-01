using System;
using MyCryptos.Core.Enums;
using MyCryptos.Core.Settings;
using Xamarin.Forms;

namespace MyCryptos.view.pages.settings
{
    public partial class SortSettingsView : ContentPage
    {
        public SortSettingsView()
        {
            InitializeComponent();
            updateCheckmarks();
        }

        void SortOrderSelected(object sender, EventArgs e)
        {
            if (sender.Equals(SortOrderAlphabetical))
            {
                ApplicationSettings.SortOrder = SortOrder.ALPHABETICAL;
            }
            else if (sender.Equals(SortOrderByUnits))
            {
                ApplicationSettings.SortOrder = SortOrder.BY_UNITS;
            }
            else if (sender.Equals(SortOrderByValue))
            {
                ApplicationSettings.SortOrder = SortOrder.BY_VALUE;
            }
            else if (sender.Equals(SortDirectionAscending))
            {
                ApplicationSettings.SortDirection = SortDirection.ASCENDING;
            }
            else if (sender.Equals(SortDirectionDescending))
            {
                ApplicationSettings.SortDirection = SortDirection.DESCENDING;
            }
            updateCheckmarks();
        }

        void updateCheckmarks()
        {
            SortOrderAlphabetical.ShowIcon = ApplicationSettings.SortOrder.Equals(SortOrder.ALPHABETICAL);
            SortOrderByUnits.ShowIcon = ApplicationSettings.SortOrder.Equals(SortOrder.BY_UNITS);
            SortOrderByValue.ShowIcon = ApplicationSettings.SortOrder.Equals(SortOrder.BY_VALUE);
            SortDirectionAscending.ShowIcon = ApplicationSettings.SortDirection.Equals(SortDirection.ASCENDING);
            SortDirectionDescending.ShowIcon = ApplicationSettings.SortDirection.Equals(SortDirection.DESCENDING);
        }
    }
}
