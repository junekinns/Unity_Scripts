using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class J_PlayerFire : MonoBehaviour
{
    #region 전역변수
    [Header("탄창")]
    public Transform mag_R_InitPos;
    public Transform mag_L_InitPos;
    public GameObject magazine_R;
    public GameObject magazine_L;
    [Header("총구 위치")]
    // 총구 위치
    public Transform rightGunPos;
    public Transform leftGunPos;

    [Header("크로스헤어")]
    // 크로스헤어
    int wallLayerMask = 1 << 15;
    int playerLayerMask = 1 << 14;
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
    bool readyVib_R, readyVib_L;
    public AudioClip vibration;
    public AudioClip ReVibration;
    OVRHapticsClip vibrationClip;
    OVRHapticsClip ReVibrationClip;
    // 오디오
    AudioSource[] rightGunSound;
    AudioSource[] leftGunSound;

    // 레이 거리

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
    bool isGunSound;
    J_SceneManager _sceneManager;
    //총알자국효과는 나중에

    [Header("총알 갯수")]
    public Text magNumText_R;
    public Text magNumText_L; //총알 갯수 출력할 텍스트
    public Text hexText_R; //hex라고 써주는 텍스트(개귀찮네), HUD에 달려있음
    public Text hexText_L;
    string magHex_R, magHex_L; //총알을 16진수로 표현할 문자열 변수
    int magNum_R = 64, magNum_L = 64;
    #endregion
    // Use this for initialization
    void Start()
    {
        _sceneManager = GameObject.Find("J_SceneManager").GetComponent<J_SceneManager>();
        //초기 총알 갯수 출력
        PrintBulletText();// 사운드
        rightGunSound = GameObject.Find("RightGun").GetComponents<AudioSource>();
        leftGunSound = GameObject.Find("LeftGun").GetComponents<AudioSource>();
        //진동
        vibrationClip = new OVRHapticsClip(vibration);
        ReVibrationClip = new OVRHapticsClip(ReVibration);
        // 초기 거리값
        rightOriginSize = rightCrosshair.localScale;
        leftOriginSize = leftCrosshair.localScale;
    }
    // Update is called once per frame
#region 오큘러스진동
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
    public void VibrationR()
    {
        OVRHaptics.RightChannel.Preempt(vibrationClip);
    }
    public void VibrationL()
    {
        OVRHaptics.LeftChannel.Preempt(vibrationClip);
    }
    public void Re_VibrationR()
    {
        OVRHaptics.RightChannel.Preempt(ReVibrationClip);
    }
    public void Re_VibrationL()
    {
        OVRHaptics.LeftChannel.Preempt(ReVibrationClip);
    }
    #endregion

    IEnumerator CoExitSound() //ExitSound 소리
    {
        J_UISoundManager.Instance.PlayExitSound();
        yield return null;
        _sceneManager.enabled = true;
        _sceneManager.SceneIndex = 0; //로비 씬인덱스0번
    }
    private IEnumerator Reload()
    {
        //오른쪽 탄창이 떨어졌으면,
        if (magazine_R.gameObject.GetComponent<Rigidbody>().isKinematic == false)
        {
            yield return new WaitForSeconds(0.45f);
            VibrationR();
            magNum_R = 64;
            rightGunSound[3].Stop();
            rightGunSound[3].Play();
            magazine_R.gameObject.GetComponent<Rigidbody>().isKinematic = true;
            magazine_R.transform.position = mag_R_InitPos.position;
            isReloading_R = false;
        }
        //왼쪽 탄창이 떨어졌으면,
        if (magazine_L.gameObject.GetComponent<Rigidbody>().isKinematic == false)
        {
            yield return new WaitForSeconds(0.45f);
            VibrationL();
            magNum_L = 64;
            leftGunSound[3].Stop();
            leftGunSound[3].Play();
            magazine_L.gameObject.GetComponent<Rigidbody>().isKinematic = true;
            magazine_L.transform.position = mag_L_InitPos.position;
            isReloading_L = false;
        }

    }
    private void PrintBulletText()
    {
        magHex_R = magNum_R.ToString("X");
        magHex_L = magNum_L.ToString("X");

        if (isReloading_R)
        {
            hexText_R.text = "";
            magNumText_R.text = "Reload";
        }
        else
        {
            hexText_R.text = "HEX";
            magNumText_R.text = magHex_R;
        }

        if (isReloading_L)
        {
            hexText_L.text = "";
            magNumText_L.text = "Reload";
        }
        else
        {
            hexText_L.text = "HEX";
            magNumText_L.text = magHex_L;
        }
    }
    bool isReloading_R = false;
    bool isReloading_L = false;
    const string OnRaycastExitMessage = "OnRaycastExit";
    const string OnRaycastEnterMessage = "OnRaycastEnter";

    GameObject previous;

    void SendMessageTo(GameObject target, string message)
    {
        if (target)
            target.SendMessage(message, gameObject,
                    SendMessageOptions.DontRequireReceiver);
    }
    void FixedUpdate()
    {
        PrintBulletText();
        leftCrosshair.LookAt(leftGunPos);
        rightCrosshair.LookAt(rightGunPos);
        //여기부터 현정이 건비트 스크립트 배껴옴
        Ray rightRay = new Ray(rightGunPos.position, rightGunPos.forward);
        RaycastHit righthitInfo;
        Ray leftRay = new Ray(leftGunPos.position, leftGunPos.forward);
        RaycastHit leftHitInfo;
        #region 크로스헤어
        // 크로스헤어
        // 적 이외의 어딘가에 닿았을 때
        // ('나 자신'과 '벽'은 제외)
        if (Physics.Raycast(rightRay, out righthitInfo, rayDis, ~wallLayerMask & ~playerLayerMask))
        {
            rightCrosshair.GetComponent<SpriteRenderer>().color = Color.blue;
            // 파란색 크로스헤어
            rightCrosshair.position = righthitInfo.point;
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
            }
            else
            {
                readyVib_R = true;
            }

            if (righthitInfo.collider.tag == "EnemyBike" || righthitInfo.collider.tag == "Drone") //Tag로 받아와서 색 변경
            {
                // 빨갛게
                rightCrosshair.GetComponent<SpriteRenderer>().color = Color.red;
            }

        }
        // 아무데도 닿은데가 없으면?
        else
        {
            SendMessageTo(previous, OnRaycastExitMessage);
            previous = null;
            readyVib_R = true;
            rightCrosshair.GetComponent<SpriteRenderer>().color = Color.blue;
            // 레이의 끝점에 크로스헤어를 위치시키는 명령..?
            rightCrosshair.position = righthitInfo.point;
            rightCrosshair.localScale = rightOriginSize * righthitInfo.distance * kSizeAdjust;
        }

        if (Physics.Raycast(leftRay, out leftHitInfo, rayDis, ~wallLayerMask & ~playerLayerMask))
        {
            leftCrosshair.GetComponent<SpriteRenderer>().color = Color.blue;
            // 파란색 크로스헤어
            leftCrosshair.position = leftHitInfo.point;
            // 거리에 따라 크기 보정
            leftCrosshair.localScale = leftOriginSize * leftHitInfo.distance * kSizeAdjust;

            if (leftHitInfo.collider.name.Contains("Button")) //Button을 포인팅 하면
            {
                GameObject current = leftHitInfo.collider.gameObject;
                if (previous != current)
                {
                    SendMessageTo(previous, OnRaycastExitMessage);
                    SendMessageTo(current, OnRaycastEnterMessage);
                    previous = current;
                }
                UIVibrationL();
            }
            else
            {
                SendMessageTo(previous, OnRaycastExitMessage);
                previous = null;
                readyVib_L = true;
            }

            if (leftHitInfo.collider.tag == "EnemyBike" || leftHitInfo.collider.tag == "Drone")
            {
                //빨갛게 
                leftCrosshair.GetComponent<SpriteRenderer>().color = Color.red;
            }

        }
        // 아무데도 닿은데가 없으면?
        else
        {
            readyVib_L = true;
            leftCrosshair.GetComponent<SpriteRenderer>().color = Color.blue;
            leftCrosshair.position = leftHitInfo.point;
            leftCrosshair.localScale = leftOriginSize * leftHitInfo.distance * kSizeAdjust;
        }
        #endregion
        #region 0발체크 일단 잘 안됨
        #endregion
        #region 민주튜토리얼 양쪽 총알 체크
        // 만약 왼쪽 총알이 0이면 튜토리얼 매니저에게 알린다.
        if (MJ_Tutorial.Instance != null)
        {
            if (magNum_L == 0)
            {
                MJ_Tutorial.Instance.isLeftZero = true;
            }
            else if (magNum_L != 0)
            {
                MJ_Tutorial.Instance.isLeftZero = false;
            }
            // 만약 오른쪽 총알이 0이면 튜토리얼 매니저에게 알린다.
            if (magNum_R == 0)
            {
                MJ_Tutorial.Instance.isRightZero = true;
            }
            else if (magNum_R != 0)
            {
                MJ_Tutorial.Instance.isRightZero = false;
            }
        }
        
        #endregion
        #region 오른쪽 총
        //오른쪽 총은 연사를 위해 Get으로 입력을 받음
        if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch) && magNum_R > 0)
        {
            if (Physics.Raycast(rightRay, out righthitInfo, rayDis, ~wallLayerMask & ~playerLayerMask))
            {
                currentTime_R += Time.fixedDeltaTime;
                if (currentTime_R > fireRate)
                {
                    //총구
                    rightEffect[0].Stop();
                    rightEffect[0].Play();

                    // 격발소리
                    rightGunSound[0].Stop();
                    rightGunSound[0].Play();
                    if (righthitInfo.collider.name == "J_BacktoLobby_Temp")
                    {
                        GameManager.Instance.BackToLobbyCall();
                    }
                    else if (righthitInfo.collider.name == "J_Restart_Temp")
                    {
                        GameManager.Instance.RestartCall();
                    }
                    if (righthitInfo.collider.tag == "Drone")
                    {
                        //rightGunSound[0].Stop(); //그냥 총소리 정지
                        // 맞은 것이 드론이면, 드론 Hit impact효과(Spark)
                        rightEffect[1].transform.forward = righthitInfo.normal;
                        rightEffect[1].transform.position = righthitInfo.point;
                        rightEffect[1].Stop();
                        rightEffect[1].Play();
                        rightGunSound[4].Stop();
                        rightGunSound[4].Play(); //5번째 오디오소스 소리 재생(뚝배기 깨지는 소리)
                        MJ_CityDrone drone = righthitInfo.collider.transform.GetComponent<MJ_CityDrone>();
                        if (drone != null)
                        {
                            drone.MinusHP(rightRay.direction);
                        }
                    }
                    else if (righthitInfo.collider.tag == "EnemyBike")
                    {
                        rightEffect[2].transform.forward = righthitInfo.normal;
                        rightEffect[2].transform.position = righthitInfo.point;
                        rightEffect[2].Stop();
                        rightEffect[2].Play();
                        rightGunSound[4].Stop();
                        rightGunSound[4].Play(); //5번째 오디오소스 소리 재생
                        MJ_EnemyHP bike = righthitInfo.collider.transform.GetComponent<MJ_EnemyHP>();
                        if (bike != null)
                        {
                            bike.EnemyDamaged();
                        }
                    }
                    else if (righthitInfo.collider.tag == "Trap")
                    {
                        J_Trap trap = righthitInfo.collider.transform.GetComponent<J_Trap>();
                        if (trap != null)
                        {
                            trap.Explosion();
                        }
                    }
                    else if (righthitInfo.collider.name == "LobbyButton") //로비버튼
                    {
                        rightGunSound[0].Stop();
                        StartCoroutine(CoExitSound());
                    }
                    // 진동!
                    VibrationR();
                    currentTime_R = 0;
                    magNum_R--;
                }
            }
            // 아무데도 닿은데가 없으면?
            else
            {
                currentTime_R += Time.fixedDeltaTime;
                if (currentTime_R > fireRate)
                {
                    //총구
                    rightEffect[0].Stop();
                    rightEffect[0].Play();

                    // 격발소리
                    rightGunSound[0].Stop();
                    rightGunSound[0].Play();
                    // 진동!
                    VibrationR();
                    currentTime_R = 0;
                    magNum_R--;
                }
            }
             // 만약 연속으로 발사중이라면 사운드에 딜레이 주기

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
        //오른쪽 총알이 다 떨어진지 체크한다.
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch) && magNum_R == 0)
        {
            VibrationR();
            //총알이 모자라면?
            //틱 틱 소리 재생
            rightGunSound[1].Stop();
            rightGunSound[1].Play();
        }
        if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.RTouch) && !isReloading_R && magNum_R != 64)
        {
            VibrationR();
            isReloading_R = true;
            magNum_R = 0;
            magazine_R.gameObject.GetComponent<Rigidbody>().isKinematic = false;
            rightGunSound[2].Stop();
            rightGunSound[2].Play();
            //여기서 갈아끼우는 코루틴 호출하자!
            StartCoroutine(Reload());
        }

        #endregion
        #region 왼쪽 총

        if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.LTouch) && magNum_L > 0)
        {
            if (Physics.Raycast(leftRay, out leftHitInfo, rayDis, ~wallLayerMask & ~playerLayerMask))
            {
                currentTime_L += Time.fixedDeltaTime;
                if (currentTime_L > fireRate)
                {
                    //총구 이펙트
                    leftEffect[0].Stop();
                    leftEffect[0].Play();

                    leftGunSound[0].Stop();
                    leftGunSound[0].Play();
                    if (leftHitInfo.collider.tag == "Drone")
                    {
                        leftEffect[1].transform.forward = leftHitInfo.normal;
                        leftEffect[1].transform.position = leftHitInfo.point;
                        leftEffect[1].Stop();
                        leftEffect[1].Play();
                        leftGunSound[4].Stop();
                        leftGunSound[4].Play(); //5번째 오디오소스 소리 재생
                        // 맞은 것이 Drone이라면, 드론 Hit impact효과(Spark)  

                        MJ_CityDrone drone = leftHitInfo.collider.transform.GetComponent<MJ_CityDrone>();
                        if (drone != null)
                        {
                            drone.MinusHP(leftRay.direction);
                        }
                    }
                    else if (leftHitInfo.collider.tag == "EnemyBike")
                    {
                        leftEffect[2].transform.forward = leftHitInfo.normal;
                        leftEffect[2].transform.position = leftHitInfo.point;
                        leftEffect[2].Stop();
                        leftEffect[2].Play();
                        leftGunSound[4].Stop();
                        leftGunSound[4].Play(); //5번째 오디오소스 소리 재생
                        // 맞은 것이 Drone이라면, 드론 Hit impact효과(Spark)  
                        MJ_EnemyHP bike = leftHitInfo.collider.transform.GetComponent<MJ_EnemyHP>();
                        if (bike != null)
                        {
                            bike.EnemyDamaged();
                        }
                    }
                    else if (leftHitInfo.collider.tag == "Trap")
                    {
                        J_Trap trap = leftHitInfo.collider.transform.GetComponent<J_Trap>();
                        if (trap != null)
                        {
                            trap.Explosion();
                        }
                    }
                    else if (leftHitInfo.collider.name == "LobbyButton") //로비버튼
                    {
                        leftGunSound[0].Stop();
                        StartCoroutine(CoExitSound());
                    }
                    // 진동
                    VibrationL();
                    currentTime_L = 0;
                    magNum_L--;
                }
            }
            else
            {
                currentTime_L += Time.fixedDeltaTime;
                if (currentTime_L > fireRate)
                {
                    leftGunSound[0].Stop();
                    leftGunSound[0].Play();

                    //총구 이펙트
                    leftEffect[0].Stop();
                    leftEffect[0].Play();

                    // 진동
                    VibrationL();
                    currentTime_L = 0;
                    magNum_L--;
                }
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
            currentTime_L = fireRate; //총 발사 딜레이맞춰주기위해
        }
        //왼쪽 총알이 다 떨어진지 체크한다.
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.LTouch) && magNum_L == 0)
        {
            VibrationL();
            //총알이 모자라면?
            //틱 틱 소리 재생
            leftGunSound[1].Stop();
            leftGunSound[1].Play();
        }
        if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.LTouch) && !isReloading_L && magNum_L != 64)
        {
            VibrationL();
            isReloading_L = true;
            magNum_L = 0;
            magazine_L.gameObject.GetComponent<Rigidbody>().isKinematic = false;
            leftGunSound[2].Stop();
            leftGunSound[2].Play();
            //여기서 갈아끼우는 코루틴 호출하자!
            StartCoroutine(Reload());
        }
#endregion
    }
}
