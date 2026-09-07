using UnityEngine;

public class PuzzleView : MonoBehaviour
{
    [Header("ステージデータ")]
    [SerializeField] private StageData stageData;

    [Header("タイル用Prefab")]
    [SerializeField] private GameObject floorPrefab;
    [SerializeField] private GameObject wallPrefab;
    [SerializeField] private GameObject goalPrefab;

    [Header("キャラクター用Prefab")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject blockPrefab;

    private GameObject playerObject;

    void Start()
    {
        DrawTiles();
        SpawnPlayer();
        SpawnBlocks();
    }

    // 床・壁・ゴールを並べる
    private void DrawTiles()
    {
        for (int y = 0; y < stageData.height; y++)
        {
            for (int x = 0; x < stageData.width; x++)
            {
                TileType tile = stageData.GetTile(x, y);
                GameObject prefab = GetPrefabForTile(tile);

                Vector3 pos = new Vector3(x, -y, 0); // Y座標は下に向かって並べる
                Instantiate(prefab, pos, Quaternion.identity, transform);
            }
        }
    }

    private GameObject GetPrefabForTile(TileType tile)
    {
        switch (tile)
        {
            case TileType.Wall: return wallPrefab;
            case TileType.Goal: return goalPrefab;
            default: return floorPrefab;
        }
    }

    private void SpawnPlayer()
    {
        Vector3 pos = GridToWorld(stageData.playerStart);
        playerObject = Instantiate(playerPrefab, pos, Quaternion.identity, transform);
    }

    private void SpawnBlocks()
    {
        foreach (var blockStart in stageData.blockStarts)
        {
            Vector3 pos = GridToWorld(blockStart);
            Instantiate(blockPrefab, pos, Quaternion.identity, transform);
        }
    }

    // グリッド座標(x, y)をワールド座標に変換する共通処理
    private Vector3 GridToWorld(Vector2Int gridPos)
    {
        return new Vector3(gridPos.x, -gridPos.y, -1); // -1でタイルより手前に表示
    }
}