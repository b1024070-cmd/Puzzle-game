using UnityEngine;

[CreateAssetMenu(fileName = "StageData", menuName = "Puzzle/StageData")]
public class StageData : ScriptableObject
{
    public int width;
    public int height;

    // タイルの配置を、横一列ずつ並べた1次元配列で管理する
    // 例: width=5 なら、tiles[0]~tiles[4] が1行目、tiles[5]~tiles[9] が2行目
    public TileType[] tiles;

    public Vector2Int playerStart;
    public Vector2Int[] blockStarts;

    // 制限時間(秒)。フェーズ2で使用します
    public float timeLimit = 60f;

    // (i, j)座標のタイルを取得するヘルパー関数
    public TileType GetTile(int x, int y)
    {
        return tiles[y * width + x];
    }
}