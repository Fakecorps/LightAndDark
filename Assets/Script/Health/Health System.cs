using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // 添加 TMPro 命名空间

public class HealthSystem : MonoBehaviour
{
    [Header("血量设置")]
    [SerializeField] private int maxHealth = 100; // 最大血量值
    [SerializeField] private int currentHealth;   // 当前血量值

    [Header("UI元素")]
    [SerializeField] private Image healthBar;     // 血条Image组件
    [SerializeField] private TMP_Text healthText; // 使用TMP文本组件
    [SerializeField] private Image healthBarBackground; // 血条背景（可选）
    [SerializeField] private Color fullHealthColor = Color.green; // 满血颜色
    [SerializeField] private Color lowHealthColor = Color.red;    // 低血颜色
    [SerializeField] private int lowHealthThreshold = 30;         // 低血阈值

    void Start()
    {
        InitializeHealth();
    }

    // 初始化血量
    void InitializeHealth()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    // 更新血量UI
    void UpdateHealthUI()
    {
        // 更新血条长度 (0.0 - 1.0)
        if (healthBar != null)
        {
            float fillAmount = (float)currentHealth / maxHealth;
            healthBar.fillAmount = fillAmount;

            // 根据血量改变血条颜色
            healthBar.color = Color.Lerp(lowHealthColor, fullHealthColor, fillAmount / (lowHealthThreshold / (float)maxHealth));
        }

        // 更新血量文本
        if (healthText != null)
        {
            healthText.text = $"{currentHealth}/{maxHealth}";

            // 当血量低于阈值时改变文本颜色
            healthText.color = currentHealth <= lowHealthThreshold ? lowHealthColor : Color.white;
        }
    }

    // 受到伤害
    public void TakeDamage(int damageAmount)
    {
        currentHealth = Mathf.Max(0, currentHealth - damageAmount);
        UpdateHealthUI();
        CheckDeath();
    }

    // 恢复生命
    public void Heal(int healAmount)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + healAmount);
        UpdateHealthUI();
    }

    // 检查死亡
    void CheckDeath()
    {
        if (currentHealth <= 0)
        {
            Debug.Log("角色已死亡！");
            // 这里可以添加死亡处理逻辑
        }
    }

    // 重置血量
    public void ResetHealth()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    // 属性访问器（可选）
    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
}