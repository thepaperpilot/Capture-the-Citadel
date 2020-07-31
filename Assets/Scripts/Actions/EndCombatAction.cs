using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndCombatAction : AbstractAction
{
    public IEnumerator Run()
    {
        CombatManager.Instance.player.statusController.ResetStatuses();
        yield return null;
    }
}
