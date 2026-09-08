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
    [SerializeField] private GameObject chestPrefab;

    private GameObject playerObject;
    private List<GameObject> blockObjects = new List<GameObject>();
    private Dictionary<Vector2Int, GameObject> chestObjects = new Dictionary<Vector2Int, GameObject>();
    private Dictionary<Vector2Int, GameObject> tileObjects = new Dictionary<Vector2Int, GameObject>();

    public StageData StageData => stageData;
    public PlayerController PlayerController { get; private set; }

    public void Initialize()
    {
        DrawTiles();
        SpawnPlayer();
        SpawnBlocks();
        SpawnChests();
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
        for (int y = 0; y < stageData.height; y++)
        {
            for (int x = 0; x < stageData.width; x++)
            {
                TileType tile = stageData.GetTile(x, y);
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

    private void SpawnChests()
    {
        foreach (var chestPos in stageData.chestPositions)
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

    // 壁が追加された時、見た目を床 → 壁に差し替える
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