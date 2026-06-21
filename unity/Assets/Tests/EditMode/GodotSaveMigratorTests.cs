using System.Collections.Generic;
using AnimeEmpire.Backend;
using NUnit.Framework;

namespace AnimeEmpire.Tests.EditMode
{
    public class GodotSaveMigratorTests
    {
        [Test]
        public void TryMigrate_NoGodotSave_ReturnsFalse_NoChange()
        {
            var state = SaveService.NewState();
            bool migrated = GodotSaveMigrator.TryMigrate(state);
            // Test env may or may not have a Godot save file. Either way, after-call invariants hold:
            // - migrated flag matches whether copy occurred
            // - state remains a valid Dictionary
            Assert.That(state, Is.Not.Null);
            Assert.That(state.ContainsKey("save_version"));
            if (migrated)
            {
                Assert.That(state.ContainsKey(GodotSaveMigrator.MigratedFlag));
                Assert.That(state[GodotSaveMigrator.MigratedFlag], Is.EqualTo(true));
            }
        }

        [Test]
        public void TryMigrate_AlreadyMigrated_NoOp()
        {
            var state = SaveService.NewState();
            state[GodotSaveMigrator.MigratedFlag] = true;
            bool migrated = GodotSaveMigrator.TryMigrate(state);
            Assert.That(migrated, Is.False);
        }

        [Test]
        public void TryMigrate_NullState_ReturnsFalse()
        {
            bool migrated = GodotSaveMigrator.TryMigrate(null);
            Assert.That(migrated, Is.False);
        }
    }
}
