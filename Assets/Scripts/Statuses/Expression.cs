using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public class Expression
{
    public enum Effects {
        DAMAGE,
        DEFENSE
    }
    
    public enum Modifiers {
        ADD,
        MULTIPLY
    }

    
    [HorizontalGroup("Expression"), HideLabel, ValueDropdown("GetModifiers")]
    public Modifiers modifier;
    [HorizontalGroup("Expression"), HideLabel]
    public int amount;
    [HorizontalGroup("Expression"), HideLabel]
    public Effects effect;

#if UNITY_EDITOR
    private static IEnumerable GetModifiers = new ValueDropdownList<Modifiers>()
    {
        { "+", Modifiers.ADD },
        { "*", Modifiers.MULTIPLY }
    };
#endif
}
