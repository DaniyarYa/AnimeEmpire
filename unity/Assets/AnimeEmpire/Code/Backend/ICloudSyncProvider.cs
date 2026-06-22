using System.Collections.Generic;
using System.Threading.Tasks;

namespace AnimeEmpire.Backend
{
    /// Phase 2 swap point. Phase 1 ships LocalNoopProvider — no remote.
    /// Phase 2 UGS Cloud Save impl plugs in same interface w/ no SaveService change.
    public interface ICloudSyncProvider
    {
        bool IsAuthenticated { get; }

        /// Returns null if no remote save present or auth fails.
        Task<Dictionary<string, object>> DownloadAsync();

        /// Returns true if upload acked by remote.
        Task<bool> UploadAsync(IReadOnlyDictionary<string, object> state);

        /// Sign-in or session refresh. Idempotent.
        Task<bool> AuthenticateAsync();
    }

    public class LocalNoopProvider : ICloudSyncProvider
    {
        public bool IsAuthenticated => false;
        public Task<Dictionary<string, object>> DownloadAsync() => Task.FromResult<Dictionary<string, object>>(null);
        public Task<bool> UploadAsync(IReadOnlyDictionary<string, object> state) => Task.FromResult(false);
        public Task<bool> AuthenticateAsync() => Task.FromResult(false);
    }
}
