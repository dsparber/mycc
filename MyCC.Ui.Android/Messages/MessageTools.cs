using System;
using System.Collections.Generic;
using System.Globalization;
using Xamarin.Forms;

namespace MyCC.Ui.Android.Messages
{
    public static class MessageTools
    {
        public static void Unsubscribe(this string message, object subscriber) =>
            MessagingCenter.Unsubscribe<string>(subscriber, message);

        public static void Subscribe(this string message, object subscriber, Action action) =>
            MessagingCenter.Subscribe<string>(subscriber, message, s => action());

        public static void Subscribe(this string message, object subscriber, Action<bool> action) =>
        MessagingCenter.Subscribe<string>(subscriber, message, s => action(bool.Parse(s)));


        public static void Subscribe(this string message, object subscriber, Action<double> action) =>
            MessagingCenter.Subscribe<string>(subscriber, message, s => action(double.Parse(s, CultureInfo.InvariantCulture)));


        public static void Send(this string message) =>
            MessagingCenter.Send("X", message);

        public static void Send(this IEnumerable<string> messages)
        {
            foreach (var m in messages) m.Send();
        }

        public static void Send(this string message, double value) =>
            MessagingCenter.Send(value.ToString(CultureInfo.InvariantCulture), message);

        public static void Send(this string message, bool value) =>
            MessagingCenter.Send(value.ToString(), message);


    }
}
