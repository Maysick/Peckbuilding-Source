using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterInfoUI : MonoBehaviour {

    Text text;

    public Fighter fighter;

    private void Awake() {
        text = GetComponent<Text>();
    }

    private void Update() {
        text.text = "" + fighter.deck.currentDeck.Count + " Cards\n" + fighter.deck.discard.Count + " Cards\n" + fighter.woundCount;
    }
}
