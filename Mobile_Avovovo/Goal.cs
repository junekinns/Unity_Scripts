using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour {
    public float rotSpeed;
	void Update () {
        transform.Rotate(Vector3.back *rotSpeed * Time.deltaTime);        
    }
}
