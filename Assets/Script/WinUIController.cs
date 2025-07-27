using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class WinUIController : MonoBehaviour
{
    [Header("��ҳ��ʾ")]
    [SerializeField] private List<Sprite> images;
    [SerializeField] private Image pageImage1;
    [SerializeField] private Image pageImage2;
    [SerializeField] private Button nextPageButton;

    [Header("���������水ť")]
    [SerializeField] private Button mainButton;

    [Header("��Ч������")]
    [SerializeField] private AudioClip backgroundMusic;    // ����ѭ������
    [SerializeField] private AudioClip pageFlipSfx;        // ��ҳ��Ч
    private AudioSource audioSource;                       // �������

    private int currentPage = 0;
    private const int imagesPerPage = 2;
    private int totalPages;

    private void Awake()
    {
        // ������ҳ��
        totalPages = Mathf.CeilToInt((float)images.Count / imagesPerPage);

        // ���ð�ť�ص�
        nextPageButton.onClick.AddListener(OnNextPage);
        mainButton.onClick.AddListener(OnMain);

        // ��ȡ����� AudioSource
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = true;
    }

    private void Start()
    {
        // ���ű�������
        if (backgroundMusic != null)
        {
            audioSource.clip = backgroundMusic;
            audioSource.Play();
        }

        UpdatePageDisplay();
    }

    private void OnNextPage()
    {
        // ���ŷ�ҳ��Ч
        if (pageFlipSfx != null)
            audioSource.PlayOneShot(pageFlipSfx);

        // ������һҳ
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

        // ��һ��
        if (startIdx < images.Count)
        {
            pageImage1.gameObject.SetActive(true);
            pageImage1.sprite = images[startIdx];
        }
        else pageImage1.gameObject.SetActive(false);

        // �ڶ���
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
