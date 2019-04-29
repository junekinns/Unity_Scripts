using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollController : MonoBehaviour {

    Material myMat;
    public float scrollSpeed=0.01f;
    // Use this for initialization
    void Start () {
        MeshRenderer mr = gameObject.GetComponent<MeshRenderer>();
        myMat = mr.material;
    }
	
	// Update is called once per frame
	void Update () {
       myMat.mainTextureOffset += Vector2.up * scrollSpeed * Time.deltaTime;
    }
}
