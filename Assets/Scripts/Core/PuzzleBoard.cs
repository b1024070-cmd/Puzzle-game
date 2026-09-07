using System.Collections.Generic;
using UnityEngine;

public class PuzzleBoard
{
    public int Width { get; }
    public int Height { get; }

    private TileType[,] tiles;
    private HashSet<Vector2Int> blockPositions;
    private HashSet<Vector2Int> chestPositions;
    private Vector2Int playerPosition;

    private bool wallBreakPending;
    private Vector2Int? lastWallBroken;

    public PuzzleBoard(int width, int height)
    {
        Width = width;
        Height = height;
        tiles = new TileType[Width, Height];
        blockPositions = new HashSet<Vector2Int>();
        chestPositions = new HashSet<Vector2Int>();
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
        chestPositions = new HashSet<Vector2Int>(data.chestPositions);
        playerPosition = data.playerStart;
    }

    public bool TryMove(Vector2Int direction)
    {
        Vector2Int nextPos = playerPosition + direction;

        if (!IsInside(nextPos)) return false;

        if (tiles[nextPos.x, nextPos.y] == TileType.Wall)
        {
            // 壁破壊モードが有効なら、進む代わりに壁を壊す
            if (wallBreakPending)
            {
                tiles[nextPos.x, nextPos.y] = TileType.Floor;
                wallBreakPending = false;
                lastWallBroken = nextPos;
            }
            return false;
        }

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

    public Vector2Int PlayerPosition => playerPosition;
    public IReadOnlyCollection<Vector2Int> BlockPositions => blockPositions;
    public IReadOnlyCollection<Vector2Int> ChestPositions => chestPositions;

    public bool TryOpenChest(Vector2Int pos)
    {
        return chestPositions.Remove(pos);
    }

    // 壁破壊モードを有効にする(次に壁へ向かった時に発動)
    public void ActivateWallBreak()
    {
        wallBreakPending = true;
    }

    public bool IsWallBreakPending => wallBreakPending;

    // 直近で壊された壁の座標を取得し、内部の記録は消費する(1回だけ通知するため)
    public Vector2Int? ConsumeLastWallBroken()
    {
        Vector2Int? result = lastWallBroken;
        lastWallBroken = null;
        return result;
    }
}