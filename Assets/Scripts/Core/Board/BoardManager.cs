using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    private enum EndState
    {
        None,
        Win,
        Lose
    }

    [Header("Level")]
    [SerializeField] private LevelDataSO levelData;

    [Header("Tile")]
    [SerializeField] private TileView tilePrefab;
    [SerializeField] private Transform boardRoot;

    [Header("Rack")]
    [SerializeField] private TileRack tileRack;

    [Header("Result UI")]
    [SerializeField] private GameResultOverlay resultOverlay;

    [Header("Board Visual Settings")]
    [SerializeField] private float offsetX = 0.25f;
    [SerializeField] private float offsetY = 0.25f;
    [SerializeField] private float tileGapX = 0.8f;
    [SerializeField] private float tileGapY = 0.8f;
    [SerializeField] private Vector3 boardOrigin = Vector3.zero;

    private readonly List<TileView> allTiles = new();
    private Vector2 tileSize;
    private EndState endState = EndState.None;

    private async void Start()
    {
        if (tileRack != null)
        {
            tileRack.OnMatchRemovedFromRack += HandleMatchRemovedFromRack;
            tileRack.OnRackOverflow += HandleRackOverflow;
        }

        Initialize();
        await AssetsLoader.LoadAsync();

        if (GameManager.Instance != null && GameManager.Instance.Context != null && GameManager.Instance.Context.CurrentLevelData != null)
            levelData = GameManager.Instance.Context.CurrentLevelData;

        if (tileRack != null && levelData != null)
            tileRack.SetCapacity(levelData.RackSize);

        endState = EndState.None;

        if (resultOverlay != null)
            resultOverlay.Hide();

        GenerateBoard();
    }

    private void OnDestroy()
    {
        if (tileRack != null)
        {
            tileRack.OnMatchRemovedFromRack -= HandleMatchRemovedFromRack;
            tileRack.OnRackOverflow -= HandleRackOverflow;
        }

        AssetsLoader.Release();
    }

    private void HandleMatchRemovedFromRack(IReadOnlyList<TileView> tiles)
    {
        for (int i = 0; i < tiles.Count; i++)
        {
            TileView tile = tiles[i];

            if (tile != null)
                allTiles.Remove(tile);
        }

        EvaluateWinState();
    }

    private void HandleRackOverflow()
    {
        ShowLose();
    }

    private void Initialize()
    {
        BoxCollider2D boxCollider = tilePrefab.GetComponent<BoxCollider2D>();
        tileSize = boxCollider.bounds.size;
    }

    private void GenerateBoard()
    {
        if (levelData == null)
        {
            Debug.LogError("LevelData is NULL");
            return;
        }

        ClearBoard();

        for (int layer = levelData.Layers.Count - 1; layer >= 0; layer--)
        {
            int[,] grid = levelData.Layers[layer].Tiles;

            if (grid == null)
            {
                Debug.LogError($"Layer {layer} grid is NULL");
                continue;
            }

            int width = grid.GetLength(0);
            int height = grid.GetLength(1);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int value = grid[x, y];

                    if (value == 0)
                        continue;

                    CreateTile(x, y, layer, (TileType)value);
                }
            }
        }

        RefreshExposure();
        EvaluateWinState();
    }

    private void CreateTile(int x, int y, int layer, TileType type)
    {
        TileModel model = new TileModel
        {
            Type = type,
            Layer = layer,
            GridPosition = new Vector2Int(x, y),
            IsRemoved = false,
            IsExposed = false
        };

        Vector3 worldPosition = GridToWorldPosition(x, y, layer);
        TileView tile = Instantiate(tilePrefab, worldPosition, Quaternion.identity, boardRoot);
        Sprite sprite = GetSprite(type);

        tile.Initialize(model, sprite);
        tile.SetSortingOrder(layer * 10);
        tile.OnClicked += HandleTileClicked;

        allTiles.Add(tile);
    }

    private Vector3 GridToWorldPosition(int x, int y, int layer)
    {
        float layerOffsetX = layer * offsetX;
        float layerOffsetY = layer * offsetY;
        float spacingX = tileSize.x + tileGapX;
        float spacingY = tileSize.y + tileGapY;

        return new Vector3(x * spacingX + layerOffsetX, -y * spacingY - layerOffsetY, 0f) + boardOrigin;
    }

    private Sprite GetSprite(TileType type)
    {
        if (AssetsLoader.TileMapping == null)
        {
            Debug.LogError("TileMapping not loaded");
            return null;
        }

        return AssetsLoader.TileMapping.GetSprite(type);
    }

    private void HandleTileClicked(TileView tile)
    {
        if (endState != EndState.None)
            return;

        if (!tile.Model.IsExposed)
            return;

        if (tileRack == null)
        {
            Debug.LogError("TileRack is not assigned on BoardManager.");
            return;
        }

        if (!tileRack.TryAccept(tile))
        {
            Debug.Log("Rack is full.");
            return;
        }

        tile.OnClicked -= HandleTileClicked;
        tile.Model.IsRemoved = true;
        tile.Model.IsExposed = true;
        tile.SetBoardInteractable(false);
        tile.Refresh();

        Debug.Log($"Moved to rack: {tile.Model.Type}");
        RefreshExposure();
        EvaluateWinState();
    }

    private void RefreshExposure()
    {
        if (endState != EndState.None)
            return;

        foreach (TileView tile in allTiles)
        {
            if (tile.Model.IsRemoved)
                continue;

            bool blocked = IsBlocked(tile);
            tile.Model.IsExposed = !blocked;
            tile.Refresh();
        }
    }

    private bool IsBlocked(TileView tile)
    {
        Bounds tileBounds = tile.GetWorldBounds();

        foreach (TileView other in allTiles)
        {
            if (other == tile)
                continue;

            if (other.Model.IsRemoved)
                continue;

            if (other.Model.Layer <= tile.Model.Layer)
                continue;

            if (tileBounds.Intersects(other.GetWorldBounds()))
                return true;
        }

        return false;
    }

    private void ClearBoard()
    {
        foreach (TileView tile in allTiles)
        {
            if (tile != null)
                Destroy(tile.gameObject);
        }

        allTiles.Clear();
    }

    private void EvaluateWinState()
    {
        if (endState != EndState.None)
            return;

        for (int i = 0; i < allTiles.Count; i++)
        {
            TileView tile = allTiles[i];

            if (tile != null && !tile.Model.IsRemoved)
                return;
        }

        ShowWin();
    }

    private void ShowWin()
    {
        if (endState != EndState.None)
            return;

        endState = EndState.Win;
        DisableBoardInteraction();

        if (resultOverlay != null)
            resultOverlay.ShowWin();
    }

    private void ShowLose()
    {
        if (endState != EndState.None)
            return;

        endState = EndState.Lose;
        DisableBoardInteraction();

        if (resultOverlay != null)
            resultOverlay.ShowLose();
    }

    private void DisableBoardInteraction()
    {
        for (int i = 0; i < allTiles.Count; i++)
        {
            TileView tile = allTiles[i];

            if (tile != null)
                tile.SetBoardInteractable(false);
        }
    }
}