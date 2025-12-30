using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Video;

public class GameManager : MonoBehaviour
{
    public static GameManager gManager
    {
        get;
        private set;
    }
    //게임 상태를 명시적 구분과 유지관리의 편리성을 위해 열거형을 사용.
    public enum FailureType
    {
        Timeout,
        VehicleDestroyed
    };
    public enum GameState
    {
        Playing,
        Success,
        Failure,
        WatchingVideo
    };

    public GameState currentState; //현재 상태

    [Header("UI")]
    public GameObject endGameUI;
    public TextMeshProUGUI endText;
    public Button restartButton;
    public GameObject pauseMenuUI;

    [Header("Video UI")]
    public GameObject videoPanel;
    public VideoPlayer videoPlayer;
    public VideoClip successClip;
    public VideoClip hullBrakeClip;
    public VideoClip lateClip;

    //private bool isPaused = false;

    private void Awake()
    {
        if (gManager == null)
        {
            gManager = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //게임 시작시 초기상태를 playing으로 설정
        currentState = GameState.Playing;

        //게임을 다시 시작했을 때를 생각해 다시 초기값으로 복구
        Time.timeScale = 1f;
        AudioListener.pause = false;

        if (endGameUI != null) endGameUI.SetActive(false);
        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
        if (videoPanel != null) videoPanel.SetActive(false);

        restartButton.onClick.AddListener(RestartGame);

        //영상이 끝나면 무엇을 할지 등록 - loopPointReached 사용
        if(videoPlayer != null)
        {
            videoPlayer.loopPointReached += OnVideoFinished;
        }
    }

    // Update is called once per frame
    void Update()
    {
        /*
        //test용
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) Resume();
            else Pause();
        }
        */
    }

    public void GameSuccess()
    {
        if (currentState != GameState.Playing) return;

        //바로 UI를 띄우지 않고 영상 재생 함수부터 실행
        ProcessGameEnd("Success", successClip);

        AudioListener.pause = true;
        /*{
            currentState = GameState.Success;
            Time.timeScale = 0f;
            endText.text = "Success";
            endGameUI.SetActive(true);
            Debug.Log("게임 성공");
            AudioListener.pause = true;
        }*/
    }

    public void GameFailure(FailureType reason)
    {
        if (currentState != GameState.Playing) return;

        //currentState = GameState.Failure;
        //Time.timeScale = 0f;
        VideoClip clipToPlay = null;

        //실패 원인에 따라 다른 텍스트 출력
        switch (reason)
        {
            case FailureType.Timeout:
                endText.text = "You Late..";
                //Debug.Log("Game Failed : Time Over");
                //여기에 지각 컷신 재생 로직 추가 필요
                clipToPlay = lateClip;
                break;
            case FailureType.VehicleDestroyed:
                endText.text = "Totaled..";
                //Debug.Log("Game Over : Car Totaled");
                clipToPlay = hullBrakeClip;
                break;
        }
        //endGameUI.SetActive(true);
        AudioListener.pause = true;
        ProcessGameEnd("Failed", clipToPlay);
    }

    private void ProcessGameEnd(string result, VideoClip clip)
    {
        currentState = GameState.WatchingVideo;
        Time.timeScale = 0f; // 게임 정지

        if (endText != null) endText.text = result;

        if (videoPanel != null && videoPlayer != null && clip != null)
        {
            videoPanel.SetActive(true);
            videoPlayer.clip = clip; // 상황에 맞는 클립으로 교체
            videoPlayer.Play();
        }
        else
        {
            // 영상이 없으면 바로 결과창
            ShowResultUI();
        }
    }

    public void OnVideoFinished(VideoPlayer vp)
    {
        if (videoPanel != null) videoPanel.SetActive(false); // 비디오 끄기
        ShowResultUI(); // 결과창 켜기
    }

    public void SkipVideo()
    {
        if (videoPlayer != null && videoPlayer.isPlaying)
        {
            videoPlayer.Stop();
        }
        OnVideoFinished(videoPlayer);
    }

    private void ShowResultUI()
    {
        currentState = GameState.Success; // 버튼 작동을 위한 상태 전환

        if (endGameUI != null) endGameUI.SetActive(true);
        AudioListener.pause = true; // 소리 끄기
    }

    public void Pause()
    {
        //isPaused = true;
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f; //ice age

        AudioListener.pause = true;
    }
    public void Resume()
    {
        //isPaused = false;
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f; //시간 "정상화"

        AudioListener.pause = false;
    }
    public void GoToTitle()
    {
        Time.timeScale = 1f; //정지를 했을때 메인메뉴로 돌아가는거니, 시간을 다시 "정상화"해야함
        SceneManager.LoadScene("TitleScene"); // 씬이름이 정해지면 그때 활성화 시키기
        AudioListener.pause = false;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        AudioListener.pause = false;
    }

    public void Quit()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public void OnClickSkipVideo()
    {
        videoPlayer.Stop();
        OnVideoFinished(videoPlayer);
    }
}
