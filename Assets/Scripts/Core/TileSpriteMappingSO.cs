using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "TileSpriteMapping",
    menuName = "ScriptableObjects/Tile Sprite Mapping SO")]
public class TileSpriteMappingSO : ScriptableObject
{
    [SerializeField]
    private List<TileSpritePair> pairs;

    private Dictionary<TileType, Sprite> spriteMap;

    public void Initialize()
    {
        spriteMap = new Dictionary<TileType, Sprite>();

        foreach (TileSpritePair pair in pairs)
        {
            if (!spriteMap.ContainsKey(pair.type))
            {
                spriteMap.Add(pair.type, pair.sprite);
            }
        }
    }

    public Sprite GetSprite(TileType type)
    {
        if (spriteMap == null)
        {
            Initialize();
        }

        if (spriteMap.TryGetValue(type, out Sprite sprite))
        {
            return sprite;
        }

        Debug.LogWarning(
            $"No sprite found for tile type: {type}");

        return null;
    }
}

[System.Serializable]
public class TileSpritePair
{
    public TileType type;

    public Sprite sprite;
}