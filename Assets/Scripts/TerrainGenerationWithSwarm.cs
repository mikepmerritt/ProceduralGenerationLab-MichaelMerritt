using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerationWithSwarm : MonoBehaviour
{
    // this code is adapted from https://gamedevacademy.org/complete-guide-to-procedural-level-generation-in-unity-part-1/
    // also parts from https://github.com/greggddqu/ProceduralGenExs/blob/main/Assets/Scripts/MyLevelGen.cs
    
    // this code is very similar to the original map generation for level 1, but adds the particle swarm enemy for level 3

    [SerializeField]
    private int numTilesWidth, numTilesDepth;
    [SerializeField]
    private GameObject tilePrefab, playerPrefab, goalPrefab, swarmPrefab;
    private List<GameObject> tiles;
    private GameObject player, goal, swarm;

    private int playerSpawnTileIndex, playerSpawnVertexIndex;
    
    void Start()
    {
        tiles = new List<GameObject>();
        GenerateTerrain();
    }

    void Update()
    {
        // spawn player if it does not exist currently
        if(player == null)
        {
            // pick tile to spawn player on
            playerSpawnTileIndex = Random.Range(0, tiles.Count);

            // if the tile is fully generated, place the player above it
            if((tiles[playerSpawnTileIndex].GetComponent<TileGeneration>() != null && (tiles[playerSpawnTileIndex].GetComponent<TileGeneration>().finishedGenerating))
                || (tiles[playerSpawnTileIndex].GetComponent<PathTileGeneration>() != null && (tiles[playerSpawnTileIndex].GetComponent<PathTileGeneration>().finishedGenerating)))
            {
                player = Instantiate(playerPrefab);
                Vector3[] vertices = tiles[playerSpawnTileIndex].GetComponent<MeshFilter>().mesh.vertices;
                playerSpawnVertexIndex = Random.Range(0, vertices.Length);
                // if on an edge vertex, reroll
                while(playerSpawnVertexIndex < 11 || playerSpawnVertexIndex > 109 || playerSpawnVertexIndex % 11 == 0 || (playerSpawnVertexIndex - 1) % 11 == 0)
                {
                    playerSpawnVertexIndex = Random.Range(0, vertices.Length);
                }
                player.transform.position = vertices[playerSpawnVertexIndex] + tiles[playerSpawnTileIndex].transform.position + new Vector3(0, 1f, 0);
            }
        }

        // spawn goal if it does not exist currently
        if(goal == null)
        {
            // pick tile to spawn goal on
            int goalSpawnTileIndex = Random.Range(0, tiles.Count);

            // if the tile is fully generated, place the goal above it
            if((tiles[goalSpawnTileIndex].GetComponent<TileGeneration>() != null && (tiles[goalSpawnTileIndex].GetComponent<TileGeneration>().finishedGenerating))
                || (tiles[goalSpawnTileIndex].GetComponent<PathTileGeneration>() != null && (tiles[goalSpawnTileIndex].GetComponent<PathTileGeneration>().finishedGenerating)))
            {
                goal = Instantiate(goalPrefab);
                Vector3[] vertices = tiles[goalSpawnTileIndex].GetComponent<MeshFilter>().mesh.vertices;
                int goalSpawnVertexIndex = Random.Range(0, vertices.Length);
                // if on an edge vertex, reroll
                while(goalSpawnVertexIndex < 11 || goalSpawnVertexIndex > 109 || goalSpawnVertexIndex % 11 == 0 || (goalSpawnVertexIndex - 1) % 11 == 0)
                {
                    goalSpawnVertexIndex = Random.Range(0, vertices.Length);
                }
                goal.transform.position = vertices[goalSpawnVertexIndex] + tiles[goalSpawnTileIndex].transform.position;
            }
        }

        // spawn swarm if it does not exist currently
        if(swarm == null)
        {
            // pick tile to spawn swarm on
            int swarmSpawnTileIndex = Random.Range(0, tiles.Count);

            // if the tile is fully generated, place the swarm above it
            if((tiles[swarmSpawnTileIndex].GetComponent<TileGeneration>() != null && (tiles[swarmSpawnTileIndex].GetComponent<TileGeneration>().finishedGenerating))
                || (tiles[swarmSpawnTileIndex].GetComponent<PathTileGeneration>() != null && (tiles[swarmSpawnTileIndex].GetComponent<PathTileGeneration>().finishedGenerating)))
            {
                swarm = Instantiate(swarmPrefab);
                Vector3[] vertices = tiles[swarmSpawnTileIndex].GetComponent<MeshFilter>().mesh.vertices;
                int swarmSpawnVertexIndex = Random.Range(0, vertices.Length);
                swarm.transform.position = vertices[swarmSpawnVertexIndex] + tiles[swarmSpawnTileIndex].transform.position + new Vector3(0, 3f, 0);
            }
        }

        if(player != null && player.transform.position.y < -3f)
        {
            Destroy(player);
        }
        if(swarm != null && swarm.transform.position.y < -3f)
        {
            Destroy(swarm);
        }
    }

    // map generation
    private void GenerateTerrain()
    {
        // get the tile dimensions from the tile prefab
        Vector3 tileSize = tilePrefab.GetComponent<MeshRenderer>().bounds.size;
        int tileWidth = (int)tileSize.x;
        int tileDepth = (int)tileSize.z;

        // instantiate 2D grid of tiles
        for (int xTileIndex = 0; xTileIndex < numTilesWidth; xTileIndex++)
        {
            for (int zTileIndex = 0; zTileIndex < numTilesDepth; zTileIndex++)
            {
                // calculate tile position using index and tile size
                Vector3 tilePosition = new Vector3(
                    this.gameObject.transform.position.x + xTileIndex * tileWidth,
                    this.gameObject.transform.position.y,
                    this.gameObject.transform.position.z + zTileIndex * tileDepth);
                // create new tile and add to list
                GameObject tile = Instantiate(tilePrefab, tilePosition, Quaternion.identity);
                tiles.Add(tile);
            }
        }
    }
}
