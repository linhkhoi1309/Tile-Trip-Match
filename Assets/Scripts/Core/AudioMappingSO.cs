using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(
    fileName = "AudioMapping",
    menuName = "ScriptableObjects/Audio Mapping SO")]
public class AudioMappingSO : ScriptableObject
{
    [Header("Tile Sounds")]
    [SerializeField] private AudioClip tileTap;
    [SerializeField] private AudioClip tileMatch;

    [Header("Background")]
    [SerializeField] private AudioClip backgroundMusic;

    public AudioClip TileTap => tileTap;
    public AudioClip TileMatch => tileMatch;
    public AudioClip BackgroundMusic => backgroundMusic;
}
