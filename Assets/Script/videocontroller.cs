using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class DualPageComic : MonoBehaviour
{
    [Header("ҳ�� Sprites (Inspector ���� ani1~ani7)")]
    public Sprite[] pages;      // �� ani1��ani7 һ�����Ͻ�����Size=7

    [Header("UI ����")]
    public Image leftPage;      // ���� LeftPage
    public Image rightPage;     // ���� RightPage
    public Button nextButton;   // ���� NextButton
    public Button skipButton;   // ���� SkipButton

    [Header("��ת/��������")]
    public string gameSceneName = "game";
    public AudioClip bgm;       // ��ѡ
    public float fadeIn = 1f;
    public float fadeOut = 0.8f;

    private int pairIndex = 0;  // ��ǰ�������� 0 ��ʼ
    private AudioSource audioSrc;
    private int totalPairs;

    void Awake()
    {
        // �� AudioSource
        audioSrc = GetComponent<AudioSource>();
        if (bgm != null)
        {
            audioSrc.clip = bgm;
            audioSrc.loop = true;
            audioSrc.volume = 0f;
            audioSrc.Play();
        }

        // �����ܶ�����ceil(7/2)=4 ��
        totalPairs = Mathf.CeilToInt(pages.Length / 2f);
    }

    void Start()
    {
        // �󶨰�ť�¼�
        nextButton.onClick.AddListener(OnNext);
        skipButton.onClick.AddListener(OnSkip);

        // ���ŵ���
        if (bgm != null)
            StartCoroutine(FadeAudio(0f, 1f, fadeIn));

        // ��ʾ�� 1 ��
        ShowPair(pairIndex);
    }

    // ��ʾ�Ժ�Ϊ idx ����ҳ
    void ShowPair(int idx)
    {
        // ��ҳ��2*idx
        int leftIdx = idx * 2;
        leftPage.sprite = pages[leftIdx];

        // ��ҳ��2*idx +1����Խ����ÿ�
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
            // ��������ҳ��������������Ϸ
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
