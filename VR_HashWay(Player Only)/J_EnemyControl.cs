using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityStandardAssets.Vehicles.Car
{
    [RequireComponent(typeof(J_EnemyCarController))]
    public class J_EnemyControl : MonoBehaviour
    {
        private J_EnemyCarController e_Car; // the car controller we want to use
        Vector3 e_bodySteer;
        public float e_steerK = 30f; //에너미 회전 강도(플레이어랑 같게? 더 크게?)
        Rigidbody e_rb;
        // Use this for initialization
        void Start()
        {
            e_rb = GetComponent<Rigidbody>();
            e_Car = GetComponent<J_EnemyCarController>();
            curentTime = 0f;
        }

        // Update is called once per frame
        private float curentTime = 0f;
        public float steerTime = 3f;
        float h;
        void Update()
        {
            float v = Input.GetAxis("Vertical");

            float h = Input.GetAxis("Horizontal");
            //진짜 사람이키보드를 누르는 것 같은 랜덤함이 있어야 한다.
            //h값이 딱 딱 정해지면, 움직임이 뚝뚝끊어진다.

            /* curentTime += Time.deltaTime;
             if (curentTime > steerTime)
             {
                 h = Random.Range(-1f, 1f);
                 curentTime = 0;
             }           //Transform이 아닌, Addforce 생각보다 괜찮음
     */
            e_rb.AddForce(h * this.transform.right * e_steerK);
            e_bodySteer = new Vector3(0, e_steerK * h, -e_steerK * h); //y,z축 기울기
            this.transform.eulerAngles = e_bodySteer;
            e_Car.Move(0, 1f, v, 0f); //Move 메서드에서의 움직임은 직진만 해야하고, enemy는 항상 풀악셀상태
            //A,D를 누르면, 차체가 좌 우로 기울고, 좌회전 우회전이 된다. 곡선형 회전이 아닌, 평행이동 느낌의 회전이 된다.
            //그렇지 않고 y freeze를 해제하고 하면 넘어짐

        }
    }
}
