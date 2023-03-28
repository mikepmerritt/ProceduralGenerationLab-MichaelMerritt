using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// this code is based on https://github.com/greggddqu/ProceduralGenRuleExs/blob/main/Assets/Scripts/MazeGenerator.cs

public class MazeGeneration : MonoBehaviour
{
    // this class takes the maze to be used on the tiles to generate the paths for level 2
    public int size;
    public char[,] maze;
    public bool stillRunning = true;

    Stack<Vector2Int> tilesToTry = new Stack<Vector2Int>();

    /*
    // nothing important happens in start normally since tiles generate a new maze to use
    // used for debugging purposes
    void Start()
    {
        tilesToTry.Push(new Vector2Int(1, 1));
        maze = new char[size, size];

        // fill maze with walls initially
        for(int x = 0; x < size; x++)
        {
            for(int y = 0; y < size; y++)
            {
                maze[x, y] = 'X';
            }
        }

        GenerateMaze();
        PrintMaze();
    }
    */

    public char[,] GenerateMaze()
    {
        // flag to prevent tile generation from continuing before mazes are generated
        stillRunning = true;

        // make empty maze
        maze = new char[size, size];

        // fill maze with walls initially
        for(int x = 0; x < size; x++)
        {
            for(int y = 0; y < size; y++)
            {
                maze[x, y] = 'X';
            }
        }

        // start with tile at (1, 1)
        tilesToTry.Push(new Vector2Int(1, 1));
        // tilesToTry.Push(new Vector2Int((int) Random.Range(1, size-1), (int) Random.Range(1, size-1))); // random maze start position instead

        // while there are still maze tiles on the stack to try, keep going
        while(tilesToTry.Count > 0)
        {
            // fetch current tile on top of stack
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
        
        // set flag to true to allow tile generation now that maze is complete and return maze for use in other scripts
        stillRunning = false;
        return maze;
    }

    // a "valid neighbor" is a tile in one of the four cardinal directions (left, right, up, down) 
    // that could be marked as a path tile right now
    // in other words, it must be a wall tile with at least one odd coordinate and three walls for neighbors
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
            // check if at least one coordinate is odd
            if(neighbor.x % 2 == 1 || neighbor.y % 2 == 1)
            {
                // check if it is a wall with three adjacent walls
                if(maze[neighbor.x, neighbor.y] == 'X' && HasThreeWallsIntact(neighbor))
                {
                    validNeighbors.Add(neighbor);
                }
            }
        }
        return validNeighbors;
    }

    // check if a tile still has three neighbors
    // used to determine if a tile could be a valid neighbor for path extension in the maze
    public bool HasThreeWallsIntact(Vector2Int tile)
    {
        int walls = 0; // counter of how many neighbors are walls

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

    // check if a tile is in the maze
    public bool InBounds(Vector2Int tile)
    {
        return tile.x >= 0 && tile.y >= 0 && tile.x <= size-1 && tile.y <= size-1;
    }

    // prints the maze as one Debug.Log message
    // used in debugging
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
