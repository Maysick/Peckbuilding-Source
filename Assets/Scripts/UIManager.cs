using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UIManager : MonoBehaviour {

    public static UIManager instance;

    public GameObject deckParent;
    public GameObject discardParent;

    public GameObject handParent;

    public GameObject cardUIPrefab;

    public GameObject deckViewerParent;
    public GameObject discardViewerParent;
    private List<GameObject> deckViewerObjects;
    private List<GameObject> discardViewerObjects;

    public GameObject victoryEndImage;
    public GameObject lossEndImage;

    public GameObject indicatorParent;

    string currentScreen = "Normal";

    void Awake() {
        instance = this;
    }

    public void SpawnIndicatorCard(Card c, bool player, bool action) {
        if (c == null) return;
        Vector2 position = new Vector2(45, 200);
        if (player) {
            if (action) position.x = 135;
        } else {
            if (action) position.x = 755;
            else position.x = 665;
        }

        GameObject card = MakeCardUI(c, indicatorParent.transform) as GameObject;
        card.GetComponent<CardUI>().Indicator();
        card.GetComponent<RectTransform>().anchoredPosition = position;
    }

    public void EndGame(bool victory) {
        GameObject go = victoryEndImage;
        if (!victory) go = lossEndImage;
        go.transform.parent.gameObject.SetActive(true);
        go.GetComponent<Animator>().SetTrigger("Display");
    }

    public GameObject MakeCardUI(Card c, Transform parent) {
        GameObject newCardUIObj = Instantiate(cardUIPrefab, parent, false) as GameObject;
        CardUI newCardUI = newCardUIObj.GetComponent<CardUI>();
        newCardUI.card = c;
        newCardUI.name.text = c.name;
        if (c.type == CardType.Normal) newCardUI.type.text = "Action";
        if (c.type == CardType.Move) newCardUI.type.text = "Move";
        if (c.type == CardType.Wound) newCardUI.type.text = "Wound";
        newCardUI.description.text = c.description;
        newCardUI.sprite.sprite = c.image;

        return newCardUIObj;
    }

    public void DisplayDrawCard(Card c) {
        GameObject cui = MakeCardUI(c, handParent.transform);
        StartCoroutine(cui.GetComponent<CardUI>().FadeIn(false));
        c.inHandUI = cui.GetComponent<CardUI>();
    }

    private void PopulateDeckViewer() {
        if (deckViewerObjects == null) deckViewerObjects = new List<GameObject>();
        DestroyDeckViewer();
        foreach(Card c in GameManager.instance.player.deck.currentDeck) {
            GameObject cui = MakeCardUI(c, deckViewerParent.transform);
            deckViewerObjects.Add(cui);
        }
    }

    private void DestroyDeckViewer() {
        foreach(GameObject go in deckViewerObjects) {
            Destroy(go);
        }
    }

    private void PopulateDiscardViewer() {
        if (discardViewerObjects == null) discardViewerObjects = new List<GameObject>();
        DestroyDiscardViewer();
        foreach (Card c in GameManager.instance.player.deck.discard) {
            GameObject cui = MakeCardUI(c, discardViewerParent.transform);
            discardViewerObjects.Add(cui);
        }
    }

    private void DestroyDiscardViewer() {
        foreach (GameObject go in discardViewerObjects) {
            Destroy(go);
        }
    }

    public void SwitchToScreen(string screenName) {
        
        if (screenName == "Deck") {
            deckParent.SetActive(true);
            PopulateDeckViewer();
        } else if (screenName == "Discard") {
            discardParent.SetActive(true);
            PopulateDiscardViewer();
        } else if (screenName == "Normal") {
            deckParent.SetActive(false);
            discardParent.SetActive(false);
        }

        currentScreen = screenName;
    }

}
