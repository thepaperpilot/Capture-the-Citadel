using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GainGoldAction : AbstractAction
{
    public int amount;

    public GainGoldAction(int amount) {
        this.amount = amount;
    }

    public IEnumerator Run()
    {
        PlayerManager.Instance.gold += amount;
        yield return null;
    }
}
