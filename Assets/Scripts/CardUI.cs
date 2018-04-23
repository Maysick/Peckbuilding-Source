using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardUI : MonoBehaviour {

    [HideInInspector]
    public Card card;

    public Vector2 startSize;
    private Vector2 goalSize;
    public RectTransform rt;
    private Vector2 lerpFrom;
    private float zoomSpeed = 5.5f;
    public float size = 1.75f;
    private float t = 1;

    public float layoutDefaultWidth;
    public float layoutDefaultHeight;
    private LayoutElement le;

    bool mouseOn;
    bool selected = false;
    bool zoomedIn = false;
    bool queueZoomIn = false;
    bool queueZoomOut = false;

    public bool inHand = false;

    public Text name;
    public Text description;
    public Text type;
    public Image sprite;
    public Image arrow;

    [HideInInspector]
    public bool reward = false;

    private void Awake() {
        le = GetComponent<LayoutElement>();
        startSize = new Vector2(1, 1);
        goalSize = startSize;
    }

    public void Destruct() {
        inHand = false;
        StartCoroutine(FadeOut());
    }

    public void Indicator() {
        rt.localScale = new Vector3(0, 0, 0);
        StartCoroutine(FadeIn(true));
    }


    public IEnumerator FadeIn(bool destruct) {
        float xt = 0;
        while (xt < 1) {
            xt += Time.deltaTime * 2f;
            rt.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, Mathf.Clamp(xt, 0, 1));
            yield return new WaitForEndOfFrame();
        }
        if (destruct) {
            yield return new WaitForSeconds(3f);
            Destruct();
        } else {
            inHand = true;
        }
    }

    IEnumerator FadeOut() {
        float xt = 0;
        while (xt < 1) {
            xt += Time.deltaTime * 2f;
            rt.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, Mathf.Clamp(1 - xt, 0, 1));
            yield return new WaitForEndOfFrame();
        }
        rt.localScale = new Vector3(0, 0, 0);
        Destroy(gameObject);
    }


    public void Select() {
        if (reward) {
            CrossSceneData.instance.ChooseReward(card);
            return;
        }
        if (!inHand) return;
        if (card.type == CardType.Wound) return;

        if (card.type == CardType.Normal) {
            if (selected) {
                selected = false;
                arrow.gameObject.SetActive(false);
                GameManager.instance.selectedCard = null;
                return;
            }

            if (GameManager.instance.selectedCard != null && GameManager.instance.selectedCard != this) {
                GameManager.instance.selectedCard.selected = false;
                GameManager.instance.selectedCard.arrow.gameObject.SetActive(false);
                GameManager.instance.selectedCard.ZoomOut();
            }
            GameManager.instance.selectedCard = this;
        } else if (card.type == CardType.Move) {
            if (selected) {
                selected = false;
                arrow.gameObject.SetActive(false);
                GameManager.instance.selectedCardMove = null;
                return;
            }

            if (GameManager.instance.selectedCardMove != null && GameManager.instance.selectedCardMove != this) {
                GameManager.instance.selectedCardMove.selected = false;
                GameManager.instance.selectedCardMove.arrow.gameObject.SetActive(false);
                GameManager.instance.selectedCardMove.ZoomOut();
            }
            GameManager.instance.selectedCardMove = this;
        }

        selected = true;
        arrow.gameObject.SetActive(true);
    }

    public void ZoomIn() {
        if (!inHand) return;
        if (!queueZoomOut) queueZoomIn = true;
        mouseOn = true;

    }

    public void ZoomOut() {
        if (!inHand) return;
        if (!queueZoomIn) queueZoomOut = true;
        mouseOn = false;

    }

    private void Update() {
        if (mouseOn && !zoomedIn && t == 1) {
            StartZoomIn();
            
        }
        if (!mouseOn && zoomedIn && t == 1) {
            StartZoomOut();
            
        }

        if (t < 1f) {
            t += Time.deltaTime * zoomSpeed;
            Vector2 lerp = Vector2.Lerp(lerpFrom, goalSize, Mathf.Clamp(Mathf.Pow(t, 3), 0, 1));
            rt.localScale = lerp;
        } else {
            t = 1;
        }
    }

    private void StartZoomIn() {
        zoomedIn = true;
        t = 1 - t;
        lerpFrom = startSize;
        goalSize = startSize * size;
        queueZoomIn = false;
    }

    private void StartZoomOut() {
        if (selected) return;
        zoomedIn = false;
        t = 1 - t;
        goalSize = startSize;
        lerpFrom = startSize * size;
        queueZoomOut = false;
    }
}
