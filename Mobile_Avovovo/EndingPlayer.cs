using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingPlayer : MonoBehaviour {
    SpriteRenderer rd;
    public Sprite happyAvo;
    // Use this for initialization
    void Start () {
        rd = GetComponent<SpriteRenderer>();
	}

    // Update is called once per frame
    private void OnTriggerEnter2D(Collider2D collision)
    {
        rd.sprite = happyAvo;
    }

    private void Update()
    {
        rd.sprite = happyAvo;
    }
}
