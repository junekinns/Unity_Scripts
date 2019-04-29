using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class J_UIButtonChange : MonoBehaviour {

    // Use this for initialization
    Color initColor;
    Vector3 initScale;
	void Start () {
        initColor = GetComponent<SpriteRenderer>().color;
        initScale = GetComponent<Transform>().localScale;
	}
    // Update is called once per frame
    void Update()
    {

    }
    void OnRaycastEnter(GameObject sender)
    {
        this.transform.GetComponent<SpriteRenderer>().color = new Color32(142,246,255,255); //컬러 예쁜수치 찾기
        this.transform.localScale = initScale * 1.15f;
    }

    // These methods will be called by RaycastExample
    void OnRaycastExit(GameObject sender)
    {
        this.transform.GetComponent<SpriteRenderer>().color = initColor;
        this.transform.localScale = initScale;
    }
}
