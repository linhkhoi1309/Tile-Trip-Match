using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

[System.Serializable]
public class LevelLayerData
{
    [OdinSerialize]
    [ShowInInspector]
    [TableMatrix(SquareCells = true)]
    public int[,] Tiles;

    public void Resize(int width, int height)
    {
        Tiles = new int[width, height];
    }
}