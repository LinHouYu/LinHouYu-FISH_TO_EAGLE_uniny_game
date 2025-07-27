using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class WinUIController : MonoBehaviour
{
    [Header("分页显示")]
    [SerializeField] private List<Sprite> images;
    [SerializeField] private Image pageImage1;
    [SerializeField] private Image pageImage2;
    [SerializeField] private Button nextPageButton;

    [Header("返回主界面按钮")]
    [SerializeField] private Button mainButton;

    [Header("音效与音乐")]
    [SerializeField] private AudioClip backgroundMusic;    // 背景循环音乐
    [SerializeField] private AudioClip pageFlipSfx;        // 翻页音效
    private AudioSource audioSource;                       // 播放组件

    private int currentPage = 0;
    private const int imagesPerPage = 2;
    private int totalPages;

    private void Awake()
    {
        // 计算总页数
        totalPages = Mathf.CeilToInt((float)images.Count / imagesPerPage);

        // 设置按钮回调
        nextPageButton.onClick.AddListener(OnNextPage);
        mainButton.onClick.AddListener(OnMain);

        // 获取或添加 AudioSource
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = true;
    }

    private void Start()
    {
        // 播放背景音乐
        if (backgroundMusic != null)
        {
            audioSource.clip = backgroundMusic;
            audioSource.Play();
        }

        UpdatePageDisplay();
    }

    private void OnNextPage()
    {
        // 播放翻页音效
        if (pageFlipSfx != null)
            audioSource.PlayOneShot(pageFlipSfx);

        // 翻到下一页
        currentPage = Mathf.Min(currentPage + 1, totalPages - 1);
        UpdatePageDisplay();
    }

    private void OnMain()
    {
        SceneManager.LoadScene(0);
    }

    private void UpdatePageDisplay()
    {
        int startIdx = currentPage * imagesPerPage;

        // 第一张
        if (startIdx < images.Count)
        {
            pageImage1.gameObject.SetActive(true);
            pageImage1.sprite = images[startIdx];
        }
        else pageImage1.gameObject.SetActive(false);

        // 第二张
        if (startIdx + 1 < images.Count)
        {
            pageImage2.gameObject.SetActive(true);
            pageImage2.sprite = images[startIdx + 1];
        }
        else pageImage2.gameObject.SetActive(false);

        bool isLast = (currentPage >= totalPages - 1);

        nextPageButton.interactable = !isLast;
        mainButton.gameObject.SetActive(isLast);
    }
}
