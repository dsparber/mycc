using System;
using System.Collections.Generic;
using System.Globalization;
using Xamarin.Forms;

namespace MyCC.Ui.Messages
{
    public static class MessageTools
    {
        public static void Subscribe(this string message, object subscriber, Action action) =>
            MessagingCenter.Subscribe<string>(subscriber, message, s => action());

        public static void Subscribe(this string message, object subscriber, Action<bool> action) =>
            MessagingCenter.Subscribe<string>(subscriber, message, s => action(bool.Parse(s)));

        public static void Subscribe(this string message, object subscriber, Action<double> action) =>
            MessagingCenter.Subscribe<string>(subscriber, message, s => action(double.Parse(s)));

        public static void SubscribeFinished(this string message, object subscriber, Action action) =>
            MessagingCenter.Subscribe<string>(subscriber, message, s =>
            {
                if (double.Parse(s, CultureInfo.InvariantCulture) > 0.99)
                {
                    action();
                }
            });

        public static void Send(this string message) =>
            MessagingCenter.Send("X", message);

        internal static void Send(this IEnumerable<string> messages)
        {
            foreach (var m in messages) m.Send();
        }

        internal static void Send(this string message, double value) =>
            MessagingCenter.Send(value.ToString(), message);

        public static void Send(this string message, bool value) =>
            MessagingCenter.Send(value.ToString(), message);


    }
}
