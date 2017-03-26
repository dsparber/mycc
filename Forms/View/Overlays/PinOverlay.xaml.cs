using System;
using MyCC.Core.Settings;
using MyCC.Core.Types;
using MyCC.Forms.Helpers;
using MyCC.Forms.Messages;
using MyCC.Forms.Resources;

namespace MyCC.Forms.View.Overlays
{
    public partial class PinOverlay
    {
        private readonly PinAction _pinAction;

        public PinOverlay(PinAction pinAction)
        {
            InitializeComponent();

            if (pinAction == PinAction.EnableOrDisable)
            {
                _pinAction = ApplicationSettings.IsPinSet ? PinAction.Disable : PinAction.Enable;
            }
            else
            {
                _pinAction = pinAction;
            }

            OldPinCell.Entry.TextChanged += (sender, e) =>
            {
                if (e.NewTextValue.Length != ApplicationSettings.PinLength) return;

                if (ApplicationSettings.IsPinValid(e.NewTextValue))
                {
                    if (_pinAction == PinAction.Disable)
                    {
                        ApplicationSettings.Pin = null;
                        Messaging.Pin.SendValueChanged();
                        Navigation.PopOrPopModal();
                    }
                    else
                    {
                        NewPinCell.Entry.Focus();
                    }
                }
                else
                {
                    DisplayAlert(I18N.Error, I18N.OldPinWrong, I18N.Cancel);
                    OldPinCell.Entry.Text = string.Empty;
                }
            };

            Title = I18N.Pin;

            switch (_pinAction)
            {
                case PinAction.Enable: Header.InfoText = I18N.EnablePin; PinTable.Root.Remove(OldPinSection); break;
                case PinAction.Disable: Header.InfoText = I18N.DisablePin; PinTable.Root.Remove(ChangePinSection); break;
                case PinAction.Change: Header.InfoText = I18N.ChangePin; break;
                case PinAction.EnableOrDisable: throw new ArgumentException();
                default: throw new ArgumentOutOfRangeException();
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (_pinAction == PinAction.Enable)
            {
                NewPinCell.Entry.Focus();
            }
            else
            {
                OldPinCell.Entry.Focus();
            }
        }

        private void CancelClicked(object sender, EventArgs e)
        {
            Navigation.PopOrPopModal();
        }

        private void SaveClicked(object sender, EventArgs e)
        {
            var oldPin = OldPinCell.Entry.Text ?? string.Empty;
            var newPin = NewPinCell.Entry.Text ?? string.Empty;

            var repeatOk = newPin.Equals(NewPinRepeatCell.Entry.Text) || _pinAction == PinAction.Disable;
            var oldPinOk = _pinAction == PinAction.Enable || ApplicationSettings.IsPinValid(oldPin);
            var pinLongEnough = newPin.Length >= 4 || _pinAction == PinAction.Disable;


            if (!oldPinOk)
            {
                DisplayAlert(I18N.Error, I18N.OldPinWrong, I18N.Cancel);
                return;
            }
            if (!pinLongEnough)
            {
                DisplayAlert(I18N.Error, I18N.PinToShort, I18N.Cancel);
                return;
            }
            if (!repeatOk)
            {
                DisplayAlert(I18N.Error, I18N.NewPinsDontMatch, I18N.Cancel);
                return;
            }

            ApplicationSettings.Pin = newPin;
            Messaging.Pin.SendValueChanged();
            Navigation.PopOrPopModal();
        }
    }
}