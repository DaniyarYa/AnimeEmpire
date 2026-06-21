# Contributing — Unity track

Unity 6 LTS (URP) parallel implementation. Godot at `../godot/` is canonical until parity validated.

## Prerequisites

- Unity 6 LTS (6000.0+) w/ Android + iOS build modules.
- Git LFS (`brew install git-lfs && git lfs install`). FBX/PSD/large PNG already tracked.
- macOS or Linux (iOS build requires macOS).

## First open

1. Clone repo, open `unity/` as Unity project. First import ~10 min.
2. `ProjectBootstrapper` runs once: Linear color space, landscape, IL2CPP, bundle id `com.animeempire.app`.
3. Run **Tools → Anime Empire → Build Phase 1 Content**. Authors SOs, animator controllers, localization, prefabs, scenes.
4. Open `Assets/AnimeEmpire/Scenes/Boot.unity`. Play.

## Code style

- **Namespaces**: `AnimeEmpire.{Core, Data, Economy, Entities, UI, Backend, Platform, Utils}`. Editor under `AnimeEmpire.Editor`.
- **C# conventions**: PascalCase public, `_camelCase` private fields, `using` directives sorted, file-scoped namespaces ok.
- **No `Find` in hot paths**: use `BuildingRegistry`, `NpcRegistry`, or cached refs. `FindObjectsByType` only during scene-init or Editor scripts.
- **Static EventBus events** (not UnityEvents). Always unsubscribe in `OnDestroy`/`OnDisable`.
- **MonoBehaviour singletons** for `Bootstrap.prefab` services. `Instance` property, null-checked in `Awake`, cleared in `OnDestroy`.
- **No `[Header]`/`[Tooltip]` clutter**: brief tooltips on inspector-edited fields only. Skip headers entirely.
- **Save data shape stays Dictionary<string,object>**. No POCO migration — must round-trip through JSON identical to Godot.

## Behavior parity rules

When editing `unity/`:

- **EconomySim tick rate** stays 10 Hz off `Update` accumulator. Don't move to `FixedUpdate` — physics tick is decoupled by design.
- **ProductionLine cycle decay** = `0.97^(level-1)`. Don't tune unless Godot tunes first.
- **NPC FSM** has 8 states matching `npc.gd`. Add states in Godot first, mirror here.
- **Save schema fields** match Godot 1:1. Field renames need migration plan in `../GDD/`.

If Unity diverges from Godot, document in `../.claude/plans/godot-soft-quokka.md` §Differences or a follow-up plan.

## Tests

- EditMode: **Window → General → Test Runner → EditMode → Run All**. 24+ tests pass.
- PlayMode: same panel → PlayMode → Run All.
- Headless: `Unity -batchmode -runTests -testPlatform editmode -projectPath unity`.
- New tests live under `Assets/Tests/{EditMode,PlayMode}/`. Always pair `[SetUp]` w/ `[TearDown]` for static state (especially `EventBus`).

## Builds

Headless via GameCI (`.github/workflows/`) or local:

```bash
# Android .aab
Unity -batchmode -nographics -quit -projectPath unity \
  -executeMethod AnimeEmpire.Editor.BuildScript.BuildAndroid \
  -logFile build/android.log

# iOS Xcode project (macOS only)
Unity -batchmode -nographics -quit -projectPath unity \
  -executeMethod AnimeEmpire.Editor.BuildScript.BuildIOS \
  -logFile build/ios.log
```

Override output path: `BUILD_OUTPUT=custom/path.aab Unity ...`.

## Adding content

Prefer code over Inspector-only edits when possible — diffs are reviewable.

- **New BuildingDef / ResourceDef / NPCDef**: add `.asset` via Tools menu OR Project window → Create → Anime Empire → ... Commit the `.asset` file.
- **New scene**: must register in `EditorBuildSettings` (ContentBuilder rewrites this — re-run after adding scenes).
- **New animator state**: extend `PlayerAnimationStateNames` + `AnimatorControllerBuilder.CanonicalStates`. Re-run **Rebuild Animator Controllers**.
- **New localization key**: add to `LocalizationSeeder.Entries` (EN+RU minimum). Re-run **Seed Localization**. Don't author keys only in Inspector — they get lost on re-seed.

## Commit conventions

Match `../godot/` style: `feat(scope): description (P-XXX)` with optional ticket refs. Russian or English ok, code identifiers stay English. Co-author trailer for AI-assisted commits.

## Branches

- `main` — green, deployable.
- Feature branches: `unity/feature-name` or `unity/p1-XXX-description`.
- Mirror Godot ticket IDs (`P1-XXX`) for parity work.

## What NOT to commit

- `Library/`, `Temp/`, `Logs/`, `UserSettings/`, `obj/`, `*.csproj`, `*.sln`, `Build/`. All in `.gitignore`.
- Personal Unity license files.
- Custom `EditorPrefs` overrides.

## Phase 1 scope reminder

In: production chain (wheat→flour→bread), 1 NPC + FSM, 4 buildings, save schema v1, RemoteConfig HTTP, RU+EN locales.

Out: Firebase SDK, Addressables, UI Toolkit, Cinemachine, cloud save, push, real IAP, Godot save migration.
