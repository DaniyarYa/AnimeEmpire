using System.Collections.Generic;
using System.Threading.Tasks;
using AnimeEmpire.Backend;
using global::Firebase.RemoteConfig;
using UnityEngine;

namespace AnimeEmpire.Firebase
{
    public class FirebaseRemoteConfigProvider : IRemoteConfigProvider
    {
        public int Version { get; private set; }

        public async Task<bool> FetchAsync()
        {
            try
            {
                await FirebaseRemoteConfig.DefaultInstance.FetchAndActivateAsync();
                Version = (int)((System.DateTimeOffset)FirebaseRemoteConfig.DefaultInstance.Info.FetchTime).ToUnixTimeSeconds();
                Debug.Log($"[FirebaseRemoteConfig] fetched, version={Version}");
                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"[FirebaseRemoteConfig] fetch failed: {e.Message}");
                return false;
            }
        }

        // Firebase RC keys must start w/ letter/underscore + only contain
        // [A-Za-z0-9_]. Internal code uses dot-separated keys
        // ("economy.cost_growth_early") for Hosting JSON. Map dots to
        // underscores at the adapter boundary.
        static string Map(string key) => key?.Replace('.', '_') ?? "";

        public float GetFloat(string key, float def)
        {
            var v = FirebaseRemoteConfig.DefaultInstance.GetValue(Map(key));
            return v.Source == ValueSource.StaticValue ? def : (float)v.DoubleValue;
        }

        public int GetInt(string key, int def)
        {
            var v = FirebaseRemoteConfig.DefaultInstance.GetValue(Map(key));
            return v.Source == ValueSource.StaticValue ? def : (int)v.LongValue;
        }

        public bool GetBool(string key, bool def)
        {
            var v = FirebaseRemoteConfig.DefaultInstance.GetValue(Map(key));
            return v.Source == ValueSource.StaticValue ? def : v.BooleanValue;
        }

        public string GetString(string key, string def)
        {
            var v = FirebaseRemoteConfig.DefaultInstance.GetValue(Map(key));
            return v.Source == ValueSource.StaticValue ? def : v.StringValue;
        }

        public IReadOnlyDictionary<string, object> ActiveVariants()
        {
            // Phase 2: parse from `ab_variants` Remote Config key as JSON dict.
            return new Dictionary<string, object>();
        }
    }
}
