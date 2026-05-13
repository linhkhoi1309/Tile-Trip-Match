using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    [Header("Level")]
    [SerializeField] private LevelDataSO levelData;

    [Header("Tile")]
    [SerializeField] private TileView tilePrefab;

    [SerializeField] private Transform boardRoot;

    [Header("Board Visual")]
    [SerializeField] private float layerOffset = 0.15f;

    [SerializeField] private float tileGap = 0f;

    private readonly List<TileView> allTiles = new();

    private Vector2 tileSize;

    private async void Start()
    {
        Initialize();
        await AssetsLoader.LoadAsync();
        GenerateBoard();
    }

    private void OnDestroy()
    {
        AssetsLoader.Release();
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

        for (int layer = 0; layer < levelData.Layers.Count; layer++)
        {
            int[,] grid = levelData.Layers[layer].Tiles;

            if (grid == null)
            {
                Debug.LogError(
                    $"Layer {layer} grid is NULL");

                continue;
            }

            int width = grid.GetLength(0);
            int height = grid.GetLength(1);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int value = grid[x, y];

                    // 0 = Empty
                    if (value == 0) continue;

                    TileType type = (TileType)value;
                    CreateTile(x, y, layer, type);
                }
            }
        }
        RefreshExposure();
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

        // Higher layer renders above lower layer
        tile.SetSortingOrder(layer * 10);

        tile.OnClicked += HandleTileClicked;

        allTiles.Add(tile);
    }

    private Vector3 GridToWorldPosition(int x, int y, int layer)
    {
        float offsetX = layer * layerOffset;

        float offsetY = layer * layerOffset;

        float spacingX = tileSize.x + tileGap;

        float spacingY = tileSize.y + tileGap;

        return new Vector3(x * spacingX + offsetX, -y * spacingY - offsetY, 0f);
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
        Debug.Log($"Clicked: {tile.Model.Type}");
        RemoveTile(tile);
    }

    private void RemoveTile(TileView tile)
    {
        tile.Model.IsRemoved = true;
        tile.gameObject.SetActive(false);
        RefreshExposure();
    }

    private void RefreshExposure()
    {
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
        foreach (TileView other in allTiles)
        {
            if (other == tile)
                continue;

            if (other.Model.IsRemoved)
                continue;

            bool higherLayer = other.Model.Layer > tile.Model.Layer;
            bool samePosition = other.Model.GridPosition == tile.Model.GridPosition;

            if (higherLayer && samePosition)
                return true;
        }

        return false;
    }

    private void ClearBoard()
    {
        foreach (TileView tile in allTiles)
        {
            if (tile != null)
            {
                Destroy(tile.gameObject);
            }
        }

        allTiles.Clear();
    }
}