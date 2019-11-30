using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Strength : BaseStatus
{
    public Strength()
    {
        decreasing = false;
        priority = 0;
    }

    public override int DamageGiven(int damage)
    {
        return damage + amount;
    }

}
