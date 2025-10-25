using UnityEngine;

public class Tile : MonoBehaviour
{
    public Vector2Int gridPos;
    public TileType type;
    public bool isFixed;

    public void Initialize(Vector2Int pos, TileType t, bool fixedTile)
    {
        gridPos = pos;
        type = t;
        isFixed = fixedTile;
    }

    void OnMouseDown()
    {
        // ��Œ�^�C���̂݃N���b�N�ŕύX�\
        if (isFixed) return;

        StageManager sm = FindObjectOfType<StageManager>();
        if (sm != null)
        {
            sm.CycleTileAt(gridPos);
        }
    }
}
