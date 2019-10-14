using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndTurnAction : AbstractAction
{
    public override IEnumerator Run()
    {
        CombatManager.Instance.EndTurn();
        yield return null;
    }
}
