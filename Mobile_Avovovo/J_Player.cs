using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/*
 * Project name: Avovovo
 * Target Game: 6180 the moon
 * 장르: 2D 횡스크롤
 * 특징: 위아래가 뚫려있고 이어져있음, 예쁜 BGM
 * 내용: 폭풍속에 들어가 미아가 된 아보카도씨앗이 몸체를 찾으러 가는 내용
 * 필요한 것: 맵, 씨앗(플레이어), 아보카도본체(골)
 */
public class J_Player : MonoBehaviour
{
    //Player의 속성들
    Animator anim;
    public static J_Player Instance; //접근가능하게 하기 위해 인스턴스 생성
    private Rigidbody2D rb;
    private Collider2D cd;
    public float moveSpeed;
    public float jumpForce;
    public float rotSpeed; //회전속도
    public int jumpCount;
    bool isJumping; //점프상태(점프카운트관련)
    bool isGoalin;
    float hori;
    public float howFar; //r의 정도(Player와 Goal의 거리 차이)
    public Transform goalPositon;
    public float forceScale; //거리 차이벡터정규화 후, 곱해주는 상수 값

    //JoyStick
    public Joystick joystick;
    public Button jumpbutton;

    float transitionTime; //현재시간,넘어가는시간(씬 넘어갈때 사용)
    
    Vector3 ceiling, floor; //천장과 바닥 위치 설정(순간이동하기위해)
    int stageLevel;
    bool isDamaged = false; //Enemy랑 부딪혔냐?
    private void Awake()
    {
        if (J_Player.Instance == null) //incetance가 비어있는지?
            J_Player.Instance = this; 
    }
    
    void Start()
    {
        anim = GetComponent<Animator>();
        stageLevel = SceneManager.GetActiveScene().buildIndex;
		rb = GetComponent<Rigidbody2D>();
        cd = GetComponent<Collider2D>();
        jumpCount = 0;
        ceiling = GameObject.Find("Ceiling").transform.position;
        floor = GameObject.Find("Floor").transform.position;
        transitionTime = 3;
        isGoalin = true;
    }

    // Update is called once per frame
    //일반적인 업데이트함수('입력'은 다 여기서 받는다.)
    void Update()
    {
        //점프 입력을 받는다.
        //Mobile
        jumpbutton.onClick.AddListener(JumpButtonClick);

        if (Input.GetKeyDown(KeyCode.UpArrow)) //위 방향키를 누르고, jumpCount가 0보다 크면,
            JumpButtonClick();
        

        if ((goalPositon.position - transform.position).magnitude < howFar)
            GoalIn();
        else Move();
       
		Jump();

        //천장에 닿으면 바닥으로 올라오게, 바닥에 닿으면 천장으로 떨어지게
		if (transform.position.y < floor.y)
			transform.position = new Vector3(transform.position.x, ceiling.y, transform.position.z);
		if (transform.position.y > ceiling.y)
			transform.position = new Vector3(transform.position.x, floor.y, transform.position.z);
		
        //낙하속도를 50 미만으로 제한.
        if (rb.velocity.y < -50)
            rb.velocity = new Vector2(rb.velocity.x, -50);
    }

    public void JumpButtonClick()
    {
        if(jumpCount<1 && !isDamaged)
        {
            //print("JumpButtonClick!");
            SoundManager.instance.PlayJumpSound();
            isJumping = true;
            jumpCount++;
        }
    }

    void Move()
    {
        if (!isDamaged) {
            //좌우이동만 가능
       //Mobile 
            hori = joystick.Horizontal;
       //PC : 
            float PChori = Input.GetAxis("Horizontal");
        
        transform.position += Vector3.right.normalized * hori * moveSpeed * Time.smoothDeltaTime;
        transform.position += Vector3.right.normalized * PChori * moveSpeed * Time.smoothDeltaTime;

            //이동할 동안 굴러가는 것처럼 보이게 하기 위해 회전을 시킨다.
            transform.Rotate(Vector3.back * rotSpeed *hori* Time.deltaTime);
            transform.Rotate(Vector3.back * rotSpeed * PChori * Time.deltaTime);
        }
    }

    void Jump()
    {
        if (!isJumping || isDamaged)
            return; //isJumping이 true일때만 점프가 되도록 한다.

        rb.AddForce(Vector3.up * jumpForce, ForceMode2D.Impulse); //중력의 영향을 받는 점프
        isJumping = false;
    }
    
    void GoalIn()
	{
        //카메라 줌
		StartCoroutine (CameraZoom ());
        //아보카도로 골인하는 상황(스테이지 클리어)
        //근처로 가면, 속도가 확 느려지면서 쏙 빨려들어가는 느낌
        if (isGoalin)
        {
            SoundManager.instance.PlayGoalSound();
            isGoalin = false;
        }
        rb.velocity = new Vector2(0, 0);
        rb.gravityScale = 0;
        transform.Rotate(Vector3.back * rotSpeed * Time.deltaTime);
        transform.position = Vector3.Lerp(transform.position, goalPositon.position, moveSpeed*Time.deltaTime);
        StartCoroutine(NextStage());
    }
    //바닥감지함수
    private void OnCollisionEnter2D(Collision2D collision)
    {
        
        switch (collision.gameObject.layer)
        {
            case 9: //layer가 9 이면,(Ground로 해놓음)
                jumpCount=0;
                 break;
            case 10: //Enemy에 부딪힌 경우!
                //print("Enemy Collision!");
                isDamaged = true;
                SoundManager.instance.PlayEnemySound();
                StartCoroutine(Die());
                break;
            case 11: //layer가 11 이면,(Bounce로 해놓음)
                SoundManager.instance.PlayHitSound();
                //print("Bounce DETECT!!!");
                jumpCount = 1;
                break;
        }
    }

    IEnumerator Die()
	{
        anim.SetTrigger("Die");
		StartCoroutine (CameraZoom ());
        cd.isTrigger = true;
        rb.velocity = new Vector2(0, 0);
        rb.gravityScale = 0.1f;
        transform.position = Vector3.Lerp(transform.position,new Vector3(transform.position.x,floor.y,0), Time.deltaTime);
        yield return new WaitForSeconds(transitionTime);
        //print("Die!");
        SceneManager.LoadScene(stageLevel);
        SoundManager.instance.PlayStartSound();
    }

	[Range(0,10.0f)]
	public float testSec;

	IEnumerator CameraZoom(){
		yield return new WaitForSeconds (0.5f);
		float t = 0.0f;
		t += Time.deltaTime*10;
		while (t!=1) {
			Camera.main.orthographicSize = Mathf.Lerp (Camera.main.orthographicSize, 2, t);
			Vector3 testDir = new Vector3 (transform.position.x, transform.position.y, -10.0f);
			Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, testDir,t);
			yield return null;
		}
	}

    IEnumerator NextStage()
    {
        yield return new WaitForSeconds(transitionTime);
        //print("Next Scene!");
        GameManager.Instance.NextScene();
    }

    //Ending 트리거
    private void OnTriggerEnter2D(Collider2D collision)
    {
        anim.SetTrigger("Happy");
    }
}
