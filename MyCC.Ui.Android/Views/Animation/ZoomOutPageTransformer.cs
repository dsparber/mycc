using System;
using Android.Support.V4.View;
using Android.Views;

namespace MyCC.Ui.Android.Views.Animation
{
    public class ZoomOutPageTransformer : Java.Lang.Object, ViewPager.IPageTransformer
    {
        private const float MinScale = 0.85f;
        private const float MinAlpha = 1f;

        public void TransformPage(View view, float position)
        {
            var pageWidth = view.Width;
            var pageHeight = view.Height;

            if (position < -1 || position > 1)
            {
                view.Alpha = 0;
            }
            else
            {
                var scaleFactor = Math.Max(MinScale, 1 - Math.Abs(position));
                var vertMargin = pageHeight * (1 - scaleFactor) / 2;
                var horzMargin = pageWidth * (1 - scaleFactor) / 2;
                if (position < 0)
                {
                    view.TranslationX = horzMargin - vertMargin / 2;
                }
                else
                {
                    view.TranslationX = -horzMargin + vertMargin / 2;
                }

                view.ScaleX = scaleFactor;
                view.ScaleY = scaleFactor;

                view.Alpha = MinAlpha + (1 - MinAlpha) * ((scaleFactor - MinScale) / (1 - MinScale));
            }
        }
    }
}