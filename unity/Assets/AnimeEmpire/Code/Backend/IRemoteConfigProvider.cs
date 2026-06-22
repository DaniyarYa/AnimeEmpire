using System.Collections.Generic;
using System.Threading.Tasks;

namespace AnimeEmpire.Backend
{
    /// Swap point for RemoteConfig data source. Default impl: HTTP fetch +
    /// disk cache (HttpRemoteConfigProvider). Phase 2: FirebaseRemoteConfigProvider.
    public interface IRemoteConfigProvider
    {
        int Version { get; }
        Task<bool> FetchAsync();
        float GetFloat(string key, float def);
        int GetInt(string key, int def);
        bool GetBool(string key, bool def);
        string GetString(string key, string def);
        IReadOnlyDictionary<string, object> ActiveVariants();
    }
}
