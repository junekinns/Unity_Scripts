using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public class J_StartSceneFire : MonoBehaviour
{
    //시작 씬 에서만 쓰이는 총 쏘는 스크립트

    #region 전역변수
    [Header("총구 위치")]
    // 총구 위치
    public Transform leftGunPos;
    public Transform rightGunPos;

    [Header("크로스헤어")]
    // 크로스헤어
    public Transform rightCrosshair;
    public Transform leftCrosshair;
    Vector3 rightOriginSize;
    Vector3 leftOriginSize;
    public float kSizeAdjust; //거리보정값

    [Header("이펙트")]
    // 이펙트들
    // 씬에 1~2개만 놓고 계속 껐다켰다하자
    // 이펙트들
    // 씬에 1~2개만 놓고 계속 껐다켰다하자
    public ParticleSystem[] rightEffect;
    public ParticleSystem[] leftEffect;
    // 파티클 배열 
    // 0 = 총구
    // 1 = 다른 충돌체 명중 시

    [Header("진동")]
    // 진동
    public AudioClip vibration;
    OVRHapticsClip vibrationClip;

    // 오디오
    AudioSource[] rightGunSound;
    AudioSource[] leftGunSound;

    // 레이 거리
    J_SceneManager _sceneManager;
    float rayDis = 200.0f;

    //연사기능을 위한 속성들
    //발사 간격 FixedUpdate이므로 setDelay / 50 초마다 발사
    public int setDelay = 5;
    //발사하고 흐른 시간
    int delay = 0;
    // 사운드 딜레이 시간
    float currentTime_R = 0, currentTime_L = 0;
    [Header("연사속도/ 숫자가 작을수록 빠름")]
    public float fireRate = 0.05f; //연사속도 조절하는 속성
                                   // 사운드 체크
    bool readyVib_R , readyVib_L;
    bool isGunSound;
    //총알자국효과는 나중에
    #endregion
    // Use this for initialization
    void Start()
    {
        InputTracking.Recenter(); //딱 로비에서만 리센터 해줘야 다른신 넘어갈때도 유지된다.
        _sceneManager = GameObject.Find("J_SceneManager").GetComponent<J_SceneManager>();
        _sceneManager.enabled = false;
        // 사운드
        rightGunSound = GameObject.Find("RightGun").GetComponents<AudioSource>();
        leftGunSound = GameObject.Find("LeftGun").GetComponents<AudioSource>();
        //진동
        vibrationClip = new OVRHapticsClip(vibration);

        // 초기 거리값
        rightOriginSize = rightCrosshair.localScale;
        leftOriginSize = leftCrosshair.localScale;
    }
    // Update is called once per frame
    public void UIVibrationR()
    {
        if (readyVib_R)
        {
            OVRHaptics.RightChannel.Preempt(vibrationClip);
            J_UISoundManager.Instance.PlayPointSound();
        }
        readyVib_R = false;
    }
    public void UIVibrationL()
    {
        if (readyVib_L)
        {
            OVRHaptics.LeftChannel.Preempt(vibrationClip);
            J_UISoundManager.Instance.PlayPointSound();
        }
        readyVib_L = false;
    }

    // - 버튼에 Ray가 닿았을 때 색깔을 변경하고 싶다.
    // 만약 Ray의 hitInfo에 닿은 collider가 버튼의 Collider 라면,
    // 해당 버튼의 색깔을 변경한다. (sprite Renderer Change)
    // 만약 Ray가 버튼의 Collider를 벗어나면 색깔을 다시 원래대로 변경한다.
    const string OnRaycastExitMessage = "OnRaycastExit";
    const string OnRaycastEnterMessage = "OnRaycastEnter";

    GameObject previous;

    void SendMessageTo(GameObject target, string message)
    {
        if (target)
            target.SendMessage(message, gameObject,
                    SendMessageOptions.DontRequireReceiver);
    }

    IEnumerator CoSelectStorymode()
    {
        J_UISoundManager.Instance.PlaySelectSound();
        yield return new WaitForSeconds(0.5f);
        _sceneManager.enabled = true;
        _sceneManager.SceneIndex = 2; //스토리모드 씬인덱스2번
    }
    IEnumerator CoSelectTutorial()
    {
        J_UISoundManager.Instance.PlaySelectSound();
        yield return new WaitForSeconds(0.5f);
        _sceneManager.enabled = true;
        _sceneManager.SceneIndex = 3; //튜토리얼 씬인덱스2번
    }
    IEnumerator CoExit()
    {
        J_UISoundManager.Instance.PlayExitSound();
        yield return new WaitForSeconds(1.0f);
        Application.Quit();
    }
    public float zoomSpeed = 1f;
    IEnumerator CoSelectZoom(RaycastHit hitInfo)
    {
        float step = zoomSpeed * Time.fixedDeltaTime;
        float t = 0;
        while (t <= 1.0f)
        {
            t += step; // Goes from 0 to 1, incrementing by step each time
            hitInfo.transform.localScale = Vector3.Lerp(hitInfo.transform.localScale,new Vector3(0.4f,0.35f,0.5f), t); // Move objectToMove closer to b
            hitInfo.collider.GetComponent<SpriteRenderer>().color = Color32.Lerp(hitInfo.collider.GetComponent<SpriteRenderer>().color, new Color32(142, 246, 255, 10), t);
           yield return new WaitForFixedUpdate();
        }
    }
    void Update()
    {
        //여기부터 현정이 건비트 스크립트 배껴옴
        Ray rightRay = new Ray(rightGunPos.position, rightGunPos.forward);
        RaycastHit righthitInfo;
        Ray leftRay = new Ray(leftGunPos.position, leftGunPos.forward);
        RaycastHit leftHitInfo;

        //레이 디버깅용으로 씬뷰에서 볼 수 있도록
        Debug.DrawRay(rightRay.origin, rightRay.direction * 50, Color.red);
        Debug.DrawRay(leftRay.origin, leftRay.direction * 50, Color.blue);
        if (Input.GetKeyDown(KeyCode.C))
        {
            //리센터 비상용 코드
            InputTracking.Recenter(); //리센터시킨다.
        }
        #region 크로스헤어
        // 크로스헤어
        // 적 이외의 어딘가에 닿았을 때
        // (나 자신은 제외)
        if (Physics.Raycast(rightRay, out righthitInfo, rayDis, ~gameObject.layer))
        {
            rightCrosshair.GetComponent<SpriteRenderer>().color = Color.gray; // 회색 크로스헤어
            rightCrosshair.position = righthitInfo.point + new Vector3(0, 0.05f, 0); //크로스헤어 위치 보정
            // 거리에 따라 크기 보정
            rightCrosshair.localScale = rightOriginSize * righthitInfo.distance * kSizeAdjust;
            
            if (righthitInfo.collider.name.Contains("Button")) //Button을 포인팅 하면
            {
                GameObject current = righthitInfo.collider.gameObject;
                if (previous != current)
                {
                    SendMessageTo(previous, OnRaycastExitMessage);
                    SendMessageTo(current, OnRaycastEnterMessage);
                    previous = current;
                }
                UIVibrationR();
                rightCrosshair.GetComponent<SpriteRenderer>().color = Color.blue;
            }
            else
            {
                readyVib_R = true;
            }
        }
        // 아무데도 닿은데가 없으면?
        else
        {
            SendMessageTo(previous, OnRaycastExitMessage);
            previous = null;
            readyVib_R = true;
            rightCrosshair.GetComponent<SpriteRenderer>().color = Color.gray;
            rightCrosshair.position = righthitInfo.point;
            rightCrosshair.localScale = rightOriginSize * righthitInfo.distance * kSizeAdjust;
        }




        if (Physics.Raycast(leftRay, out leftHitInfo, rayDis, ~gameObject.layer))
        {
            leftCrosshair.GetComponent<SpriteRenderer>().color = Color.gray;
            // 회색 크로스헤어
            leftCrosshair.position = leftHitInfo.point + new Vector3(0, 0.05f, 0); ;
            // 거리에 따라 크기 보정
            leftCrosshair.localScale = leftOriginSize * leftHitInfo.distance * kSizeAdjust;
            
            if (leftHitInfo.collider.name.Contains("Button"))//Button을 포인팅 하면
            {
                GameObject current = leftHitInfo.collider.gameObject;
                if (previous != current)
                {
                    SendMessageTo(previous, OnRaycastExitMessage);
                    SendMessageTo(current, OnRaycastEnterMessage);
                    previous = current;
                }
                UIVibrationL();
                leftCrosshair.GetComponent<SpriteRenderer>().color = Color.blue;
            }
            else
            {
                SendMessageTo(previous, OnRaycastExitMessage);
                 previous = null;
                readyVib_L = true;
            }
        }
        // 아무데도 닿은데가 없으면?
        
        else
        {
            //SendMessageTo(previous, OnRaycastExitMessage);
           // previous = null;
            readyVib_L = true;
            leftCrosshair.GetComponent<SpriteRenderer>().color = Color.gray;
            leftCrosshair.position = leftHitInfo.point;
            leftCrosshair.localScale = leftOriginSize * leftHitInfo.distance * kSizeAdjust;
        }
        #endregion
        #region 오른쪽 총
        //오른쪽 총은 연사를 위해 Get으로 입력을 받음
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
        {
            // 만약 연속으로 발사중이라면 사운드에 딜레이 주기
            currentTime_R += Time.deltaTime;
            if (currentTime_R > fireRate)
            {
                //총구(쏠 때 항상 재생)
                rightEffect[0].Stop();
                rightEffect[0].Play();

                // 격발소리
                rightGunSound[0].Stop();
                rightGunSound[0].Play();
                // 진동!
                UIVibrationR();
                currentTime_R = 0;
            }

            if (righthitInfo.collider == null)
            {
                return;
            }
            else if (righthitInfo.collider.name == "StorymodeButton") //스토리모드 버튼
            {
                // 격발소리멈춤
                rightGunSound[0].Stop();
                // 맞은 것이 Enemy라면 impact효과
                StartCoroutine(CoSelectZoom(righthitInfo));
                StartCoroutine(CoSelectStorymode());
            }
            else if (righthitInfo.collider.name == "TutorialButton") //튜토리얼 버튼
            {
                // 격발소리멈춤
                rightGunSound[0].Stop();
                StartCoroutine(CoSelectZoom(righthitInfo));
                StartCoroutine(CoSelectTutorial());
                // 맞은 것이 Enemy라면 impact효과
            }
            else if (righthitInfo.collider.name == "ExitButton") //종료 버튼
            {
                // 격발소리멈춤
                rightGunSound[0].Stop();
                StartCoroutine(CoSelectZoom(righthitInfo));
                StartCoroutine(CoExit());
                // 맞은 것이 Enemy라면 impact효과
            }
            if (delay <= 0) //딜레이가 걸려있지 않을 때만 발사
            {
                delay = setDelay;   //발사했다면 딜레이가 생긴다.
            }
            if (0 < delay)
                delay--;
        }
        else
        {
            currentTime_R = fireRate; //총 발사 딜레이맞춰주기위해
        }
        #endregion
        #region 왼쪽 총

        if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.LTouch))
        {
            currentTime_L += Time.deltaTime;
            if (currentTime_L > fireRate)
            {
                leftGunSound[0].Stop();
                leftGunSound[0].Play();

                //총구 이펙트
                leftEffect[0].Stop();
                leftEffect[0].Play();

                // 진동
                UIVibrationL();
                currentTime_L = 0;
            }
            if (delay <= 0) //딜레이가 걸려있지 않을 때만 발사
            {
                delay = setDelay;   //발사했다면 딜레이가 생긴다.
            }
            if (0 < delay)
                delay--;

            if (leftHitInfo.collider == null)
            {
                return;
            }
            else if (leftHitInfo.collider.name == "StorymodeButton") //스토리모드버튼
            {
                leftGunSound[0].Stop();
                StartCoroutine(CoSelectZoom(leftHitInfo));
                StartCoroutine(CoSelectStorymode());
            }
            else if (leftHitInfo.collider.name == "TutorialButton") //튜토리얼 버튼
            {
                leftGunSound[0].Stop();
                StartCoroutine(CoSelectZoom(leftHitInfo));
                StartCoroutine(CoSelectTutorial());
            }
            else if (leftHitInfo.collider.name == "ExitButton") //종료 버튼
            {
                leftGunSound[0].Stop();
                StartCoroutine(CoSelectZoom(leftHitInfo));
                StartCoroutine(CoExit());
                // 맞은 것이 Enemy라면 impact효과
            }
        }
        else
        {
            currentTime_L = fireRate; //총 발사 딜레이맞춰주기위해
        }
        #endregion
    }
}