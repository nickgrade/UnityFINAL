// Assets/Scripts/DoorController.cs
using UnityEngine;

public class DoorController : MonoBehaviour
{
    public int keysNeeded = 4;
    public PlayerStats playerStats;

    void Awake()
    {
        if (playerStats == null)
            playerStats = GameObject.FindWithTag("Player")
                                  .GetComponent<PlayerStats>();
        playerStats.onKeysChanged.AddListener(CheckOpen);
    }

    void CheckOpen()
    {
        if (playerStats.keysCollected >= keysNeeded)
            OpenDoor();
    }

    public void TriggerOpenDoor()
    {
        OpenDoor();
    }

    private void OpenDoor()
    {
        // 1) Remove collision
        Destroy(GetComponent<Collider2D>());

        // 2) Hide the visual(s)
        // Option A: disable the SpriteRenderer
        var sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.enabled = false;

        // Option B: disable entire GameObject
        // gameObject.SetActive(false);

        // … your open-door VFX here …
    }

    public void CloseDoor()
    {
        // Re-enable collision
        if (GetComponent<Collider2D>() == null)
        {
            gameObject.AddComponent<BoxCollider2D>();
        }

        // Show the visual(s)
        var sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.enabled = true;

        // Optionally, re-enable the entire GameObject
        // gameObject.SetActive(true);

        // … your close-door VFX here …
    }
}
