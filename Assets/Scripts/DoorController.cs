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

    void OpenDoor()
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
}
