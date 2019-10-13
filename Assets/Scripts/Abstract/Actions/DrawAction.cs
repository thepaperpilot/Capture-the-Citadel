using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawAction : AbstractAction
{
    public int amount;

    public DrawAction(int amount, AbstractAction[] chainedEvents = null) {
        this.amount = amount;
        this.chainedEvents = chainedEvents;
    }

    public override IEnumerator Run()
    {
        yield return CardsManager.Instance.Draw(amount);
    }
}
