using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTurnAction : AbstractAction
{
    public IEnumerator Run()
    {
        yield return PlayerManager.Instance.StartTurn();
    }
}
