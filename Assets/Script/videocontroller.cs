using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class DualPageComic : MonoBehaviour
{
    [Header("页面 Sprites (Inspector 拖入 ani1~ani7)")]
    public Sprite[] pages;      // 把 ani1…ani7 一次性拖进来，Size=7

    [Header("UI 引用")]
    public Image leftPage;      // 拖入 LeftPage
    public Image rightPage;     // 拖入 RightPage
    public Button nextButton;   // 拖入 NextButton
    public Button skipButton;   // 拖入 SkipButton

    [Header("跳转/音乐设置")]
    public string gameSceneName = "game";
    public AudioClip bgm;       // 可选
    public float fadeIn = 1f;
    public float fadeOut = 0.8f;

    private int pairIndex = 0;  // 当前对数，从 0 开始
    private AudioSource audioSrc;
    private int totalPairs;

    void Awake()
    {
        // 绑定 AudioSource
        audioSrc = GetComponent<AudioSource>();
        if (bgm != null)
        {
            audioSrc.clip = bgm;
            audioSrc.loop = true;
            audioSrc.volume = 0f;
            audioSrc.Play();
        }

        // 计算总对数：ceil(7/2)=4 对
        totalPairs = Mathf.CeilToInt(pages.Length / 2f);
    }

    void Start()
    {
        // 绑定按钮事件
        nextButton.onClick.AddListener(OnNext);
        skipButton.onClick.AddListener(OnSkip);

        // 播放淡入
        if (bgm != null)
            StartCoroutine(FadeAudio(0f, 1f, fadeIn));

        // 显示第 1 对
        ShowPair(pairIndex);
    }

    // 显示对号为 idx 的两页
    void ShowPair(int idx)
    {
        // 左页：2*idx
        int leftIdx = idx * 2;
        leftPage.sprite = pages[leftIdx];

        // 右页：2*idx +1，若越界就置空
        int rightIdx = leftIdx + 1;
        rightPage.sprite = rightIdx < pages.Length ? pages[rightIdx] : null;
    }

    void OnNext()
    {
        pairIndex++;
        if (pairIndex < totalPairs)
        {
            ShowPair(pairIndex);
        }
        else
        {
            // 看完所有页，淡出并加载游戏
            StartCoroutine(EndAndLoad());
        }
    }

    void OnSkip()
    {
        StartCoroutine(EndAndLoad());
    }

    IEnumerator EndAndLoad()
    {
        if (bgm != null)
            yield return FadeAudio(audioSrc.volume, 0f, fadeOut);
        SceneManager.LoadScene("game");
    }

    IEnumerator FadeAudio(float from, float to, float tMax)
    {
        float t = 0f;
        while (t < tMax)
        {
            t += Time.deltaTime;
            audioSrc.volume = Mathf.Lerp(from, to, t / tMax);
            yield return null;
        }
        audioSrc.volume = to;
    }
}
