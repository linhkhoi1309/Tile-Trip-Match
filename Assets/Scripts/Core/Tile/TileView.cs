using System;
using UnityEngine;

public class TileView : MonoBehaviour
{
    [SerializeField] private SpriteRenderer iconSprite;
    [SerializeField] private SpriteRenderer backgroundSprite;
    [SerializeField] private GameObject darkOverlay;
    private BoxCollider2D boxCollider;
    private TileAudio tileAudio;

    public TileModel Model { get; private set; }

    public event Action<TileView> OnClicked;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        tileAudio = GetComponent<TileAudio>();
    }

    public void Initialize(TileModel model, Sprite sprite)
    {
        Model = model;
        iconSprite.sprite = sprite;

        if (boxCollider == null)
            boxCollider = GetComponent<BoxCollider2D>();

        Refresh();
    }

    public Bounds GetWorldBounds()
    {
        return boxCollider.bounds;
    }

    public void Refresh()
    {
        bool exposed = Model.IsExposed;
        darkOverlay.SetActive(!exposed);
    }

    public void SetBoardInteractable(bool enabled)
    {
        if (boxCollider == null)
            boxCollider = GetComponent<BoxCollider2D>();

        if (boxCollider != null)
            boxCollider.enabled = enabled;
    }

    private void OnMouseDown()
    {
        if (Model == null)
            return;

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D[] hits = Physics2D.RaycastAll(worldPos, Vector2.zero);

        if (hits.Length == 0)
            return;

        TileView topExposedTile = null;
        int maxLayer = -1;

        foreach (RaycastHit2D hit in hits)
        {
            TileView tileView = hit.collider.GetComponent<TileView>();
            if (tileView == null || tileView.Model.IsRemoved || !tileView.Model.IsExposed)
                continue;

            if (tileView.Model.Layer > maxLayer)
            {
                topExposedTile = tileView;
                maxLayer = tileView.Model.Layer;
            }
        }

        if (topExposedTile != null)
        {
            topExposedTile.PlayTapSound();
            topExposedTile.OnClicked?.Invoke(topExposedTile);
        }
    }

    private void PlayTapSound()
    {
        if (tileAudio == null)
            tileAudio = GetComponent<TileAudio>();

        tileAudio?.PlayTap();
    }

    public void SetSortingOrder(int order)
    {
        iconSprite.sortingOrder = order;
        backgroundSprite.sortingOrder = order - 1;
    }
}