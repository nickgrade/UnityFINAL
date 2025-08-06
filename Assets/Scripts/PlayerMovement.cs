// PlayerMovement.cs
using UnityEngine;
public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 6f;
    Rigidbody2D rb;
    Camera cam;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        cam = Camera.main;
    }
    void FixedUpdate()
    {
        // WASD movement (axis raw = immediate response)
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        rb.MovePosition(rb.position + input * moveSpeed * Time.fixedDeltaTime);

        // Rotate toward mouse
        Vector2 mouse = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = mouse - rb.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        rb.MoveRotation(angle);
    }
}