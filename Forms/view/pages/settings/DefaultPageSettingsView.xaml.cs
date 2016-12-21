using System;
using MyCryptos.Core.settings;
using MyCryptos.Forms.Messages;

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
			Messaging.DefaultView.SendValueChanged();
			UpdateCheckmarks();
		}

		private void UpdateCheckmarks()
		{
			GraphOptionCell.ShowIcon = ApplicationSettings.DefaultPage.Equals(Core.Types.StartupPage.GraphView);
			TableOptionCell.ShowIcon = ApplicationSettings.DefaultPage.Equals(Core.Types.StartupPage.TableView);
		}
	}
}
