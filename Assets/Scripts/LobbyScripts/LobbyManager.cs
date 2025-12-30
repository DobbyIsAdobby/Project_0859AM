using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject mainPanel;
    public GameObject videoPanel;
    public GameObject modeSelectPanel;

    [Header("Video Settings")]
    public VideoPlayer introVideoPlayer;

    void Start()
    {
        mainPanel.SetActive(true);
        videoPanel.SetActive(false);
        modeSelectPanel.SetActive(false);

        introVideoPlayer.loopPointReached += OnVideoFinished;
    }

    public void OnClickStart()
    {
        mainPanel.SetActive(false);
        videoPanel.SetActive(true);

        introVideoPlayer.Play();
        //SceneManager.LoadScene("GameScene");
        //AudioListener.pause = false;
    }
    public void OnVideoFinished(VideoPlayer vp)
    {
        videoPanel.SetActive(false);
        modeSelectPanel.SetActive(true);
    }
    public void OnClickSkipVideo()
    {
        introVideoPlayer.Stop();
        OnVideoFinished(introVideoPlayer);
    }
    public void OnClickNormalMode()
    {
        SceneManager.LoadScene("NormalScene");
        AudioListener.pause = false;
    }
    public void OnClickHardMode()
    {
        SceneManager.LoadScene("GameScene");
        AudioListener.pause = false;
    }
    public void OnClickQuit()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
