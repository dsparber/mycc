using System;
using MyCryptos.Core.Enums;
using MyCryptos.Core.Settings;
using MyCryptos.Forms.helpers;
using MyCryptos.Forms.Messages;
using MyCryptos.view.overlays;
using Plugin.Fingerprint;

namespace MyCryptos.Forms.view.pages.settings
{
    public partial class PinSettingsView
    {
        public PinSettingsView()
        {
            InitializeComponent();

            SetPinCells();

            FingerprintCell.Switch.Toggled += (sender, e) => { ApplicationSettings.IsFingerprintEnabled = e.Value; Messaging.Pin.SendValueChanged(); };

            if (ApplicationSettings.IsFingerprintEnabled)
            {
                FingerprintCell.Switch.IsToggled = true;
            }
            else
            {
                Table.Root.Remove(FingerprintSection);
            }

            Messaging.Pin.SubscribeValueChanged(this, SetPinCells);
        }

        private void SetPinCells()
        {
            ActionSection.Remove(EnablePinCell);
            ActionSection.Remove(DisablePinCell);
            ActionSection.Remove(ChangePinCell);

            if (!ApplicationSettings.IsPinSet)
            {
                ActionSection.Add(EnablePinCell);
                FingerprintCell.Switch.IsEnabled = false;
            }
            else
            {
                ActionSection.Add(DisablePinCell);
                ActionSection.Add(ChangePinCell);
                FingerprintCell.Switch.IsEnabled = true;
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (!await CrossFingerprint.Current.IsAvailableAsync()) return;

            if (!Table.Root.Contains(FingerprintSection))
            {
                Table.Root.Add(FingerprintSection);
            }
        }

        private void EnablePin(object sender, EventArgs e)
        {
            Navigation.PushOrPushModal(new PinOverlay(PinAction.ENABLE));
        }

        private void DisablePin(object sender, EventArgs e)
        {
            Navigation.PushOrPushModal(new PinOverlay(PinAction.DISABLE));
        }

        private void ChangePin(object sender, EventArgs e)
        {
            Navigation.PushOrPushModal(new PinOverlay(PinAction.CHANGE));
        }
    }
}
