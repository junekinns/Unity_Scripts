using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RetroAesthetics;
using System;

public class J_PlayerHP : MonoBehaviour
{
    private GameObject damageEffectUI;
    private GameObject player;
    public float playerDamageK; //Player피 다는 정도 결정하는 상수. 클수록 많이 달음
    public GameObject loseScoreRoom;
    private float maxHp = 1f;
    private float hp = 0f;
    private MeshRenderer hpMesh;
    private float ranRed,ranGreen, ranBlue;
    public static J_PlayerHP Instance;
    bool isHudOn = true;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    // 플레이어 HP 상태
    // Idle : HP가 줄어들지 않고, 죽지도 않는 상태
    // DamageMode : 공격받아 hp가 줄어들고 있는 상태
    // DieMode : 죽은 상태 (GameOver)
    enum PlayerHPState
    {
        Idle,
        DamageMode,
        Die
    }
    private PlayerHPState pState;
    public float HP {
        get { return hp; }
        set
        {
            hp = Mathf.Clamp(value, 0, maxHp);
            hpMesh.material.SetFloat("_Guage", hp / maxHp); //실시간으로 hp바 업데이트
            if (hp <= 0)
            {
                EnemyWin();
            }
        }
    }

    void Start()
    {
        loseScoreRoom.SetActive(false); //처음엔 loseScoreRoom을 꺼준다.
        isHudOn = true;
        // 처음엔 Idle 상태
        pState = PlayerHPState.Idle;

        // ========= HP ==========
        hpMesh = GetComponent<MeshRenderer>();
        HP = maxHp;
        player = GameObject.Find("Player");

        // ======== Effect ========
        damageEffectUI = GameObject.Find("DMG_Effect_UI"); //플레인 휘어서 매터리얼로 효과 표현.
        // 처음에는 Damage, Die 이펙트가 보이지 않아야 한다.
        damageEffectUI.SetActive(false);
    }

    void FixedUpdate()
    {
        switch (pState)
        {
            case PlayerHPState.Idle:
                Idle();
                break;
            case PlayerHPState.DamageMode:
                Damage();
                break;
            case PlayerHPState.Die:
                Die();
                break;
        }
    }

    private void Idle()
    {
        hpMesh.material.SetFloat("_Red", 0.2f);
        hpMesh.material.SetFloat("_Green", 0.9f);
        hpMesh.material.SetFloat("_Blue", 0.6f); //맘에드는 수치를 찾아야 하긴 함
        // 아무일도 일어나지 않음.
        // Hp 유지
        HP *=1; //여기서 회복하게 하면 상태머신 꼬임
    }

    public void PlayerDamaged() //공격받는 그 순간
    {
        if (HP > 0)
        {
            damageEffectUI.SetActive(true);
               pState = PlayerHPState.DamageMode;
        }
    }
    public void PlayerDamagedbyObs() //공격받는 그 순간(장애물로 부터)
    {
        if (HP > 0)
        {
            damageEffectUI.SetActive(true);
            pState = PlayerHPState.DamageMode;
        }
    }
    float idleDelay = 0.5f; //Damage에서 Idle로 전환되는 딜레이, 이 시간을 증가시키면 더 오래 데미지를 입음
    float currentTime = 0;
    void Damage()
    {
        // 플레이어가 아직 죽지 않았으면 (hp가 남았으면)
            // HP bar 줄어드는 효과
            HPBarBlink();
            // 플레이어 Hp 줄어들게.
            HP -= playerDamageK;
        // 1초(idleDelay) 가 지나면,
        currentTime += Time.fixedDeltaTime;
        if (currentTime > idleDelay)
        {
            damageEffectUI.SetActive(false); //DMG effect UI 없애기
            pState = PlayerHPState.Idle;
            currentTime = 0;
        }
    }


    public void EnemyWin()
    {
        pState = PlayerHPState.Die;
        if (isHudOn)
        {
            isHudOn = false;
            GameObject.Find("HUD_Plane").SetActive(false);
        }// gameover되는 한 순간에만 작동해야하는 것들
        loseScoreRoom.SetActive(true); //LoseScoreRoom을 활성화 시킨다.
        GameManager.Instance.isGameEnd = true;
        GameManager.Instance.AppearLoseText();
        damageEffectUI.SetActive(false);
        GameObject.FindWithTag("Enemy").GetComponent<EnemyMove>().isPlayerDie = true;
        GameObject.FindWithTag("Enemy").GetComponent<EnemyAvoidMove>().enabled = false;
        player.GetComponent<Rigidbody>().isKinematic = true;
        player.GetComponent<AudioSource>().Stop();
        player.GetComponent<AudioSource>().Play();
        //여기서 진 결말 공간으로 이동
    }

    private void Die()
    {
        Debug.Log("PlayerDie");
    }

    //HP Bar가 (무지개처럼)깜빡이게 하도록 하는스크립트
    private void HPBarBlink()
    {
        ranRed = UnityEngine.Random.Range(0f, 1f);
        ranGreen = UnityEngine.Random.Range(0f, 1f);
        ranBlue = UnityEngine.Random.Range(0f, 1f);
        hpMesh.material.SetFloat("_Red", ranRed);
        hpMesh.material.SetFloat("_Green", ranGreen);
        hpMesh.material.SetFloat("_Blue", ranBlue);
    }
}