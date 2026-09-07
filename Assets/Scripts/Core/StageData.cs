using UnityEngine;

[CreateAssetMenu(fileName = "StageData", menuName = "Puzzle/StageData")]
public class StageData : ScriptableObject
{
    public int width;
    public int height;

    public TileType[] tiles;

    public Vector2Int playerStart;
    public Vector2Int[] blockStarts;
    public Vector2Int[] chestPositions; // 宝箱の位置

    public float timeLimit = 60f;

    public TileType GetTile(int x, int y)
    {
        return tiles[y * width + x];
    }
}