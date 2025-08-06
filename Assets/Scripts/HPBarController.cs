using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(HorizontalLayoutGroup))]
public class HPBarController : MonoBehaviour
{
    [Header("Pip Template")]
    [Tooltip("Drag in the single, disabled Image you want to clone")]
    public Image pipTemplate;

    [Header("Player Health")]
    [Tooltip("Drag your Player's Health component here")]
    public Health playerHealth;

    [Header("Optional Override")]
    [Tooltip("If >0, use this instead of playerHealth.maxHP")]
    public int maxHPOverride = 0;

    [Header("Pip Size (px)")]
    public Vector2 pipSize = new Vector2(50f, 100f);

    Image[] pips;

    void Start()
    {
        // 1) Basic null checks
        if (pipTemplate == null)
        {
            Debug.LogError("HPBarController: pipTemplate is not assigned!", this);
            return;
        }
        if (playerHealth == null && maxHPOverride <= 0)
        {
            Debug.LogError("HPBarController: no playerHealth and no maxHPOverride!", this);
            return;
        }

        // 2) Figure out how many pips
        int maxHP = (maxHPOverride > 0) 
            ? maxHPOverride 
            : playerHealth.maxHP;

        // 3) Prepare array & hide the original
        pips = new Image[maxHP];
        pipTemplate.gameObject.SetActive(false);

        // 4) Ensure the LayoutGroup respects our pip size
        var layout = GetComponent<HorizontalLayoutGroup>();
        layout.childAlignment = TextAnchor.MiddleLeft;
        layout.spacing = 5f;           // tweak as needed
        layout.childControlWidth = false;
        layout.childControlHeight = false;

        // 5) Instantiate clones
        for (int i = 0; i < maxHP; i++)
        {
            var pip = Instantiate(pipTemplate, pipTemplate.transform.parent);
            pip.gameObject.SetActive(true);

            // Force the RectTransform size to 50×100
            var rt = pip.GetComponent<RectTransform>();
            rt.sizeDelta = pipSize;

            pips[i] = pip;
        }
    }

    void Update()
    {
        // 6) Bail if we never finished Start()
        if (pips == null || pips.Length == 0) return;

        // 7) Calculate how many to show
        float norm = (playerHealth != null) 
            ? playerHealth.Normalized 
            : 1f;
        int toShow = Mathf.CeilToInt(norm * pips.Length);

        // 8) Toggle them on/off
        for (int i = 0; i < pips.Length; i++)
            pips[i].enabled = (i < toShow);
    }
}
