using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitlePeck : MonoBehaviour {

    float speed = 1f;
    SpriteRenderer sr;
    private float t = 0;

    private float initY = 0;

    private void Awake() {
        sr = GetComponent<SpriteRenderer>();
        Restart();
    }


    void Restart() {
        transform.position = new Vector3(-10, Random.Range(0.9f, 4.1f), 0);
        initY = transform.position.y;
        speed = Random.Range(0.5f, 1.5f);
        sr.color = Random.ColorHSV(0, 1, 0.8f, 1f, 0.9f, 1f);
    }

    private void Update() {
        t += Time.deltaTime;
        if (t > Mathf.PI * 2) t -= Mathf.PI * 2;
        transform.position = new Vector3(transform.position.x + Time.deltaTime * speed, initY + Mathf.Sin(t), 0);

        if (transform.position.x >= 10) {
            Restart();
        }
    }


}
