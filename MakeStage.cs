using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeStage : MonoBehaviour
{
    public enum Tiles
    {
        right,
        left,
        up,
        down,
        start,
        goal,
        warp,
        none
    }

    [Header("Prefab Settings")]
    public GameObject startPrefab;
    public GameObject goalPrefab;
    public GameObject leftPrefab;
    public GameObject rightPrefab;
    public GameObject upPrefab;
    public GameObject downPrefab;
    public GameObject warpPrefab;

    [Header("Tile Settings")]
    public float tileSize = 1.0f;

    // �ݒu�ς݃^�C��
    private Dictionary<Vector2Int, GameObject> placedTiles = new Dictionary<Vector2Int, GameObject>();
    private Dictionary<Vector2Int, Tiles> tileTypes = new Dictionary<Vector2Int, Tiles>();

    // Warp�y�A�Ǘ�
    private Dictionary<int, List<Vector2Int>> warpPairs = new Dictionary<int, List<Vector2Int>>();

    // �I�𒆂̃^�C��
    public Tiles selectedTile = Tiles.right;
    public int selectedWarpID = 1; // Warp�ݒu���Ɏg��ID

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // ���N���b�N�Ŕz�u
        {
            PlaceTile();

            // ���N���b�N�̂��тɎ�ނ�؂�ւ�
            CycleTileType();
        }
    }

    void PlaceTile()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2Int gridPos = new Vector2Int(
            Mathf.RoundToInt(mousePos.x / tileSize),
            Mathf.RoundToInt(mousePos.y / tileSize)
        );

        GameObject prefab = GetPrefabByTile(selectedTile);
        if (prefab == null) return;

        // ���ɒu����Ă��������
        if (placedTiles.ContainsKey(gridPos))
        {
            Destroy(placedTiles[gridPos]);
            placedTiles.Remove(gridPos);
            tileTypes.Remove(gridPos);
        }

        Vector3 spawnPos = new Vector3(gridPos.x * tileSize, gridPos.y * tileSize, 0);
        GameObject tileObj = Instantiate(prefab, spawnPos, Quaternion.identity, this.transform);

        placedTiles[gridPos] = tileObj;
        tileTypes[gridPos] = selectedTile;

        // Warp�Ȃ�ID��ݒ�
        if (selectedTile == Tiles.warp)
        {
            WarpTile warpTile = tileObj.AddComponent<WarpTile>();
            warpTile.warpID = selectedWarpID;

            if (!warpPairs.ContainsKey(selectedWarpID))
                warpPairs[selectedWarpID] = new List<Vector2Int>();

            warpPairs[selectedWarpID].Add(gridPos);
        }
    }

    void CycleTileType()
    {
        // enum �����Ԃɉ�
        selectedTile++;
        if ((int)selectedTile > (int)Tiles.warp) // �Ō�܂ł�������߂�
        {
            selectedTile = Tiles.right;
        }
    }

    GameObject GetPrefabByTile(Tiles tile)
    {
        switch (tile)
        {
            case Tiles.left: return leftPrefab;
            case Tiles.right: return rightPrefab;
            case Tiles.up: return upPrefab;
            case Tiles.down: return downPrefab;
            default: return null;
        }
    }

    // Warp�̓]������擾
    public Vector2Int? GetWarpDestination(Vector2Int from, int warpID)
    {
        if (!warpPairs.ContainsKey(warpID)) return null;
        if (warpPairs[warpID].Count != 2) return null;

        if (warpPairs[warpID][0] == from) return warpPairs[warpID][1];
        if (warpPairs[warpID][1] == from) return warpPairs[warpID][0];

        return null;
    }
}

// Warp�̏������R���|�[�l���g
public class WarpTile : MonoBehaviour
{
    public int warpID;
}
