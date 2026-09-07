using System.Collections.Generic;
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
    private List<GameObject> blockObjects = new List<GameObject>();

    public StageData StageData => stageData;
    public PlayerController PlayerController { get; private set; }

    // StageManagerから呼び出す、盤面の初期描画
    public void Initialize()
    {
        DrawTiles();
        SpawnPlayer();
        SpawnBlocks();
    }

    // StageManagerから毎フレーム呼び出す、見た目の位置更新
    public void UpdateVisuals(PuzzleBoard board)
    {
        playerObject.transform.position = GridToWorld(board.PlayerPosition);

        var positions = new List<Vector2Int>(board.BlockPositions);
        for (int i = 0; i < blockObjects.Count && i < positions.Count; i++)
        {
            blockObjects[i].transform.position = GridToWorld(positions[i]);
        }
    }

    private void DrawTiles()
    {
        for (int y = 0; y < stageData.height; y++)
        {
            for (int x = 0; x < stageData.width; x++)
            {
                TileType tile = stageData.GetTile(x, y);
                GameObject prefab = GetPrefabForTile(tile);

                Vector3 pos = new Vector3(x, -y, 0);
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
        PlayerController = playerObject.GetComponent<PlayerController>();
    }

    private void SpawnBlocks()
    {
        foreach (var blockStart in stageData.blockStarts)
        {
            Vector3 pos = GridToWorld(blockStart);
            GameObject block = Instantiate(blockPrefab, pos, Quaternion.identity, transform);
            blockObjects.Add(block);
        }
    }

    private Vector3 GridToWorld(Vector2Int gridPos)
    {
        return new Vector3(gridPos.x, -gridPos.y, -1);
    }
}