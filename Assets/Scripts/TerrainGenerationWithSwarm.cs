using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerationWithSwarm : MonoBehaviour
{
    // this code is adapted from https://gamedevacademy.org/complete-guide-to-procedural-level-generation-in-unity-part-1/
    // also parts from https://github.com/greggddqu/ProceduralGenExs/blob/main/Assets/Scripts/MyLevelGen.cs

    [SerializeField]
    private int numTilesWidth, numTilesDepth;
    [SerializeField]
    private GameObject tilePrefab, playerPrefab, goalPrefab, swarmPrefab;
    private List<GameObject> tiles;
    private GameObject player, goal, swarm;

    int playerSpawnTileIndex, playerSpawnVertexIndex;
    
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
            player = Instantiate(playerPrefab);
            Vector3[] vertices = tiles[playerSpawnTileIndex].GetComponent<MeshFilter>().mesh.vertices;
            playerSpawnVertexIndex = Random.Range(0, vertices.Length);
            player.transform.position = vertices[playerSpawnVertexIndex] + tiles[playerSpawnTileIndex].transform.position + new Vector3(0, 1f, 0);
        }

        if(goal == null)
        {
            // pick tile to spawn goal on
            int goalSpawnTileIndex = Random.Range(0, tiles.Count);
            goal = Instantiate(goalPrefab);
            Vector3[] vertices = tiles[goalSpawnTileIndex].GetComponent<MeshFilter>().mesh.vertices;
            int goalSpawnVertexIndex = Random.Range(0, vertices.Length);
            goal.transform.position = vertices[goalSpawnVertexIndex] + tiles[goalSpawnTileIndex].transform.position;
        }

        if(swarm == null)
        {
            // pick tile to spawn swarm on
            int swarmSpawnTileIndex = Random.Range(0, tiles.Count);
            swarm = Instantiate(swarmPrefab);
            Vector3[] vertices = tiles[swarmSpawnTileIndex].GetComponent<MeshFilter>().mesh.vertices;
            int swarmSpawnVertexIndex = Random.Range(0, vertices.Length);
            swarm.transform.position = vertices[swarmSpawnVertexIndex] + tiles[swarmSpawnTileIndex].transform.position + new Vector3(0, 3f, 0);
        }

        if(player.transform.position.y < -3f)
        {
            Destroy(player);
        }
        if(swarm.transform.position.y < -3f)
        {
            Destroy(swarm);
        }
    }

    // map generation
    private void GenerateTerrain()
    {
        // get the tile dimensions from the tile Prefab
        Vector3 tileSize = tilePrefab.GetComponent<MeshRenderer>().bounds.size;
        int tileWidth = (int)tileSize.x;
        int tileDepth = (int)tileSize.z;

        // for each Tile, instantiate a Tile in the correct position
        for (int xTileIndex = 0; xTileIndex < numTilesWidth; xTileIndex++)
        {
            for (int zTileIndex = 0; zTileIndex < numTilesDepth; zTileIndex++)
            {
                // calculate the tile position based on the X and Z indices
                Vector3 tilePosition = new Vector3(
                    this.gameObject.transform.position.x + xTileIndex * tileWidth,
                    this.gameObject.transform.position.y,
                    this.gameObject.transform.position.z + zTileIndex * tileDepth);
                // instantiate a new Tile
                GameObject tile = Instantiate(tilePrefab, tilePosition, Quaternion.identity) as GameObject;
                tiles.Add(tile);
            }
        }
    }
}
