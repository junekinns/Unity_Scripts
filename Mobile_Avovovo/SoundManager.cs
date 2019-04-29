using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour {

    public AudioClip jumpSound; //점프 소리 변수 선언
    public AudioClip goalSound; //골인 소리 변수 선언
    public AudioClip enemySound; //적 소리 변수 선언
    public AudioClip hitSound; //벽 충돌 소리 변수 선언
    public AudioClip startSound; //시작 소리 변수 선언
    public AudioClip endSound; //시작 소리 변수 선언
    AudioSource myAudio; //AudioSorce 컴포넌트를 변수로 선언
    public static SoundManager instance;  //자기자신을 변수로 선언(Static)

    //Start보다 먼저, 객체가 생성될때 호출되는 Awake사용
    void Awake() 
    {
        if (SoundManager.instance == null) //incetance가 비어있는지?
            SoundManager.instance = this; //비었다면 자기 자신을 담는다.
    }

    void Start()
    {
        myAudio = this.gameObject.GetComponent<AudioSource>();

        DontDestroyOnLoad(myAudio);
    }
    private void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex > 5)
            myAudio.Stop();
    }
    public void PlayJumpSound()
    {
        myAudio.PlayOneShot(jumpSound); //jumpSound 재생    
    }

    public void PlayGoalSound()
    {
        myAudio.PlayOneShot(goalSound); //goalSound 재생    
    }

    public void PlayEnemySound()
    {
        myAudio.PlayOneShot(enemySound); //enemySound 재생    
    }

    public void PlayHitSound()
    {
        myAudio.PlayOneShot(hitSound); //enemySound 재생    
    }

    public void PlayStartSound()
    {
        myAudio.PlayOneShot(startSound); //enemySound 재생    
    }

    public void PlayEndSound()
    {
        myAudio.PlayOneShot(endSound); //enemySound 재생    
    }
}
