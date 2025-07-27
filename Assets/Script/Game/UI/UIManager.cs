using UnityEngine;
using TMPro;

public class GemUIController : MonoBehaviour
{
    public static GemUIController Instance { get; private set; }

    [Header("拖入场景中 GemCountText 的 TextMeshProUGUI 组件")]
    [SerializeField] private TextMeshProUGUI gemCountText;

    private int currentGemCount;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void AddGems(int count = 1)
    {
        currentGemCount += count;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (gemCountText != null)
            gemCountText.text = $"Gems: {currentGemCount}";
    }
}
