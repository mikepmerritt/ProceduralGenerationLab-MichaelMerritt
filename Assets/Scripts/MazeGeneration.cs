using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// this code is based on https://github.com/greggddqu/ProceduralGenRuleExs/blob/main/Assets/Scripts/MazeGenerator.cs

public class MazeGeneration : MonoBehaviour
{
    public int size;
    public char[,] maze;
    public bool stillRunning = true;

    Stack<Vector2Int> tilesToTry = new Stack<Vector2Int>();

    // void Start()
    // {
    //     tilesToTry.Push(new Vector2Int(1, 1));
        // maze = new char[size, size];

        // // fill maze with unvisited initially
        // for(int x = 0; x < size; x++)
        // {
        //     for(int y = 0; y < size; y++)
        //     {
        //         maze[x, y] = 'X';
        //     }
        // }

        // GenerateMaze();
        // PrintMaze();
    // }

    public char[,] GenerateMaze()
    {
        stillRunning = true;
        maze = new char[size, size];

        // fill maze with unvisited initially
        for(int x = 0; x < size; x++)
        {
            for(int y = 0; y < size; y++)
            {
                maze[x, y] = 'X';
            }
        }

        // start with tile at (1, 1)
        tilesToTry.Push(new Vector2Int(1, 1));

        while(tilesToTry.Count > 0)
        {
            // set tile on top of stack to path
            Vector2Int currentPos = tilesToTry.Pop();            

            // set current tile to path
            maze[currentPos.x, currentPos.y] = 'O';

            // get neighbors
            List<Vector2Int> neighbors = GetValidNeighbors(currentPos);

            // if the tile has neighbors look at them
            if(neighbors.Count > 0)
            {
                // push current tile to come back to it and check for more neighbors once new path is finished
                tilesToTry.Push(currentPos);

                // pick one of the neighbors at random as the start of the new path
                tilesToTry.Push(neighbors[(int) Random.Range(0, neighbors.Count)]);
            }
            // else dead end, do nothing
        }
        
        stillRunning = false;
        return maze;
    }

    public List<Vector2Int> GetValidNeighbors(Vector2Int tile)
    {
        List<Vector2Int> validNeighbors = new List<Vector2Int>();

        List<Vector2Int> allNeighbors = new List<Vector2Int>();
        allNeighbors.Add(new Vector2Int(tile.x-1, tile.y));
        allNeighbors.Add(new Vector2Int(tile.x+1, tile.y));
        allNeighbors.Add(new Vector2Int(tile.x, tile.y-1));
        allNeighbors.Add(new Vector2Int(tile.x, tile.y+1));

        foreach(Vector2Int neighbor in allNeighbors)
        {
            if(neighbor.x % 2 == 1 || neighbor.y % 2 == 1)
            {
                if(maze[neighbor.x, neighbor.y] == 'X' && HasThreeWallsIntact(neighbor))
                {
                    validNeighbors.Add(neighbor);
                }
            }
        }
        return validNeighbors;
    }

    public bool HasThreeWallsIntact(Vector2Int tile)
    {
        int walls = 0;

        List<Vector2Int> allNeighbors = new List<Vector2Int>();
        allNeighbors.Add(new Vector2Int(tile.x-1, tile.y));
        allNeighbors.Add(new Vector2Int(tile.x+1, tile.y));
        allNeighbors.Add(new Vector2Int(tile.x, tile.y-1));
        allNeighbors.Add(new Vector2Int(tile.x, tile.y+1));

        foreach(Vector2Int neighbor in allNeighbors)
        {
            if(InBounds(neighbor) && maze[neighbor.x, neighbor.y] == 'X')
            {
                walls++;
            }
        }

        return (walls == 3);
    }

    public bool InBounds(Vector2Int tile)
    {
        return tile.x >= 0 && tile.y >= 0 && tile.x <= size-1 && tile.y <= size-1;
    }

    public void PrintMaze()
    {
        string mazeMsg = "";
        for(int x = 0; x < size; x++)
        {
            for(int y = 0; y < size; y++)
            {
                mazeMsg += maze[x, y];
            }
            mazeMsg += '\n';
        }
        Debug.Log(mazeMsg);
    }
}
