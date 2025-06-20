using System.Collections.Generic;
using UnityEngine;

public class EF_Build : MonoBehaviour
{
    [Header("������������")]
    [Tooltip("������Ϊ���������BoxCollider2D���")]
    public BoxCollider2D triggerArea;

    [Header("���������б�")]
    [Tooltip("��Ҫ���Ƽ���״̬�������б�")]
    public List<GameObject> targetObjects = new List<GameObject>();

    [Header("��������")]
    [Tooltip("��������İ���")]
    public KeyCode activationKey = KeyCode.F;

    [Tooltip("��������ʱ�Ƿ��Զ���������")]
    public bool activateOnEnter = false;

    [Tooltip("�뿪����ʱ�Ƿ��Զ���������")]
    public bool deactivateOnExit = false;

    private bool playerInZone = false;
    private bool objectsActive = false;

    void Start()
    {
        // �Զ���ȡBoxCollider2D���
        if (triggerArea == null)
        {
            triggerArea = GetComponent<BoxCollider2D>();
        }

        // ��֤�������
        if (triggerArea == null)
        {
            Debug.LogError("δ�ҵ�BoxCollider2D������������������");
        }
        else
        {
            triggerArea.isTrigger = true;
        }

        // ��ʼ������״̬
        UpdateObjectsState();
    }

    void Update()
    {
        // ������������ڰ���ָ������ʱ�л�����״̬
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

            // ��������˽���ʱ�Զ�����
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

            // ����������뿪ʱ�Զ�����
            if (deactivateOnExit)
            {
                objectsActive = false;
                UpdateObjectsState();
            }
        }
    }

    // ������������ļ���״̬
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