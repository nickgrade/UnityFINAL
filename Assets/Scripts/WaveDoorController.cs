// Assets/Scripts/WaveDoorController.cs
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D), typeof(SpriteRenderer))]
public class WaveDoorController : MonoBehaviour
{
    [Tooltip("Drag your WaveSpawner here")]
    public WaveSpawner waveSpawner;

    [Header("Optional VFX / Audio on Open")]
    public UnityEvent onOpen;

    Collider2D   _col;
    SpriteRenderer _sr;

void Awake()
{
    _col = GetComponent<Collider2D>();
    _sr  = GetComponent<SpriteRenderer>();

    // *** Start open ***
    OpenDoor();

    // Subscribe to spawner events
    if (waveSpawner != null)
    {
        waveSpawner.onWaveSequenceStart.AddListener(CloseDoor);
        waveSpawner.onAllWavesComplete.AddListener(OpenDoor);
    }
}

    /// <summary>
    /// Blocks the doorway visually and physically.
    /// </summary>
    public void CloseDoor()
    {
        _col.enabled = true;
        _sr.enabled  = true;
    }

    /// <summary>
    /// Unblocks the doorway and fires any VFX or sound.
    /// </summary>
    public void OpenDoor()
    {
        _col.enabled = false;
        _sr.enabled  = false;
        onOpen?.Invoke();
    }
}
