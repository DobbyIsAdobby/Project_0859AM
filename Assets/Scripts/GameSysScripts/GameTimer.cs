using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{
    public float timeLimit = 180f;
    public TextMeshProUGUI timerText;

    private float timeRemain;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timeRemain = timeLimit;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (timeRemain > 0)
        {
            //매 프레임 지난 시간 줄이기
            timeRemain -= Time.deltaTime;

            //디스플레이 업데이트
            DisplayTime(timeRemain);
        }
        else
        {
            Debug.Log("Time Out");
            timeRemain = 0;
            timerText.text = "Time: 00:00";

            //초기형 - 시간 초과 엔딩 연결
            GameManager.gManager.GameFailure(GameManager.FailureType.Timeout);
        }
    }

    void DisplayTime(float timer)
    {
        if (timer < 0)
        {
            timer = 0;
        }

        //분, 초 계산
        float min = Mathf.FloorToInt(timer / 60);
        float sec = Mathf.FloorToInt(timer % 60);

        timerText.text = string.Format("Time: {0:00}:{1:00}", min, sec);
    }
}
