using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DeathUIController : MonoBehaviour
{
    [Header("UI ����")]
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button retryButton;

    [Header("��������")]
    [Tooltip("���˵��������ƻ�����")]
    [SerializeField] private string mainMenuScene = "main";
    [Tooltip("��Ϸ�������ƻ�����")]
    [SerializeField] private string gameScene = "game";

    [Header("����")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip deathMusic;

    private void Awake()
    {
        // ȷ�� AudioSource ����
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        // ����ѭ����������
        if (deathMusic != null)
        {
            audioSource.clip = deathMusic;
            audioSource.loop = true;
            audioSource.playOnAwake = false;
            audioSource.Play();
        }

        // ��ť�¼���
        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(OnMainMenuClicked);

        if (retryButton != null)
            retryButton.onClick.AddListener(OnRetryClicked);
    }

    private void OnMainMenuClicked()
    {
        // ��ת���˵�
        SceneManager.LoadScene("main");
    }

    private void OnRetryClicked()
    {
        // ���¿�ʼ��Ϸ����
        SceneManager.LoadScene("game");
    }
}
