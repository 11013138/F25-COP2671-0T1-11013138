using System.Diagnostics.Contracts;
using UnityEngine;

[CreateAssetMenu(fileName = "SeedPacket", menuName = "Scriptable Objects/SeedPacket")]
public class SeedPacket : ScriptableObject
{
    [Header("Crop Information")]
    public string cropName;

    [Header("Growth Sprites")]
    public Sprite[] growthSprites = new Sprite[4];

    [Header("UI")]
    public Sprite coverImage;

    [Header("Harvest")]
    public GameObject harvestablePrefab;

    [Header("Growth Settings")]
    public float growthTimePerStage = 10f;

    public float totalGrowthTime = 40f;
}
