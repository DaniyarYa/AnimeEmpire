using System.Collections;
using AnimeEmpire.Data;
using AnimeEmpire.Entities;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace AnimeEmpire.Tests.PlayMode
{
    public class NpcFsmSmokeTests
    {
        GameObject _buildingGO;
        GameObject _npcGO;

        [SetUp]
        public void SetUp()
        {
            // Spawn ground so CharacterController has floor.
            var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.transform.localScale = new Vector3(10, 1, 10);

            var wheat = ScriptableObject.CreateInstance<ResourceDef>();
            wheat.Id = "wheat"; wheat.BaseSellPrice = 1;

            var farmDef = ScriptableObject.CreateInstance<BuildingDef>();
            farmDef.Id = "wheat_farm"; farmDef.Category = "generator";
            farmDef.OutputResource = wheat; farmDef.OutputAmount = 1;
            farmDef.BaseCycleSeconds = 0.5f;

            var marketDef = ScriptableObject.CreateInstance<BuildingDef>();
            marketDef.Id = "market"; marketDef.Category = "service";

            _buildingGO = new GameObject("Farm");
            _buildingGO.transform.position = new Vector3(2, 0, 0);
            _buildingGO.AddComponent<BoxCollider>();
            var farm = _buildingGO.AddComponent<Building>();
            farm.Def = farmDef;

            var marketGO = new GameObject("Market");
            marketGO.transform.position = new Vector3(-5, 0, 0);
            marketGO.AddComponent<BoxCollider>();
            var market = marketGO.AddComponent<Building>();
            market.Def = marketDef;

            var npcDef = ScriptableObject.CreateInstance<NPCDef>();
            npcDef.Id = "gatherer"; npcDef.BaseSpeed = 6f; npcDef.BaseCapacity = 2; npcDef.BaseEfficiency = 1f;
            npcDef.AttachedBuildingCategory = "generator";

            _npcGO = new GameObject("NPC");
            _npcGO.transform.position = new Vector3(0, 1, 0);
            var agent = _npcGO.AddComponent<UnityEngine.AI.NavMeshAgent>();
            agent.height = 1.6f; agent.radius = 0.3f; agent.speed = 6f;
            var npc = _npcGO.AddComponent<NPC>();
            npc.Def = npcDef;
            npc.AssignedBuilding = farm;
        }

        [TearDown]
        public void TearDown()
        {
            foreach (var go in Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None))
                if (go != null) Object.Destroy(go);
        }

        [UnityTest]
        public IEnumerator NPC_RegistersInRegistry()
        {
            yield return null;
            Assert.That(NpcRegistry.All.Count, Is.GreaterThan(0), "NPC must register in NpcRegistry on Awake");
        }

        [UnityTest]
        public IEnumerator NPC_FindAvailableForCategory_AfterIdle()
        {
            // NPC starts in Move state (auto-assigned in Start). It's not available.
            yield return null;
            var found = NpcRegistry.FindAvailableForCategory("generator");
            Assert.That(found, Is.Null, "NPC mid-task should not be available");
        }
    }
}
