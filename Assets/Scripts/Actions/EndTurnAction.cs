using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndTurnAction : AbstractAction
{
    public IEnumerator Run()
    {
        // TODO Remove any other EndTurnActions from the queue
        CombatManager.Instance.EndTurn();
        yield return null;
    }
}
