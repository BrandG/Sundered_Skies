using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SSTerrain : MonoBehaviour
{
    public Terrain terrainObj;
    public List<Texture2D> textureList;
    public int textureIndex;
    private TerrainLayer[] terrainLayers;
    public int numberOfPeaks;
    public float peakBaseRadius;
    public float peakHeight;
    public float rockiness;
    private float[,] heights;
    private TerrainData terrainData;
    private int width;
    private int height;
    private readonly Vector2[] tileSizes = {
        new(25, 25),
        new(4, 4),
        new(2.25f, 2.25f),
        new(6, 6),
        new(3, 3),
        new(40, 40),
        new(25, 25),
        new(15, 15)
         };
    private List<GameObject> structures = new List<GameObject>();

    void Awake()
    {
        ServiceLocator.Instance.RegisterTerrain(terrainObj);
    }

    void Start()
    {
        terrainData = terrainObj.terrainData;
        width = terrainData.heightmapResolution;
        height = terrainData.heightmapResolution;
        terrainLayers = terrainData.terrainLayers;
        CreateRandomizedTerrain();
        AddProps();
    }

    public void CreateRandomizedTerrain()
    {
        // textureIndex = UnityEngine.Random.Range(0, textureList.Count);
        heights = new float[width, height];
        CreatePeaks();
        SmoothHeightmap();
        AddRockiness();
        terrainData.SetHeights(0, 0, heights);
        ApplyTexture();
    }

    private void CreatePeaks()
    {
        float minRadius = 200f; // Minimum distance from center
        float maxRadius = 400f; // Maximum distance from center 
        Vector2 terrainCenter = new(width / 2.0f, height / 2.0f);

        // Create multiple peaks
        for (int i = 0; i < numberOfPeaks; i++)
        {
            float radius = UnityEngine.Random.Range(minRadius, maxRadius);
            float angle = UnityEngine.Random.Range(0f, 360f) * Mathf.Deg2Rad; // Convert angle to radians

            Vector2 offset = new(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius);
            Vector2 peakPosition = terrainCenter + offset;

            for (int x = 0; x < heights.GetLength(0); x++)
            {
                for (int z = 0; z < heights.GetLength(1); z++)
                {
                    float distance = Vector2.Distance(new Vector2(x, z), peakPosition);

                    if (distance <= peakBaseRadius)
                    {
                        float elevation = 1.0f - (distance / peakBaseRadius);
                        heights[z, x] += peakHeight * elevation * elevation; // Smoothly raise the terrain
                    }
                }
            }
        }
    }

    void SmoothHeightmap()
    {
        float[,] smoothedHeights = new float[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                float totalHeight = 0.0f;
                int numNeighbors = 0;

                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dz = -1; dz <= 1; dz++)
                    {
                        int neighborX = x + dx;
                        int neighborZ = z + dz;

                        // Check for valid bounds within the heightmap
                        if (neighborX >= 0 && neighborX < width && neighborZ >= 0 && neighborZ < height)
                        {
                            totalHeight += heights[neighborZ, neighborX];
                            numNeighbors++;
                        }
                    }
                }

                // Weight the center point more 
                totalHeight += 4 * heights[z, x]; // Adjust the weight as needed
                numNeighbors += 4;

                smoothedHeights[z, x] = totalHeight / numNeighbors;
            }
        }

        // Replace the original heightmap with the smoothed one
        heights = smoothedHeights;
    }

    private void AddRockiness()
    {
        for (int x = 0; x < heights.GetLength(0); x++)
        {
            for (int z = 0; z < heights.GetLength(1); z++)
            {
                heights[z, x] += UnityEngine.Random.Range(-rockiness, rockiness) * 0.0185f;
            }
        }
    }

    public void ApplyTexture(int layer = -1)
    {
        int localLayer = layer >= 0 ? layer : textureIndex;

        if ((terrainLayers.Length <= 0) || (textureList.Count < localLayer))
        {
            Debug.LogError("Invalid layer index. Number of layers " + terrainLayers.Length + " must be greater than index " + localLayer + ".");
            return;
        }

        // Assign the texture to the first terrain layer
        terrainLayers[0].diffuseTexture = textureList[localLayer];
        terrainLayers[0].tileSize = tileSizes[localLayer];

        // Apply the updated terrain layers
        terrainData.terrainLayers = terrainLayers;

        terrainObj.Flush(); // Refresh the terrain's appearance
    }

    private void AddProps()
    {
        // "Desert/Branch_01",
        // "Desert/Branch_02",
        // "Desert/Branch_03",
        // "Desert/Broken_woods_01",
        // "Desert/Broken_woods_02",
        // "Desert/Cactus_01",
        // "Desert/Cactus_02",
        // "Desert/Cactus_03",
        // "Desert/Cactus_04",
        // "Desert/Tumbleweed_01" };

        GameObject myPrefab = Resources.Load<GameObject>("Desert/Cactus_01");
        for (int i = 0; i < 10; i++)
        {
            GameObject newObject = Instantiate(myPrefab, new Vector3(
                UnityEngine.Random.Range(-50f, 50f),
                0f,
                UnityEngine.Random.Range(-50f, 50f)), Quaternion.identity);
            // newObject.transform.localScale = new Vector3(10f, 10f, 10f);
            structures.Add(newObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}
