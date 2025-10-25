using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    [System.Serializable]
    public class TilePrefab
    {
        public string type;       // "Start", "Goal", "ArrowDown" など
        public GameObject prefab; // 対応するPrefab
    }

    [System.Serializable]
    public class ButtonChangeData
    {
        public Vector2Int position;
        public TileType newType;
    }

    public string csvFile = "stage1.csv";
    public TilePrefab[] tilePrefabs;
    public Transform tileParent;
    public int rows = 5;
    public int cols = 5;
    public float tileSize = 4f;
    public bool button1Activated = false;

    public List<ButtonChangeData> button1Changes = new List<ButtonChangeData>();

    private Dictionary<string, GameObject> prefabDict;
    private Dictionary<Vector2Int, Tile> tiles;
    private List<Vector2Int> warpInList = new List<Vector2Int>();
    private List<Vector2Int> warpOutList = new List<Vector2Int>();

    private TileType[] cycleTypes = new TileType[]
    {
        TileType.Empty, TileType.ArrowUp, TileType.ArrowDown,
        TileType.ArrowLeft, TileType.ArrowRight
    };

    void Awake()
    {
        prefabDict = new Dictionary<string, GameObject>();
        foreach (var t in tilePrefabs)
        {
            if (!prefabDict.ContainsKey(t.type))
                prefabDict.Add(t.type, t.prefab);
        }

        tiles = new Dictionary<Vector2Int, Tile>();
        string csv = PlayerPrefs.GetString("NextCSV", csvFile);
        LoadCSV(csv);
    }

    /// <summary>
    /// CSVからステージをロード
    /// </summary>
    public void LoadCSV(string filename)
    {
        // 全マスをEmptyで埋める
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                SpawnTile(pos, TileType.Empty, false);
            }
        }

        string path = Path.Combine(Application.streamingAssetsPath, filename);
        if (!File.Exists(path))
        {
            Debug.LogError($"CSV not found: {path}");
            return;
        }

        string[] lines = File.ReadAllLines(path);
        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#")) continue;

            string[] parts = line.Split(',');
            if (parts.Length < 3) continue;

            int y = int.Parse(parts[0]);
            int x = int.Parse(parts[1]);
            string typeStr = parts[2].Trim();

            TileType t = StringToTileType(typeStr);
            Vector2Int pos = new Vector2Int(x, y);

            // 上書き生成
            SpawnTile(pos, t, t != TileType.Empty);
            Debug.Log($"Tile placed: pos={pos}, type={t}");
        }

        Debug.Log($"CSV loaded: {filename}, tiles={tiles.Count}");
    }

    /// <summary>
    /// 指定位置にタイル生成
    /// </summary>
    private void SpawnTile(Vector2Int pos, TileType type, bool fixedTile)
    {
        // 既存削除
        if (tiles.ContainsKey(pos))
        {
            Destroy(tiles[pos].gameObject);
            tiles.Remove(pos);
        }

        string key = type.ToString();
        if (!prefabDict.ContainsKey(key))
        {
            Debug.LogWarning($"No prefab for {key}");
            return;
        }

        GameObject obj = Instantiate(prefabDict[key], tileParent);
        obj.transform.position = new Vector3(pos.x * tileSize - 16, -pos.y * tileSize + 8, 0);

        Tile tileComp = obj.GetComponent<Tile>();
        tileComp.Initialize(pos, type, fixedTile);
        tiles[pos] = tileComp;

        if (type == TileType.WarpIn) warpInList.Add(pos);
        if (type == TileType.WarpOut) warpOutList.Add(pos);
    }

    /// <summary>
    /// 文字列をTileTypeに変換
    /// </summary>
    private TileType StringToTileType(string s)
    {
        switch (s)
        {
            case "Start": return TileType.Start;
            case "Goal": return TileType.Goal;
            case "ArrowUp": return TileType.ArrowUp;
            case "ArrowDown": return TileType.ArrowDown;
            case "ArrowLeft": return TileType.ArrowLeft;
            case "ArrowRight": return TileType.ArrowRight;
            case "Block": return TileType.Block;
            case "WarpIn": return TileType.WarpIn;
            case "WarpOut": return TileType.WarpOut;
            case "Empty": return TileType.Empty;
            default:
                Debug.LogWarning($"Unknown tile type string: {s}");
                return TileType.Empty;
        }
    }

    public Tile GetTile(Vector2Int pos)
    {
        tiles.TryGetValue(pos, out Tile tile);
        return tile;
    }

    public Vector2Int GetStartPos()
    {
        foreach (var kv in tiles)
        {
            Debug.Log($"Check tile: pos={kv.Key}, type={kv.Value.type}");
            if (kv.Value.type == TileType.Start)
                return kv.Key;
        }
        Debug.LogError("Start position not found!");
        return new Vector2Int(-1, -1);
    }


    public Vector2Int GetWarpPair(Vector2Int currentPos)
    {
        int index = warpInList.IndexOf(currentPos);
        if (index >= 0 && index < warpOutList.Count)
            return warpOutList[index];
        return currentPos;
    }

    public void CycleTileAt(Vector2Int pos)
    {
        if (!tiles.ContainsKey(pos)) return;
        Tile oldTile = tiles[pos];
        if (oldTile.isFixed) return;

        int idx = System.Array.IndexOf(cycleTypes, oldTile.type);
        int nextIdx = (idx + 1) % cycleTypes.Length;
        TileType newType = cycleTypes[nextIdx];

        SpawnTile(pos, newType, false);
        Debug.Log($"Tile at {pos} changed to {newType}");
    }

    public void OnButtonPressed()
    {
        if (button1Activated) return;
        button1Activated = true;

        Debug.Log("Button1 Activated!");

        foreach (var change in button1Changes)
        {
            Vector2Int pos = change.position;
            TileType newType = change.newType;

            if (!tiles.ContainsKey(pos)) continue;

            Destroy(tiles[pos].gameObject);
            tiles.Remove(pos);

            SpawnTile(pos, newType, true);

            Debug.Log($"Button1: Tile at {pos} changed to {newType}");
        }
    }

}
