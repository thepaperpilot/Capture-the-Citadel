using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTurnAction : AbstractAction
{
    public int turn = -1;

    public IEnumerator Run()
    {
        yield return PlayerManager.Instance.StartTurn(turn);
    }
}
