using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HealthSystemManager : MonoBehaviour
{
    public static HealthSystem PlayerHealth { get; private set; }
    public static HealthSystemManager Instance { get; private set; } // ������̬ʵ��

    [SerializeField] private HealthSystem sharedHealthSystem;

    private void Awake()
    {
        // ������ʼ��
        if (Instance == null)
        {
            Instance = this;
            if (PlayerHealth == null && sharedHealthSystem != null)
            {
                PlayerHealth = sharedHealthSystem;
                PlayerHealth.isPlayer = true;
                DontDestroyOnLoad(gameObject);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ������ȫ�����÷���
    public void ResetLevelAfterDelay(float delay)
    {
        StartCoroutine(DoResetLevelAfterDelay(delay));
    }

    private IEnumerator DoResetLevelAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        PlayerHealth.ResetHealth(); // �������Ѫ��
    }
}