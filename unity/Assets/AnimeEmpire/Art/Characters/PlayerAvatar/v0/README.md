# Player Avatar · v0

Starter player avatar. Generated via **Meshy AI** ("Little Explorer biped" template). Mirror of `../../../../../godot/assets/characters/player_avatar/v0/`.

## Metadata

| Field | Value |
|-------|-------|
| Source | Meshy AI "Little Explorer biped" template |
| Format | FBX (mesh + skeleton + animation per file) |
| Texture | 2048×2048 PBR diffuse (metallic/normal/roughness in `_unused_pbr/` on Godot side, dropped here — URP/Lit ships diffuse only Phase 1) |
| License | Commercial use allowed (see `../../../../../docs/architecture/08_asset_pipeline.md` ADR-003) |
| Phase | 0 — pipeline validation |

## Structure

```
v0/
├── player_avatar.fbx              ← rig + base mesh (idle clip embedded)
├── Animations/
│   ├── idle.fbx
│   ├── walk.fbx
│   ├── walk_inplace.fbx
│   ├── run.fbx
│   ├── carry_walk.fbx
│   ├── work_harvest.fbx
│   └── celebrate.fbx
├── Materials/PlayerAvatarMat.mat  ← URP/Lit, BaseMap = diffuse_2048
└── Textures/diffuse_2048.png      ← sRGB, ASTC 6×6 on mobile
```

## Import settings

Applied via `Editor/PlayerAssetPostprocessor.cs`:

- **Rig**: Generic (NOT Humanoid) — preserves Meshy bone names + clip mappings 1:1.
- **Avatar**: `player_avatar.fbx` is rig source. Animation FBXs use `CopyFromOther` pointing at the rig avatar.
- **Animation clip rename**: postprocessor maps file name → canonical state name (`idle.fbx → "idle"`). Required because Animator state hashes lookup by canonical name.
- **Loop Time**: ON for idle/walk/walk_inplace/run/carry_walk/work_harvest. OFF for celebrate (one-shot).
- **Texture**: sRGB, 2048 max, mipmaps + streaming, Android+iOS override ASTC 6×6 (ETC2 fallback for old Android).
- **Material**: URP/Lit Opaque, BaseMap = diffuse_2048, authored via `MaterialBuilder.EnsurePlayerMaterial()`.

## Animator wiring

`AnimatorControllerBuilder.RebuildBoth()` authors `PlayerController.controller` + `NpcController.controller` at `Assets/AnimeEmpire/ScriptableObjects/AnimatorControllers/`. States match `PlayerAnimationStateNames` constants (`idle`, `walk`, `run`, `work_sit`, `work_gather`, `work_stand`, `carry_walk`, `celebrate`). No Mecanim transitions — code calls `Animator.CrossFadeInFixedTime(stateHash, 0.1f)` from `PlayerAnimationController` + `NpcAnimationController`.

`work_sit` / `work_stand` currently share `work_gather` clip (no dedicated FBX yet). Add the FBX + extend `LoadCanonicalClips()` when ready.

## Known issues / TODO

- [ ] Tris count not measured. Decimate to ≤1000 in Blender if needed for mobile-low tier.
- [ ] FBX files each carry skin mesh — wasteful. Phase 2: extract anim-only `.fbx` per clip, leave only `player_avatar.fbx` w/ skin.
- [ ] Bone naming convention against `architecture/08_asset_pipeline.md` not verified. Probably Mixamo-ish.
- [ ] Origin location — verify feet at Y=0 (not hips).
- [ ] Loop seamlessness — visual review per clip.
- [ ] Rig orientation: if Player faces backward at runtime, rotate `Model` child 180° on Y in `Player.prefab` (Godot scene had same yaw flip).

## Next steps

1. Open Unity → first import takes ~10 min (FBX rig analysis).
2. Run **Tools → Anime Empire → Build Phase 1 Content** to author controllers + material + prefabs.
3. Open `Boot.unity` → Play → verify idle clip plays on spawn.
4. If looks correct, mark P0-031 / P0-033 done.
