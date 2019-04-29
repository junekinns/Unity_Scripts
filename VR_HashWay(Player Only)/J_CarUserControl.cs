using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Vehicles.Car;
    [RequireComponent(typeof (J_CarController))]
    public class J_CarUserControl : MonoBehaviour
    {  //�÷��̾�� ������ ���� ���°��ִ�.
       /*

       2.Driving
    - HMD�� �� �� �� �� ��ġ�� �� GetAxis�� ���� ������ �Ѵ�.
    - �ӵ��� ���� Ÿ�ֿ̹� �°� �������� �鸰��.
    - PostProcessing���� �����ϴ� ȿ���� �ش�.

           3.Attack
           - �� ���� ������ Enemy�� �����ϰ� ���� ���θ��� ��ֹ��� �μ���.
           - Ʈ���� ��ư���� ���� ��� ���簡 �ȴ�. ������ �´�.

           4.Damage
           - Barrier ��ȹ�� ���߾� ��ȹ
           - Enemy�κ��� ������ ���ϰų�, �տ� ��ֹ��� �浹�ϸ� ü���� ���δ�.

           5.Die
           - Player�� ���� ����� Top View�� �Կ��ϴ�Death Cam�� �ߵ��ȴ�.
           - ȭ���� ������� �ȴ�
           - Game Over���� UI�� ���´�.
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
        Vector3 initCenterEyePos; //ó�� HMD ������
        Transform centerEyeAnchor; //���� HMD ������
        float HMD_h, HMD_v;
    float player_h; //Player���� ��
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
            //�ʱ� HMD ��ġ���� initCenerEyePos ���Ϳ� ����, CenterEyeAnchor Ʈ�������� ����ؼ� �������ġ�� ���
            initCenterEyePos = centerEyeAnchor.localPosition;
        }
        private void Start()
        {
            rb = GetComponent<Rigidbody>();
        }
        //GetAxis Vertical�� Z ���̷�, Horizontal�� X���̷�. ���Ʒ��� ����
        //���� Property���� Vector3�� float���� ������ �� �ְ��ϸ� �ȴ�.
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
        rb.AddForce(HMD_h*1000 * this.transform.right * steerK); //1000�� ������ ������, HMD_h���� �ʹ��۾Ƽ� �ڳʸ��� �ȵ�.
            wholeBodySteer = new Vector3(0, steerK * HMD_h, -steerK * HMD_h); //y,z�� ����
            wholeBody.transform.eulerAngles = wholeBodySteer;
            m_Car.Move(0, 1f, 0f, 0f);
        //Tramsform�̵������ �ణ ���������� ���� �־ Move���� �¿� ������ �޵��� ��, ���� ���� ����
        J_PNI.Instance.axes.roll = 10000 + (int)(HMD_h * PNI_K);
        J_PNI.Instance.axes.heave = 10000 + (int)(UnityEngine.Random.Range(-1, 2)*m_Car.CurrentSpeed / 2f); //��� ���� ���� ��¦
        currentSpeedText.text = ((int)m_Car.CurrentSpeed).ToString();
        }

    float vibSum = 0;
    public float vibTime = 0.5f;
    public float PNI_HEAVE = 300f; //300~500������ ����
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
