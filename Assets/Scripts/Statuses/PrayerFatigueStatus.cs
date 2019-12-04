using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrayerFatigueStatus : Status
{
    public PrayerFatigueStatus()
    {
        name = Name.PRAYER_FATIGUE;
        type = StatusType.DEBUFF;
        priority = 0;
    }

    public override void OnTurnEnd()
    {
        CauseStatus(Name.STRENGTH, -amount);
        controller.RemoveStatus(this);
    }
}
