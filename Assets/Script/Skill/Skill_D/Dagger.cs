using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dagger : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float stopThreshold = 0.1f;
    [SerializeField] private GameObject darkFieldPrefab;

    private Rigidbody2D rb;
    public GameObject daggerImage;
    private int direction;
    private bool hasHit;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void SetupDagger(int facingDir, float speed)
    {
        direction = facingDir;
        if (facingDir != 1)
        { 
            daggerImage.transform.Rotate(0, 0, 180);
        }

        rb.velocity = new Vector2(speed * direction, 0);
    }

    private void Update()
    {
        if (hasHit) return;

        if (Mathf.Abs(rb.velocity.x) < stopThreshold)
        {
            ActivateDarkField();
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit) return;

        if (other.TryGetComponent<Enemy>(out _))
        {
            ActivateDarkField();
            Destroy(gameObject);
        }
    }

    private void ActivateDarkField()
    {
        GameObject field = Instantiate(darkFieldPrefab, transform.position, Quaternion.identity);
        Destroy(field, 5f); // 领域持续5秒
    }

}
