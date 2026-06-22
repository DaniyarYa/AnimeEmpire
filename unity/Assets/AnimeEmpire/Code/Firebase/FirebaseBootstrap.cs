using AnimeEmpire.Backend;
using global::Firebase;
using UnityEngine;

namespace AnimeEmpire.Firebase
{
    /// Auto-installs all Firebase adapters before scene load. Requires
    /// FIREBASE_ENABLED define + Firebase Unity SDK present.
    public static class FirebaseBootstrap
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static async void Install()
        {
            DependencyStatus status;
            try
            {
                status = await FirebaseApp.CheckAndFixDependenciesAsync();
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"[FirebaseBootstrap] dependency check threw: {e.Message}");
                return;
            }
            if (status != DependencyStatus.Available)
            {
                Debug.LogWarning($"[FirebaseBootstrap] dependencies not available: {status}");
                return;
            }
            _ = FirebaseApp.DefaultInstance;

            CrashReporter.SetProvider(new FirebaseCrashlyticsReporter());
            AnalyticsBus.RegisterSink(new FirebaseAnalyticsSink());
            RemoteConfig.RegisterProvider(new FirebaseRemoteConfigProvider());
            await FirebaseAuthService.SignInAnonymouslyAsync();
            await FirebaseMessagingProvider.Instance.RegisterAsync();

            Debug.Log("[FirebaseBootstrap] all adapters installed");
        }
    }
}
