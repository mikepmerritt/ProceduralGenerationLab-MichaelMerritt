using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGeneration : MonoBehaviour
{
    // this code is adapted from https://gamedevacademy.org/complete-guide-to-procedural-level-generation-in-unity-part-1/
    // also parts from https://github.com/greggddqu/ProceduralGenExs/blob/main/Assets/Scripts/MyLevelGen.cs

    // this is the standard terrain generation used for level 1
    // it is also used for level 2 with the path tile prefab instead of the normal tile
    // the other levels adapt this one

    [SerializeField]
    private int numTilesWidth, numTilesDepth;
    [SerializeField]
    private GameObject tilePrefab, playerPrefab, goalPrefab;
    private List<GameObject> tiles;
    private GameObject player, goal;

    private int playerSpawnTileIndex, playerSpawnVertexIndex;
    
    void Start()
    {
        tiles = new List<GameObject>();
        GenerateTerrain();
    }

    void Update()
    {
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
                // Debug.Log("P " + vertices[playerSpawnVertexIndex]);
                player.transform.position = vertices[playerSpawnVertexIndex] + tiles[playerSpawnTileIndex].transform.position + new Vector3(0, 3f, 0);
            }
        }

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
                // Debug.Log("G " + vertices[goalSpawnVertexIndex]);
                goal.transform.position = vertices[goalSpawnVertexIndex] + tiles[goalSpawnTileIndex].transform.position;
            }
        }

        // destroy player so they can be respawned if they fall off the map
        if(player != null && player.transform.position.y < -3f)
        {
            Destroy(player);
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
