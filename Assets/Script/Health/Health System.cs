using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour
{
    [Header("����ֵ����")]
    [SerializeField] private int maxHealth = 100; // �������ֵ
    [SerializeField] private int currentHealth;   // ��ǰ����ֵ

    [Header("UI���")]
    [SerializeField] private Slider healthSlider; // Ѫ��Slider���
    [SerializeField] private Text healthText;     // Ѫ���������
    void Start()
    {
        InitializeHealth();
    }

    // ��ʼ������ֵ
    void InitializeHealth()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();

        // ����Slider���
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.minValue = 0;
            healthSlider.value = currentHealth;
        }
    }

    // ��������ֵUI
    void UpdateHealthUI()
    {
        if (healthSlider != null)
            healthSlider.value = currentHealth;

        if (healthText != null)
            healthText.text = $"{currentHealth}/{maxHealth}";
    }

    // �ܵ��˺�
    public void TakeDamage(int damageAmount)
    {
        currentHealth = Mathf.Max(0, currentHealth - damageAmount);
        UpdateHealthUI();
        CheckDeath();
    }

    // ���ƻָ�
    public void Heal(int healAmount)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + healAmount);
        UpdateHealthUI();
    }

    // �������
    void CheckDeath()
    {
        if (currentHealth <= 0)
        {
            // ���������������߼�
            Debug.Log("��ɫ������");
        }
    }

    // ��������ֵ�����������¿�ʼ��Ϸʱ��
    public void ResetHealth()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

}
