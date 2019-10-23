using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndTurnAction : AbstractAction
{
    public IEnumerator Run()
    {
        CombatManager.Instance.EndTurn();
        yield return null;
    }
}
