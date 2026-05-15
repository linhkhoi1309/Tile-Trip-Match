using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class TileRack : MonoBehaviour
{
    [SerializeField]
    [Min(1)]
    private int capacity = 7;

    [SerializeField]
    private Transform rackRoot;

    [SerializeField]
    private Vector3 firstSlotLocalPosition;

    [SerializeField]
    private Vector3 rackLayoutDirection = new(1f, 0f, 0f);

    [Tooltip("Tile size along the rack layout axis (local space), used with gap to set center-to-center stride.")]
    [SerializeField]
    [Min(0.001f)]
    private float rackTileFootprintAlongAxis = 1.2f;

    [Tooltip("Extra space between adjacent tiles along the layout axis (local space).")]
    [SerializeField]
    [Min(0f)]
    private float rackGapBetweenTiles;

    [SerializeField]
    [Min(0f)]
    private float moveDuration = 0.2f;

    [SerializeField]
    private Ease moveEase = Ease.OutQuad;

    [SerializeField]
    private int sortingOrderBase = 200;

    [Header("Match-3 removal")]
    [SerializeField]
    [Min(0.01f)]
    private float matchRemoveDuration = 0.25f;

    [SerializeField]
    private Ease matchRemoveEase = Ease.InBack;

    private TileRackAudio rackAudio;

    /// <summary>Fired after a triple is matched, before tiles are destroyed. Lets board drop references.</summary>
    public event Action<IReadOnlyList<TileView>> OnMatchRemovedFromRack;

    private readonly List<TileView> placedTiles = new();

    private bool isResolvingMatches;

    public int Capacity => capacity;

    public int Count => placedTiles.Count;

    public bool IsFull => placedTiles.Count >= capacity;

    private Transform RackParent => rackRoot != null ? rackRoot : transform;

    private void Awake()
    {
        rackAudio = GetComponent<TileRackAudio>();
        if (rackAudio == null)
            rackAudio = gameObject.AddComponent<TileRackAudio>();
    }

    public bool TryAccept(TileView tile)
    {
        if (tile == null)
            return false;

        if (isResolvingMatches)
            return false;

        if (placedTiles.Count >= capacity)
            return false;

        int slotIndex = placedTiles.Count;
        placedTiles.Add(tile);
        MoveTileIntoSlot(tile, slotIndex);
        return true;
    }

    private Vector3 GetSlotLocalPosition(int index)
    {
        Vector3 dir = rackLayoutDirection.sqrMagnitude > 0.0001f
            ? rackLayoutDirection.normalized
            : Vector3.right;

        float stride = rackTileFootprintAlongAxis + rackGapBetweenTiles;
        return firstSlotLocalPosition + dir * (stride * index);
    }

    private void MoveTileIntoSlot(TileView tile, int slotIndex)
    {
        Transform parent = RackParent;
        Vector3 targetLocal = GetSlotLocalPosition(slotIndex);
        Vector3 worldEnd = parent.TransformPoint(targetLocal);
        float duration = Mathf.Max(0.0001f, moveDuration);

        Transform t = tile.transform;
        t.DOKill(false);

        t.DOMove(worldEnd, duration)
            .SetEase(moveEase)
            .SetTarget(tile.gameObject)
            .OnComplete(() => FinishSlotPlacement(tile, parent, targetLocal, slotIndex));
    }

    private void FinishSlotPlacement(TileView tile, Transform parent, Vector3 targetLocal, int slotIndex)
    {
        if (tile == null)
            return;

        Transform t = tile.transform;
        t.SetParent(parent, false);
        t.localPosition = targetLocal;
        t.localRotation = Quaternion.identity;
        tile.SetSortingOrder(sortingOrderBase + slotIndex);
        tile.SetBoardInteractable(false);

        TryResolveMatchesChain();
    }

    private void TryResolveMatchesChain()
    {
        List<TileView> triple = FindFirstTriple(placedTiles);
        if (triple == null)
        {
            isResolvingMatches = false;
            return;
        }

        isResolvingMatches = true;
        rackAudio?.PlayMatchSound();

        float duration = Mathf.Max(0.01f, matchRemoveDuration);
        Sequence removeSeq = DOTween.Sequence();
        foreach (TileView matched in triple)
        {
            if (matched == null)
                continue;

            Transform mt = matched.transform;
            mt.DOKill(false);
            removeSeq.Join(mt.DOScale(Vector3.zero, duration).SetEase(matchRemoveEase).SetTarget(matched.gameObject));
        }

        removeSeq.OnComplete(() =>
        {
            OnMatchRemovedFromRack?.Invoke(triple);

            foreach (TileView matched in triple)
            {
                if (matched == null)
                    continue;

                matched.transform.DOKill(false);
                placedTiles.Remove(matched);
                Destroy(matched.gameObject);
            }

            RecompactRackThenResolveFurther();
        });
    }

    private void RecompactRackThenResolveFurther()
    {
        if (placedTiles.Count == 0)
        {
            TryResolveMatchesChain();
            return;
        }

        Transform parent = RackParent;
        float duration = Mathf.Max(0.0001f, moveDuration);
        Sequence moveSeq = DOTween.Sequence();

        for (int i = 0; i < placedTiles.Count; i++)
        {
            TileView tile = placedTiles[i];
            if (tile == null)
                continue;

            Vector3 worldEnd = parent.TransformPoint(GetSlotLocalPosition(i));
            Transform tr = tile.transform;
            tr.DOKill(false);
            moveSeq.Join(tr.DOMove(worldEnd, duration).SetEase(moveEase).SetTarget(tile.gameObject));
        }

        moveSeq.OnComplete(() =>
        {
            SnapAllTilesToSlots();
            TryResolveMatchesChain();
        });
    }

    private void SnapAllTilesToSlots()
    {
        Transform parent = RackParent;
        for (int i = 0; i < placedTiles.Count; i++)
        {
            TileView tile = placedTiles[i];
            if (tile == null)
                continue;

            tile.transform.SetParent(parent, false);
            tile.transform.localPosition = GetSlotLocalPosition(i);
            tile.transform.localRotation = Quaternion.identity;
            tile.SetSortingOrder(sortingOrderBase + i);
        }
    }

    /// <summary>Three tiles of the same type anywhere in the rack (list order does not matter).</summary>
    private static List<TileView> FindFirstTriple(List<TileView> rack)
    {
        var byType = new Dictionary<TileType, List<TileView>>();
        foreach (TileView tile in rack)
        {
            if (tile == null || tile.Model == null || tile.Model.Type == TileType.None)
                continue;

            if (!byType.TryGetValue(tile.Model.Type, out List<TileView> list))
            {
                list = new List<TileView>();
                byType[tile.Model.Type] = list;
            }

            list.Add(tile);
        }

        foreach (KeyValuePair<TileType, List<TileView>> kv in byType)
        {
            if (kv.Value.Count >= 3)
                return kv.Value.GetRange(0, 3);
        }

        return null;
    }
}
