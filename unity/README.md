# Anime Empire — Unity port

Unity 6 LTS (URP, mobile) port of the Godot project at `../godot/`. Goal: behavior parity with Godot canonical implementation, evaluation track for mobile target.

## Status

**~20 commits shipped.** Phase 1 vertical slice + Phase 2 hooks all in tree:

| System | Status |
|---|---|
| Production chain (wheat → flour → bread) | ✅ |
| 4 buildings + 1 NPC FSM (8 states) | ✅ |
| EconomySim 10 Hz tick + offline catch-up | ✅ |
| Save/load (JSON, 2s debounce, OnPause flush) | ✅ |
| RemoteConfig (HTTP w/ disk cache + defaults) | ✅ |
| Localization (Unity Localization, 6 locales EN+RU seeded) | ✅ |
| HUD + VirtualJoystick + BuildingModal + PauseMenu + Settings | ✅ |
| Animator controllers + URP/Lit material + FBX postprocessor | ✅ |
| Tutorial flow (3 sample steps) | ✅ |
| Audio service stub + EventBus hooks | ✅ |
| Mobile notifications (local, Unity Mobile Notifications package) | ✅ |
| Addressables fallback in AppBootstrap | ✅ |
| Save migration Godot save_0.bin → Unity save_0.json | ✅ |
| Mobile safe area handling | ✅ |
| IAP catalog SO (3 sample products) | ✅ |
| CrashReporter interface + LocalLogReporter | ✅ |
| Cloud sync provider interface | ✅ |
| UIThemePalette (Godot main.theme.tres parity) | ✅ |
| Asset validator + Debug overlay + Versioned builds | ✅ |
| GameCI workflows (test + Android + iOS builds) | ✅ |
| 40+ EditMode + PlayMode tests | ✅ |
| Firebase swap layer (FIREBASE_ENABLED define gate) | ✅ — see [docs/firebase-setup.md](docs/firebase-setup.md) |
| UGS Cloud Save, real IAP, FCM remote push | Phase 2+ — requires credentials |

## First-time setup

1. Install **Unity 6 LTS (6000.0.x)** with **Android + iOS** build modules.
2. Open this folder (`unity/`) as a Unity project. First import will take ~10 minutes — packages compile, FBX assets process, Player avatar rig generated.
3. On open, `ProjectBootstrapper` runs once and applies: color space Linear, landscape orientation, IL2CPP defaults, bundle id `com.animeempire.app`.
4. Run **Tools → Anime Empire → Build Phase 1 Content**. This menu authors:
   - ScriptableObjects: 4 buildings, 3 resources, 1 NPC, BackendConfig.
   - AnimatorControllers: PlayerController + NpcController, w/ canonical states wired to FBX clips.
   - Localization: locales en/ru/es/fr/de/ja + "UI" string table seeded with building/resource/NPC names and UI labels (en + ru filled, others placeholder).
   - Prefabs: Bootstrap (Resources/), Player, Building, NPC, HUD, BuildingModal — Animator + AnimationController fields auto-wired.
   - Scenes: `Assets/AnimeEmpire/Scenes/Boot.unity` + `World.unity`.
   - Build settings: Boot at index 0, World at 1.

   Other useful menus:
   - **Tools → Anime Empire → Rebuild Animator Controllers** — re-derive controllers from FBX after rig/clip edits.
   - **Tools → Anime Empire → Seed Localization (UI table)** — re-seed string table entries.
5. Open `Boot.unity` and press **Play**. Splash → World → joystick (left half), tap buildings (right half), assign NPC, sell at Market.

## Layout

```
Assets/AnimeEmpire/
├── Code/         — C# runtime (Core / Data / Economy / Entities / UI / Backend / Platform / Utils)
├── Editor/       — InitializeOnLoad project settings + ContentBuilder + FBX/texture postprocessors
├── Data/         — see Code/Data for SO definitions
├── ScriptableObjects/ — authored .asset files (buildings/resources/npcs)
├── Resources/    — Bootstrap.prefab + BackendConfig.asset (Resources.Load roots)
├── Prefabs/      — Player/Building/NPC/HUD/BuildingModal
├── Scenes/       — Boot, World
├── Art/          — Characters/PlayerAvatar (FBX + diffuse 2048) + UI/buildings/fx placeholders
├── Localization/ — Unity Localization string tables (to be authored, see Phase 1 scope)
└── Settings/     — URP-Mobile, InputActions, Quality
Assets/Tests/{EditMode,PlayMode}/  — Unity Test Framework
Packages/manifest.json             — package list
ProjectSettings/ProjectVersion.txt — Unity 6 LTS pin
.github/workflows/                 — GameCI test + Android/iOS build
```

