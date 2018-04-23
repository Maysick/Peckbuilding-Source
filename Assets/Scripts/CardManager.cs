using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CardType {
    Normal,
    Move,
    Wound
}

public class Card {

    public string name;
    public Sprite image;
    public string description;
    public int aiBonus = 0;
    public int priority = 0;

    public CardUI inHandUI;

    public Fighter owner;

    public CardType type = CardType.Normal;
    public bool playable = true;

    public Action<Card> onPlay;

    public Card(string _name) {
        name = _name;
        GetImage();
    }

    public void GetImage() {
        foreach (CardImage ci in CardManager.instance.images) {
            if (ci.name == name) {
                image = ci.image;
                return;
            }
        }
    }

    public Card CreateInstance() {
        Card newCard = new Card(name);
        newCard.image = image;
        newCard.description = description;

        newCard.type = type;
        newCard.playable = playable;

        newCard.onPlay = onPlay;

        newCard.priority = priority;
        newCard.aiBonus = aiBonus;

        return newCard;
    }
}

[System.Serializable]
public class CardImage {
    public string name;
    public Sprite image;
}

public class CardManager : MonoBehaviour {

    public static CardManager instance;
    [SerializeField]
    public CardImage[] images;

    public List<Card> allCards;

    private void Awake() {
        instance = this;
        allCards = new List<Card>();

        GenerateCards();
    }

