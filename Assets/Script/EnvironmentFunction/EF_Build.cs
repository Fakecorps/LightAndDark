using System.Collections.Generic;
using UnityEngine;

public class EF_Build : MonoBehaviour
{
    [Header("触发区域设置")]
    [Tooltip("拖入作为触发区域的BoxCollider2D组件")]
    public BoxCollider2D triggerArea;

    [Header("控制物体列表")]
    [Tooltip("需要控制激活状态的物体列表")]
    public List<GameObject> targetObjects = new List<GameObject>();

    [Header("按键设置")]
    [Tooltip("激活物体的按键")]
    public KeyCode activationKey = KeyCode.F;

    [Tooltip("进入区域时是否自动激活物体")]
    public bool activateOnEnter = false;

    [Tooltip("离开区域时是否自动禁用物体")]
    public bool deactivateOnExit = false;

    private bool playerInZone = false;
    private bool objectsActive = false;

    void Start()
    {
        // 自动获取BoxCollider2D组件
        if (triggerArea == null)
        {
            triggerArea = GetComponent<BoxCollider2D>();
        }

        // 验证组件设置
        if (triggerArea == null)
        {
            Debug.LogError("未找到BoxCollider2D组件！请拖入或添加组件");
        }
        else
        {
            triggerArea.isTrigger = true;
        }

        // 初始化物体状态
        UpdateObjectsState();
    }

    void Update()
    {
        // 当玩家在区域内按下指定按键时切换物体状态
        if (playerInZone && Input.GetKeyDown(activationKey))
        {
            objectsActive = !objectsActive;
            UpdateObjectsState();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = true;

            // 如果设置了进入时自动激活
            if (activateOnEnter)
            {
                objectsActive = true;
                UpdateObjectsState();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = false;

            // 如果设置了离开时自动禁用
            if (deactivateOnExit)
            {
                objectsActive = false;
                UpdateObjectsState();
            }
        }
    }

    // 更新所有物体的激活状态
    private void UpdateObjectsState()
    {
        foreach (GameObject obj in targetObjects)
        {
            if (obj != null)
            {
                obj.SetActive(objectsActive);
            }
        }
    }
}