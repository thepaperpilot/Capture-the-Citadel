using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrayerFatigueStatus : Status
{
    public PrayerFatigueStatus()
    {
        name = NAME.PRAYER_FATIGUE;
        type = STATUS_TYPE.DEBUFF;
        priority = 0;
    }

    public override void OnTurnEnd()
    {
        controller.AddStatus(new StrengthStatus(), -amount);
        controller.RemoveStatus(this);
    }
}
