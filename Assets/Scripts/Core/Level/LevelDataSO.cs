using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

[CreateAssetMenu(
    menuName = "ScriptableObjects/LevelData",
    fileName = "LevelData")]
public class LevelDataSO : SerializedScriptableObject
{
    public int Width = 4;

    public int Height = 4;

    [OdinSerialize]
    [ListDrawerSettings(Expanded = true)]
    public List<LevelLayerData> Layers =
        new();

    [Button]
    public void InitializeLayers()
    {
        foreach (LevelLayerData layer in Layers)
        {
            layer.Resize(Width, Height);
        }
    }
}