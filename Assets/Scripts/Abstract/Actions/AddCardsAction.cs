using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddCardsAction : AbstractAction
{
    public AbstractCard[] cards;

    public AddCardsAction(AbstractCard[] cards, AbstractAction[] chainedEvents = null) {
        this.cards = cards;
        this.chainedEvents = chainedEvents;
    }

    public override IEnumerator Run()
    {
        CardsManager.Instance.deck.AddRange(cards);
        yield return null;
    }
}
