using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerationWithSafety : MonoBehaviour
{
    // this code is adapted from https://gamedevacademy.org/complete-guide-to-procedural-level-generation-in-unity-part-1/
    // also parts from https://github.com/greggddqu/ProceduralGenExs/blob/main/Assets/Scripts/MyLevelGen.cs

    [SerializeField]
    private int numTilesWidth, numTilesDepth;
    [SerializeField]
    private GameObject tilePrefab, playerPrefab, goalPrefab, swarmPrefab, fencePrefab, fenceConnectorPrefab;
    private List<GameObject> tiles;
    private GameObject player, goal, swarm;
    private int playerSpawnTileIndex, playerSpawnVertexIndex;
    private List<GameObject> safeTiles;
    
    void Start()
    {
        tiles = new List<GameObject>();
        safeTiles = new List<GameObject>();
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

        // make safe tiles if they don't exist
        // number of safe tiles is the side length of the map divided by 2
        // default size is 10 - default number of save tiles is 5
        // rounds down if necessary

        // while(safeTiles.Count < 1) // debugging with only one safe tile since it is simpler that way
        while(safeTiles.Count < (numTilesWidth / 2))
        {
            // pick and add a random tile to the list of safe tiles
            int safeTileIndex = Random.Range(0, tiles.Count);
            safeTiles.Add(tiles[safeTileIndex]);

            // get the new safe tile's vertices
            Vector3[] vertices = tiles[safeTileIndex].GetComponent<MeshFilter>().mesh.vertices;

            // make a list of fence posts
            List<GameObject> fencePosts = new List<GameObject>();

            // we want to add a fence post on every second edge vertex
            // start at top left and instantiate them clockwise
            // order is important and duplicates must be avoided since we will use order to connect them
            int sideLength = (int) Mathf.Sqrt(vertices.Length); // this will be 11

            // warning: if the tile size is less than 4x4, this will crash
            // however, the tiles are always 11x11, so this isn't an issue

            // add top border posts (0, 2, 4, 6, 8, 10)
            for(int vertexIndex = 0; vertexIndex < sideLength; vertexIndex += 2)
            {
                GameObject fp = Instantiate(fencePrefab);
                fp.transform.position = vertices[vertexIndex] + tiles[safeTileIndex].transform.position;
                fencePosts.Add(fp);
                // Debug.Log(vertexIndex);
            }
            // add right border posts (10 [repeat so skip], 32, 54, 76, 98, 120)
            for(int vertexIndex = (sideLength * 3) - 1; vertexIndex < sideLength * sideLength; vertexIndex += (sideLength * 2))
            {
                GameObject fp = Instantiate(fencePrefab);
                fp.transform.position = vertices[vertexIndex] + tiles[safeTileIndex].transform.position;
                fencePosts.Add(fp);
                // Debug.Log(vertexIndex);
            }
            // add bottom border posts (120 [repeat so skip], 118, 116, 114, 112, 110)
            for(int vertexIndex = (sideLength * sideLength) - 3; vertexIndex > (sideLength * (sideLength - 1)) - 1; vertexIndex -= 2)
            {
                GameObject fp = Instantiate(fencePrefab);
                fp.transform.position = vertices[vertexIndex] + tiles[safeTileIndex].transform.position;
                fencePosts.Add(fp);
                // Debug.Log(vertexIndex);
            }
            // add left border posts (110 [repeat so skip], 88, 66, 44, 22, 0 [repeat so skip])
            for(int vertexIndex = (sideLength * (sideLength - 3)); vertexIndex > 0; vertexIndex -= (sideLength * 2))
            {
                GameObject fp = Instantiate(fencePrefab);
                fp.transform.position = vertices[vertexIndex] + tiles[safeTileIndex].transform.position;
                fencePosts.Add(fp);
                // Debug.Log(vertexIndex);
            }

            // second pass to set nextPost for fences
            // this will connect all the fence posts to the next one, giving the square border

            // link all fence posts with references
            for(int i = 0; i < fencePosts.Count - 1; i++)
            {
                fencePosts[i].GetComponent<FencePost>().next = fencePosts[i + 1];
            }
            // connect the last fence post to the first one to finish the loop
            fencePosts[fencePosts.Count - 1].GetComponent<FencePost>().next = fencePosts[0];
        }

        // destroy player or swarm so they can be respawned if they fall off the map
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
                GameObject tile = Instantiate(tilePrefab, tilePosition, Quaternion.identity);
                tiles.Add(tile);
            }
        }
    }
}
