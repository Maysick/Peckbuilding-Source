using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour {

    public GameObject peck;

    private void Awake() {
        StartCoroutine(SpawnPecks());
    }

    IEnumerator SpawnPecks() {
        for (int i = 0; i < 10; i++) {
            Instantiate(peck, new Vector3(-10, 0, 0), Quaternion.identity);
            yield return new WaitForSeconds(Random.Range(0.5f, 1.25f));
        }

    }

    public void StartGame() {
        SceneManager.LoadScene(1);
    }
}
