using System.Threading.Tasks;
using UnityEngine.Networking;

namespace AnimeEmpire.Platform
{
    public static class UnityWebRequestExtensions
    {
        public static Task<UnityWebRequest> SendAsync(this UnityWebRequest req, int timeoutSeconds = 5)
        {
            req.timeout = timeoutSeconds;
            var tcs = new TaskCompletionSource<UnityWebRequest>();
            var op = req.SendWebRequest();
            op.completed += _ => tcs.TrySetResult(req);
            return tcs.Task;
        }
    }
}
