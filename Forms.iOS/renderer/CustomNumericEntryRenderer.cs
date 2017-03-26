using MyCC.Forms.iOS.renderer;
using MyCC.Forms.View.Components.BaseComponents;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(NumericEntry), typeof(CustomNumericEntryRenderer))]
namespace MyCC.Forms.iOS.renderer
{
    public class CustomNumericEntryRenderer : EntryRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (Control == null) return;

            Control.ClearButtonMode = UITextFieldViewMode.WhileEditing;
            Control.BorderStyle = UITextBorderStyle.None;
            Control.KeyboardType = (e.NewElement as NumericEntry)?.IsPin ?? false ? UIKeyboardType.NumberPad : UIKeyboardType.DecimalPad;
        }
    }
}