using MyCC.Forms.iOS.renderer;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(Frame), typeof(CustomFrameRenderer))]
namespace MyCC.Forms.iOS.renderer
{
    public class CustomFrameRenderer : FrameRenderer
    {
        protected override void SetBackgroundColor(Color color)
        {
            //base.SetBackgroundColor(color);
        }
    }
}