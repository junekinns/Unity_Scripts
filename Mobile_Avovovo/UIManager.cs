using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour {
    // UI를 관리하는 매니저
    // 씬이 넘어갈 때, 자동으로 스테이지 숫자 변경(stage 01, 02,... 03)
    // 텍스트를 스크립트에서 접근해야한다. 
    // Use this for initialization
    public Text stageText;
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

	void Update ()
    {
        if (SceneManager.GetActiveScene().buildIndex < 6)
            stageText.text = "Stage" + " 0" + SceneManager.GetActiveScene().buildIndex;
        else
            stageText.text = "";
    }
}
