using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum GameState {
    PlayingCards,
    Animating,
    Over
}

public class GameManager : MonoBehaviour {

    public static GameManager instance;

    public Fighter player;
    public Fighter enemy;

    [HideInInspector]
    public CardUI selectedCard;
    [HideInInspector]
    public CardUI selectedCardMove;

    [HideInInspector]
    public GameState state;

    public int maxBoardSize = 4;
    //-4 to 4

    public Color[] enemyColors;

    public Image enemyFace;

    private void Awake() {
        instance = this;

    }

    private void Start() {
        enemy.transform.GetChild(0).GetComponent<SpriteRenderer>().color = enemyColors[CrossSceneData.selectedID];
        enemyFace.color = enemyColors[CrossSceneData.selectedID];

        MakePlayerDeck();
        BeginGame();
        StartTurn();
    }

    private void Update() {

    }

    public void BeginEndTurn() {
        if (state != GameState.PlayingCards) return;
        state = GameState.Animating;
        StartCoroutine(EndTurn());
    }

    IEnumerator EndTurn() {
        

        Card ourMove = null;
        if (selectedCardMove != null) ourMove = selectedCardMove.card;
        Card ourAction = null;
        if (selectedCard != null) ourAction = selectedCard.card;

        //Play our move card, if we chose one
        if (ourMove != null) {
            UIManager.instance.SpawnIndicatorCard(ourMove, true, false);
        }

        //Choose enemy move
        Card enemyCardMove = enemy.AIChooseCard(CardType.Move);
        if (enemyCardMove != null) {
            UIManager.instance.SpawnIndicatorCard(enemyCardMove, false, false);
        }

        //Choose and show enemy action
        Card enemyCard = enemy.AIChooseCard(CardType.Normal);
        if (enemyCard != null) {
            UIManager.instance.SpawnIndicatorCard(enemyCard, false, true);
        }

        //Show our action
        if (ourAction != null) {
            UIManager.instance.SpawnIndicatorCard(ourAction, true, true);
        }

        player.deck.DiscardHand();
        enemy.deck.DiscardHand();

        //give some time
        yield return new WaitForSeconds(0.5f);

        if (enemyCardMove != null) {
            enemyCardMove.onPlay(enemyCardMove);
        }

        if (ourMove != null) {
            ourMove.onPlay(ourMove);
        }


            //wait for both move animations to finish
        while (player.animationPlaying || enemy.animationPlaying) {
            yield return new WaitForSeconds(0.1f);
        }


        //bumps
        if (player.position == enemy.position) {
            player.Bump();
            enemy.Bump();
            if (player.facingRight != enemy.facingRight) {
                Debug.Log("NOrmal bump");
                player.Move(-1);
                enemy.Move(-1);
            } else {
                if (Random.Range(0, 2) == 1) {
                    player.SetFacingRight(true);
                    enemy.SetFacingRight(false);
                    player.Move(-1);
                    enemy.Move(-1);
                } else {
                    player.SetFacingRight(false);
                    enemy.SetFacingRight(true);
                    player.Move(-1);
                    enemy.Move(-1);
                }
            }
        }

        //wait for both move animations to finish
        while (player.animationPlaying || enemy.animationPlaying) {
            yield return new WaitForSeconds(0.1f);
        }

        //give some time
        yield return new WaitForSeconds(0.5f);

        //handle priorities
        if (ourAction != null && enemyCard != null) {
            if (ourAction.priority > enemyCard.priority) {
                Debug.Log("Our Priority");
                ourAction.onPlay(ourAction);
                while (player.animationPlaying || enemy.animationPlaying) {
                    yield return new WaitForSeconds(0.1f);
                }
                enemyCard.onPlay(enemyCard);
            } else if (enemyCard.priority > ourAction.priority) {
                Debug.Log("Their Priority");
                enemyCard.onPlay(enemyCard);
                while (player.animationPlaying || enemy.animationPlaying) {
                    yield return new WaitForSeconds(0.1f);
                }
                ourAction.onPlay(ourAction);
            } else {
                Debug.Log("No Priority");
                ourAction.onPlay(ourAction);
                enemyCard.onPlay(enemyCard);
            }
        } else {
            if (ourAction != null) ourAction.onPlay(ourAction);
            if (enemyCard != null) enemyCard.onPlay(enemyCard);
        }

        //verify if card was in hand?

        //wait for both move animations to finish
        while (player.animationPlaying || enemy.animationPlaying) {
            yield return new WaitForSeconds(0.1f);
        }


        selectedCard = null;
        selectedCardMove = null;

        StartTurn();
    }

    private bool CheckForGameOver() {
        int playerWoundsInHand = 0;
        foreach (Card c in player.deck.hand) {
            if (c.type == CardType.Wound) playerWoundsInHand++;
        }

        int enemyWoundsInHand = 0;
        foreach (Card c in enemy.deck.hand) {
            if (c.type == CardType.Wound) enemyWoundsInHand++;
        }

        if (enemyWoundsInHand == 3) {
            Win();
            return true;
        }

        if (playerWoundsInHand == 3) {
            GameOver();
            return true;
        }
        return false;
    }

    IEnumerator ReturnToMainMenu(float seconds) {
        yield return new WaitForSeconds(seconds);
        SceneManager.LoadScene(1);
    }

    void GameOver() {
        UIManager.instance.EndGame(false);
        StartCoroutine(ReturnToMainMenu(3));
    }

    void Win() {
        UIManager.instance.EndGame(true);
        CrossSceneData.pendingReward = CrossSceneData.selectedID;
        StartCoroutine(ReturnToMainMenu(3));
    }

    void StartTurn() {
        state = GameState.PlayingCards;

        for(int i = 0; i < 3; i++) {
            player.deck.DrawCard();
            enemy.deck.DrawCard();
        }

        if (!CheckForGameOver()) {
            player.StartTurn();
            enemy.StartTurn();
        }
    }

    void BeginGame() {
        player.deck.Initialize();
        enemy.deck.Initialize();
    }

    private void MakePlayerDeck() {
        List<Card> deck = new List<Card>();
        foreach(string s in CrossSceneData.decklist) {
            deck.Add(MakeNewCard(s, player));
        }

        player.deck = new Deck(deck);
        player.deck.owner = player;
        player.deck.player = true;

        deck.Clear();
        foreach (string s in CrossSceneData.enemyDecklist) {
            deck.Add(MakeNewCard(s, enemy));
        }

        enemy.deck = new Deck(deck);
        enemy.deck.owner = enemy;
    }

    public Card MakeNewCard(string name, Fighter f) {
        if (CardManager.instance.GetCard(name) == null) return null;
        Card c = CardManager.instance.GetCard(name).CreateInstance();
        c.owner = f;
        return c;
    }
}
