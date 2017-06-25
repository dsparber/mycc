using System;
using MyCC.Core.Settings;
using MyCC.Core.Types;
using MyCC.Forms.Resources;

namespace MyCC.Forms.View.Pages.Settings.General
{
    public partial class DefaultPageSettingsView
    {
        public DefaultPageSettingsView()
        {
            InitializeComponent();
            UpdateCheckmarks();
            RatesOptionCell.Text = I18N.Rates;
            GraphOptionCell.Text = $"{I18N.Assets} ({I18N.Graph})";
            TableOptionCell.Text = $"{I18N.Assets} ({I18N.Table})";
        }

        private void SelectionChanged(object sender, EventArgs e)
        {
            if (sender.Equals(GraphOptionCell))
            {
                ApplicationSettings.DefaultStartupPage = StartupPage.GraphView;
            }
            else if (sender.Equals(TableOptionCell))
            {
                ApplicationSettings.DefaultStartupPage = StartupPage.TableView;
            }
            else if (sender.Equals(RatesOptionCell))
            {
                ApplicationSettings.DefaultStartupPage = StartupPage.RatesView;
            }
            UpdateCheckmarks();
        }

        private void UpdateCheckmarks()
        {
            GraphOptionCell.ShowIcon = ApplicationSettings.DefaultStartupPage.Equals(StartupPage.GraphView);
            TableOptionCell.ShowIcon = ApplicationSettings.DefaultStartupPage.Equals(StartupPage.TableView);
            RatesOptionCell.ShowIcon = ApplicationSettings.DefaultStartupPage.Equals(StartupPage.RatesView);
            Header.Info = ApplicationSettings.DefaultStartupPage.Equals(StartupPage.GraphView) ? $"{I18N.Assets} ({I18N.Graph})" : ApplicationSettings.DefaultStartupPage.Equals(StartupPage.TableView) ? $"{I18N.Assets} ({I18N.Table})" : I18N.Rates;
        }
    }
}
