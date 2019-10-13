using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawAction : AbstractAction
{
    public AbstractCard[] cards;

    public DrawAction(AbstractCard[] cards, AbstractAction[] chainedEvents) {
        this.cards = cards;
        this.chainedEvents = chainedEvents;
    }

    public override IEnumerator Run()
    {
        yield return null;
    }
}
