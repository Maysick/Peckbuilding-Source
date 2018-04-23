using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Feather : MonoBehaviour {

    public int id;

    [SerializeField]
    public List<string> decklist;

	// Use this for initialization
	void Start () {
        if (CrossSceneData.beaten[id]) {
            transform.localEulerAngles = new Vector3(0, 0, -111f);
            transform.localPosition = new Vector3(-0.2f, 1.12f);
            GetComponent<RockBob>().enabled = false;
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
