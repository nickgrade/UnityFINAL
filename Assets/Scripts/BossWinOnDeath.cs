// Assets/Scripts/BossWinOnDeath.cs
using UnityEngine;

[RequireComponent(typeof(Health))]
public class BossWinOnDeath : MonoBehaviour
{
    void Awake()
    {
        GetComponent<Health>().onDeath.AddListener(HandleWin);
    }

    void HandleWin()
    {
        if (WinManager.Instance != null)
            WinManager.Instance.ShowWin();
    }

    void OnDestroy()
    {
        if (WinManager.Instance != null)
            GetComponent<Health>().onDeath.RemoveListener(HandleWin);
    }
}
