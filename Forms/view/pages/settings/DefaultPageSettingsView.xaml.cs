using System;
using MyCryptos.Core.settings;
using MyCryptos.Forms.Messages;
using MyCryptos.Forms.Resources;

namespace MyCryptos.Forms.view.pages.settings
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
				ApplicationSettings.DefaultPage = Core.Types.StartupPage.GraphView;
			}
			else if (sender.Equals(TableOptionCell))
			{
				ApplicationSettings.DefaultPage = Core.Types.StartupPage.TableView;
			}
			else if (sender.Equals(RatesOptionCell))
			{
				ApplicationSettings.DefaultPage = Core.Types.StartupPage.RatesView;
			}
			Messaging.DefaultView.SendValueChanged();
			UpdateCheckmarks();
		}

		private void UpdateCheckmarks()
		{
			GraphOptionCell.ShowIcon = ApplicationSettings.DefaultPage.Equals(Core.Types.StartupPage.GraphView);
			TableOptionCell.ShowIcon = ApplicationSettings.DefaultPage.Equals(Core.Types.StartupPage.TableView);
			RatesOptionCell.ShowIcon = ApplicationSettings.DefaultPage.Equals(Core.Types.StartupPage.RatesView);
			Header.InfoText = $"{I18N.DefaultView}: {(GraphOptionCell.ShowIcon ? I18N.AssetsGraph : TableOptionCell.ShowIcon ? I18N.AssetsTable : I18N.Rates)}";
		}
	}
}
