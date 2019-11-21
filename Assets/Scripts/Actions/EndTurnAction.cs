using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndTurnAction : AbstractAction
{
    public IEnumerator Run()
    {
        if (CombatManager.Instance.IsPlayerTurn())
            yield return PlayerManager.Instance.EndTurn();
        CombatManager.Instance.EndTurn();
        yield return null;
    }
}
