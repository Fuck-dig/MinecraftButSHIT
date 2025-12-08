using UnityEngine;
using System.Collections.Generic;

public class TerrainGenerator : MonoBehaviour
{
    [Header("Terrain Settings")]
    public int chunkSize = 16;
    public int chunkHeight = 64;
    public float noiseScale = 0.1f;
    public int seed = 0;
    
    [Header("Block Prefabs")]
    public GameObject dirtBlock;
    public GameObject grassBlock;
    public GameObject stoneBlock;
    public GameObject sandBlock;
    public GameObject waterBlock;
    public GameObject treeTrunkBlock;
    public GameObject leafBlock;
    
    [Header("Generation")]
    public bool generateOnStart = true;
    public int renderDistance = 2; // chunks around player
    
    private Dictionary<Vector2Int, GameObject> chunks = new Dictionary<Vector2Int, GameObject>();
    private Transform player;
    
    void Start()
    {
        player = Camera.main.transform;
        
        if (generateOnStart)
        {
            GenerateInitialTerrain();
        }
    }
    
    void GenerateInitialTerrain()
    {
        Vector2Int playerChunk = GetChunkPosition(player.position);
        
        for (int x = -renderDistance; x <= renderDistance; x++)
        {
            for (int z = -renderDistance; z <= renderDistance; z++)
            {
                Vector2Int chunkPos = playerChunk + new Vector2Int(x, z);
                GenerateChunk(chunkPos);
            }
        }
    }
    
    Vector2Int GetChunkPosition(Vector3 worldPos)
    {
        return new Vector2Int(
            Mathf.FloorToInt(worldPos.x / chunkSize),
            Mathf.FloorToInt(worldPos.z / chunkSize)
        );
    }
    
    void GenerateChunk(Vector2Int chunkPos)
    {
        if (chunks.ContainsKey(chunkPos)) return;
        
        GameObject chunk = new GameObject($"Chunk_{chunkPos.x}_{chunkPos.y}");
        chunk.transform.parent = transform;
        chunks[chunkPos] = chunk;
        
        for (int x = 0; x < chunkSize; x++)
        {
            for (int z = 0; z < chunkSize; z++)
            {
                int worldX = chunkPos.x * chunkSize + x;
                int worldZ = chunkPos.y * chunkSize + z;
                
                // Generate terrain height using Perlin noise
                float height = Mathf.PerlinNoise(
                    (worldX + seed) * noiseScale,
                    (worldZ + seed) * noiseScale
                );
                
                int terrainHeight = Mathf.RoundToInt(height * chunkHeight * 0.3f) + 5;
                
                // Place blocks
                for (int y = 0; y <= terrainHeight; y++)
                {
                    Vector3 pos = new Vector3(worldX, y, worldZ);
                    GameObject blockPrefab = GetBlockType(y, terrainHeight);
                    
                    if (blockPrefab != null)
                    {
                        GameObject block = Instantiate(blockPrefab, pos, Quaternion.identity, chunk.transform);
                        block.name = $"Block_{worldX}_{y}_{worldZ}";
                    }
                }
                
                // Add water at low levels (increased water level from 5 to 8)
                if (terrainHeight < 8)
                {
                    for (int y = terrainHeight + 1; y <= 8; y++)
                    {
                        Vector3 pos = new Vector3(worldX, y, worldZ);
                        if (waterBlock != null)
                        {
                            GameObject water = Instantiate(waterBlock, pos, Quaternion.identity, chunk.transform);
                            water.name = $"Water_{worldX}_{y}_{worldZ}";
                        }
                    }
                }
                
                // Add decorations (trees, grass) - only on dry land
                if (Random.value < 0.05f && terrainHeight > 8) // 5% chance for tree
                {
                    PlaceTree(new Vector3(worldX, terrainHeight + 1, worldZ), chunk.transform);
                }
            }
        }
    }
    
    GameObject GetBlockType(int y, int surfaceHeight)
    {
        if (y == surfaceHeight && surfaceHeight > 8)
        {
            return grassBlock; // Top layer on dry land
        }
        else if (y >= surfaceHeight - 3 && surfaceHeight > 8)
        {
            return dirtBlock; // Dirt layer
        }
        else if (y == surfaceHeight && surfaceHeight <= 8)
        {
            return sandBlock; // Sand near/under water
        }
        else if (y >= surfaceHeight - 2 && surfaceHeight <= 8)
        {
            return sandBlock; // More sand layers underwater
        }
        else
        {
            return stoneBlock; // Stone below
        }
    }
    
    void PlaceTree(Vector3 position, Transform parent)
    {
        if (treeTrunkBlock == null || leafBlock == null) return;
        
        // Trunk (4-5 blocks tall with variation)
        int trunkHeight = Random.Range(4, 6);
        for (int i = 0; i < trunkHeight; i++)
        {
            Instantiate(treeTrunkBlock, position + Vector3.up * i, Quaternion.identity, parent);
        }
        
        // Leaves - create a nice canopy (5x5 instead of 3x3)
        Vector3 leafBase = position + Vector3.up * (trunkHeight - 1);
        
        // Bottom layer of leaves (5x5)
        for (int x = -2; x <= 2; x++)
        {
            for (int z = -2; z <= 2; z++)
            {
                // Skip far corners for natural look
                if (Mathf.Abs(x) == 2 && Mathf.Abs(z) == 2)
                    continue;
                    
                Vector3 leafPos = leafBase + new Vector3(x, 0, z);
                Instantiate(leafBlock, leafPos, Quaternion.identity, parent);
            }
        }
        
        // Middle layer of leaves (5x5)
        for (int x = -2; x <= 2; x++)
        {
            for (int z = -2; z <= 2; z++)
            {
                // Skip far corners
                if (Mathf.Abs(x) == 2 && Mathf.Abs(z) == 2)
                    continue;
                    
                Vector3 leafPos = leafBase + new Vector3(x, 1, z);
                Instantiate(leafBlock, leafPos, Quaternion.identity, parent);
            }
        }
        
        // Top layer of leaves (3x3)
        for (int x = -1; x <= 1; x++)
        {
            for (int z = -1; z <= 1; z++)
            {
                Vector3 leafPos = leafBase + new Vector3(x, 2, z);
                Instantiate(leafBlock, leafPos, Quaternion.identity, parent);
            }
        }
        
        // Top single leaf
        Instantiate(leafBlock, leafBase + Vector3.up * 3, Quaternion.identity, parent);
    }
}