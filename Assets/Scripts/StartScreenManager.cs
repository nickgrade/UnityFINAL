using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScreenManager : MonoBehaviour
{
    [Header("Main Game Scene")]
    [Tooltip("The name of your main game scene (must match the .unity filename)")]
    [SerializeField] private string sceneName = "MainGameScene";
    [Tooltip("Or use this build index instead of the name (0-based in Build Settings)")]
    [SerializeField] private int sceneIndex = 1;

    // Called by the “Start” button
    public void StartGame()
    {
        // Normal mode: clear any easy‐mode flag
        PlayerPrefs.SetInt("EasyMode", 0);
        // Reset death counter
        PlayerPrefs.SetInt("DeathCount", 0);
        PlayerPrefs.Save();
        
        LoadMainScene();
    }

    // Called by the “Easy Mode” button
    public void StartEasyMode()
    {
        PlayerPrefs.SetInt("EasyMode", 1);
        PlayerPrefs.SetInt("DeathCount", 0);
        PlayerPrefs.Save();
        
        LoadMainScene();
    }

    // Called by the “Quit” button
    public void QuitGame()
    {
    #if UNITY_EDITOR
        // Stop Play mode in the Editor
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        Application.Quit();
    #endif
    }

    private void LoadMainScene()
    {
        // If you prefer numeric indices, comment out one of these
        if (!string.IsNullOrEmpty(sceneName))
            SceneManager.LoadScene(sceneName);
        else
            SceneManager.LoadScene(sceneIndex);
    }
}
