# Unused PBR maps (Phase 3+ Forward+ candidates)

These textures (`metallic.png`, `normal.png`, `roughness.png`) ship with the Meshy AI "Little Explorer biped" base avatar. URP/Lit Opaque material currently in use (`Materials/PlayerAvatarMat.mat`) consumes only `diffuse_2048.png` (BaseMap).

When Phase 3 migrates to Forward+ Lit material w/ full PBR (or stays URP/Lit but enables metallic workflow), these maps can be plugged in:

- `_BaseMap` ← `diffuse_2048.png` (already assigned)
- `_BumpMap` ← `normal.png` (set TextureImporter type = NormalMap)
- `_MetallicGlossMap` ← combined Metallic R + Smoothness A from `metallic.png` + inverted `roughness.png` (pack via TexturePacker or shader graph)

LFS: tracked via `*.png filter=lfs` rule for files >50MB (these are 0.5-5MB, plain git OK).

Not assigned to any material in Phase 1/2.
