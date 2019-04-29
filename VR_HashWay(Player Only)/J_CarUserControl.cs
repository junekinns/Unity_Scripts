using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Vehicles.Car;
    [RequireComponent(typeof (J_CarController))]
    public class J_CarUserControl : MonoBehaviour
    {  //플레이어는 다음과 같은 상태가있다.
       /*

       2.Driving
    - HMD의 동 서 남 북 위치가 곧 GetAxis와 같은 역할을 한다.
    - 속도와 기어변속 타이밍에 맞게 엔진음이 들린다.
    - PostProcessing으로 질주하는 효과를 준다.

           3.Attack
           - 양 손의 총으로 Enemy를 공격하고 앞을 가로막는 장애물을 부순다.
           - 트리거 버튼으로 총을 쏘고 연사가 된다. 진동이 온다.

           4.Damage
           - Barrier 기획에 맞추어 기획
           - Enemy로부터 공격을 당하거나, 앞에 장애물에 충돌하면 체력이 깎인다.

           5.Die
           - Player가 죽은 모습을 Top View로 촬영하는Death Cam이 발동된다.
           - 화면이 흑백으로 된다
           - Game Over같은 UI가 나온다.
        */
        enum PlayerState
        {
            Driving,
            Attack,
            Barrier,
            Damage,
            Die
        }

        PlayerState _pState;
        private J_CarController m_Car; // the car controller we want to use
        Vector3 initCenterEyePos; //처음 HMD 포지션
        Transform centerEyeAnchor; //현재 HMD 포지션
        float HMD_h, HMD_v;
    float player_h; //Player높이 값
        public Text currentSpeedText;
        public GameObject wholeBody;
        public float steerK = 50, PNI_K = 50;
        Rigidbody rb;
        Vector3 wholeBodySteer;
        private void Awake()
        {
            // get the car controller
            _pState = PlayerState.Driving;
            m_Car = GetComponent<J_CarController>();
            centerEyeAnchor = GameObject.Find("CenterEyeAnchor").transform;
            //초기 HMD 위치값을 initCenerEyePos 벡터에 저장, CenterEyeAnchor 트랜스폼은 계속해서 사용자위치를 기억
            initCenterEyePos = centerEyeAnchor.localPosition;
        }
        private void Start()
        {
            rb = GetComponent<Rigidbody>();
        }
        //GetAxis Vertical을 Z 차이로, Horizontal을 X차이로. 위아래는 없음
        //위의 Property에서 Vector3의 float값만 접근할 수 있게하면 된다.
        public Vector3 CenterEyeDistance
        {
            get
            {
                return (centerEyeAnchor.localPosition - initCenterEyePos);
            }
        }
    private void FixedUpdate()
        {
            switch (_pState)
            {
                case PlayerState.Driving:
                    Driving();
                    break;
                case PlayerState.Attack:
                    break;
                case PlayerState.Barrier:
                    break;
                case PlayerState.Damage:
                    break;
                case PlayerState.Die:
                    break;
            }

        }
        private void Driving()
        {
            // pass the input to the car!
            HMD_h = CenterEyeDistance.x;
            HMD_v = CenterEyeDistance.z;
        rb.AddForce(HMD_h*1000 * this.transform.right * steerK); //1000을 곱하지 않으면, HMD_h값이 너무작아서 코너링이 안됨.
            wholeBodySteer = new Vector3(0, steerK * HMD_h, -steerK * HMD_h); //y,z축 기울기
            wholeBody.transform.eulerAngles = wholeBodySteer;
            m_Car.Move(0, 1f, 0f, 0f);
        //Tramsform이동방식은 약간 억지스러운 면이 있어서 Move에서 좌우 움직임 받도록 함, 추후 변경 가능
        J_PNI.Instance.axes.roll = 10000 + (int)(HMD_h * PNI_K);
        J_PNI.Instance.axes.heave = 10000 + (int)(UnityEngine.Random.Range(-1, 2)*m_Car.CurrentSpeed / 2f); //노면 진동 아주 살짝
        currentSpeedText.text = ((int)m_Car.CurrentSpeed).ToString();
        }

    float vibSum = 0;
    public float vibTime = 0.5f;
    public float PNI_HEAVE = 300f; //300~500정도가 적당
    IEnumerator CoSimVib()
    {
        while (vibSum < vibTime)
        {
            //J_PNI.Instance.axes.heave = 10000 + (int)(PNI_HEAVE);
            J_PNI.Instance.axes.surge = 10000 + (int)(PNI_HEAVE);
            vibSum += 0.05f;
            PNI_HEAVE = -(int)(PNI_HEAVE);
            yield return new WaitForSeconds(0.03f);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Obstacle")
        {
            vibSum = 0;
            StartCoroutine(CoSimVib());
        }
    }
    public void TrapCrash()
    {
        vibSum = 0;
        StartCoroutine(CoSimVib());
    }
}
