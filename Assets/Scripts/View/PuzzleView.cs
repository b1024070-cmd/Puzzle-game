using System.Collections.Generic;
using UnityEngine;

public class PuzzleView : MonoBehaviour
{
    [Header("タイル用Prefab")]
    [SerializeField] private GameObject floorPrefab;
    [SerializeField] private GameObject wallPrefab;
    [SerializeField] private GameObject goalPrefab;

    [Header("キャラクター用Prefab")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject blockPrefab;
    [SerializeField] private GameObject chestPrefab;

    private GameObject playerObject;
    private List<GameObject> blockObjects = new List<GameObject>();
    private Dictionary<Vector2Int, GameObject> chestObjects = new Dictionary<Vector2Int, GameObject>();
    private Dictionary<Vector2Int, GameObject> tileObjects = new Dictionary<Vector2Int, GameObject>();

    public StageData StageData { get; private set; }
    public PlayerController PlayerController { get; private set; }

    // どのステージを表示するかを外部(StageManager)から指定して初期化する
    public void Initialize(StageData stageData)
    {
        StageData = stageData;

        DrawTiles();
        SpawnPlayer();
        SpawnBlocks();
        SpawnChests();
    }

    // すでに生成済みのオブジェクトを全部消す(リトライ・ステージ切り替え時に使う)
    public void ClearBoard()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        blockObjects.Clear();
        chestObjects.Clear();
        tileObjects.Clear();
    }

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
        for (int y = 0; y < StageData.height; y++)
        {
            for (int x = 0; x < StageData.width; x++)
            {
                TileType tile = StageData.GetTile(x, y);
                GameObject prefab = GetPrefabForTile(tile);

                Vector2Int gridPos = new Vector2Int(x, y);
                Vector3 pos = new Vector3(x, -y, 0);
                GameObject tileObj = Instantiate(prefab, pos, Quaternion.identity, transform);
                tileObjects[gridPos] = tileObj;
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
        Vector3 pos = GridToWorld(StageData.playerStart);
        playerObject = Instantiate(playerPrefab, pos, Quaternion.identity, transform);
        PlayerController = playerObject.GetComponent<PlayerController>();
    }

    private void SpawnBlocks()
    {
        foreach (var blockStart in StageData.blockStarts)
        {
            Vector3 pos = GridToWorld(blockStart);
            GameObject block = Instantiate(blockPrefab, pos, Quaternion.identity, transform);
            blockObjects.Add(block);
        }
    }

    private void SpawnChests()
    {
        foreach (var chestPos in StageData.chestPositions)
        {
            Vector3 pos = GridToWorld(chestPos);
            GameObject chest = Instantiate(chestPrefab, pos, Quaternion.identity, transform);
            chestObjects[chestPos] = chest;
        }
    }

    public void RemoveChestVisual(Vector2Int chestPos)
    {
        if (chestObjects.TryGetValue(chestPos, out GameObject chest))
        {
            Destroy(chest);
            chestObjects.Remove(chestPos);
        }
    }

    public void BreakWallVisual(Vector2Int pos)
    {
        ReplaceTileVisual(pos, floorPrefab);
    }

    public void AddWallVisual(Vector2Int pos)
    {
        ReplaceTileVisual(pos, wallPrefab);
    }

    private void ReplaceTileVisual(Vector2Int pos, GameObject newPrefab)
    {
        if (tileObjects.TryGetValue(pos, out GameObject oldTile))
        {
            Destroy(oldTile);
        }

        Vector3 worldPos = new Vector3(pos.x, -pos.y, 0);
        GameObject newTile = Instantiate(newPrefab, worldPos, Quaternion.identity, transform);
        tileObjects[pos] = newTile;
    }

    private Vector3 GridToWorld(Vector2Int gridPos)
    {
        return new Vector3(gridPos.x, -gridPos.y, -1);
    }
}