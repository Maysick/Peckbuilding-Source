using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fighter : MonoBehaviour {

    public int position;
    public bool facingRight;

    public Deck deck;

    public bool player;
    public SpriteRenderer sprite;

    [HideInInspector]
    public bool animationPlaying;

    public Fighter oppositeFighter;

    private Animator anim;

    public int block = 0;

    public int woundCount = 0;

    public int thorns = 0;

    public int bonusAttack = 0;
    public int bonusAttackTurns = 0;

    private Vector3 startScale;
    private Vector3 flippedScale;

    private void Awake() {
        anim = GetComponent<Animator>();
        startScale = sprite.transform.localScale;
        flippedScale = new Vector3(-startScale.x, startScale.y, startScale.z);
        if (!facingRight) {
            sprite.transform.localScale = flippedScale;
        }
    }

    public void StartTurn() {
        block = 0;
        bonusAttackTurns--;
        CorrectRootMotion();
        Reorient();
    }

    public void Bump() {
        if(thorns > 0) {
            thorns = 0;
            Attack(oppositeFighter.position);
        }
    }

    public void Move(int x) {
        animationPlaying = true;
        if (facingRight) {
            x = Mathf.Clamp(x, -GameManager.instance.maxBoardSize - position, GameManager.instance.maxBoardSize - position);
            position += x;
        }    
        else{
            x = Mathf.Clamp(x, -GameManager.instance.maxBoardSize + position, GameManager.instance.maxBoardSize + position);
            position -= x;
        }
        StartCoroutine(PlayMoveAnimation(x));
    }

    //returns absolute position of difference x based on direction
    public int GetDirectionalPosition(int x) {
        if (facingRight) return position + x;
        else return position - x;
    }

    IEnumerator PlayMoveAnimation(int times) {
        if (times > 0) {
            for (int i = 0; i < times; i++) {
                if (facingRight) anim.SetTrigger("ForwardRight");
                else anim.SetTrigger("ForwardLeft");
                yield return new WaitForSeconds(0.51f);
            }
        } else if (times < 0) {
            times = Mathf.Abs(times);
            for (int i = 0; i < times; i++) {
                if (facingRight) anim.SetTrigger("BackwardRight");
                else anim.SetTrigger("BackwardLeft");
                yield return new WaitForSeconds(0.51f);
            }
        }
        CorrectRootMotion();
        animationPlaying = false;
    }

    public void Reorient() {
        if (position < oppositeFighter.position) {
            SetFacingRight(true);
        } else if (position > oppositeFighter.position) {
            SetFacingRight(false);
        }
    }

    public void SetFacingRight(bool b) {
        facingRight = b;
        if (facingRight) {
            sprite.transform.localScale = startScale;
        } else {
            sprite.transform.localScale = flippedScale;
        }
    }

    public void Attack(int pos) {
        if (oppositeFighter.position == pos) {
            int damage = 1;
            if (bonusAttackTurns >= 1) {
                if (bonusAttack < 0) return;
                damage = 1 + bonusAttack;
            }
            oppositeFighter.TakeDamage(damage);
        }
    }

    void CorrectRootMotion() {
        transform.position = new Vector3(position, 0, 0);
        transform.localEulerAngles = new Vector3(0, 0, 0);
    }

    public void TakeDamage(int amount) {
        //Shuffle amount wounds into discard
        block -= amount;
        if (block >= 0) {
            WingsAnim();
            return;
        }
        amount = Mathf.Abs(block);
        anim.SetTrigger("Hurt");
        for (int i = 0; i < amount; i++) {
            woundCount++;
            deck.AddWoundToDiscard();
        }
    }

    public void SlashAnim(float pos) {
        Vector3 slashPos = new Vector3(pos, 0.25f, 0);
        GameObject slash = Instantiate(PrefabManager.instance.GetPrefabByName("SlashParticle"), slashPos, Quaternion.identity) as GameObject;
        if (pos <= position) slash.transform.localScale = new Vector3(-1, 1, 1);
    }

    public void WindAnim(float pos) {
        Vector3 slashPos = new Vector3(pos, 0.25f, 0);
        GameObject slash = Instantiate(PrefabManager.instance.GetPrefabByName("WindParticle"), slashPos, Quaternion.identity) as GameObject;
        if (!facingRight) {
            slash.transform.localScale = new Vector3(-1, 1, 1);
            slash.GetComponent<Translate>().speed *= -1;
        }
    }

    public void SwipeAnim(float pos) {
        Vector3 slashPos = new Vector3(pos, 0.25f, 0);
        GameObject slash = Instantiate(PrefabManager.instance.GetPrefabByName("SwipeParticle"), slashPos, Quaternion.identity) as GameObject;
        if (pos <= position) slash.transform.localScale = new Vector3(-1, 1, 1);
    }

    public void WingsAnim() {
        Vector3 wingsPos = new Vector3(position, 0.25f, 0);
        GameObject slash = Instantiate(PrefabManager.instance.GetPrefabByName("WingsParticle"), wingsPos, Quaternion.identity) as GameObject;
    }

    public void BuffAnim() {
        Vector3 wingsPos = new Vector3(position, 0.75f, 0);
        GameObject slash = Instantiate(PrefabManager.instance.GetPrefabByName("BuffParticle"), wingsPos, Quaternion.identity) as GameObject;
    }

    public Card AIChooseCard(CardType type) {
        List<Card> options = new List<Card>();
        foreach (Card c in deck.hand) {
            if (c.type == type)
                options.Add(c);
        }
        if (options.Count == 0) return null;
        if (options.Count == 1) return options[0];

        int highest = 0;
        Card highestCard = null;
        foreach (Card c in options) {
            if (c.aiBonus >= highest) {
                highestCard = c;
                highest = c.aiBonus;
            }
        }
        return highestCard;
    }

    public Card DeprecratedAIChooseCardInHand() {
        int totalWeight = 0;
        for (int i = 0; i < deck.hand.Count; i++) {
            totalWeight += deck.hand[i].aiBonus + 1;
        }
        int random = Random.Range(0, totalWeight) + 1;
        int t = 0;
        bool chosen = false;
        Card choice = null;
        for (int i = 0; i < deck.hand.Count; i++) {
            if (t + deck.hand[i].aiBonus + 1 >= random && !chosen) {
                chosen = true;
                choice = deck.hand[i];
            } else {
                t += deck.hand[i].aiBonus + 1;
            }
        }
        //check
        
        if (!chosen) choice = deck.hand[Random.Range(0, deck.hand.Count)];

        return choice;
    }
}
