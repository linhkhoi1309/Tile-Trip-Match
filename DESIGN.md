# Architecture Decisions

- Separation of Concerns: BoardManager handles board spawn/exposure and win/lose flow, TileRack handles rack placement/match resolution, LevelManager handles level selection/scene flow.
- Data-driven: Levels are ScriptableObjects (LevelDataSO) containing a list of LevelLayerData layers for easy authoring and reuse.
- Runtime singletons: LevelManager is persistent (DontDestroyOnLoad) so UI and gameplay scenes share level state.
- Addressables / AssetsLoader: tile sprites and audio are loaded at runtime, keeping binary size and startup flexible.

# Level Data Structure

- Layers: Each level is a List<LevelLayerData> stored on LevelDataSO (LevelDataSO.cs:1).
- Grid: Each LevelLayerData contains an int[,] Tiles sized by LevelDataSO.Width/Height (LevelLayerData.cs:1).
- IDs: 0 = empty; 1..N = tile type IDs mapped to sprites by the tile mapping (via AssetsLoader.TileMapping).
- Inspector tooling: Odin TableMatrix is used for grid editing to make patterns easier to author.

# How solvability is ensured

- Intentional matches: Levels include deliberately placed triples (or patterns that lead to triples) so the initial board contains at least one resolvable match.

# Trade-offs and areas to improve

- Trade-offs: Using int[,] is compact and editor-friendly but less explicit than per-tile ScriptableObjects (harder to embed metadata per tile).
- Blocking complexity: Layer stacking introduces blocking interactions that make formal solvability proofs harder; guaranteeing solvability requires either conservative design rules or a solver/generator.
- Improvements with more time:
    - Add an editor-time level validator that simulates gameplay to guarantee solvability before shipping.
    - Implement a constraint-based level generator that outputs provably solvable layouts.
    - Provide a visual exposure preview in the editor so authors can see which tiles are initially exposed.
    - Add timer for restricting time completion for each level