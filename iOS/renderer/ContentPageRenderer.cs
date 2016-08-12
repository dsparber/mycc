using System.Collections.Generic;
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
			leftNativeButtons.RemoveAll(b => true);
			rightNativeButtons.RemoveAll(b => true);


			allButtons.ForEach(nativeItem =>
			{
				var info = GetButtonInfo(itemsInfo, nativeItem.Title);

				UIBarButtonItem systemItem = null;

				if (info != null)
				{
					if (info.Text.Equals(Resources.Save))
					{
						systemItem = new UIBarButtonItem(UIBarButtonSystemItem.Done);
					}
					else if (info.Text.Equals(Resources.Add))
					{
						systemItem = new UIBarButtonItem(UIBarButtonSystemItem.Add);
						var x = UIBarButtonSystemItem.Add;
						x.ToString();
					}
					else if (info.Text.Equals(Resources.Cancel))
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
				if (info != null && info.Text.Equals(Resources.Cancel))
				{
					leftNativeButtons.Add(nativeItem);
				}
				else {
					rightNativeButtons.Add(nativeItem);
				}
			});

			navigationItem.RightBarButtonItems = rightNativeButtons.ToArray();
			navigationItem.LeftBarButtonItems = leftNativeButtons.ToArray();
		}

		private ToolbarItem GetButtonInfo(IList<ToolbarItem> items, string name)
		{
			if (string.IsNullOrEmpty(name) || items == null)
				return null;

			return items.ToList().FirstOrDefault(itemData => name.Equals(itemData.Text));
		}
	}
}

