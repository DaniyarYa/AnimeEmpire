using System.IO;
using AnimeEmpire.Backend;
using AnimeEmpire.Core;
using AnimeEmpire.Data;
using AnimeEmpire.Economy;
using AnimeEmpire.Entities;
using AnimeEmpire.UI;
using AnimeEmpire.Utils;
using TMPro;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace AnimeEmpire.Editor
{
    public static class ContentBuilder
    {
        const string SoBuildings = "Assets/AnimeEmpire/ScriptableObjects/Buildings";
        const string SoResources = "Assets/AnimeEmpire/ScriptableObjects/ResourcesChain";
        const string SoNpcs = "Assets/AnimeEmpire/ScriptableObjects/NPCs";
        const string ResourcesPath = "Assets/AnimeEmpire/Resources";
        const string PrefabsEntities = "Assets/AnimeEmpire/Prefabs/Entities";
        const string PrefabsUI = "Assets/AnimeEmpire/Prefabs/UI";
        const string ScenesPath = "Assets/AnimeEmpire/Scenes";
        const string BootScene = ScenesPath + "/Boot.unity";
        const string WorldScene = ScenesPath + "/World.unity";

        [MenuItem("Tools/Anime Empire/Build Phase 1 Content")]
        public static void BuildAll()
        {
            EnsureFolders();
            var resources = BuildResourceDefs();
            var buildings = BuildBuildingDefs(resources);
            var npc = BuildNpcDef();
            BuildBackendConfig();
            BuildIapCatalog();
            BuildUIThemePalette();
            AnimatorControllerBuilder.RebuildBoth();
            LocalizationSeeder.Seed();
            MaterialBuilder.EnsurePlayerMaterial();
            var playerController = AssetDatabase.LoadAssetAtPath<AnimatorController>("Assets/AnimeEmpire/ScriptableObjects/AnimatorControllers/PlayerController.controller");
            var npcController = AssetDatabase.LoadAssetAtPath<AnimatorController>("Assets/AnimeEmpire/ScriptableObjects/AnimatorControllers/NpcController.controller");
            var bootstrap = BuildBootstrapPrefab();
            var playerPrefab = BuildPlayerPrefab(playerController);
            var buildingPrefab = BuildBuildingPrefab();
            var npcPrefab = BuildNpcPrefab(npcController);
            var modalPrefab = BuildBuildingModalPrefab(resources);
            var pauseMenuPrefab = BuildPauseMenuPrefab();
            var tutorialFlow = BuildTutorialFlow();
            var hudPrefab = BuildHUDPrefab(modalPrefab, pauseMenuPrefab, tutorialFlow);
            BuildBootScene();
            BuildWorldScene(playerPrefab, buildingPrefab, npcPrefab, hudPrefab, buildings, npc);
            AddScenesToBuildSettings();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[ContentBuilder] All Phase 1 content built. Open Assets/AnimeEmpire/Scenes/Boot.unity and press Play.");
        }

        static void EnsureFolders()
        {
            EnsureFolder(SoBuildings);
            EnsureFolder(SoResources);
            EnsureFolder(SoNpcs);
            EnsureFolder(ResourcesPath);
            EnsureFolder(PrefabsEntities);
            EnsureFolder(PrefabsUI);
            EnsureFolder(ScenesPath);
        }

        static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path)) return;
            var parts = path.Split('/');
            var cur = parts[0];
            for (int i = 1; i < parts.Length; i++)
            {
                var next = cur + "/" + parts[i];
                if (!AssetDatabase.IsValidFolder(next)) AssetDatabase.CreateFolder(cur, parts[i]);
                cur = next;
            }
        }

        struct ResourceTrio { public ResourceDef Wheat, Flour, Bread; }

        static ResourceTrio BuildResourceDefs()
        {
            var trio = new ResourceTrio
            {
                Wheat = CreateOrUpdate<ResourceDef>($"{SoResources}/wheat.asset", r =>
                {
                    r.Id = "wheat"; r.DisplayNameKey = "resource.wheat.name";
                    r.BaseSellPrice = 1; r.Stage = 0; r.Tier = "common";
                }),
                Flour = CreateOrUpdate<ResourceDef>($"{SoResources}/flour.asset", r =>
                {
                    r.Id = "flour"; r.DisplayNameKey = "resource.flour.name";
                    r.BaseSellPrice = 3; r.Stage = 1; r.Tier = "common";
                }),
                Bread = CreateOrUpdate<ResourceDef>($"{SoResources}/bread.asset", r =>
                {
                    r.Id = "bread"; r.DisplayNameKey = "resource.bread.name";
                    r.BaseSellPrice = 15; r.Stage = 2; r.Tier = "common";
                }),
            };
            return trio;
        }

        struct BuildingSet { public BuildingDef WheatFarm, Mill, Bakery, Market; }

        static BuildingSet BuildBuildingDefs(ResourceTrio r)
        {
            return new BuildingSet
            {
                WheatFarm = CreateOrUpdate<BuildingDef>($"{SoBuildings}/wheat_farm.asset", b =>
                {
                    b.Id = "wheat_farm"; b.DisplayNameKey = "building.wheat_farm.name";
                    b.Category = "generator";
                    b.InputResource = null; b.InputAmount = 0;
                    b.OutputResource = r.Wheat; b.OutputAmount = 1;
                    b.BaseCycleSeconds = 1f; b.BaseCostGold = 100;
                    b.CostGrowth = 1.12f; b.MaxLevel = 25; b.UnlockLevel = 1; b.NpcSlots = 1;
                }),
                Mill = CreateOrUpdate<BuildingDef>($"{SoBuildings}/mill.asset", b =>
                {
                    b.Id = "mill"; b.DisplayNameKey = "building.mill.name";
                    b.Category = "processor";
                    b.InputResource = r.Wheat; b.InputAmount = 3;
                    b.OutputResource = r.Flour; b.OutputAmount = 1;
                    b.BaseCycleSeconds = 10f; b.BaseCostGold = 500;
                    b.CostGrowth = 1.12f; b.MaxLevel = 25; b.UnlockLevel = 3; b.NpcSlots = 1;
                }),
                Bakery = CreateOrUpdate<BuildingDef>($"{SoBuildings}/bakery.asset", b =>
                {
                    b.Id = "bakery"; b.DisplayNameKey = "building.bakery.name";
                    b.Category = "processor";
                    b.InputResource = r.Flour; b.InputAmount = 2;
                    b.OutputResource = r.Bread; b.OutputAmount = 1;
                    b.BaseCycleSeconds = 30f; b.BaseCostGold = 2500;
                    b.CostGrowth = 1.12f; b.MaxLevel = 25; b.UnlockLevel = 8; b.NpcSlots = 1;
                }),
                Market = CreateOrUpdate<BuildingDef>($"{SoBuildings}/market.asset", b =>
                {
                    b.Id = "market"; b.DisplayNameKey = "building.market.name";
                    b.Category = "service";
                    b.InputResource = null; b.OutputResource = null;
                    b.InputAmount = 0; b.OutputAmount = 0;
                    b.BaseCycleSeconds = 0f; b.BaseCostGold = 200;
                    b.CostGrowth = 1.12f; b.MaxLevel = 5; b.UnlockLevel = 1; b.NpcSlots = 0;
                }),
            };
        }

        static NPCDef BuildNpcDef()
        {
            return CreateOrUpdate<NPCDef>($"{SoNpcs}/gatherer_farmer.asset", n =>
            {
                n.Id = "gatherer_farmer"; n.DisplayNameKey = "npc.gatherer_farmer.name";
                n.Category = "gatherer"; n.Rarity = "common";
                n.BaseSpeed = 2f; n.BaseCapacity = 5; n.BaseEfficiency = 0.75f;
                n.HireCostGold = 1000; n.AttachedBuildingCategory = "generator";
            });
        }

        static void BuildBackendConfig()
        {
            CreateOrUpdate<BackendConfig>($"{ResourcesPath}/BackendConfig.asset", c =>
            {
                c.ConfigUrl = "https://animeempire-a8eee.web.app/config.json";
                c.RequestTimeoutSeconds = 5;
            });
        }

        const string SoIap = "Assets/AnimeEmpire/ScriptableObjects/IAP";

        static void BuildIapCatalog()
        {
            EnsureFolder(SoIap);
            var gems100 = CreateOrUpdate<IapProductDef>($"{SoIap}/gems_100.asset", p =>
            {
                p.Id = "gems_100"; p.DisplayNameKey = "iap.gems_100.name";
                p.Sku = "com.animeempire.gems_100"; p.Kind = "consumable";
                p.FallbackPriceUsd = 0.99f; p.GemGrant = 100;
            });
            var gems500 = CreateOrUpdate<IapProductDef>($"{SoIap}/gems_500.asset", p =>
            {
                p.Id = "gems_500"; p.DisplayNameKey = "iap.gems_500.name";
                p.Sku = "com.animeempire.gems_500"; p.Kind = "consumable";
                p.FallbackPriceUsd = 4.99f; p.GemGrant = 600;
            });
            var noAds = CreateOrUpdate<IapProductDef>($"{SoIap}/no_ads.asset", p =>
            {
                p.Id = "no_ads"; p.DisplayNameKey = "iap.no_ads.name";
                p.Sku = "com.animeempire.no_ads"; p.Kind = "non_consumable";
                p.FallbackPriceUsd = 2.99f; p.NoAdsUnlock = true;
            });
            CreateOrUpdate<IapCatalog>($"{ResourcesPath}/IapCatalog.asset", c =>
            {
                c.Products = new System.Collections.Generic.List<IapProductDef> { gems100, gems500, noAds };
            });
        }

        static void BuildUIThemePalette()
        {
            // Mirror godot/themes/main.theme.tres palette. Re-creating asset preserves
            // Inspector edits via CreateOrUpdate idempotency.
            CreateOrUpdate<UIThemePalette>($"{ResourcesPath}/UIThemePalette.asset", p =>
            {
                p.ButtonNormal = new Color(1f, 0.722f, 0.302f, 1f);
                p.ButtonHover = new Color(1f, 0.78f, 0.4f, 1f);
                p.ButtonPressed = new Color(0.85f, 0.6f, 0.2f, 1f);
                p.ButtonDisabled = new Color(0.612f, 0.576f, 0.596f, 1f);
                p.TextDefault = new Color(0.176f, 0.165f, 0.18f, 1f);
                p.TextPressed = new Color(1f, 1f, 1f, 1f);
                p.TextDisabled = new Color(1f, 1f, 1f, 0.6f);
                p.PanelBg = new Color(1f, 0.957f, 0.878f, 1f);
            });
            _cachedPalette = null; // force reload after rebuild
        }

        const string SoTutorial = "Assets/AnimeEmpire/ScriptableObjects/Tutorial";

        static TutorialFlow BuildTutorialFlow()
        {
            EnsureFolder(SoTutorial);
            var welcome = CreateOrUpdate<TutorialStep>($"{SoTutorial}/step_welcome.asset", s =>
            {
                s.Id = "welcome"; s.MessageKey = "tutorial.welcome";
                s.Advance = TutorialAdvance.TapAnywhere;
            });
            var clickFarm = CreateOrUpdate<TutorialStep>($"{SoTutorial}/step_click_farm.asset", s =>
            {
                s.Id = "click_farm"; s.MessageKey = "tutorial.click_farm";
                s.Advance = TutorialAdvance.BuildingClicked;
                s.ExpectedBuildingId = "wheat_farm";
            });
            var sellAtMarket = CreateOrUpdate<TutorialStep>($"{SoTutorial}/step_sell.asset", s =>
            {
                s.Id = "sell_at_market"; s.MessageKey = "tutorial.sell_at_market";
                s.Advance = TutorialAdvance.ResourceSold;
            });
            return CreateOrUpdate<TutorialFlow>($"{SoTutorial}/PhaseOneTutorial.asset", f =>
            {
                f.Steps = new System.Collections.Generic.List<TutorialStep> { welcome, clickFarm, sellAtMarket };
            });
        }

        static T CreateOrUpdate<T>(string path, System.Action<T> configure) where T : ScriptableObject
        {
            var asset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<T>();
                AssetDatabase.CreateAsset(asset, path);
            }
            configure(asset);
            EditorUtility.SetDirty(asset);
            return asset;
        }

        static GameObject BuildBootstrapPrefab()
        {
            var path = $"{ResourcesPath}/Bootstrap.prefab";
            var go = new GameObject("Bootstrap");
            go.AddComponent<GameState>();
            go.AddComponent<Localization>();
            go.AddComponent<RemoteConfig>();
            go.AddComponent<SaveService>();
            go.AddComponent<EconomySim>();
            go.AddComponent<NPCSystem>();
            go.AddComponent<AnalyticsBus>();
            go.AddComponent<MonetizationService>();
            go.AddComponent<AudioService>();
            go.AddComponent<NotificationService>();
            go.AddComponent<AnimeEmpire.Core.SettingsService>();
            go.AddComponent<SceneRouter>();
            var prefab = PrefabUtility.SaveAsPrefabAsset(go, path);
            Object.DestroyImmediate(go);
            return prefab;
        }

        static GameObject BuildPlayerPrefab(AnimatorController controller)
        {
            var path = $"{PrefabsEntities}/Player.prefab";
            var go = new GameObject("Player");
            var cc = go.AddComponent<CharacterController>();
            cc.height = 1.6f; cc.radius = 0.35f; cc.center = new Vector3(0, 0.8f, 0);
            var player = go.AddComponent<Player>();

            GameObject model = TryInstantiatePlayerModel(go, controller, out var animator);
            var animCtrl = (model != null ? model : go).AddComponent<PlayerAnimationController>();
            var so = new SerializedObject(animCtrl);
            so.FindProperty("_animator").objectReferenceValue = animator;
            so.ApplyModifiedPropertiesWithoutUndo();
            var playerSo = new SerializedObject(player);
            playerSo.FindProperty("_animController").objectReferenceValue = animCtrl;
            playerSo.ApplyModifiedPropertiesWithoutUndo();

            var prefab = PrefabUtility.SaveAsPrefabAsset(go, path);
            Object.DestroyImmediate(go);
            return prefab;
        }

        static GameObject TryInstantiatePlayerModel(GameObject parent, AnimatorController controller, out Animator animator)
        {
            animator = null;
            var modelAsset = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/AnimeEmpire/Art/Characters/PlayerAvatar/v0/player_avatar.fbx");
            var mat = MaterialBuilder.EnsurePlayerMaterial();
            if (modelAsset != null)
            {
                var inst = (GameObject)PrefabUtility.InstantiatePrefab(modelAsset);
                inst.name = "Model";
                inst.transform.SetParent(parent.transform, false);
                animator = inst.GetComponentInChildren<Animator>();
                if (animator == null) animator = inst.AddComponent<Animator>();
                animator.runtimeAnimatorController = controller;
                animator.applyRootMotion = false;
                MaterialBuilder.AssignToRenderers(inst, mat);
                return inst;
            }
            var fallback = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            fallback.name = "Model";
            fallback.transform.SetParent(parent.transform, false);
            fallback.transform.localPosition = new Vector3(0, 0.8f, 0);
            Object.DestroyImmediate(fallback.GetComponent<CapsuleCollider>());
            animator = fallback.AddComponent<Animator>();
            animator.runtimeAnimatorController = controller;
            animator.applyRootMotion = false;
            return fallback;
        }

        static GameObject BuildBuildingPrefab()
        {
            var path = $"{PrefabsEntities}/Building.prefab";
            var go = new GameObject("Building");
            go.AddComponent<Building>();
            var model = GameObject.CreatePrimitive(PrimitiveType.Cube);
            model.name = "Mesh";
            Object.DestroyImmediate(model.GetComponent<BoxCollider>());
            model.transform.SetParent(go.transform, false);
            model.transform.localScale = new Vector3(2.5f, 2.5f, 2.5f);
            model.transform.localPosition = new Vector3(0, 1.25f, 0);
            var col = go.AddComponent<BoxCollider>();
            col.size = new Vector3(2.5f, 2.5f, 2.5f);
            col.center = new Vector3(0, 1.25f, 0);
            var prefab = PrefabUtility.SaveAsPrefabAsset(go, path);
            Object.DestroyImmediate(go);
            return prefab;
        }

        static GameObject BuildNpcPrefab(AnimatorController controller)
        {
            var path = $"{PrefabsEntities}/NPC.prefab";
            var go = new GameObject("NPC");
            var cc = go.AddComponent<CharacterController>();
            cc.height = 1.6f; cc.radius = 0.3f; cc.center = new Vector3(0, 0.8f, 0);
            var npc = go.AddComponent<NPC>();
            GameObject model = TryInstantiatePlayerModel(go, controller, out var animator);
            var animCtrl = (model != null ? model : go).AddComponent<NpcAnimationController>();
            var so = new SerializedObject(animCtrl);
            so.FindProperty("_animator").objectReferenceValue = animator;
            so.ApplyModifiedPropertiesWithoutUndo();
            var npcSo = new SerializedObject(npc);
            npcSo.FindProperty("_anim").objectReferenceValue = animCtrl;
            npcSo.ApplyModifiedPropertiesWithoutUndo();
            var prefab = PrefabUtility.SaveAsPrefabAsset(go, path);
            Object.DestroyImmediate(go);
            return prefab;
        }

        static GameObject BuildBuildingModalPrefab(ResourceTrio r)
        {
            var path = $"{PrefabsUI}/BuildingModal.prefab";
            var root = new GameObject("BuildingModal");
            var rt = root.AddComponent<RectTransform>();
            rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;

            var modal = root.AddComponent<BuildingModal>();
            var panel = new GameObject("Panel");
            panel.transform.SetParent(root.transform, false);
            var panelRT = panel.AddComponent<RectTransform>();
            panelRT.anchorMin = new Vector2(0.5f, 0.5f);
            panelRT.anchorMax = new Vector2(0.5f, 0.5f);
            panelRT.sizeDelta = new Vector2(560, 420);
            var bg = panel.AddComponent<Image>();
            bg.color = new Color(0, 0, 0, 0.7f);

            var title = MakeText(panel.transform, "Title", new Vector2(0.05f, 0.78f), new Vector2(0.95f, 0.95f), 36);
            var info = MakeText(panel.transform, "Info", new Vector2(0.05f, 0.42f), new Vector2(0.95f, 0.76f), 24);
            var actionBtn = MakeButton(panel.transform, "ActionButton", new Vector2(0.05f, 0.28f), new Vector2(0.95f, 0.4f), "Start production");
            var assignBtn = MakeButton(panel.transform, "AssignButton", new Vector2(0.05f, 0.15f), new Vector2(0.45f, 0.27f), "Assign");
            var dismissBtn = MakeButton(panel.transform, "DismissButton", new Vector2(0.55f, 0.15f), new Vector2(0.95f, 0.27f), "Dismiss");
            var closeBtn = MakeButton(panel.transform, "CloseButton", new Vector2(0.05f, 0.03f), new Vector2(0.95f, 0.13f), "Close");

            var so = new SerializedObject(modal);
            so.FindProperty("_root").objectReferenceValue = root;
            so.FindProperty("_title").objectReferenceValue = title;
            so.FindProperty("_info").objectReferenceValue = info;
            so.FindProperty("_actionButton").objectReferenceValue = actionBtn.GetComponent<Button>();
            so.FindProperty("_assignButton").objectReferenceValue = assignBtn.GetComponent<Button>();
            so.FindProperty("_dismissButton").objectReferenceValue = dismissBtn.GetComponent<Button>();
            so.FindProperty("_closeButton").objectReferenceValue = closeBtn.GetComponent<Button>();
            so.FindProperty("_actionButtonLabel").objectReferenceValue = actionBtn.GetComponentInChildren<TMP_Text>();
            so.FindProperty("_wheat").objectReferenceValue = r.Wheat;
            so.FindProperty("_flour").objectReferenceValue = r.Flour;
            so.FindProperty("_bread").objectReferenceValue = r.Bread;
            so.ApplyModifiedPropertiesWithoutUndo();

            var prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
            Object.DestroyImmediate(root);
            return prefab;
        }

        static TMP_Text MakeText(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax, int size)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rt = go.AddComponent<RectTransform>();
            rt.anchorMin = anchorMin; rt.anchorMax = anchorMax;
            rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = name;
            tmp.fontSize = size;
            tmp.color = Color.white;
            tmp.alignment = TextAlignmentOptions.Center;
            return tmp;
        }

        static UIThemePalette _cachedPalette;
        static UIThemePalette Palette
        {
            get
            {
                if (_cachedPalette != null) return _cachedPalette;
                _cachedPalette = AssetDatabase.LoadAssetAtPath<UIThemePalette>($"{ResourcesPath}/UIThemePalette.asset");
                return _cachedPalette;
            }
        }

        static GameObject MakeButton(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax, string label)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rt = go.AddComponent<RectTransform>();
            rt.anchorMin = anchorMin; rt.anchorMax = anchorMax;
            rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
            var img = go.AddComponent<Image>();
            var p = Palette;
            img.color = p != null ? p.ButtonNormal : new Color(0.2f, 0.2f, 0.25f, 0.9f);
            var btn = go.AddComponent<Button>();
            btn.targetGraphic = img;
            if (p != null)
            {
                var colors = btn.colors;
                colors.normalColor = p.ButtonNormal;
                colors.highlightedColor = p.ButtonHover;
                colors.pressedColor = p.ButtonPressed;
                colors.disabledColor = p.ButtonDisabled;
                btn.colors = colors;
            }
            var tmp = MakeText(go.transform, "Label", new Vector2(0, 0), new Vector2(1, 1), 24);
            if (p != null) tmp.color = p.TextDefault;
            tmp.text = label;
            return go;
        }

        static GameObject BuildPauseMenuPrefab()
        {
            var path = $"{PrefabsUI}/PauseMenu.prefab";
            var root = new GameObject("PauseMenu");
            var rt = root.AddComponent<RectTransform>();
            rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;

            var pm = root.AddComponent<PauseMenu>();

            // Open button: top-right gear.
            var openBtn = MakeButton(root.transform, "OpenButton", new Vector2(0.95f, 0.86f), new Vector2(0.995f, 0.97f), "⚙");

            // Modal root (hidden by default — PauseMenu.Awake disables).
            var modal = new GameObject("ModalRoot");
            modal.transform.SetParent(root.transform, false);
            var mRT = modal.AddComponent<RectTransform>();
            mRT.anchorMin = Vector2.zero; mRT.anchorMax = Vector2.one;
            mRT.offsetMin = Vector2.zero; mRT.offsetMax = Vector2.zero;
            var dim = modal.AddComponent<Image>();
            dim.color = new Color(0, 0, 0, 0.65f);

            var panel = new GameObject("Panel");
            panel.transform.SetParent(modal.transform, false);
            var pRT = panel.AddComponent<RectTransform>();
            pRT.anchorMin = new Vector2(0.5f, 0.5f);
            pRT.anchorMax = new Vector2(0.5f, 0.5f);
            pRT.sizeDelta = new Vector2(640, 520);
            var pBg = panel.AddComponent<Image>();
            pBg.color = new Color(0.05f, 0.07f, 0.1f, 0.95f);

            MakeText(panel.transform, "Title", new Vector2(0.05f, 0.85f), new Vector2(0.95f, 0.97f), 44).text = "Pause";
            var resume = MakeButton(panel.transform, "ResumeButton", new Vector2(0.1f, 0.66f), new Vector2(0.9f, 0.78f), "Resume");
            var settingsBtn = MakeButton(panel.transform, "SettingsButton", new Vector2(0.1f, 0.51f), new Vector2(0.9f, 0.63f), "Settings");
            var quit = MakeButton(panel.transform, "QuitButton", new Vector2(0.1f, 0.36f), new Vector2(0.9f, 0.48f), "Quit");
            var version = MakeText(panel.transform, "VersionLabel", new Vector2(0.1f, 0.04f), new Vector2(0.9f, 0.12f), 20);
            version.text = "v0.0.1"; version.alignment = TextAlignmentOptions.Center;

            // Settings sub-panel (hidden by default).
            var settingsPanel = new GameObject("SettingsPanel");
            settingsPanel.transform.SetParent(modal.transform, false);
            var sRT = settingsPanel.AddComponent<RectTransform>();
            sRT.anchorMin = new Vector2(0.5f, 0.5f);
            sRT.anchorMax = new Vector2(0.5f, 0.5f);
            sRT.sizeDelta = new Vector2(640, 520);
            var sBg = settingsPanel.AddComponent<Image>();
            sBg.color = new Color(0.05f, 0.07f, 0.1f, 0.95f);

            MakeText(settingsPanel.transform, "SettingsTitle", new Vector2(0.05f, 0.85f), new Vector2(0.95f, 0.97f), 40).text = "Settings";
            MakeText(settingsPanel.transform, "SfxLabel", new Vector2(0.05f, 0.7f), new Vector2(0.4f, 0.8f), 24).text = "SFX";
            var sfxSlider = MakeSlider(settingsPanel.transform, "SfxSlider", new Vector2(0.4f, 0.71f), new Vector2(0.95f, 0.79f));
            MakeText(settingsPanel.transform, "MusicLabel", new Vector2(0.05f, 0.56f), new Vector2(0.4f, 0.66f), 24).text = "Music";
            var musicSlider = MakeSlider(settingsPanel.transform, "MusicSlider", new Vector2(0.4f, 0.57f), new Vector2(0.95f, 0.65f));
            MakeText(settingsPanel.transform, "LocaleLabel", new Vector2(0.05f, 0.42f), new Vector2(0.4f, 0.52f), 24).text = "Language";
            var localeDD = MakeDropdown(settingsPanel.transform, "LocaleDropdown", new Vector2(0.4f, 0.43f), new Vector2(0.95f, 0.51f));
            var back = MakeButton(settingsPanel.transform, "BackButton", new Vector2(0.1f, 0.08f), new Vector2(0.9f, 0.2f), "Back");

            var so = new SerializedObject(pm);
            so.FindProperty("_root").objectReferenceValue = modal;
            so.FindProperty("_settingsPanel").objectReferenceValue = settingsPanel;
            so.FindProperty("_openButton").objectReferenceValue = openBtn.GetComponent<Button>();
            so.FindProperty("_resumeButton").objectReferenceValue = resume.GetComponent<Button>();
            so.FindProperty("_settingsButton").objectReferenceValue = settingsBtn.GetComponent<Button>();
            so.FindProperty("_backFromSettingsButton").objectReferenceValue = back.GetComponent<Button>();
            so.FindProperty("_quitButton").objectReferenceValue = quit.GetComponent<Button>();
            so.FindProperty("_sfxSlider").objectReferenceValue = sfxSlider;
            so.FindProperty("_musicSlider").objectReferenceValue = musicSlider;
            so.FindProperty("_localeDropdown").objectReferenceValue = localeDD;
            so.FindProperty("_versionLabel").objectReferenceValue = version;
            so.ApplyModifiedPropertiesWithoutUndo();

            var prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
            Object.DestroyImmediate(root);
            return prefab;
        }

        static Slider MakeSlider(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rt = go.AddComponent<RectTransform>();
            rt.anchorMin = anchorMin; rt.anchorMax = anchorMax;
            rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
            var bg = go.AddComponent<Image>();
            bg.color = new Color(1, 1, 1, 0.2f);
            var slider = go.AddComponent<Slider>();
            slider.minValue = 0f; slider.maxValue = 1f; slider.value = 0.5f;
            var fillArea = new GameObject("Fill Area");
            fillArea.transform.SetParent(go.transform, false);
            var fillRT = fillArea.AddComponent<RectTransform>();
            fillRT.anchorMin = Vector2.zero; fillRT.anchorMax = Vector2.one;
            fillRT.offsetMin = Vector2.zero; fillRT.offsetMax = Vector2.zero;
            var fill = new GameObject("Fill");
            fill.transform.SetParent(fillArea.transform, false);
            var fRT = fill.AddComponent<RectTransform>();
            fRT.anchorMin = Vector2.zero; fRT.anchorMax = Vector2.one;
            fRT.offsetMin = Vector2.zero; fRT.offsetMax = Vector2.zero;
            var fImg = fill.AddComponent<Image>();
            fImg.color = new Color(0.4f, 0.65f, 1f, 0.95f);
            slider.fillRect = fRT;
            slider.targetGraphic = bg;
            return slider;
        }

        static void BuildTutorialPanelInline(Transform parent, TutorialFlow flow)
        {
            var go = new GameObject("Tutorial");
            go.transform.SetParent(parent, false);
            var rt = go.AddComponent<RectTransform>();
            rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;

            var modal = new GameObject("ModalRoot");
            modal.transform.SetParent(go.transform, false);
            var mRT = modal.AddComponent<RectTransform>();
            mRT.anchorMin = new Vector2(0, 0); mRT.anchorMax = new Vector2(1, 0.18f);
            mRT.offsetMin = new Vector2(40, 100); mRT.offsetMax = new Vector2(-40, 0);
            var bg = modal.AddComponent<Image>();
            bg.color = new Color(0, 0, 0, 0.78f);

            var msg = MakeText(modal.transform, "Message", new Vector2(0.05f, 0.1f), new Vector2(0.95f, 0.9f), 32);
            msg.alignment = TextAlignmentOptions.Center;
            msg.text = "Welcome";

            var tap = MakeButton(modal.transform, "TapAnywhereButton", new Vector2(0.7f, 0.05f), new Vector2(0.98f, 0.95f), "Tap");
            // Transparent overlay covering most of modal so anywhere triggers.
            var tapImg = tap.GetComponent<Image>(); if (tapImg != null) tapImg.color = new Color(0, 0, 0, 0.0f);

            var skip = MakeButton(modal.transform, "SkipButton", new Vector2(0.0f, 0.0f), new Vector2(0.2f, 0.3f), "×");
            var skipText = skip.GetComponentInChildren<TMP_Text>();
            if (skipText != null) skipText.fontSize = 36;

            var ctrl = go.AddComponent<TutorialController>();
            var so = new SerializedObject(ctrl);
            so.FindProperty("_flow").objectReferenceValue = flow;
            so.FindProperty("_root").objectReferenceValue = modal;
            so.FindProperty("_messageLabel").objectReferenceValue = msg;
            so.FindProperty("_skipButton").objectReferenceValue = skip.GetComponent<Button>();
            so.FindProperty("_tapAnywhereButton").objectReferenceValue = tap.GetComponent<Button>();
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        static TMP_Dropdown MakeDropdown(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rt = go.AddComponent<RectTransform>();
            rt.anchorMin = anchorMin; rt.anchorMax = anchorMax;
            rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
            var bg = go.AddComponent<Image>();
            bg.color = new Color(0.2f, 0.2f, 0.25f, 0.95f);
            var dd = go.AddComponent<TMP_Dropdown>();
            var label = MakeText(go.transform, "Label", new Vector2(0.05f, 0), new Vector2(0.95f, 1), 22);
            dd.captionText = label;
            dd.targetGraphic = bg;
            return dd;
        }

        static GameObject BuildHUDPrefab(GameObject modalPrefab, GameObject pauseMenuPrefab, TutorialFlow tutorialFlow)
        {
            var path = $"{PrefabsUI}/HUD.prefab";
            var root = new GameObject("HUD");
            var canvas = root.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;
            var scaler = root.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;
            root.AddComponent<GraphicRaycaster>();
            root.AddComponent<SafeAreaFitter>();

            // TopBar
            var topBar = new GameObject("TopBar");
            topBar.transform.SetParent(root.transform, false);
            var topRT = topBar.AddComponent<RectTransform>();
            topRT.anchorMin = new Vector2(0, 1); topRT.anchorMax = new Vector2(1, 1);
            topRT.pivot = new Vector2(0.5f, 1f);
            topRT.sizeDelta = new Vector2(0, 80);
            var topBg = topBar.AddComponent<Image>();
            topBg.color = new Color(0, 0, 0, 0.45f);

            var gold = MakeText(topBar.transform, "GoldLabel", new Vector2(0.02f, 0f), new Vector2(0.5f, 1f), 36);
            gold.text = "💰 0"; gold.alignment = TextAlignmentOptions.MidlineLeft;
            var inv = MakeText(topBar.transform, "InventoryLabel", new Vector2(0.5f, 0f), new Vector2(0.98f, 1f), 32);
            inv.text = "🌾 0   🌾→ 0   🍞 0"; inv.alignment = TextAlignmentOptions.MidlineRight;

            // VirtualJoystick
            var joystick = new GameObject("VirtualJoystick");
            joystick.transform.SetParent(root.transform, false);
            var jRT = joystick.AddComponent<RectTransform>();
            jRT.anchorMin = Vector2.zero; jRT.anchorMax = Vector2.one;
            jRT.offsetMin = Vector2.zero; jRT.offsetMax = Vector2.zero;
            var vj = joystick.AddComponent<VirtualJoystick>();
            var bg = new GameObject("Background");
            bg.transform.SetParent(joystick.transform, false);
            var bgRT = bg.AddComponent<RectTransform>();
            bgRT.sizeDelta = new Vector2(180, 180);
            var bgImg = bg.AddComponent<Image>();
            bgImg.color = new Color(1, 1, 1, 0.18f);
            var cg = bg.AddComponent<CanvasGroup>();
            cg.alpha = 0.35f;
            cg.blocksRaycasts = false;
            var knob = new GameObject("Knob");
            knob.transform.SetParent(bg.transform, false);
            var knobRT = knob.AddComponent<RectTransform>();
            knobRT.sizeDelta = new Vector2(80, 80);
            var knobImg = knob.AddComponent<Image>();
            knobImg.color = new Color(1, 1, 1, 0.45f);

            var jso = new SerializedObject(vj);
            jso.FindProperty("_background").objectReferenceValue = bgRT;
            jso.FindProperty("_knob").objectReferenceValue = knobRT;
            jso.FindProperty("_backgroundCanvasGroup").objectReferenceValue = cg;
            jso.FindProperty("_canvas").objectReferenceValue = canvas;
            jso.ApplyModifiedPropertiesWithoutUndo();

            // BuildingModal nested instance
            if (modalPrefab != null)
            {
                var modal = (GameObject)PrefabUtility.InstantiatePrefab(modalPrefab);
                modal.transform.SetParent(root.transform, false);
                modal.name = "BuildingModal";
            }

            // PauseMenu nested instance
            if (pauseMenuPrefab != null)
            {
                var pm = (GameObject)PrefabUtility.InstantiatePrefab(pauseMenuPrefab);
                pm.transform.SetParent(root.transform, false);
                pm.name = "PauseMenu";
            }

            // Tutorial root (collapsed panel + message label).
            BuildTutorialPanelInline(root.transform, tutorialFlow);

            var prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
            Object.DestroyImmediate(root);
            return prefab;
        }

        static void BuildBootScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            var canvasGO = new GameObject("Canvas");
            var canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasGO.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            canvasGO.AddComponent<GraphicRaycaster>();

            var palette = Palette;
            var bgColor = palette != null ? palette.ButtonNormal : new Color(1f, 0.722f, 0.302f, 1f);
            var textColor = palette != null ? palette.TextDefault : new Color(0.176f, 0.165f, 0.18f, 1f);

            // Golden full-screen background — mirrors godot/scenes/boot/boot.tscn ColorRect.
            var bgGO = new GameObject("Background");
            bgGO.transform.SetParent(canvasGO.transform, false);
            var bgRT = bgGO.AddComponent<RectTransform>();
            bgRT.anchorMin = Vector2.zero; bgRT.anchorMax = Vector2.one;
            bgRT.offsetMin = Vector2.zero; bgRT.offsetMax = Vector2.zero;
            var bgImg = bgGO.AddComponent<Image>();
            bgImg.color = bgColor;
            bgImg.raycastTarget = false;

            // Centered title + status stack.
            var titleGO = new GameObject("TitleLabel");
            titleGO.transform.SetParent(canvasGO.transform, false);
            var titleRT = titleGO.AddComponent<RectTransform>();
            titleRT.anchorMin = new Vector2(0.5f, 0.5f); titleRT.anchorMax = new Vector2(0.5f, 0.5f);
            titleRT.anchoredPosition = new Vector2(0f, 60f);
            titleRT.sizeDelta = new Vector2(1200, 120);
            var titleTmp = titleGO.AddComponent<TextMeshProUGUI>();
            titleTmp.text = "Anime Empire";
            titleTmp.fontSize = 88;
            titleTmp.fontStyle = FontStyles.Bold;
            titleTmp.alignment = TextAlignmentOptions.Center;
            titleTmp.color = textColor;

            var statusGO = new GameObject("StatusLabel");
            statusGO.transform.SetParent(canvasGO.transform, false);
            var rt = statusGO.AddComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 0.5f); rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = new Vector2(0f, -40f);
            rt.sizeDelta = new Vector2(800, 80);
            var tmp = statusGO.AddComponent<TextMeshProUGUI>();
            tmp.text = "Загрузка...";
            tmp.fontSize = 48;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = textColor;

            var bootGO = new GameObject("Boot");
            var boot = bootGO.AddComponent<BootController>();
            var bso = new SerializedObject(boot);
            bso.FindProperty("_statusLabel").objectReferenceValue = tmp;
            bso.ApplyModifiedPropertiesWithoutUndo();

            EnsureEventSystem();

            EditorSceneManager.SaveScene(scene, BootScene);
        }

        static void BuildWorldScene(GameObject playerPrefab, GameObject buildingPrefab, GameObject npcPrefab, GameObject hudPrefab, BuildingSet b, NPCDef npcDef)
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            var worldGO = new GameObject("World");
            var world = worldGO.AddComponent<WorldController>();

            var light = new GameObject("Directional Light");
            var dl = light.AddComponent<Light>();
            dl.type = LightType.Directional;
            dl.intensity = 1.2f;
            light.transform.rotation = Quaternion.Euler(40, 35, 0);

            var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Ground";
            ground.transform.localScale = new Vector3(4f, 1f, 4f);
            var gMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            gMat.color = new Color(0.498f, 0.741f, 0.404f);
            ground.GetComponent<MeshRenderer>().sharedMaterial = gMat;

            GameObject Spawn(GameObject prefab, string nm, Vector3 pos)
            {
                var inst = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                inst.name = nm;
                inst.transform.position = pos;
                return inst;
            }

            var player = Spawn(playerPrefab, "Player", new Vector3(0, 0, 4));

            var wheatFarm = Spawn(buildingPrefab, "WheatFarm", new Vector3(-6, 0, -2));
            wheatFarm.GetComponent<Building>().Def = b.WheatFarm;
            var mill = Spawn(buildingPrefab, "Mill", new Vector3(-2, 0, -2));
            mill.GetComponent<Building>().Def = b.Mill;
            var bakery = Spawn(buildingPrefab, "Bakery", new Vector3(2, 0, -2));
            bakery.GetComponent<Building>().Def = b.Bakery;
            var market = Spawn(buildingPrefab, "Market", new Vector3(6, 0, -2));
            market.GetComponent<Building>().Def = b.Market;

            var npc = Spawn(npcPrefab, "GathererFarmer", new Vector3(-6, 0, 2));
            var npcComp = npc.GetComponent<NPC>();
            npcComp.Def = npcDef;
            npcComp.AssignedBuilding = wheatFarm.GetComponent<Building>();

            var camGO = new GameObject("Main Camera");
            var cam = camGO.AddComponent<Camera>();
            cam.tag = "MainCamera";
            cam.backgroundColor = new Color(0.40f, 0.65f, 0.85f);
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.transform.position = new Vector3(0, 8, -8);
            cam.transform.LookAt(player.transform.position);
            var rig = camGO.AddComponent<CameraRig>();
            rig.FollowTarget = player.transform;
            rig.FollowOffset = new Vector3(0, 8, -12);
            camGO.AddComponent<UnityEngine.AudioListener>();
            camGO.AddComponent<PhysicsRaycaster>();

            EnsureEventSystem();

            var hud = (GameObject)PrefabUtility.InstantiatePrefab(hudPrefab);
            hud.name = "HUD";

            var so = new SerializedObject(world);
            so.FindProperty("_player").objectReferenceValue = player.GetComponent<Player>();
            so.FindProperty("_camera").objectReferenceValue = rig;
            so.FindProperty("_joystick").objectReferenceValue = hud.transform.Find("VirtualJoystick").GetComponent<VirtualJoystick>();
            so.FindProperty("_goldLabel").objectReferenceValue = hud.transform.Find("TopBar/GoldLabel").GetComponent<TMP_Text>();
            so.FindProperty("_inventoryLabel").objectReferenceValue = hud.transform.Find("TopBar/InventoryLabel").GetComponent<TMP_Text>();
            so.FindProperty("_modal").objectReferenceValue = hud.transform.Find("BuildingModal").GetComponent<BuildingModal>();
            so.ApplyModifiedPropertiesWithoutUndo();

            EditorSceneManager.SaveScene(scene, WorldScene);
        }

        static void EnsureEventSystem()
        {
            if (Object.FindAnyObjectByType<EventSystem>() != null) return;
            var es = new GameObject("EventSystem");
            es.AddComponent<EventSystem>();
            es.AddComponent<InputSystemUIInputModule>();
        }

        static void AddScenesToBuildSettings()
        {
            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene(BootScene, true),
                new EditorBuildSettingsScene(WorldScene, true),
            };
        }
    }
}
