using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class CrossSceneData : MonoBehaviour {

    public static int pendingReward = -1;

    public static List<string> decklist;
    public static List<string> enemyDecklist;

    public static bool rewardsSet = false;

    public static List<string>[] rewards;
    public static bool[] beaten;

    public static CrossSceneData instance;

    public GameObject rewardsParent;
    public GameObject rewardsHolder;

    private bool selectedLevel = false;
    public static int selectedID = -1;
    public GameObject indicator;

    public GameObject startButton;

    public GameObject finalBoss;

    public GameObject tutorialParent;

    public void ShowTut() {
        tutorialParent.SetActive(true);
    }

    public void HideTut() {
        tutorialParent.SetActive(false);
    }

    private void Awake() {
        instance = this;

        if (decklist == null) {
            decklist = new List<string>();
            decklist.Add("Hop");
            decklist.Add("Leap");
            decklist.Add("Dodge");
            decklist.Add("Peck");
            decklist.Add("Flurry");
            decklist.Add("Gust");
        }

        if (!rewardsSet) {
            beaten = new bool[4];

            rewardsSet = true;
            rewards = new List<string>[4];

            rewards[0] = new List<string>();
            rewards[0].Add("Eagle Eye");
            rewards[0].Add("Drill Peck");
            rewards[0].Add("Flap");

            rewards[1] = new List<string>();
            rewards[1].Add("Talon Storm");
            rewards[1].Add("Hurricane Gale");
            rewards[1].Add("Storm's Eye");

            rewards[2] = new List<string>();
            rewards[2].Add("Sharp Claws");
            rewards[2].Add("Take Off");
            rewards[2].Add("Fury Swipe");

            rewards[3] = new List<string>();
            rewards[3].Add("ULTRA PECK");

        }



    }

    private void Start() {
        if (pendingReward != -1) {
            beaten[selectedID] = true;
            if (rewards[pendingReward].Count != 0) {
                //show rewards screen
                ShowRewards();
                //add 3 cards to "hand"
                //whatever is clicked, add to deck
            }
        }

        if (beaten[0] && beaten[1] && beaten[2]) {
            finalBoss.SetActive(true);
        }
    }

    public void ShowRewards() {
        rewardsParent.SetActive(true);
        foreach(string s in rewards[pendingReward]) {
            GameObject newCard = UIManager.instance.MakeCardUI(CardManager.instance.GetCard(s), rewardsHolder.transform);
            newCard.GetComponent<CardUI>().reward = true;
        }
        
    }

    public void Quit() {
        Application.Quit();
    }

    public void ChooseReward(Card c) {
        string s = c.name;
        rewards[pendingReward].Remove(s);
        decklist.Add(s);
        HideRewards();
    }

    public void HideRewards() {
        rewardsParent.SetActive(false);
    }

    public void StartGame() {
        pendingReward = -1;
        SceneManager.LoadScene(2);
    }

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null) {
                if (hit.collider.gameObject.tag == "Feather") {
                    selectedLevel = true;
                    startButton.SetActive(true);
                    indicator.SetActive(true);
                    selectedID = hit.collider.gameObject.GetComponent<Feather>().id;
                    enemyDecklist = hit.collider.gameObject.GetComponent<Feather>().decklist;
                    Transform rock = hit.collider.transform.parent;
                    indicator.transform.position = new Vector3(rock.position.x, rock.position.y + 2f, rock.position.z);
                }
            }
        }
    }


}
