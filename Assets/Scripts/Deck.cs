using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck {

    public static void Shuffle<T>(List<T> list) {  
        for (int length = list.Count - 1; length > 1; length--) {
            int j = Random.Range(0, length);
            T swap = list[j];
            list[j] = list[length];
            list[length] = swap;
        }
    }

    public void Initialize(){
        currentDeck.Clear();
        hand.Clear();
        discard.Clear();

        foreach(Card c in startingDeck) {
            currentDeck.Add(c);
        }
    }

    public bool player = false;
    public Fighter owner;

    public List<Card> startingDeck;

    public List<Card> currentDeck;
    public List<Card> hand;
    public List<Card> discard;

    public void AddWoundToDiscard() {
        discard.Add(GameManager.instance.MakeNewCard("Wound", owner));
    }

    public Deck (List<Card> cards) {
        startingDeck = new List<Card>();
        foreach(Card c in cards) {
            startingDeck.Add(c);
        }
        currentDeck = new List<Card>();
        hand = new List<Card>();
        discard = new List<Card>();
    }

    public Card DrawCard() {
        if (currentDeck.Count == 0) {
            DiscardIntoDeck();
        }
        //discard was empty too
        if (currentDeck.Count == 0) {
            return null;
        }


        Shuffle(currentDeck);
        Card c = currentDeck[0];
        currentDeck.Remove(c);
        if (player) {
            UIManager.instance.DisplayDrawCard(c);
        }

        PutCardInHand(c);
        return c;
    }

    public Card[] DrawCards(int amount) {
        Card[] cards = new Card[amount];
        for(int i = 0; i < amount; i++) {
            cards[i] = DrawCard();
        }
        return cards;
    }

    public void DiscardIntoDeck() {
        foreach(Card c in discard) {
            currentDeck.Add(c);
        }
        discard.Clear();
        Shuffle(currentDeck);
    }

    public void PutCardInHand(Card c) {
        hand.Add(c);
    }

    public void DiscardHand() {
        foreach(Card c in hand) {
            discard.Add(c);
            if (player && c.inHandUI != null) {
                c.inHandUI.Destruct();
            }
        }
        hand.Clear();
    }


}