using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathTileGeneration : MonoBehaviour
{
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private MeshCollider meshCollider;
    [SerializeField]
    private float heightScale, curveMultiplier;
    [SerializeField]
    private AnimationCurve heightCurve;
    [SerializeField]
    private TerrainType[] terrainTypes;
    [SerializeField]
    private Wave[] waves;
    private MazeGeneration mazeGenerator;
    [SerializeField]
    private Color pathColor;
    private char[,] pathMaze;

    enum IsPath
    {
        yes, 
        no, 
        notapplicable
    }
    private int mazeSize;

    // objects borrowed from class examples and https://gamedevacademy.org/complete-guide-to-procedural-level-generation-in-unity-part-1/
    [System.Serializable]
    public class Wave
    {
        public float seed;
        public float frequency;
        public float amplitude;
    }

    [System.Serializable]
    public class TerrainType
    {
        public string name;
        public float height;
        public Color color;
    }

    // Start is called before the first frame update
    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        meshCollider = GetComponent<MeshCollider>();
        mazeGenerator = GetComponent<MazeGeneration>();

        // get maze for path generation
        pathMaze = mazeGenerator.GenerateMaze();
        Debug.Log(mazeGenerator.stillRunning);
        while(mazeGenerator.stillRunning)
        {
            // do nothing, we need to wait for the maze to load
            Debug.LogWarning("waiting for finish");
        }
        // mazeGenerator.PrintMaze();
        mazeSize = mazeGenerator.size;

        GenerateTile();
    }

    void Update()
    {
        // mazeGenerator.PrintMaze();
    }

    // this code is adapted from https://gamedevacademy.org/complete-guide-to-procedural-level-generation-in-unity-part-1/
    // also parts from https://github.com/greggddqu/ProceduralGenExs/blob/main/Assets/Scripts/MyTileGeneration.cs
    // and https://github.com/greggddqu/ProceduralGenExs/blob/main/Assets/Scripts/NoiseMapGeneration.cs
    
    // height map generation using a noise map
    public float[,] GenerateHeightMap(int mapDepth, int mapWidth, float scale, float offsetX, float offsetZ, Wave[] waves)
    {
        // create an empty noise map with the mapDepth and mapWidth coordinates
        float[,] noiseMap = new float[mapDepth, mapWidth];

        for(int zIndex = 0; zIndex < mapDepth; zIndex++)
        {
            for(int xIndex = 0; xIndex < mapWidth; xIndex++)
            {
                // calculate sample indices based on the coordinates and the scale
                float sampleX = (xIndex + offsetX) / scale;
                float sampleZ = (zIndex + offsetZ) / scale;

                // generate noise value using PerlinNoise
                float noise = 0f;
                float normalization = 0f;
                foreach(Wave wave in waves)
                {
                    // generate noise value using PerlinNoise for a given Wave
                    noise += wave.amplitude * Mathf.PerlinNoise(sampleX * wave.frequency + wave.seed, sampleZ * wave.frequency + wave.seed);
                    normalization += wave.amplitude;
                }
                // normalize the noise value so that it is within 0 and 1
                noise /= normalization;
                noiseMap[zIndex, xIndex] = noise;
            }
        }
        // this generated noise map will be used as the height map for the tiles
        return noiseMap;
    }

    // generate the tile using the height map
    void GenerateTile()
    {
        Vector3[] meshVertices = meshFilter.mesh.vertices;
        int tileDepth = (int) Mathf.Sqrt(meshVertices.Length); // since the mesh is a square, the square root gives us the length
        int tileWidth = tileDepth; // since the mesh is a square, the sides are the same length

        // find offsets using tile position
        float offsetX = -transform.position.x;
        float offsetZ = -transform.position.z;

        // use the tile size, height scale, offsets, and noise waves to create a height map for the tile
        float[,] heightMap = GenerateHeightMap(tileDepth, tileWidth, heightScale, offsetX, offsetZ, waves);

        // using the height map, determine and apply colors
        Texture2D tileTexture = GenerateTileColors(heightMap);
        meshRenderer.material.mainTexture = tileTexture;
        RepositionMeshVertices(heightMap);
    }

    // create the colored texture to use on the tile based on height
    public Texture2D GenerateTileColors(float[,] heightMap)
    {
        // get size of height map
        int tileDepth = heightMap.GetLength(0);
        int tileWidth = heightMap.GetLength(1);

        // for the second path pass to connect the mazes
        IsPath[,] paths = new IsPath[11, 11];

        // make color map with pixel colors
        Color[] colorMap = new Color[tileDepth * tileWidth];
        for(int mapZIndex = 0; mapZIndex < tileDepth; mapZIndex++)
        {
            for(int mapXIndex = 0; mapXIndex < tileWidth; mapXIndex++)
            {
                // transform the 2D map index to an Array index
                int colorIndex = mapZIndex * tileWidth + mapXIndex;
                float height = heightMap[mapZIndex, mapXIndex];
                
                TerrainType terrainType = ChooseTerrainType(height);
                colorMap[colorIndex] = terrainType.color;
                
                if(terrainType.name != "sand")
                {
                    paths[mapXIndex, mapZIndex] = IsPath.notapplicable;
                }
                else
                {
                    paths[mapXIndex, mapZIndex] = IsPath.no;
                }
            }
        }

        // if the tile is on sandy land and empty in the maze, make it a path color
        for(int mapZIndex = 0; mapZIndex < tileDepth; mapZIndex++)
        {
            for(int mapXIndex = 0; mapXIndex < tileWidth; mapXIndex++)
            {
                int colorIndex = mapZIndex * tileWidth + mapXIndex;
                float height = heightMap[mapZIndex, mapXIndex];
                TerrainType terrainType = ChooseTerrainType(height);

                if(terrainType.name == "sand" && pathMaze[(int) (mapXIndex/(11f/mazeSize)), (int) (mapZIndex/(11f/mazeSize))] == 'O')
                {
                    paths[mapXIndex, mapZIndex] = IsPath.yes;
                    colorMap[colorIndex] = pathColor;
                }
            }
        }

        // connect bottoms and right of paths
        for(int mapZIndex = 0; mapZIndex < 11; mapZIndex++)
        {
            for(int mapXIndex = 0; mapXIndex < 11; mapXIndex++)
            {
                int colorIndex = mapZIndex * tileWidth + mapXIndex;
                float height = heightMap[mapZIndex, mapXIndex];
                TerrainType terrainType = ChooseTerrainType(height);

                if(mapZIndex > 9 && (int) (mapXIndex/(11f/mazeSize)) % 2 == 1 && paths[mapXIndex, mapZIndex-1] == IsPath.yes)
                {
                    paths[mapXIndex, mapZIndex] = IsPath.yes;
                    colorMap[colorIndex] = pathColor;
                }
                else if(mapXIndex > 9 && (int) (mapZIndex/(11f/mazeSize)) % 2 == 1 && paths[mapXIndex-1, mapZIndex] == IsPath.yes)
                {
                    paths[mapXIndex, mapZIndex] = IsPath.yes;
                    colorMap[colorIndex] = pathColor;
                }
            }
        }

        // connect tops and lefts of paths
        for(int mapZIndex = 10; mapZIndex >= 0; mapZIndex--)
        {
            for(int mapXIndex = 10; mapXIndex >= 0; mapXIndex--)
            {
                int colorIndex = mapZIndex * tileWidth + mapXIndex;
                float height = heightMap[mapZIndex, mapXIndex];
                TerrainType terrainType = ChooseTerrainType(height);

                if(mapZIndex < 2 && (int) (mapXIndex/(11f/mazeSize)) % 2 == 1 && paths[mapXIndex, mapZIndex+1] == IsPath.yes)
                {
                    paths[mapXIndex, mapZIndex] = IsPath.yes;
                    colorMap[colorIndex] = pathColor;
                }
                if(mapXIndex < 2 && (int) (mapZIndex/(11f/mazeSize)) % 2 == 1 && paths[mapXIndex+1, mapZIndex] == IsPath.yes)
                {
                    paths[mapXIndex, mapZIndex] = IsPath.yes;
                    colorMap[colorIndex] = pathColor;
                }
            }
        }

        // create a new texture and set its pixel colors
        Texture2D tileTexture = new Texture2D(tileWidth, tileDepth);
        tileTexture.wrapMode = TextureWrapMode.Clamp;
        tileTexture.SetPixels(colorMap);
        tileTexture.Apply();

        return tileTexture;
    }

    // determine what terrain should be used for each height
    TerrainType ChooseTerrainType(float height)
    {
        // for each terrain type, check if the height is lower than the one for the terrain type
        foreach(TerrainType terrainType in terrainTypes)
        {
            // return the first terrain type whose height is higher than the generated one
            if(height < terrainType.height)
            {
                return terrainType;
            }
        }
        return terrainTypes[terrainTypes.Length - 1];
    }

    // move the vertices to the correct positions using the heightMap and heightCurve
    public void RepositionMeshVertices(float[,] heightMap)
    {
        // get size of height map
        int tileDepth = heightMap.GetLength(0);
        int tileWidth = heightMap.GetLength(1);

        // get current vertices and positions
        Vector3[] meshVertices = meshFilter.mesh.vertices;

        // go through all vertices and update them
        int currentVertexIndex = 0;
        for(int mapZIndex = 0; mapZIndex < tileDepth; mapZIndex++)
        {
            for(int mapXIndex = 0; mapXIndex < tileWidth; mapXIndex++)
            {
                float height = heightMap[mapZIndex, mapXIndex];
                Vector3 vertexPos = meshVertices[currentVertexIndex];
                // adjust height of vertex using height curve
                meshVertices[currentVertexIndex] = new Vector3(vertexPos.x, heightCurve.Evaluate(height) * curveMultiplier, vertexPos.z);
                currentVertexIndex++;
            }
        }

        // overwrite the old vertices with the new ones and recalculate the mesh
        meshFilter.mesh.vertices = meshVertices;
        meshFilter.mesh.RecalculateBounds();
        meshFilter.mesh.RecalculateNormals();
        meshCollider.sharedMesh = meshFilter.mesh;
    }
}