    private void GenerateCards() {

        Card card = new Card("Wound");
        card.description = "Unplayable. Lose if you have 3 in hand.";
        card.type = CardType.Wound;
        card.playable = false;
        card.onPlay = (c) => {
            
        };
        AddCard(card);

        card = new Card("Hop");
        card.description = "Move forward 1.";
        card.type = CardType.Move;
        card.onPlay = (c) => {
            c.owner.Move(1);
        };
        AddCard(card);

        card = new Card("Dodge");
        card.description = "Move back 1.";
        card.type = CardType.Move;
        card.onPlay = (c) => {
            c.owner.Move(-1);
        };
        AddCard(card);

        card = new Card("Leap");
        card.description = "Move forward 2.";
        card.type = CardType.Move;
        card.onPlay = (c) => {
            c.owner.Move(2);
        };
        AddCard(card);

        card = new Card("Gust");
        card.description = "Blow back any enemy in front of you by 1 space.";
        card.type = CardType.Normal;
        card.priority = 2;
        card.onPlay = (c) => {
            if (c.owner.facingRight) {
                if (c.owner.oppositeFighter.position <= c.owner.position) return;
                if (c.owner.oppositeFighter.facingRight) c.owner.oppositeFighter.Move(1);
                if (!c.owner.oppositeFighter.facingRight) c.owner.oppositeFighter.Move(-1);
            } else {
                if (c.owner.oppositeFighter.position >= c.owner.position) return;
                if (c.owner.oppositeFighter.facingRight) c.owner.oppositeFighter.Move(-1);
                if (!c.owner.oppositeFighter.facingRight) c.owner.oppositeFighter.Move(1);
            }
            c.owner.WindAnim(c.owner.GetDirectionalPosition(1));
        };
        AddCard(card);

        card = new Card("Peck");
        card.description = "Attack the two spaces in front of you.";
        card.type = CardType.Normal;
        card.onPlay = (c) => {
            int pos1 = c.owner.GetDirectionalPosition(1);
            int pos2 = c.owner.GetDirectionalPosition(2);
            c.owner.Attack(pos1);
            c.owner.Attack(pos2);
            c.owner.SwipeAnim(pos1 + ((pos2 - pos1) / 2f));
        };
        AddCard(card);

        card = new Card("Flurry");
        card.description = "Attack the space in front of and behind you.";
        card.type = CardType.Normal;
        card.onPlay = (c) => {
            c.owner.Attack(c.owner.position + 1);
            c.owner.SlashAnim(c.owner.position + 1);
            c.owner.Attack(c.owner.position - 1);
            c.owner.SlashAnim(c.owner.position - 1);
        };
        AddCard(card);

        card = new Card("Drill Peck");
        card.description = "Attack an enemy more than 3 spaces away.";
        card.type = CardType.Normal;
        card.onPlay = (c) => {
            int pos1 = c.owner.position;
            int pos2 = c.owner.oppositeFighter.position;
            if (Mathf.Abs(pos2 - pos1) >= 3) {
                c.owner.Attack(pos2);
                c.owner.SlashAnim(pos2);
            }
        };
        AddCard(card);

        card = new Card("Flap");
        card.description = "Block up to 1 Wound.";
        card.type = CardType.Normal;
        card.priority = 1;
        card.onPlay = (c) => {
            c.owner.block++;
        };
        AddCard(card);

        card = new Card("Eagle Eye");
        card.description = "Next turn, inflict 2 Wounds instead of 1.";
        card.type = CardType.Normal;
        card.onPlay = (c) => {
            c.owner.bonusAttack = 1;
            c.owner.bonusAttackTurns = 2;
            c.owner.BuffAnim();
        };
        AddCard(card);

        card = new Card("Talon Storm");
        card.description = "Attack the enemy, wherever they are. You don't inflict wounds for 2 turns.";
        card.type = CardType.Normal;
        card.onPlay = (c) => {
            for (int i = -4; i < 5; i++) {
                c.owner.Attack(i);
                c.owner.SwipeAnim(i);
            }
        };
        AddCard(card);

        card = new Card("Storm's Eye");
        card.description = "Attack each space, except for the middle 3.";
        card.type = CardType.Normal;
        card.onPlay = (c) => {
            for (int i = -4; i < -1; i++) {
                c.owner.Attack(i);
                c.owner.SlashAnim(i);
            }

            for (int i = 2; i < 5; i++) {
                c.owner.Attack(i);
                c.owner.SlashAnim(i);
            }

        };
        AddCard(card);

        card = new Card("Hurricane Gale");
        card.description = "Move the enemy back 3 spaces.";
        card.type = CardType.Normal;
        card.priority = 2;
        card.onPlay = (c) => {
            c.owner.oppositeFighter.Move(-3);
            c.owner.WindAnim(c.owner.GetDirectionalPosition(1));
        };
        AddCard(card);

        card = new Card("Sharp Claws");
        card.description = "The next time you bump into the enemy, attack them.";
        card.type = CardType.Normal;
        card.onPlay = (c) => {
            c.owner.thorns = 1;
            c.owner.BuffAnim();
        };
        AddCard(card);

        card = new Card("Take Off");
        card.description = "Move 4 spaces.";
        card.type = CardType.Move;
        card.onPlay = (c) => {
            c.owner.Move(4);
        };
        AddCard(card);

        card = new Card("Fury Swipe");
        card.description = "Attack the space in front of you. Inflicts an extra wound.";
        card.type = CardType.Normal;
        card.onPlay = (c) => {
            c.owner.bonusAttack++;
            c.owner.bonusAttackTurns = 1;
            c.owner.Attack(c.owner.GetDirectionalPosition(1));
            c.owner.SlashAnim(c.owner.GetDirectionalPosition(1));
        };
        AddCard(card);

        card = new Card("ULTRA PECK");
        card.description = "Attack the enemy, wherever they are.";
        card.type = CardType.Normal;
        card.onPlay = (c) => {
            c.owner.WingsAnim();
            c.owner.BuffAnim();
            c.owner.Attack(c.owner.oppositeFighter.position);
            c.owner.SlashAnim(c.owner.oppositeFighter.position);
            c.owner.SwipeAnim(c.owner.oppositeFighter.position);
        };
        AddCard(card);

    }

    private void AddCard(Card c) {
        if (GetCard(c.name) == null) {
            allCards.Add(c);
        }
    }

    public Card GetCard(string name) {
        foreach (Card _c in allCards) {
            if (_c.name == name) return _c;
        }
        return null;
    }

}
