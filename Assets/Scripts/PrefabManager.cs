using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Prefab{
    public string name;
    public GameObject go;
}

public class PrefabManager : MonoBehaviour {

    public static PrefabManager instance;

    [SerializeField]
    public Prefab[] prefabs;

    private void Awake() {
        instance = this;
    }

    public GameObject GetPrefabByName(string name) {
        foreach(Prefab p in prefabs) {
            if (p.name == name) return p.go;
        }
        return null;
    }
}
