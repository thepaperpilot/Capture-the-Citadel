using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AegisStatus : Status
{
    public AegisStatus()
    {
        name = Name.AEGIS;
        type = StatusType.BUFF;
        priority = 100;
        decreasing = false;
    }

    public override int GetDamageTaken(int damage)
    {
        if(damage > 0)
        {
            amount--;
            CheckRemoval();
            return 0;
        }
        return damage;
    }
}