## Architecture mapping (Godot → Unity)

| Godot | Unity |
|---|---|
| Autoload singleton | `MonoBehaviour` on `Resources/Bootstrap.prefab` + `[RuntimeInitializeOnLoadMethod(BeforeSceneLoad)]` spawn + `DontDestroyOnLoad` |
| `extends Resource` (.tres) | `ScriptableObject` + `[CreateAssetMenu]` (.asset) |
| `signal foo(...)` | `static event Action<...>` on `EventBus` (auto-reset on `SubsystemRegistration`) |
| `CharacterBody3D.move_and_slide` | `CharacterController.Move(velocity * dt)` |
| `Area3D.input_event` | `BoxCollider` + `IPointerClickHandler` + Camera `Physics Raycaster` + EventSystem |
| `AnimationPlayer` + `animation_finished` | `Animator` + `AnimationEvent` callback `OnAnimationStateFinished(state)` |
| `HTTPRequest.request` | `UnityWebRequest.Get` + `SendAsync()` (TaskCompletionSource) |
| `user://save_0.bin` | `Application.persistentDataPath/save_0.json` |
| `tr("key")` | `Localization.T("key")` wrapping Unity Localization |
| `get_tree().get_nodes_in_group("buildings")` | `BuildingRegistry` static list |

## Behavior parity

- **EconomySim**: 10 Hz accumulator off `Update` (decoupled from FixedUpdate). Same `RegisterLineFromDef`, `SellInventory`, `SpendGold`, `GrantGold`, `SimulateOffline(4h cap, 0.5 efficiency)`.
- **ProductionLine**: `cycle = base * 0.97^(L-1)`, `speed` modifier identical.
- **NPC FSM**: 8 states `Idle → Move → WorkSit → WorkGather → WorkStand → Carry → Deliver → Return`. Timer-based primary, AnimationEvent secondary. Capacity-gated carry. Storage target = first building w/ category "service".
- **SaveService**: `save_version = 1`, 2s debounce, `OnApplicationPause`/`Quit` force-flush. Schema fields match Godot 1:1.
- **RemoteConfig**: same JSON schema (`schema_version`, `version`, `values`, `ab_variants`), same defaults (`economy.cost_growth_early=1.12`, etc.).
- **VirtualJoystick**: dynamic positioning, 80px max radius, 10% deadzone, left 50% active zone.
- **CameraRig**: follow target + offset, pinch zoom, free-mode timer 3s.

Differences from Godot intentional:
- Joystick `y` → world `+Z` (forward). Unity convention, opposite sign vs Godot, equivalent UX.
- NPC FSM uses timer-based transitions for SIT/STAND/DELIVER (matching Godot's defensive approach). AnimationEvents are secondary path.
- ProjectSettings.asset values not committed; ProjectBootstrapper applies on first open via InitializeOnLoad.

## Out of scope (Phase 1)

- Firebase SDK (raw HTTP only).
- Addressables, UI Toolkit, Cinemachine.
- Cloud save / cross-device sync.
- Push notifications.
- Real IAP.
- Migrating existing Godot `save_0.bin` saves to Unity.

## Risks (see `../.claude/plans/godot-soft-quokka.md` §Risks)

- FBX rig orientation may yaw-flip 180° on import. If Player faces backward, rotate `Model` child 180° on Y in Player.prefab.
- AnimationClip names from FBX renamed via `PlayerAssetPostprocessor` to canonical short names (`idle`, `walk`, etc.). If state hashes don't match Animator Controller, check `Assets/AnimeEmpire/Editor/PlayerAssetPostprocessor.cs:MapToCanonicalState`.
- Unity Localization initializes async; `Localization.T()` returns key as fallback until ready.

## Tests

- Run EditMode: **Window → General → Test Runner → EditMode → Run All**. Expect 14 passing.
- Run PlayMode: **Test Runner → PlayMode → Run All**. Expect 2 passing.
- CI runs both via GameCI on push/PR to `main`.

## Headless builds

- Android `.aab`: `Unity -batchmode -nographics -quit -projectPath unity -executeMethod AnimeEmpire.Editor.BuildScript.BuildAndroid -logFile build/android.log`
- iOS Xcode project: `Unity -batchmode -nographics -quit -projectPath unity -executeMethod AnimeEmpire.Editor.BuildScript.BuildIOS -logFile build/ios.log` (macOS runner)
- Override output via `BUILD_OUTPUT=path/to/file.aab` env var.
