using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class J_CurvedPlane : MonoBehaviour {

    public AnimationCurve ac;
    public float depth = 5f;
    Mesh mesh;
    void Start () {
        mesh = GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;
        for(int i = 0; i<vertices.Length; i++)
        {
            vertices[i].y = ac.Evaluate(Mathf.Abs(vertices[i].x)/5f) * depth;
        }
        mesh.vertices = vertices;
        mesh.RecalculateNormals();
        }
	
	// Update is called once per frame
	void Update () {
    }

    
}
