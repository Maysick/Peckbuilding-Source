using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockBob : MonoBehaviour {

    public float speed = 1f;
    public float amplitude = 0.05f;

    private float t;

    private float startY = 0;

    private void Awake() {
        startY = transform.position.y;
        t = Random.value * Mathf.PI * 2;
    }

    private void Update() {
        t += Time.deltaTime * speed;
        if (t > Mathf.PI * 2) t -= Mathf.PI * 2;
        transform.position = new Vector3(transform.position.x, startY + amplitude * Mathf.Sin(t), transform.position.z);
    }
}
