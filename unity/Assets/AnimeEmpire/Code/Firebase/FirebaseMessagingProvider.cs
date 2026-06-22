using System;
using System.Threading.Tasks;
using AnimeEmpire.Backend;
using global::Firebase.Messaging;
using UnityEngine;

namespace AnimeEmpire.Firebase
{
    public class FirebaseMessagingProvider : INotificationProvider
    {
        public static readonly FirebaseMessagingProvider Instance = new();

        public event Action<string> TokenRefreshed;
        public event Action<string> NotificationReceived;

        string _token = "";
        TaskCompletionSource<string> _tokenWaiter;

        public async Task RegisterAsync()
        {
            FirebaseMessaging.TokenReceived += OnTokenReceived;
            FirebaseMessaging.MessageReceived += OnMessageReceived;
            try
            {
                await FirebaseMessaging.SubscribeAsync("all");
                Debug.Log("[FirebaseMessaging] subscribed to 'all' topic");
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"[FirebaseMessaging] subscribe failed: {e.Message}");
            }
        }

        public Task<string> GetTokenAsync()
        {
            if (!string.IsNullOrEmpty(_token)) return Task.FromResult(_token);
            _tokenWaiter ??= new TaskCompletionSource<string>();
            return _tokenWaiter.Task;
        }

        void OnTokenReceived(object sender, TokenReceivedEventArgs args)
        {
            _token = args.Token;
            Debug.Log($"[FirebaseMessaging] token received (len={_token.Length})");
            TokenRefreshed?.Invoke(_token);
            _tokenWaiter?.TrySetResult(_token);
            _tokenWaiter = null;
        }

        void OnMessageReceived(object sender, MessageReceivedEventArgs args)
        {
            var payload = args.Message?.Data != null
                ? Newtonsoft.Json.JsonConvert.SerializeObject(args.Message.Data)
                : "{}";
            Debug.Log($"[FirebaseMessaging] message: {payload}");
            NotificationReceived?.Invoke(payload);
        }
    }
}
