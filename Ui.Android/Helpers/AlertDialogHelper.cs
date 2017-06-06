using Android.App;
using Android.Content;
using Java.Lang;
using AlertDialog = Android.Support.V7.App.AlertDialog;

namespace MyCC.Ui.Android.Helpers
{
    public static class AlertDialogHelper
    {
        public static void ShowInfoDialog(this Context context, int idTitle, int idMessage)
            => ShowInfoDialog(context, context.Resources.GetString(idTitle), context.Resources.GetString(idMessage));

        public static void ShowInfoDialog(this Context context, string title, string message)
        {
            var alertDialog = new AlertDialog.Builder(context)
                .SetTitle(title)
                .SetMessage(message)
                .Create();

            alertDialog.SetButton((int)DialogButtonType.Neutral, context.Resources.GetString(Resource.String.Ok), (sender, args) => alertDialog.Dismiss());

            try
            {
                alertDialog.Show();
            }
            catch (RuntimeException)
            {
                /* App not open - called from background */
            }
        }



        public static ProgressDialog GetLoadingDialog(this Context context, int? idTitle, int idMessage)
        {
            return ProgressDialog.Show(context, idTitle == null ? string.Empty : context.GetString(idTitle.Value), context.GetString(idMessage), true);
        }
    }
}