using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class J_Trap : MonoBehaviour {
    public ParticleSystem dieEffect;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void Explosion()
    {
        PlayDieEffect();
        J_ScoreManager.Instance.TrapKillNum++; //스코어매니저 트랩 1점 추가!
        Destroy(this.gameObject);
    }

    private void PlayDieEffect()
    {
        ParticleSystem par = Instantiate(dieEffect);
        par.transform.position = this.transform.position + new Vector3(0,0,1);
        par.Play();
        par.GetComponent<AudioSource>().Play();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            other.GetComponent<J_CarUserControl>().TrapCrash();
            J_PlayerHP.Instance.PlayerDamagedbyObs();
            Explosion();
        }
    }
}
