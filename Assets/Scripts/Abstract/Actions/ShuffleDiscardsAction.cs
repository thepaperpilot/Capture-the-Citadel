
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShuffleDiscardsAction : AbstractAction
{
    public override IEnumerator Run()
    {
        yield return CardsManager.Instance.ShuffleDiscards();
    }
}
