using System;
using System.Threading.Tasks;

namespace AnimeEmpire.Backend
{
    /// Remote push provider (FCM, APNS, etc). Local push goes through
    /// NotificationService directly via Unity Mobile Notifications package.
    public interface INotificationProvider
    {
        Task<string> GetTokenAsync();
        event Action<string> TokenRefreshed;
        event Action<string> NotificationReceived;
    }
}
