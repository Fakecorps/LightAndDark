using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HealthSystemManager : MonoBehaviour
{
    public static HealthSystem PlayerHealth { get; private set; }
    public static HealthSystemManager Instance { get; private set; } // 新增静态实例

    [SerializeField] private HealthSystem sharedHealthSystem;

    private void Awake()
    {
        // 单例初始化
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

    // 新增：全局重置方法
    public void ResetLevelAfterDelay(float delay)
    {
        StartCoroutine(DoResetLevelAfterDelay(delay));
    }

    private IEnumerator DoResetLevelAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        PlayerHealth.ResetHealth(); // 重置玩家血量
    }
}