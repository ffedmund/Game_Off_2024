using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Hands : MonoBehaviour {
    public enum Role
    {
        Dealer,
        Player
    }
    
    public List<Card> cards{get; private set;}

    public Role role;
    public float spacing = 1;
    int selectedCardIndex;
    bool hideFirstCard;
    BlackjackController blackjackController;

    void Start()
    {
        blackjackController = FindAnyObjectByType<BlackjackController>();
    }

    public void InitializeHands()
    {
        foreach(Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        hideFirstCard = role == Role.Dealer;
        selectedCardIndex = -1;
        cards?.Clear();
        cards = new List<Card>();
    }

    public void AddCardToHands(Card card)
    {
        GameObject cardObject = Instantiate(CardSpriteReference.Instance.cardPrefab, transform);
        DisplayCard displayCard = cardObject.GetComponent<DisplayCard>();
        if(role == Role.Player){ 
            int index = cards.Count;
            displayCard.OnCardClicked += ()=> {
                blackjackController.selectedHands = this;
                selectedCardIndex = index == selectedCardIndex?-1:index;
            };
        }
        displayCard.Instantiate(card);
        cards.Add(card);
        UpdateHands();
    }

    public void ShowHands()
    {
        hideFirstCard = false;
        UpdateHands();
    }

    public bool ReplaceCard(Card card, out Card replacedCard)
    {
        replacedCard = null;
        if(selectedCardIndex > -1 && selectedCardIndex < cards.Count)
        {
            replacedCard = cards[selectedCardIndex];
            cards[selectedCardIndex] = card;
            transform.GetChild(selectedCardIndex).GetComponent<DisplayCard>().Instantiate(card);
            UpdateHands();
            return true;
        }
        return false;
    }

    void UpdateHands()
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            SpriteRenderer spriteRenderer = child.GetComponent<SpriteRenderer>();
            DisplayCard card = child.GetComponent<DisplayCard>();
            child.localPosition = new Vector3(i-transform.childCount/2,0,0) * spacing;
            spriteRenderer.sortingOrder = i;
            if(hideFirstCard && i == 0)
                card.HideCard();
            else
                card.ShowCard();
        }
    }
}