using System.Collections.Generic;
using UnityEngine;

public class PuzzleBoard
{
    public int Width { get; }
    public int Height { get; }

    private TileType[,] tiles;
    private HashSet<Vector2Int> blockPositions;
    private Vector2Int playerPosition;

    public PuzzleBoard(int width, int height)
    {
        Width = width;
        Height = height;
        tiles = new TileType[Width, Height];
        blockPositions = new HashSet<Vector2Int>();
    }

    public PuzzleBoard(StageData data)
    {
        Width = data.width;
        Height = data.height;
        tiles = new TileType[Width, Height];

        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                tiles[x, y] = data.GetTile(x, y);
            }
        }

        blockPositions = new HashSet<Vector2Int>(data.blockStarts);
        playerPosition = data.playerStart;
    }

    public bool TryMove(Vector2Int direction)
    {
        Vector2Int nextPos = playerPosition + direction;

        if (!IsInside(nextPos) || tiles[nextPos.x, nextPos.y] == TileType.Wall)
            return false;

        if (blockPositions.Contains(nextPos))
        {
            Vector2Int blockNextPos = nextPos + direction;
            if (!IsInside(blockNextPos)
                || tiles[blockNextPos.x, blockNextPos.y] == TileType.Wall
                || blockPositions.Contains(blockNextPos))
            {
                return false;
            }

            blockPositions.Remove(nextPos);
            blockPositions.Add(blockNextPos);
        }

        playerPosition = nextPos;
        return true;
    }

    public bool IsCleared()
    {
        foreach (var pos in blockPositions)
        {
            if (tiles[pos.x, pos.y] != TileType.Goal)
                return false;
        }
        return true;
    }

    private bool IsInside(Vector2Int pos)
        => pos.x >= 0 && pos.x < Width && pos.y >= 0 && pos.y < Height;
}