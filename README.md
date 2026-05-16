# Tile Trip Match

Tile Trip Match is a Unity 2D tile-matching puzzle game inspired by layered mahjong-style matching games. Players choose exposed tiles from a stacked board, move them into a rack, and clear matches when three tiles of the same type are collected. The level is won when the board is empty and lost when the rack fills before a match can be resolved.

For deeper implementation notes, see [DESIGN.md](DESIGN.md).

## Overview

The game is built around three scenes:

- `LoadingScene`: loads shared Addressable assets and shows progress.
- `HomeScene`: lets the player select a level.
- `GameplayScene`: generates the board, handles tile selection, rack matching, and win/lose results.

Levels are authored as ScriptableObjects, with each level made from one or more 2D tile layers. Higher layers block lower layers when their tile bounds overlap, so the puzzle comes from choosing exposed tiles in an order that keeps the rack from filling.

## Features

- Layered tile board with overlap-based exposure rules.
- Rack-based matching where any three tiles of the same type clear automatically.
- Win and lose overlays with restart, return, and next-level flow.
- Data-driven level configuration through `LevelDataSO` assets.
- Tile sprite and audio mappings loaded through Unity Addressables.
- DOTween-powered tile movement, match removal, and UI hover feedback.
- Persistent background music, tap sounds, and match sounds.
- Custom cursor support for the desktop build.

## Technical Highlights

### Architecture

- `GameManager` owns persistent cross-scene game context.
- `LevelManager` owns level selection and scene transitions.
- `BoardManager` owns board spawning, tile exposure, click handling, and result state.
- `TileRack` owns rack capacity, movement, match detection, removal, and compaction.
- `TileModel` stores runtime tile state while `TileView` handles Unity-facing visuals, colliders, and clicks.

### Optimization

- Shared sprites and audio mappings are loaded once through Addressables.
- Board tiles are generated only when entering gameplay.
- Exposure checks use existing tile collider bounds as the source of truth, avoiding a separate blocking data structure.
- Rack animations are sequenced with DOTween and kill existing tweens before starting new movement.

### Systems

- **Level data:** `LevelDataSO` stores width, height, rack size, time limit, and a list of `LevelLayerData` matrices.
- **Tile mapping:** `TileSpriteMappingSO` maps `TileType` enum values to tile sprites.
- **Audio mapping:** `AudioMappingSO` stores tap, match, and background music clips.
- **Loading:** `LoadingController` loads Addressables, updates `LoadingUI`, then opens the home scene.
- **Gameplay:** `BoardManager` instantiates tiles from level matrices and recalculates exposure after each move.
- **Rack:** `TileRack` accepts exposed tiles, detects triples, removes matches, and reports overflow.

### Tools & Patterns

- ScriptableObjects for level and asset data.
- Odin Inspector/Serializer for editing multidimensional level grids in the Unity inspector.
- Addressables for runtime asset lookup.
- DOTween for UI and tile animation.
- Unity uGUI and TextMesh Pro for menus, loading UI, and overlays.
- Simple singleton pattern for persistent managers.

## Technical Stack

- Unity `6000.0.74f1`
- Universal Render Pipeline `17.0.4`
- Unity 2D Feature package `2.0.1`
- Addressables `2.9.1`
- Input System `1.19.0`
- uGUI `2.0.0`
- TextMesh Pro
- DOTween
- Odin Inspector and Odin Serializer

## Project Structure

```text
Assets/
  Audio/                         Audio clips
  Images/
    Tiles/                       Tile icon sprites
    UI/                          Backgrounds, buttons, logo, cursor art
  Prefabs/                       Tile and level button prefabs
  Scenes/                        Loading, home, and gameplay scenes
  ScriptableObjects/
    Audios/                      AudioMapping asset
    Levels/                      LevelDataSO assets
    Tiles/                       TileSpriteMapping asset
  Scripts/
    Audio/                       Audio mapping and playback helpers
    Configs/                     Scene name constants
    Core/
      Board/                     BoardManager
      Game/                      GameManager and GameContext
      Level/                     Level data and level session/manager
      Tile/                      Tile model, view, rack, and mappings
    UI/                          Home, gameplay, loading, and common UI scripts
    Utilities/                   Addressable loading helpers
```

## Challenges & Solutions

- **Layer blocking:** Tiles are blocked by higher-layer tiles whose collider bounds intersect them. This keeps board logic aligned with the actual visual layout.
- **Unity serialization:** Level grids use `int[,]`, which Unity does not serialize natively. The project uses Odin serialization and `TableMatrix` editing for author-friendly level data.
- **Rack matching:** Matches are detected by grouping rack tiles by `TileType`, so triples can clear no matter where they are placed in the rack.
- **Scene state:** The selected level is stored in `GameContext` before loading gameplay, which avoids relying on scene object order or static level variables.
- **Shared assets:** Sprites and audio are accessed through `AssetsLoader`, keeping gameplay and UI scripts from directly referencing individual asset files.

## How to Run

1. Open the repository in Unity Editor `6000.0.74f1` or a compatible Unity 6 version.
2. Let Unity restore packages from `Packages/manifest.json`.
3. Open `Assets/Scenes/LoadingScene.unity`.
4. Press Play.

The build scene order is already configured as:

1. `Assets/Scenes/LoadingScene.unity`
2. `Assets/Scenes/HomeScene.unity`
3. `Assets/Scenes/GameplayScene.unity`

To edit levels, open assets under `Assets/ScriptableObjects/Levels`. Tile values use the `TileType` enum, where `0` means empty and non-zero values map to tile sprites through `Assets/ScriptableObjects/Tiles/TileSpriteMapping.asset`.

## Future Improvements

- Add an editor validator for level tile counts, missing sprites, invalid tile IDs, and unreachable layouts.
- Add a solver or generator so levels can be proven solvable before shipping.
- Connect `TimeLimitSeconds` to an actual gameplay timer.
- Make the home level selector read from the assigned level list instead of a separate `totalLevels` value.
- Improve Addressables lifetime management with reference counting or a persistent asset service.
- Add automated play mode tests for exposure, rack overflow, match removal, compaction, win state, and lose state.
