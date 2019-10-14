using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbstractStatus : ScriptableObject
{
    public int priority;

    public bool affectsStrength;
    public bool affectsDexterity;

    public int ModifyStrength(int currentStrength) {
        return currentStrength;
    }

    public int ModifyDexterity(int currentDexterity) {
        return currentDexterity;
    }
}
