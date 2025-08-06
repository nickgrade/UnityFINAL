using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Tooltip("Drag your Player's Transform here")]
    public Transform target;

    [Tooltip("Offset from the player's position")]
    public Vector3 offset = new Vector3(0, 0, -10);

    void LateUpdate()
    {
        if (target == null) return;
        // Match camera position to player + offset
        transform.position = target.position + offset;
    }
}
