using System;
using UnityEngine;

public class TileView : MonoBehaviour
{
    [SerializeField] private SpriteRenderer iconSprite;
    [SerializeField] private SpriteRenderer backgroundSprite;
    [SerializeField] private GameObject darkOverlay;
    private BoxCollider2D boxCollider;

    public TileModel Model { get; private set; }

    public event Action<TileView> OnClicked;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }

    public void Initialize(TileModel model, Sprite sprite)
    {
        Model = model;
        iconSprite.sprite = sprite;

        if (boxCollider == null)
            boxCollider = GetComponent<BoxCollider2D>();

        Refresh();
    }

    public void Refresh()
    {
        bool exposed = Model.IsExposed;
        darkOverlay.SetActive(!exposed);
        boxCollider.enabled = exposed;
    }

    private void OnMouseDown()
    {
        if (Model == null || !Model.IsExposed)
            return;

        // Cast ray and get all colliders at mouse position
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D[] hits = Physics2D.RaycastAll(worldPos, Vector2.zero);

        if (hits.Length == 0)
            return;

        // Find the tile with highest layer
        TileView topTile = null;
        int maxLayer = -1;

        foreach (RaycastHit2D hit in hits)
        {
            TileView tileView = hit.collider.GetComponent<TileView>();
            if (tileView != null && tileView.Model.Layer > maxLayer)
            {
                topTile = tileView;
                maxLayer = tileView.Model.Layer;
            }
        }

        // Only invoke click if this is the top tile
        if (topTile == this)
            OnClicked?.Invoke(this);
    }

    public void SetSortingOrder(int order)
    {
        iconSprite.sortingOrder = order;
        backgroundSprite.sortingOrder = order - 1;
    }
}