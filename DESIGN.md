# Tile Trip Match Design

Tile Trip Match is a Unity 2D match-3 tile puzzle. The player selects only exposed tiles from a layered board, moves them into a rack, and clears tiles when three of the same `TileType` are present in the rack. The player wins when every board tile has been removed and loses when the rack fills before a match can be resolved.

The project is built with Unity `6000.0.74f1`, URP 2D, uGUI/TextMesh Pro, Addressables, DOTween, and Odin Inspector/Serializer.

## Architecture Decisions

### Scene flow

The enabled build scenes are:

1. `Assets/Scenes/LoadingScene.unity`
2. `Assets/Scenes/HomeScene.unity`
3. `Assets/Scenes/GameplayScene.unity`

`LoadingController` loads shared Addressables, updates `LoadingUI`, then loads the home scene. The home scene owns level selection. `LevelManager.StartLevel` stores the selected level in `GameManager.Instance.Context`, then loads `GameplayScene`.

### Persistent game context

`GameManager` is a `DontDestroyOnLoad` singleton that owns a simple `GameContext`.

`GameContext` stores:

- `LevelSession`, currently the selected level number.
- `CurrentLevelData`, the `LevelDataSO` that should be used by the gameplay scene.

This keeps cross-scene state intentionally small. Scenes still own their own UI and gameplay object references through inspector wiring.

### Level ownership

`LevelManager` is also persistent and keeps an ordered inspector list of `LevelDataSO` assets. Level numbers are one-based, so level 1 maps to `levels[0]`.

The gameplay scene can also hold a fallback `levelData` reference on `BoardManager`, but runtime selection prefers `GameManager.Instance.Context.CurrentLevelData` when available.

### Board and rack separation

`BoardManager` owns board construction, tile exposure, click handling, win/lose state, and result overlay display.

`TileRack` owns rack capacity, tile movement into rack slots, match detection, match removal animation, rack compaction, and overflow reporting.

They communicate through events:

- `TileRack.OnMatchRemovedFromRack` lets `BoardManager` remove matched tiles from its active board list and re-check win state.
- `TileRack.OnRackOverflow` lets `BoardManager` enter the lose state.

This split keeps board visibility rules out of the rack and keeps rack matching rules out of board generation.

### Tile model/view split

Each tile is represented by:

- `TileModel`: plain runtime state (`Type`, `Layer`, `GridPosition`, `IsRemoved`, `IsExposed`).
- `TileView`: Unity-facing behavior for sprites, collider bounds, overlay visibility, tap audio, and click dispatch.

`TileView.OnMouseDown` raycasts all colliders under the pointer and chooses the exposed tile with the highest layer. This protects against clicking a lower exposed collider when multiple tiles overlap.

### Addressable shared assets

`AssetsLoader` loads two Addressable assets by string address:

- `TileSpriteMapping`
- `AudioMapping`

`TileSpriteMappingSO` maps `TileType` values to fruit/vegetable sprites. `AudioMappingSO` maps shared tap, match, and background music clips. The same loader is used by loading, gameplay, tile audio, rack audio, UI audio, and background music.

## Level Data Structure

Levels are authored as `LevelDataSO` assets in `Assets/ScriptableObjects/Levels`.

`LevelDataSO` contains:

- `Width`: number of grid columns.
- `Height`: number of grid rows.
- `RackSize`: maximum number of tiles allowed in the rack.
- `TimeLimitSeconds`: stored on the asset, but not currently enforced by gameplay.
- `Layers`: a list of `LevelLayerData`.

Each `LevelLayerData` contains an `int[,] Tiles` matrix serialized through Odin. Unity's built-in serializer does not handle multidimensional arrays cleanly, so `LevelDataSO` derives from `SerializedScriptableObject` and the matrix uses `[OdinSerialize]`.

Each matrix cell is interpreted as:

- `0`: empty cell.
- Non-zero integer: cast to `TileType`.

