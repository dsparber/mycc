using System;
using MyCryptos.Core.settings;
using MyCryptos.Core.Types;
using MyCryptos.Forms.Messages;
using Xamarin.Forms;

namespace MyCryptos.Forms.view.pages.settings
{
    public partial class SortSettingsView
    {
        public SortSettingsView()
        {
            InitializeComponent();
            updateCheckmarks();
        }

        private void SortOrderSelected(object sender, EventArgs e)
        {
            if (sender.Equals(SortOrderAlphabetical))
            {
                ApplicationSettings.SortOrder = SortOrder.Alphabetical;
            }
            else if (sender.Equals(SortOrderByUnits))
            {
                ApplicationSettings.SortOrder = SortOrder.ByUnits;
            }
            else if (sender.Equals(SortOrderByValue))
            {
                ApplicationSettings.SortOrder = SortOrder.ByValue;
            }
            else if (sender.Equals(SortDirectionAscending))
            {
                ApplicationSettings.SortDirection = SortDirection.Ascending;
            }
            else if (sender.Equals(SortDirectionDescending))
            {
                ApplicationSettings.SortDirection = SortDirection.Descending;
            }
            Messaging.SortOrder.SendValueChanged();
            updateCheckmarks();
        }

        void updateCheckmarks()
        {
            SortOrderAlphabetical.ShowIcon = ApplicationSettings.SortOrder.Equals(SortOrder.Alphabetical);
            SortOrderByUnits.ShowIcon = ApplicationSettings.SortOrder.Equals(SortOrder.ByUnits);
            SortOrderByValue.ShowIcon = ApplicationSettings.SortOrder.Equals(SortOrder.ByValue);
            SortDirectionAscending.ShowIcon = ApplicationSettings.SortDirection.Equals(SortDirection.Ascending);
            SortDirectionDescending.ShowIcon = ApplicationSettings.SortDirection.Equals(SortDirection.Descending);
        }
    }
}
