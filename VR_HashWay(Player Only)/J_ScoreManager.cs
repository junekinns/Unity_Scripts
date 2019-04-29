using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class J_ScoreManager : MonoBehaviour
{
    //스코어매니저에서 점수를 관리하고 게임 결과를 출력해준다.
    //점수 목록
    //1. 소요시간 , 2.주행거리, 3.처치한 드론, 4.없앤 장애물
    //소요시간->게임매니저관리, 주행거리->플레이어포지션, 처지드론->드론스크립트, 없앤장애물->트랩스크립트

    //각 각 스크립트에서 접근할 수 있도록 싱글톤 화 해주자.
    public float endDelayTime = 5.0f;
    public static J_ScoreManager Instance;
    public Text timeText; //게임 경과시간  나오는 텍스트
    public Text mileageText; //주행거리  나오는 텍스트
    public Text droneKillText; //드론 킬 수 나오는 텍스트
    public Text trapRemovedText; //장애물 없앤 수  나오는 텍스트
    GameManager gm; //게임매니저 스크립트에 접근하기 위한 변수.
    GameObject player;
    Vector3 playerInitPos;
    float _spentTime = 0f;
    bool isResultTextEnd = false;
    public float SpentTime { get { return _spentTime; } set { _spentTime = value; } }

    float _mileage = 0f;
    public float Mileage { get { return _mileage; } set { _mileage = value; } }


    int _droneKillNum = 0;
    public int DroneKillNum { get { return _droneKillNum; } set { _droneKillNum = value; } }

    int _trapKillNum = 0;
    public int TrapKillNum { get { return _trapKillNum; } set { _trapKillNum = value; } }

    // Use this for initialization
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    void Start()
    {
        gm = GameObject.Find("J_GameManager").GetComponent<GameManager>(); //gm에서 시간관리 하니까!
        player = GameObject.Find("Player"); //Player위치값 받아오려고  
        playerInitPos = player.transform.position; //Player 맨 처음 위치값.
        GameResultClean();
    }

    // Update is called once per frame
    void Update()
    {
        //EnemyHP와 PlayerHP스크립트에서 죽는 순간에 Gamemanager스크립트에 접근해서 isGameEnd = true로 해준다.
        //isGameEnd가 되면 게임 결과를 출력한다.
        if (gm.isGameEnd && !isResultTextEnd)
        {
            GameResult();
        }
    }
    //isGameEnd되는 그 순간에 Z 값을 저장시켜야 한다.

    private void GameResult()
    {
        isResultTextEnd = true; //ResultPrint가 한 번만 찍히도록 하려고!
        //드론/트랩 죽는 순간 싱글톤으로 접근해서 각 프로퍼티 ++해주고,
        //시간은 게임매니저 시간출력함수 필요하니까 그걸로 출력하고,
        //나머지 출력은 여기서 해결하자. 
        timeText.text = (gm.TimerTextPrint(timeText, gm.spentTime - gm.totalTime));
        mileageText.text = ((int)(player.transform.position.z - playerInitPos.z) / 10 + " km");
        droneKillText.text = (DroneKillNum.ToString());
        trapRemovedText.text = (TrapKillNum.ToString());
        StartCoroutine(MovePlayerToEndPos());
    }
    IEnumerator MovePlayerToEndPos()
    {
        yield return new WaitForSeconds(endDelayTime);
        //이 부분에 트랜지션 있어야 함(페이드아웃/인 효과)
        GameObject.Find("WindEffect").gameObject.SetActive(false); //속도이펙트 끄기
        player.transform.position = GameObject.Find("GameEndTransform").transform.position;
        player.GetComponent<Rigidbody>().isKinematic = true;
        //이긴텍스트이미지, 진 텍스트 이미지 꺼야한다.
        if (GameObject.Find("DMG_Effect_UI") != null)
        {
            GameObject.Find("DMG_Effect_UI").SetActive(false);
        }
    }
    private void GameResultClean()
    {
        timeText.text = ("");
        mileageText.text = ("");
        droneKillText.text = ("");
        trapRemovedText.text = ("");
    }
    
}