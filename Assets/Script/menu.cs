using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainMenu : MonoBehaviour
{
    public Button playButton;
    public Button settingsButton;
    public Button quitButton;

    public AudioSource backgroundMusic;

    void Start()
    {
        if (backgroundMusic != null && !backgroundMusic.isPlaying)
        {
            backgroundMusic.loop = true;
            backgroundMusic.Play();
        }

        playButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("video");
        });

        settingsButton.onClick.AddListener(() =>
        {
            Debug.Log("打开设置菜单");
        });

        quitButton.onClick.AddListener(() =>
        {
            Debug.Log("退出游戏");

#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        });
    }
}
