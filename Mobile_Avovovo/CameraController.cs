using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public J_Player thePlayer;
    // Use this for initialization
    Vector3 cameraPosition, targetPosition;
    public float followSpeed = 5;
    void Start()
    {
        thePlayer = FindObjectOfType<J_Player>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        targetPosition.Set(thePlayer.transform.position.x, thePlayer.transform.position.y, thePlayer.transform.position.z-10);
        this.transform.position = Vector3.Lerp(this.transform.position, targetPosition, followSpeed * Time.deltaTime);
    }
}
