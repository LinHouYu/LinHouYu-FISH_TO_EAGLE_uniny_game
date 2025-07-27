using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DeathUIController : MonoBehaviour
{
    [Header("UI 引用")]
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button retryButton;

    [Header("场景配置")]
    [Tooltip("主菜单场景名称或索引")]
    [SerializeField] private string mainMenuScene = "main";
    [Tooltip("游戏场景名称或索引")]
    [SerializeField] private string gameScene = "game";

    [Header("音乐")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip deathMusic;

    private void Awake()
    {
        // 确保 AudioSource 存在
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        // 播放循环死亡音乐
        if (deathMusic != null)
        {
            audioSource.clip = deathMusic;
            audioSource.loop = true;
            audioSource.playOnAwake = false;
            audioSource.Play();
        }

        // 按钮事件绑定
        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(OnMainMenuClicked);

        if (retryButton != null)
            retryButton.onClick.AddListener(OnRetryClicked);
    }

    private void OnMainMenuClicked()
    {
        // 跳转主菜单
        SceneManager.LoadScene("main");
    }

    private void OnRetryClicked()
    {
        // 重新开始游戏场景
        SceneManager.LoadScene("game");
    }
}
