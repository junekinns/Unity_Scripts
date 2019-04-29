using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class J_UISoundManager : MonoBehaviour
{
    public static J_UISoundManager Instance;

    public AudioClip pointSound; //포인팅 소리 변수 선언
    public AudioClip selectSound; //선택 소리 변수 선언
    public AudioClip exitSound; //게임종료소리 변수 선언
    AudioSource _uiAudioSource; //AudioSorce 컴포넌트를 변수로 선언

    //Start보다 먼저, 객체가 생성될때 호출되는 Awake사용
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    void Start()
    {
        _uiAudioSource = GetComponent<AudioSource>();
    }
    public void PlayPointSound()
    {
        _uiAudioSource.PlayOneShot(pointSound); //jumpSound 재생    
    }

    public void PlaySelectSound()
    {
        _uiAudioSource.PlayOneShot(selectSound); //goalSound 재생    
    }

    public void PlayExitSound()
    {
        _uiAudioSource.PlayOneShot(exitSound); //enemySound 재생    
    }   
}
