using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class J_TrapExplosionEffect : MonoBehaviour {
    private void OnEnable()
    {
        Destroy(this.gameObject, 2.0f);
    }
}
