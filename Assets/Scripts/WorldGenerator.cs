using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldGenerator : MonoBehaviour
{
    [Header("Полотна (Tilemaps)")]
    public Tilemap groundMap;
    public Tilemap decorationMap;

    [Header("Налаштування Фону")]
    public GameObject backgroundPrefab; 
    public float backgroundWidth = 20f;  

    [Header("Тайли Землі")]
    public TileBase ruleTileGround;

    [Header("Колекції декору")]
    public TileBase[] plantTiles;
    public TileBase[] fenceTiles;
    public TileBase spikeTile;

    public TileBase[] treeTiles; 
    public TileBase[] signTiles; 

    [Header("Налаштування Світу")]
    public int worldWidth = 150;
    public float smoothness = 0.04f;
    public int heightMultiplier = 6;
    [Tooltip("Наскільки високо чи низько буде знаходитися земля. Зробіть мінусовим, щоб опустити.")]
    public int baseHeightOffset = -5;

    [Header("Ймовірність появи декору")]
    [Range(0f, 1f)] public float plantChance = 0.25f;
    [Range(0f, 1f)] public float treeChance = 0.08f;
    [Range(0f, 1f)] public float fenceChance = 0.05f;
    [Range(0f, 1f)] public float signChance = 0.02f;
    [Range(0f, 1f)] public float spikeChance = 0.02f;

    void Start()
    {
        GenerateWorld();
    }

    public void GenerateWorld()
    {
        groundMap.ClearAllTiles();
        decorationMap.ClearAllTiles();
        
        if (backgroundPrefab != null)
        {
            int copiesNeeded = Mathf.CeilToInt(worldWidth / backgroundWidth);
            float startX = -worldWidth / 2f;

            for (int i = 0; i <= copiesNeeded; i++)
            {
                Vector3 spawnPos = new Vector3(startX + (i * backgroundWidth), 0, 10);
                
                GameObject bgCopy = Instantiate(backgroundPrefab, spawnPos, Quaternion.identity);
                bgCopy.transform.parent = this.transform;
            }
        }
        
        float seed = Random.Range(-10000f, 10000f);
        for (int x = -worldWidth / 2; x < worldWidth / 2; x++)
        {
            float noise = Mathf.PerlinNoise((x + seed) * smoothness, seed);
            
            int surfaceY = Mathf.RoundToInt(noise * heightMultiplier) + baseHeightOffset;
            
            for (int y = surfaceY; y >= surfaceY - 15; y--)
            {
                groundMap.SetTile(new Vector3Int(x, y, 0), ruleTileGround);
            }
            
            Vector3Int surfacePos = new Vector3Int(x, surfaceY + 1, 0);
            float rand = Random.value;
            
            if (rand < spikeChance && spikeTile != null)
            {
                decorationMap.SetTile(surfacePos, spikeTile);
            }
            else if (rand < spikeChance + signChance && signTiles.Length > 0)
            {
                TileBase sign = signTiles[Random.Range(0, signTiles.Length)];
                decorationMap.SetTile(surfacePos, sign);
            }
            else if (rand < spikeChance + signChance + treeChance && treeTiles.Length > 0)
            {
                TileBase tree = treeTiles[Random.Range(0, treeTiles.Length)];
                decorationMap.SetTile(surfacePos, tree);
            }
            else if (rand < spikeChance + signChance + treeChance + fenceChance && fenceTiles.Length > 0)
            {
                TileBase fence = fenceTiles[Random.Range(0, fenceTiles.Length)];
                decorationMap.SetTile(surfacePos, fence);
            }
            else if (rand < spikeChance + signChance + treeChance + fenceChance + plantChance && plantTiles.Length > 0)
            {
                TileBase plant = plantTiles[Random.Range(0, plantTiles.Length)];
                decorationMap.SetTile(surfacePos, plant);
            }
        }
    }
}