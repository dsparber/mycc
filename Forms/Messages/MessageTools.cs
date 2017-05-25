using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;


namespace MyCC.Forms.Messages
{
    public static class MessageTools
    {
        private static void Subscribe(this string message, object subscriber, List<Tuple<MessageInfo, Action>> actions = null)
        {
            MessagingCenter.Subscribe<MessageInfo>(subscriber, message, i =>
            {
                var action = actions.FirstOrDefault(a => a.Item1.Equals(i));

                action?.Item2();
            });
        }

        public static void Unsubscribe(this string message, object subscriber)
        {
            MessagingCenter.Unsubscribe<MessageInfo>(subscriber, message);
        }

        public static void SubscribeValueChanged(this string message, object subscriber, Action action)
        {
            Subscribe(message, subscriber, new List<Tuple<MessageInfo, Action>> { Tuple.Create(MessageInfo.ValueChanged, action) });
        }

        public static void SubscribeFinished(this string message, object subscriber, Action action)
        {
            Subscribe(message, subscriber, new List<Tuple<MessageInfo, Action>> { Tuple.Create(MessageInfo.Finished, action) });
        }

        public static void SubscribeStartedAndFinished(this string message, object subscriber, Action startedAction, Action finishedAction)
        {
            Subscribe(message, subscriber, new List<Tuple<MessageInfo, Action>> { Tuple.Create(MessageInfo.Finished, finishedAction), Tuple.Create(MessageInfo.Started, startedAction) });
        }

        public static void Send(this string message, double value)
        {
            MessagingCenter.Send(value.ToString(), message);
        }

        public static void Send(this string message, bool value)
        {
            MessagingCenter.Send(value.ToString(), message);
        }

        public static void SubscribeToBool(this string message, object subscriber, Action<bool> action)
        {
            MessagingCenter.Subscribe<string>(subscriber, message, s => action(bool.Parse(s)));
        }

        public static void SubscribeToComplete(this string message, object subscriber, Action action)
        {
            MessagingCenter.Subscribe<string>(subscriber, message, s =>
            {
                if (double.Parse(s) > 0.99) action();
            });
        }

        public static void SendValueChanged(this string message)
        {
            MessagingCenter.Send(MessageInfo.ValueChanged, message);

        }
        public static void SendStarted(this string message)
        {
            MessagingCenter.Send(MessageInfo.Started, message);
        }

        public static void SendFinished(this string message)
        {
            MessagingCenter.Send(MessageInfo.Finished, message);
        }
    }
}
