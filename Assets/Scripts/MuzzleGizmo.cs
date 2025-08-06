#if UNITY_EDITOR
using UnityEngine;
public class MuzzleGizmo : MonoBehaviour
{
    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + transform.up * 0.8f);
        Gizmos.DrawSphere(transform.position, 0.05f);
    }
}
#endif
