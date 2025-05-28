using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Decoy : MonoBehaviour
{
    public float explosionRadius;
    public int explosionDamage;
    
    public void Initialize(float radius, int damage)
    {
        explosionRadius = radius;
        explosionDamage = damage;
    }
    void Awake()
    { 
        
    }

    public void Explosion()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)
            {
                hit.GetComponent<Enemy>().TakeDamage(explosionDamage);
                hit.GetComponent<Enemy>().stateMachine.ChangeState(hit.GetComponent<Enemy>().dizzyState);
            }
        }
        Destroy(gameObject);
    }

}
