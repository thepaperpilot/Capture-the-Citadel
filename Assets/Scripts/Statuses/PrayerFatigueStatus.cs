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
        controller.AddStatus(new StrengthStatus(), -amount);
        controller.RemoveStatus(this);
    }
}
