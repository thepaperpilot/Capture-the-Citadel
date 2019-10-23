using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(DeckController))]
public class PlayerController : CombatantController
{
    [AssetsOnly]
    public GameObject cardPrefab;

    [InfoBox("This should probably be a sibling. Definitely don't make it a child")]
    [SerializeField, SceneObjectsOnly]
    private Transform hand;
    [SceneObjectsOnly]
    public Transform playArea;

    private DeckController deckController;

    private void Awake() {
        deckController = GetComponent<DeckController>();
    }

    public IEnumerator StartTurn() {
        deckController.SetDeckSize(CardsManager.Instance.drawPile.Count);
        yield return deckController.SlideOut();
    }

    public IEnumerator Draw(IEnumerable<AbstractCard> cardsToDraw) {
        foreach (AbstractCard card in cardsToDraw) {
            GameObject cardObject = Instantiate(cardPrefab, hand);
            //cardObject.transform.position = deckController.cardPosition;
            cardObject.GetComponent<CardController>().Setup(card);
            deckController.SetDeckSize(deckController.deckSize - 1);
            // TODO move card to somewhere
            // TODO let them pick up/select cards, handle any gestures necessary,
            //  and perform the PlayCard action on successful gesturing
        }
        yield return null;
    }
}
