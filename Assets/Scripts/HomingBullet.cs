using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class HomingBullet : MonoBehaviour
{
    public float speed = 6f;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;

        // Ensure the bullet moves forward based on its initial rotation
        rb.velocity = transform.up * speed;
    }

    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}