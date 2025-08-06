using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class FlashOnHit : MonoBehaviour
{
    [Tooltip("Color to flash (white is classic)")]
    public Color flashColor = Color.white;
    [Tooltip("Duration of the flash in seconds")]
    public float flashDuration = 0.1f;

    SpriteRenderer sr;
    Color           originalColor;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;
    }

    /// <summary>
    /// Call this to make the sprite flash.
    /// </summary>
    public void Flash()
    {
        StopAllCoroutines();
        StartCoroutine(FlashRoutine());
    }

    IEnumerator FlashRoutine()
    {
        sr.color = flashColor;
        yield return new WaitForSeconds(flashDuration);
        sr.color = originalColor;
    }
}
