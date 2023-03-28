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
    private GameObject tilePrefab, playerPrefab, goalPrefab, swarmPrefab, fencePrefab, despawnerPrefab;
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

        // this is where the fenced-in safe zones are generated for level 4
        // make safe tiles if they don't exist
        // number of safe tiles is the side length of the map divided by 2
        // default size is 10 - default number of save tiles is 5
        // rounds down if necessary

        if(safeTiles.Count < (numTilesWidth / 2))
        {
            // pick and add a random tile to the list of safe tiles that isn't in the list already
            int safeTileIndex = Random.Range(0, tiles.Count);
            while(safeTiles.Contains(tiles[safeTileIndex]))
            {
                safeTileIndex = Random.Range(0, tiles.Count);
            }

            if((tiles[safeTileIndex].GetComponent<TileGeneration>() != null && (tiles[safeTileIndex].GetComponent<TileGeneration>().finishedGenerating))
                || (tiles[safeTileIndex].GetComponent<PathTileGeneration>() != null && (tiles[safeTileIndex].GetComponent<PathTileGeneration>().finishedGenerating)))
            {
                safeTiles.Add(tiles[safeTileIndex]);

                // add a particle field despawner to this tile to make it "safe"
                GameObject despawner = Instantiate(despawnerPrefab, tiles[safeTileIndex].transform);

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
                    GameObject fp = Instantiate(fencePrefab, tiles[safeTileIndex].transform);
                    fp.transform.position = vertices[vertexIndex] + tiles[safeTileIndex].transform.position;
                    fencePosts.Add(fp);
                    // Debug.Log(vertexIndex);
                }
                // add right border posts (10 [repeat so skip], 32, 54, 76, 98, 120)
                for(int vertexIndex = (sideLength * 3) - 1; vertexIndex < sideLength * sideLength; vertexIndex += (sideLength * 2))
                {
                    GameObject fp = Instantiate(fencePrefab, tiles[safeTileIndex].transform);
                    fp.transform.position = vertices[vertexIndex] + tiles[safeTileIndex].transform.position;
                    fencePosts.Add(fp);
                    // Debug.Log(vertexIndex);
                }
                // add bottom border posts (120 [repeat so skip], 118, 116, 114, 112, 110)
                for(int vertexIndex = (sideLength * sideLength) - 3; vertexIndex > (sideLength * (sideLength - 1)) - 1; vertexIndex -= 2)
                {
                    GameObject fp = Instantiate(fencePrefab, tiles[safeTileIndex].transform);
                    fp.transform.position = vertices[vertexIndex] + tiles[safeTileIndex].transform.position;
                    fencePosts.Add(fp);
                    // Debug.Log(vertexIndex);
                }
                // add left border posts (110 [repeat so skip], 88, 66, 44, 22, 0 [repeat so skip])
                for(int vertexIndex = (sideLength * (sideLength - 3)); vertexIndex > 0; vertexIndex -= (sideLength * 2))
                {
                    GameObject fp = Instantiate(fencePrefab, tiles[safeTileIndex].transform);
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

                // make the fence posts connect by rotating the connector beams
                foreach(GameObject fp in fencePosts)
                {
                    fp.GetComponent<FencePost>().TurnToNext();
                }
            }
        }

        // destroy player or swarm so they can be respawned if they fall off the map
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
