using System.Collections.Generic;
using System.IO;
using AnimeEmpire.Backend;
using Newtonsoft.Json;
using NUnit.Framework;

namespace AnimeEmpire.Tests.EditMode
{
    public class SaveServiceRoundTripTests
    {
        [Test]
        public void NewState_JsonSerializes_NoTypeNameHandling()
        {
            var state = SaveService.NewState();
            var json = JsonConvert.SerializeObject(state, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.None,
            });
            // No "$type" markers allowed — they break IL2CPP stripping + are a security risk.
            Assert.That(json, Does.Not.Contain("$type"));
            Assert.That(json, Does.Contain("save_version"));
        }

        [Test]
        public void NewState_RoundTrip_PreservesSchema()
        {
            var state = SaveService.NewState();
            var json = JsonConvert.SerializeObject(state);
            var rt = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
            Assert.That(rt, Is.Not.Null);
            Assert.That(rt.ContainsKey("save_version"));
            Assert.That(rt.ContainsKey("currencies"));
            Assert.That(rt.ContainsKey("prestige"));
            Assert.That(rt.ContainsKey("tutorial"));
            Assert.That(rt.ContainsKey("settings"));
        }

        [Test]
        public void TempFile_WriteRead_StateMatches()
        {
            var path = Path.Combine(Path.GetTempPath(), "anime_empire_save_test.json");
            try
            {
                var state = SaveService.NewState();
                state["currencies"] = new Dictionary<string, object> { { "gold", 123 }, { "gems", 7 } };
                File.WriteAllText(path, JsonConvert.SerializeObject(state));
                var loaded = JsonConvert.DeserializeObject<Dictionary<string, object>>(File.ReadAllText(path));
                Assert.That(loaded, Is.Not.Null);
                var c = JsonConvert.DeserializeObject<Dictionary<string, object>>(loaded["currencies"].ToString());
                Assert.That(System.Convert.ToInt32(c["gold"]), Is.EqualTo(123));
                Assert.That(System.Convert.ToInt32(c["gems"]), Is.EqualTo(7));
            }
            finally
            {
                if (File.Exists(path)) File.Delete(path);
            }
        }
    }
}
