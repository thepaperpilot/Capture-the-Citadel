using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddCardsAction : AbstractAction
{
    public AbstractCard[] cards;

    public AddCardsAction(AbstractCard[] cards) {
        this.cards = cards;
    }

    public IEnumerator Run()
    {
        CardsManager.Instance.deck.AddRange(cards);
        yield return null;
    }
}
