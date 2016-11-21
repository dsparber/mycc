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

			rearrangeButtons();
		}

		ToolbarItem GetButtonInfo(IList<ToolbarItem> items, string name)
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
					if (info.Text.Equals(I18N.Save))
					{
						systemItem = new UIBarButtonItem(UIBarButtonSystemItem.Save);
					}
                    if (info.Text.Equals(I18N.Done))
                    {
                        systemItem = new UIBarButtonItem(UIBarButtonSystemItem.Done);
                    }
                    if (info.Text.Equals(I18N.Edit))
					{
						systemItem = new UIBarButtonItem(UIBarButtonSystemItem.Edit);
					}
					else if (info.Text.Equals(I18N.Add))
					{
						systemItem = new UIBarButtonItem(UIBarButtonSystemItem.Add);
						var x = UIBarButtonSystemItem.Add;
						x.ToString();
					}
					else if (info.Text.Equals(I18N.Cancel))
					{
						systemItem = new UIBarButtonItem(UIBarButtonSystemItem.Cancel);
					}
                    else if (info.Text.Equals(I18N.Refresh))
                    {
                        nativeItem.Image = new UIImage("refresh.png");
                    }
                }
				if (systemItem != null)
				{
					systemItem.Title = nativeItem.Title;
					systemItem.Action = nativeItem.Action;
					systemItem.Target = nativeItem.Target;
					nativeItem = systemItem;
				}

                if (nativeItem.Image != null)
                {
                    nativeItem.Title = null;
                }

				if (info != null && (info.Text.Equals(I18N.Cancel) || info.Text.Equals(I18N.Refresh)))
				{
					leftNativeButtons.Add(nativeItem);
				}
				else {
					rightNativeButtons.Add(nativeItem);
				}
			});
			rightNativeButtons.RemoveAll(b => leftNativeButtons.Contains(b));

			navigationItem.RightBarButtonItems = rightNativeButtons.Distinct().ToArray();
			navigationItem.LeftBarButtonItems = leftNativeButtons.Distinct().ToArray();
		}
	}
}

