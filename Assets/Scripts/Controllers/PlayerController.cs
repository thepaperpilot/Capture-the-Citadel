using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class PlayerController : CombatantController
{
    [AssetsOnly]
    public GameObject cardPrefab;

    public IEnumerator Draw(IEnumerable<AbstractCard> cardsToDraw) {
        foreach (AbstractCard card in cardsToDraw) {
            GameObject cardObject = Instantiate(cardPrefab, transform);
            cardObject.GetComponent<CardController>().Setup(card);
            // TODO move card to somewhere
            // TODO let them pick up/select cards, handle any gestures necessary,
            //  and perform the PlayCard action on successful gesturing
        }
        yield return null;
    }
}
