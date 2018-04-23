using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfter : MonoBehaviour {

    public float time;

    private void Awake() {
        Destroy(gameObject, time);
    }
}
