using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystemManager : MonoBehaviour
{
    public static HealthSystem PlayerHealth { get; private set; }

    [SerializeField] private HealthSystem sharedHealthSystem;

    private void Awake()
    {
        if (PlayerHealth == null && sharedHealthSystem != null)
        {
            PlayerHealth = sharedHealthSystem;
            PlayerHealth.isPlayer = true;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
