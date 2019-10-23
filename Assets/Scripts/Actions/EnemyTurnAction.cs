using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTurnAction : AbstractAction
{
    private EnemyController controller;

    public EnemyTurnAction(EnemyController controller) {
        this.controller = controller;
    }

    public IEnumerator Run()
    {
        controller.PlayTurn();
        yield return null;
    }
}
