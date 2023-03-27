using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerationInfinite : MonoBehaviour
{
    // this code is adapted from https://gamedevacademy.org/complete-guide-to-procedural-level-generation-in-unity-part-1/
    // also parts from https://github.com/greggddqu/ProceduralGenExs/blob/main/Assets/Scripts/MyLevelGen.cs

    [SerializeField]
    private int renderDistance;
    [SerializeField]
    private GameObject tilePrefab, playerPrefab;
    private List<GameObject> tiles;
    private GameObject player;

    private int playerSpawnTileIndex, playerSpawnVertexIndex;
    private int prevRenderTileX, prevRenderTileZ;
    
    void Start()
    {
        tiles = new List<GameObject>();

        // make first tile at (0, 0) to spawn player on
        GameObject firstTile = Instantiate(tilePrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
        tiles.Add(firstTile);
    }

    void Update()
    {
        if(player == null)
        {
            // spawn player at (0, 0) tile
            playerSpawnTileIndex = Random.Range(0, tiles.Count);
            player = Instantiate(playerPrefab);
            Vector3[] vertices = tiles[0].GetComponent<MeshFilter>().mesh.vertices;
            player.transform.position = vertices[60] + new Vector3(0, 3f, 0);
            GenerateTerrain();
        }

        // destroy the player if they fall off the map
        if(player.transform.position.y < -3f)
        {
            Destroy(player);

            // remake tile at (0, 0) to respawn player on
            GameObject firstTile = Instantiate(tilePrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
            tiles.Add(firstTile);
        }

        // check if the player has moved far enough that the tiles need to be re-rendered
        if(player.transform.position.x / 10 != prevRenderTileX || player.transform.position.z / 10 != prevRenderTileZ)
        {
            GenerateTerrain();
        }
    }

    // map generation
    private void GenerateTerrain()
    {
        // destroy old tiles
        DestroyAllTiles();

        // get the tile dimensions from the tile Prefab
        Vector3 tileSize = tilePrefab.GetComponent<MeshRenderer>().bounds.size;
        int tileWidth = (int)tileSize.x;
        int tileDepth = (int)tileSize.z;

        // get player position so we can determine which tiles need to be created
        int playerTileX = (int) (player.transform.position.x / 10);
        int playerTileZ = (int) (player.transform.position.z / 10);

        // for each tile, instantiate a tile in the correct position
        // in this case, we go in a square with a side length of (renderDistance * 2 + 1)
        for (int xTileIndex = playerTileX - renderDistance; xTileIndex <= playerTileX + renderDistance; xTileIndex++)
        {
            for (int zTileIndex = playerTileZ - renderDistance; zTileIndex <= playerTileZ + renderDistance; zTileIndex++)
            {
                // calculate the tile position based on the X and Z indices
                Vector3 tilePosition = new Vector3(
                    this.gameObject.transform.position.x + xTileIndex * tileWidth,
                    this.gameObject.transform.position.y,
                    this.gameObject.transform.position.z + zTileIndex * tileDepth);
                // instantiate a new tile
                GameObject tile = Instantiate(tilePrefab, tilePosition, Quaternion.identity);
                tiles.Add(tile);
            }
        }

        // update previous player position used to render the new tiles
        prevRenderTileX = playerTileX;
        prevRenderTileZ = playerTileZ;
    }

    // destroy all tiles
    public void DestroyAllTiles()
    {
        for(int i = tiles.Count - 1; i >= 0; i--)
        {
            GameObject tile = tiles[i];
            tiles.Remove(tile);
            Destroy(tile);
        }
    }
}
