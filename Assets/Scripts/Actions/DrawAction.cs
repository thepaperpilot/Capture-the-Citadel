using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawAction : AbstractAction
{
    public int amount;

    public DrawAction(int amount, AbstractAction[] chainedEvents = null) {
        this.amount = amount;
    }

    public IEnumerator Run()
    {
        yield return CardsManager.Instance.Draw(amount);
    }
}
