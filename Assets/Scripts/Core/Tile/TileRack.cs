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

    private readonly List<TileView> placedTiles = new();

    public int Capacity => capacity;

    public int Count => placedTiles.Count;

    public bool IsFull => placedTiles.Count >= capacity;

    private Transform RackParent => rackRoot != null ? rackRoot : transform;

    public bool TryAccept(TileView tile)
    {
        if (tile == null)
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
    }
}
