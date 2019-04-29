using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    //씬 전환하는 스크립트
    //시작화면(start)에서는 시작 버튼을 누르면 stage 01(첫번째 씬)로 가고 = 게임 시작,
    // stage 01 부터는 골에 들어가면 다음 씬으로 간다.
    public static GameManager Instance;
    int stageLevel;
    private void Awake()
    {
        if (GameManager.Instance == null)
            GameManager.Instance = this;
    }

    private void Start()
    {
        stageLevel = SceneManager.GetActiveScene().buildIndex;
    }

    public void NextScene()
    {
        if (stageLevel < 6)
        {
            SceneManager.LoadScene(stageLevel + 1);
            if (stageLevel < 5)
                SoundManager.instance.PlayStartSound();
            else
                SoundManager.instance.PlayEndSound();
        }
        else
            print("Coming Soon!");
    }
}
