using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    //게임을 관리하는 스크립트
    //0번 씬: 검은 방에서 시작 비디오 재생. 총이 있고, 스타트 버튼을 쏘면, 1번 씬으로 전환
    //트랜지션 효과 ?(나중)
    //1번 씬: 시작 위치에서 게임 시작.
    // Use this for initialization
    //Simple Coroutine
    public static GameManager Instance;
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }
    J_SceneManager _sceneManager;
    public float totalTime = 60; //1 minute
    public float spentTime;
    public Text gmTimer;
    public bool isGameEnd = false;
    bool isPause = false;
    public GameObject pausePanel; //블랙핑크
    void Start()
    {
        loseText.SetActive(false);
        winText.SetActive(false);
        pausePanel.SetActive(false);
        _sceneManager = GameObject.Find("J_SceneManager").GetComponent<J_SceneManager>();
        Time.timeScale = 1;
        spentTime = totalTime;
    }

    void PauseGame()
    {
        Debug.Log("PAUSE");
        Time.timeScale = 0;
        pausePanel.SetActive(true);
    }
    void ContinueGame()
    {
        Debug.Log("CONTINUE");
        Time.timeScale = 1;
        pausePanel.SetActive(false);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            switch (pausePanel.activeInHierarchy)
            {
                case true:
                    ContinueGame();
                    break;

                case false:
                    PauseGame();
                    break;
            }
        }
        if (!isGameEnd) //isGameEnd가 false일때만(게임이끝나지않았을때만) 시간이 흐르게하자.
        {
            totalTime -= Time.deltaTime;
        }
        totalTime = Mathf.Max(0, totalTime);
        if (totalTime == 0) //시간이 0이되면 플레이어가 진다.
        {
            J_PlayerHP.Instance.EnemyWin();
        }
        UpdateLevelTimer(totalTime);
        if (totalTime < 10) //10초부터 텍스트가 빨간색으로 변한다.
        {
            gmTimer.GetComponent<Outline>().effectColor = Color.red;
            gmTimer.color = Color.red;
        }
    }
    
    public void UpdateLevelTimer(float totalSeconds)
    {
        TimerTextPrint(gmTimer, totalSeconds);
    }

    public string TimerTextPrint(Text timerText, float seconds)
    {

        if (seconds <= 0)
        {
            return (timerText.text = "0.00");
        }

        else
        {
            string[] ti = seconds.ToString().Split('.');
            if (ti.Length == 1)
            {
                timerText.text = ti[0] + ".00";
                return (timerText.text);
            }
            else
            {
                if (ti[1].Length == 1)
                {
                    timerText.text = ti[0] + "." + ti[1][0] + "0";
                    return (timerText.text);
                }
                else
                {
                    timerText.text = ti[0] + "." + ti[1][0] + ti[1][1];
                    return (timerText.text);
                }
            }
        }

    }

    public void BackToLobbyCall()
    {
        StartCoroutine(BackToLobby());
    }
    IEnumerator BackToLobby()
    {
        yield return new WaitForSeconds(3.0f);
        _sceneManager.enabled = true;
        _sceneManager.SceneIndex = 0; //로비 씬 0번
    }
    
    public void RestartCall()
    {
        StartCoroutine(RestartStorymode());
    }
    IEnumerator RestartStorymode()
    {
        yield return new WaitForSeconds(3.0f);
        _sceneManager.enabled = true;
        _sceneManager.SceneIndex = 2; //스토리모드 씬 2번
    }
    public GameObject loseText;
    public GameObject winText;

    float disTime = 5.0f;
    public void AppearWinText()
    {
        winText.SetActive(true);
        Invoke("DisappearText", disTime);
    }
    public void AppearLoseText()
    {
        loseText.SetActive(true);
        Invoke("DisappearText", disTime);
    }

    void DisappearText()
    {

        if (winText != null)
            winText.SetActive(false);

        if (loseText != null)
            loseText.SetActive(false);
    }
}
