using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    private StageManager stage;
    private Vector2Int pos;
    private bool isMoving = false;

    public float moveDelay = 0.5f; // 移動間隔（秒）
    public float zOffset = -0.5f;  // Tile より手前に表示するための Z オフセット

    /// <summary>
    /// 初期化
    /// </summary>
    public void Initialize(StageManager stageManager, Vector2Int startPos)
    {
        stage = stageManager;
        pos = startPos;

        Tile tile = stage.GetTile(pos);
        if (tile == null)
        {
            Debug.LogError($"No tile at start position {pos}");
            return;
        }

        // Tile の位置に合わせて Player を少し手前に Z オフセット
        transform.position = tile.transform.position + new Vector3(0, 0, zOffset);

        // SpriteRenderer の Sorting Layer を確実に Player 用に
        var sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.sortingOrder = 100; // タイルより前に描画
        }
    }

    /// <summary>
    /// 移動開始
    /// </summary>
    public void StartMoving()
    {
        if (!isMoving)
        {
            StartCoroutine(MoveRoutine());
        }
    }

    /// <summary>
    /// タイルに従って移動するコルーチン
    /// </summary>
    private IEnumerator MoveRoutine()
    {
        isMoving = true;

        while (true)
        {
            Tile tile = stage.GetTile(pos);
            if (tile == null)
            {
                Debug.Log("No tile found, stop");
                break;
            }

            Vector2Int nextPos = pos;

            switch (tile.type)
            {
                case TileType.ArrowUp: nextPos += new Vector2Int(0, -1); break;
                case TileType.ArrowDown: nextPos += new Vector2Int(0, 1); break;
                case TileType.ArrowLeft: nextPos += new Vector2Int(-1, 0); break;
                case TileType.ArrowRight: nextPos += new Vector2Int(1, 0); break;
                case TileType.WarpIn: nextPos = stage.GetWarpPair(pos); break;
                case TileType.Start: nextPos += new Vector2Int(1, 0); break;
                case TileType.Goal:
                    Debug.Log("Goal Reached!");
                    yield break;
                default: // Block, Empty
                    Debug.Log("Stopped!");
                    yield break;
            }


            Tile nextTile = stage.GetTile(nextPos);
            if (nextTile == null || nextTile.type == TileType.Block)
            {
                Debug.Log($"Hit a wall or no tile at {nextPos}, stop");
                yield break;
            }

            pos = nextPos;

            // Tile の位置に合わせて Player を少し手前に Z オフセット
            transform.position = nextTile.transform.position + new Vector3(0, 0, zOffset);

            yield return new WaitForSeconds(moveDelay);
        }

        isMoving = false;
    }
}
