using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour
{
    [Header("生命值设置")]
    [SerializeField] private int maxHealth = 100; // 最大生命值
    [SerializeField] private int currentHealth;   // 当前生命值

    [Header("UI组件")]
    [SerializeField] private Slider healthSlider; // 血条Slider组件
    [SerializeField] private Text healthText;     // 血量文字组件
    void Start()
    {
        InitializeHealth();
    }

    // 初始化生命值
    void InitializeHealth()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();

        // 配置Slider组件
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.minValue = 0;
            healthSlider.value = currentHealth;
        }
    }

    // 更新生命值UI
    void UpdateHealthUI()
    {
        if (healthSlider != null)
            healthSlider.value = currentHealth;

        if (healthText != null)
            healthText.text = $"{currentHealth}/{maxHealth}";
    }

    // 受到伤害
    public void TakeDamage(int damageAmount)
    {
        currentHealth = Mathf.Max(0, currentHealth - damageAmount);
        UpdateHealthUI();
        CheckDeath();
    }

    // 治疗恢复
    public void Heal(int healAmount)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + healAmount);
        UpdateHealthUI();
    }

    // 死亡检查
    void CheckDeath()
    {
        if (currentHealth <= 0)
        {
            // 这里可以添加死亡逻辑
            Debug.Log("角色死亡！");
        }
    }

    // 重置生命值（可用于重新开始游戏时）
    public void ResetHealth()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

}