Coordinates use `grid[x, y]`. `BoardManager` translates them to world space with:

- horizontal spacing from tile collider width plus `tileGapX`
- vertical spacing from tile collider height plus `tileGapY`
- per-layer visual offsets `offsetX` and `offsetY`
- a scene-level `boardOrigin`

Layers are indexed from bottom to top. Higher layer indexes visually sit above lower layers and block them when their collider bounds overlap.

## Runtime Gameplay Flow

1. `BoardManager.Start` subscribes to rack events, initializes tile size from the prefab collider, loads shared assets, chooses the current level data, applies the rack capacity, hides the result overlay, and generates the board.
2. `GenerateBoard` clears existing tiles and instantiates one `TileView` for every non-zero cell in every layer.
3. Each tile receives a `TileModel`, sprite from `TileSpriteMappingSO`, sorting order based on layer, and a click callback.
4. `RefreshExposure` marks each non-removed tile as exposed only when no higher-layer tile intersects its world bounds.
5. Clicking an exposed tile sends it to `TileRack.TryAccept`.
6. The rack animates the tile into the next slot using DOTween.
7. Once placement finishes, the rack searches for any three tiles of the same type, regardless of slot order.
8. A matched triple scales out, is destroyed, and the rack compacts remaining tiles.
9. `BoardManager` re-checks exposure and win state after each move or match removal.

## How solvability is ensured

Solvability is not currently guaranteed by a solver or validator. The runtime enforces the rules of play, but it does not prove that a level can be completed.

A level is solvable when there exists a click sequence where:

- every clicked tile is exposed at the time it is clicked;
- the rack never reaches capacity before a triple can be removed;
- all non-empty board tiles can eventually be removed in triples.

The current design relies on authoring discipline:

- Tile counts should be created in multiples of three for every `TileType`.
- Upper layers should reveal useful lower-layer tiles in an order that gives the player enough rack space to progress.
- Each level should be manually playtested after editing.

At runtime, exposure is recalculated using collider intersections against higher layers. This makes visual overlap the source of truth for blocking, which is convenient for hand-authored boards but means solvability depends on both matrix data and prefab/collider layout.

## Trade-offs and areas to improve

- Add an editor validation pass for `LevelDataSO`. It should check that every non-zero tile value maps to a defined `TileType`, every type count is divisible by three, and no level references missing sprites.
- Add a solvability checker. A depth-first or breadth-first search over remaining tiles plus rack contents could verify that at least one completion path exists.
- Fix or re-check level data invariants. While reading the serialized assets, `Level4Data` appears to contain 43 non-empty tiles with four entries of tile type `6`, which violates the triple-count rule.
- Use `TimeLimitSeconds` or remove it. The field exists in level data but no gameplay code currently counts down or fails the level on time.
- Make level selector count data-driven. `LevelSelectorUI` is configured for `totalLevels = 10`, while the home scene currently assigns seven level assets to `LevelManager`.
- Centralize Addressables lifetime. `LoadingController` and `BoardManager` both call `AssetsLoader.LoadAsync`, and `BoardManager.OnDestroy` releases the handles. A reference-counted or persistent asset service would avoid accidental release while persistent audio/UI systems still depend on the assets.
- Avoid stringly typed Addressable lookups. `AssetsLoader` currently depends on `"TileSpriteMapping"` and `"AudioMapping"` addresses. Asset references or constants would reduce typo risk.
- Tighten class/file naming. `Assets/Scripts/UI/Gameplay/NextLevelButton.cs` declares `NextButton`, which works in Unity but is harder to navigate and maintain.
- Consider replacing `OnMouseDown` with the Input System/UI event path. The project already includes the Input System, and a consistent input layer would make mobile/touch behavior easier to control.
- Add automated play mode tests for core rules: exposure refresh, rack overflow, triple removal, rack compaction, win state, and lose state.
