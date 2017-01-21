using System;
using MyCC.Core.Settings;
using MyCC.Core.Types;
using MyCC.Forms.Messages;
using MyCC.Forms.Resources;

namespace MyCC.Forms.view.pages.settings
{
    public partial class DefaultPageSettingsView
    {
        public DefaultPageSettingsView()
        {
            InitializeComponent();
            UpdateCheckmarks();
        }

        private void SelectionChanged(object sender, EventArgs e)
        {
            if (sender.Equals(GraphOptionCell))
            {
                ApplicationSettings.DefaultPage = StartupPage.GraphView;
            }
            else if (sender.Equals(TableOptionCell))
            {
                ApplicationSettings.DefaultPage = StartupPage.TableView;
            }
            else if (sender.Equals(RatesOptionCell))
            {
                ApplicationSettings.DefaultPage = StartupPage.RatesView;
            }
            Messaging.DefaultView.SendValueChanged();
            UpdateCheckmarks();
        }

        private void UpdateCheckmarks()
        {
            GraphOptionCell.ShowIcon = ApplicationSettings.DefaultPage.Equals(StartupPage.GraphView);
            TableOptionCell.ShowIcon = ApplicationSettings.DefaultPage.Equals(StartupPage.TableView);
            RatesOptionCell.ShowIcon = ApplicationSettings.DefaultPage.Equals(StartupPage.RatesView);
            Header.InfoText = $"{I18N.DefaultView}: {(GraphOptionCell.ShowIcon ? I18N.AssetsGraph : TableOptionCell.ShowIcon ? I18N.AssetsTable : I18N.Rates)}";
        }
    }
}
