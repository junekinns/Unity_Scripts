using System.Collections;
using System.Collections.Generic;
// 외부 라이브러리(DLL, so, java, jar 파일등...) 와 연결이 필요할 경우 추가
using System.Runtime.InteropServices;
using UnityEngine;
// 사용할 6축 값 선언
public struct MotionAxes
{
    // 이동
    public int surge;
    public int sway;
    public int heave;
    // 회전
    public int roll;
    public int pitch;
    public int yaw;
}
public class J_PNI : MonoBehaviour {
    // 사용할 dll import
    [DllImport("AvSimDllMotionExternC")]
    public static extern int MotionControl__Initial();
    [DllImport("AvSimDllMotionExternC")]
    public static extern int MotionControl__Destroy();
    [DllImport("AvSimDllMotionExternC")]
    public static extern void MotionControl__DOF_and_Blower(int nRoll, int nPitch, int nYaw, int nSway, int nSurge, int nHeave, int nSpeed, int nBlower);

    // 싱글톤객체
    public static J_PNI Instance;
    // 모션기기의 6축 값을 변수로 지정해 놓은 구조체
    public MotionAxes axes;

    //모션 장치의 기본 값"
    public static int kDefaultAxesValue = 10000;
    [Tooltip("모션에 변경해줄 증감 값")]
    public int changeValue = 600;
    [Tooltip("모션속도")]
    public int motionSpeed = 3;
    [Tooltip("모션 Blower 값")]
    public int blowerValue = 10;

    [Tooltip("기기의 속도 증가를 보정하기 위한 변수")]
    public float tuning = 2.72f;
    private void Awake()
    {
        Instance = this;
        // 모션기기 초기화
        MotionControl__Initial();
    }

    void Start()
    {
        // 기기 값 초기화 시키기
        InitMotionAxes();
    }

    // 모션기기 초기화
    private void InitMotionAxes()
    {
        axes.surge = kDefaultAxesValue;
        axes.sway = kDefaultAxesValue;
        axes.heave = kDefaultAxesValue;
        axes.roll = kDefaultAxesValue;
        axes.pitch = kDefaultAxesValue;
        axes.yaw = kDefaultAxesValue;
        MotionControl__DOF_and_Blower(axes.roll, axes.pitch, axes.yaw, axes.sway, axes.surge, axes.heave, motionSpeed, blowerValue);
    }

    // Update is called once per frame
    void Update ()
    {
        MotionControl__DOF_and_Blower(axes.roll, axes.pitch, axes.yaw, axes.sway, axes.surge, axes.heave, motionSpeed, blowerValue);
    }

    private void OnDestroy()
    {
        MotionControl__DOF_and_Blower(10000, 10000, 10000, 10000, 10000, 10000, 3, 0);
        MotionControl__Destroy(); //버그가능성
    }



    private void OnApplicationQuit()
    {
        MotionControl__DOF_and_Blower(10000, 10000, 10000, 10000, 10000, 10000, 3, 0);
        MotionControl__Destroy();
    }
}
