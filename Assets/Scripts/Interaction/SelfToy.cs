using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfToy : Toy
{

    void Update()
    {
        if (PlayerManager.Instance.GetRightHand().GetTriggerPressed())
        {
            Trigger(CombatManager.Instance.player.gameObject);
        }
    }
}
