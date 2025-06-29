using System.Collections;
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

    [Header("延迟设置")]
    [Tooltip("离开区域后维持状态的时间（秒）")]
    public float exitDelay = 3f;

    private bool playerInZone = false;
    private bool objectsActive = false;
    private Coroutine delayCoroutine; // 用于存储延迟协程的引用

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
        if (playerInZone)
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

            // 如果存在延迟协程则停止（玩家在延迟期间返回）
            if (delayCoroutine != null)
            {
                StopCoroutine(delayCoroutine);
                delayCoroutine = null;
            }
                objectsActive = true;
                UpdateObjectsState();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player1"))
        {
            playerInZone = false;
            delayCoroutine = StartCoroutine(DelayedDeactivation());
        }
    }

    // 延迟禁用协程
    private IEnumerator DelayedDeactivation()
    {
        // 等待指定时间
        yield return new WaitForSeconds(exitDelay);

        // 延迟结束后检查玩家是否仍然不在区域内
        if (!playerInZone)
        {
            objectsActive = false;
            UpdateObjectsState();
        }

        delayCoroutine = null;
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