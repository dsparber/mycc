using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MyCryptos.resources;
using renderer;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ContentPage), typeof(ContentPageRenderer))]
namespace renderer
{
	public class ContentPageRenderer : PageRenderer
	{
		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);

			rearrangeButtons();
		}

		private ToolbarItem GetButtonInfo(IList<ToolbarItem> items, string name)
		{
			if (string.IsNullOrEmpty(name) || items == null)
				return null;

			return items.ToList().FirstOrDefault(itemData => name.Equals(itemData.Text));
		}

		void rearrangeButtons()
		{
			if (NavigationController == null)
			{
				return;
			}

			var itemsInfo = (Element as ContentPage).ToolbarItems;

			var navigationItem = NavigationController.TopViewController.NavigationItem;
			var leftNativeButtons = (navigationItem.LeftBarButtonItems ?? new UIBarButtonItem[] { }).ToList();
			var rightNativeButtons = (navigationItem.RightBarButtonItems ?? new UIBarButtonItem[] { }).ToList();
			var allButtons = new List<UIBarButtonItem>(leftNativeButtons);
			allButtons.AddRange(rightNativeButtons);
			rightNativeButtons.RemoveAll(b => true);

			allButtons.ForEach(nativeItem =>
			{
				var info = GetButtonInfo(itemsInfo, nativeItem.Title);

				UIBarButtonItem systemItem = null;

				if (info != null)
				{
					if (info.Text.Equals(InternationalisationResources.Save))
					{
						systemItem = new UIBarButtonItem(UIBarButtonSystemItem.Done);
					}
					else if (info.Text.Equals(InternationalisationResources.Add))
					{
						systemItem = new UIBarButtonItem(UIBarButtonSystemItem.Add);
						var x = UIBarButtonSystemItem.Add;
						x.ToString();
					}
					else if (info.Text.Equals(InternationalisationResources.Cancel))
					{
						systemItem = new UIBarButtonItem(UIBarButtonSystemItem.Cancel);
					}
				}
				if (systemItem != null)
				{
					systemItem.Title = nativeItem.Title;
					systemItem.Action = nativeItem.Action;
					systemItem.Target = nativeItem.Target;
					nativeItem = systemItem;
				}

				if (info != null && info.Text.Equals(InternationalisationResources.Cancel))
				{
					leftNativeButtons.Add(nativeItem);
				}
				else {
					rightNativeButtons.Add(nativeItem);
				}
			});
			rightNativeButtons.RemoveAll(b => leftNativeButtons.Contains(b));

			navigationItem.RightBarButtonItems = rightNativeButtons.ToArray();
			navigationItem.LeftBarButtonItems = leftNativeButtons.ToArray();
		}
	}
}

