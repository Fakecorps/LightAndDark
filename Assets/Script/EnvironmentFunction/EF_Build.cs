using System.Collections;
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

    [Header("�ӳ�����")]
    [Tooltip("�뿪�����ά��״̬��ʱ�䣨�룩")]
    public float exitDelay = 3f;

    private bool playerInZone = false;
    private bool objectsActive = false;
    private Coroutine delayCoroutine; // ���ڴ洢�ӳ�Э�̵�����

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

            // ��������ӳ�Э����ֹͣ��������ӳ��ڼ䷵�أ�
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

    // �ӳٽ���Э��
    private IEnumerator DelayedDeactivation()
    {
        // �ȴ�ָ��ʱ��
        yield return new WaitForSeconds(exitDelay);

        // �ӳٽ�����������Ƿ���Ȼ����������
        if (!playerInZone)
        {
            objectsActive = false;
            UpdateObjectsState();
        }

        delayCoroutine = null;
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