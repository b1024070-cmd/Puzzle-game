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
    private Vector2Int? lastWallAdded;

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

    public void ActivateWallBreak()
    {
        wallBreakPending = true;
    }

    public bool IsWallBreakPending => wallBreakPending;

    public Vector2Int? ConsumeLastWallBroken()
    {
        Vector2Int? result = lastWallBroken;
        lastWallBroken = null;
        return result;
    }

    // ランダムな床マスを1つ壁に変える(妨害効果: 壁追加)
    public void AddRandomWall()
    {
        List<Vector2Int> candidates = new List<Vector2Int>();

        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                Vector2Int pos = new Vector2Int(x, y);

                if (tiles[x, y] != TileType.Floor) continue; // 床マス以外は対象外
                if (pos == playerPosition) continue;
                if (blockPositions.Contains(pos)) continue;
                if (chestPositions.Contains(pos)) continue;

                candidates.Add(pos);
            }
        }

        if (candidates.Count == 0) return; // 置ける場所がない場合は何もしない

        Vector2Int chosen = candidates[Random.Range(0, candidates.Count)];
        tiles[chosen.x, chosen.y] = TileType.Wall;
        lastWallAdded = chosen;
    }

    public Vector2Int? ConsumeLastWallAdded()
    {
        Vector2Int? result = lastWallAdded;
        lastWallAdded = null;
        return result;
    }
}