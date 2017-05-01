using Android.App;
using Android.Content;
using AlertDialog = Android.Support.V7.App.AlertDialog;

namespace MyCC.Ui.Android.Helpers
{
    public static class AlertDialogHelper
    {
        public static void ShowInfoDialog(this Context context, int idTitle, int idMessage)
        {
            var alertDialog = new AlertDialog.Builder(context)
                    .SetTitle(idTitle)
                    .SetMessage(idMessage)
                    .Create();

            alertDialog.SetButton((int)DialogButtonType.Neutral, context.Resources.GetString(Resource.String.Ok), (sender, args) => alertDialog.Dismiss());

            alertDialog.Show();
        }

        public static ProgressDialog GetLoadingDialog(this Context context, int? idTitle, int idMessage)
        {
            return ProgressDialog.Show(context, idTitle == null ? string.Empty : context.GetString(idTitle.Value), context.GetString(idMessage), true);
        }
    }
}