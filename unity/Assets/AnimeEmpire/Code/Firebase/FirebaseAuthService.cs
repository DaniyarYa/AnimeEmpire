using System.Threading.Tasks;
using AnimeEmpire.Backend;
using global::Firebase.Auth;
using UnityEngine;

namespace AnimeEmpire.Firebase
{
    public static class FirebaseAuthService
    {
        public static string CurrentUserId => FirebaseAuth.DefaultInstance.CurrentUser?.UserId ?? "";

        public static async Task<bool> SignInAnonymouslyAsync()
        {
            if (FirebaseAuth.DefaultInstance.CurrentUser != null)
            {
                BroadcastUserId(FirebaseAuth.DefaultInstance.CurrentUser.UserId);
                return true;
            }
            try
            {
                var result = await FirebaseAuth.DefaultInstance.SignInAnonymouslyAsync();
                if (result?.User != null)
                {
                    BroadcastUserId(result.User.UserId);
                    Debug.Log($"[FirebaseAuth] anonymous sign-in ok, uid={result.User.UserId}");
                    return true;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"[FirebaseAuth] anonymous sign-in failed: {e.Message}");
            }
            return false;
        }

        static void BroadcastUserId(string uid)
        {
            CrashReporter.Current?.SetUserId(uid);
            AnalyticsBus.Sink?.SetUserId(uid);
        }
    }
}
