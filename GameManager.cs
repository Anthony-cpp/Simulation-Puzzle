using UnityEngine;

public class GameManager : MonoBehaviour
{
    public StageManager stageManager;
    public Player playerPrefab;

    private Player playerInstance;

    void Start()
    {
        // プレイヤーだけ生成しておく
        Vector2Int startPos = stageManager.GetStartPos();
        if (startPos == new Vector2Int(-1, -1))
        {
            Debug.LogError("Start tile not found in stage!");
            return;
        }

        playerInstance = Instantiate(playerPrefab);
        playerInstance.Initialize(stageManager, startPos);
    }

    // UIボタンから呼ぶメソッド
    public void StartGame()
    {
        if (playerInstance != null)
        {
            playerInstance.StartMoving();
        }
    }
}
