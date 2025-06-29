using UnityEngine;

public class EF_TP : MonoBehaviour
{
    [Header("传送位置设置")]
    [Tooltip("玩家1传送目标位置")]
    public Transform teleportTarget1;
    [Tooltip("玩家2传送目标位置")]
    public Transform teleportTarget2;

    [Header("交互设置")]
    [Tooltip("交互键 (默认为F)")]
    public KeyCode interactKey = KeyCode.F;

    private GameObject playerInZone; // 当前在交互区域内的玩家
    private bool player1InZone;     // 玩家1是否在区域内
    private bool player2InZone;     // 玩家2是否在区域内

    void Update()
    {
        // 检测按键输入
        if (Input.GetKeyDown(interactKey))
        {
            // 如果玩家1在区域内且有传送位置1
            if (player1InZone && teleportTarget1 != null)
            {
                TeleportPlayer("Player1", teleportTarget1.position);
            }
            // 如果玩家2在区域内且有传送位置2
            else if (player2InZone && teleportTarget2 != null)
            {
                TeleportPlayer("Player2", teleportTarget2.position);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // 检测玩家进入区域
        if (other.CompareTag("Player1"))
        {
            playerInZone = other.gameObject;
            player1InZone = true;
        }
        else if (other.CompareTag("Player2"))
        {
            playerInZone = other.gameObject;
            player2InZone = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // 检测玩家离开区域
        if (other.CompareTag("Player1"))
        {
            player1InZone = false;
            CheckPlayersInZone();
        }
        else if (other.CompareTag("Player2"))
        {
            player2InZone = false;
            CheckPlayersInZone();
        }
    }

    void TeleportPlayer(string playerTag, Vector3 targetPosition)
    {
        // 查找对应玩家
        GameObject player = GameObject.FindGameObjectWithTag(playerTag);
        if (player != null)
        {
            // 传送玩家
            player.transform.position = targetPosition;
        }
    }

    void CheckPlayersInZone()
    {
        // 检查是否还有玩家在区域内
        if (!player1InZone && !player2InZone)
        {
            playerInZone = null;
        }
    }
}