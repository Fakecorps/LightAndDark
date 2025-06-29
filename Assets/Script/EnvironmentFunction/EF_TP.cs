using UnityEngine;

public class EF_TP : MonoBehaviour
{
    [Header("����λ������")]
    [Tooltip("���1����Ŀ��λ��")]
    public Transform teleportTarget1;
    [Tooltip("���2����Ŀ��λ��")]
    public Transform teleportTarget2;

    [Header("��������")]
    [Tooltip("������ (Ĭ��ΪF)")]
    public KeyCode interactKey = KeyCode.F;

    private GameObject playerInZone; // ��ǰ�ڽ��������ڵ����
    private bool player1InZone;     // ���1�Ƿ���������
    private bool player2InZone;     // ���2�Ƿ���������

    void Update()
    {
        // ��ⰴ������
        if (Input.GetKeyDown(interactKey))
        {
            // ������1�����������д���λ��1
            if (player1InZone && teleportTarget1 != null)
            {
                TeleportPlayer("Player1", teleportTarget1.position);
            }
            // ������2�����������д���λ��2
            else if (player2InZone && teleportTarget2 != null)
            {
                TeleportPlayer("Player2", teleportTarget2.position);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // �����ҽ�������
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
        // �������뿪����
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
        // ���Ҷ�Ӧ���
        GameObject player = GameObject.FindGameObjectWithTag(playerTag);
        if (player != null)
        {
            // �������
            player.transform.position = targetPosition;
        }
    }

    void CheckPlayersInZone()
    {
        // ����Ƿ��������������
        if (!player1InZone && !player2InZone)
        {
            playerInZone = null;
        }
    }
}