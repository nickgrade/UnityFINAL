// Assets/Scripts/WinManager.cs
using UnityEngine;

public class WinManager : MonoBehaviour
{
    public static WinManager Instance { get; private set; }
    
    [Tooltip("Assign your 'Congratulations!' Canvas or Panel here")]
    public GameObject winScreen;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // optional: DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    public void ShowWin()
    {
        if (winScreen != null)
        {
            winScreen.SetActive(true);
            Time.timeScale = 0f; // pause game
        }
    }
}
