using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTurnAction : AbstractAction
{
    private PlayerController controller;

    public PlayerTurnAction(PlayerController controller) {
        this.controller = controller;
    }

    public IEnumerator Run()
    {
        yield return controller.StartTurn();
    }
}
